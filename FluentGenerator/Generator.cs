using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using Fluent.Net.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Fluent.Net.Ast;
using SyntaxNode = Fluent.Net.Ast.SyntaxNode;
using System.Threading;

namespace SourceGenerator.Configuration;


[SourceGenerator.Helper.CopyCode.Copy]
[Serializable]
internal class MissingConfigurationException : Exception {


    public MissingConfigurationException(string key) : base($"Key {key} is missing from Configuration") {
    }


    [Obsolete]
    protected MissingConfigurationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {
    }
}

[Generator]
public class ConfiguationvGeneratord : ISourceGenerator {


    private static Regex ftlRegex = new(@"\.\w\w(-\w\w)?\.ftl$");

    public void Initialize(GeneratorInitializationContext context) {
        // Register the attribute source
        //context.RegisterForPostInitialization((i) => {
        //    i.AddSource($"Attribute.g.cs", SourceGenerator.Helper.CopyCode.Copy.SourceGeneratorConfigurationGenerateConfigurationPropertiesAttribute);
        //    i.AddSource($"Exception.g.cs", SourceGenerator.Helper.CopyCode.Copy.SourceGeneratorConfigurationMissingConfigurationException);
        //});

    }


    public void Execute(GeneratorExecutionContext context) {

        var filesToConsume = context.AdditionalFiles.Where(x => ftlRegex.IsMatch(x.Path)).ToList();

        try {
            // get Project dir
            var projectDir = Path.GetDirectoryName(context.Compilation.SyntaxTrees.First().FilePath);

            context.AddSource("GeneratedFluent.cs", SourceText.From(Generat("Fluent", filesToConsume, context.CancellationToken), Encoding.UTF8));

        } catch (Exception e) {
            //context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FluentGenerator", "FluentGenerator", e.Message, "FluentGenerator", DiagnosticSeverity.Error, true, description:e.StackTrace), Location.None));


            var errorText = e.Message + "\n\n\n" + e.StackTrace;
            //prepend every line in the errorText with "// "
            errorText = "// " + errorText.Replace("\n", "\n// ");
            errorText = "#ERROR \"SOMETHING Went Wrong\" \n" + errorText;


            context.AddSource("GeneratedFluent.cs", SourceText.From(errorText, Encoding.UTF8));

        }





    }


    public static string Generat(string rootNamespace, IList<AdditionalText> ftlFiles, CancellationToken cancellationToken) {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(@"using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Fluent.Net;
");

        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"// handling {ftlFiles.Count} files");
        stringBuilder.AppendLine();

        foreach (var f in ftlFiles) {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"// handling {f.Path}");
            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f.Path));
            var text = f.GetText(cancellationToken);

            var parent = rootNamespace;// + "." + Path.GetDirectoryName(f).Replace('\\', '.');
            parent = parent.TrimEnd('.');

            Test(stringBuilder, name, parent, text.ToString());
        }

        return stringBuilder.ToString();
    }

    public static void Test(StringBuilder stringBuilder, string name, string @namespace, string ftl) {
        var parser = new Fluent.Net.Parser(false);
        using StringReader reader = new StringReader(ftl);
        var result = parser.Parse(reader);

        var quotes = "\"\"\"";
        while (ftl.Contains(quotes)) {
            quotes += "\"";
        }

        var fallbackValue = quotes + "\n" + ftl + "\n" + quotes;

        stringBuilder.AppendLine($$"""
            namespace {{@namespace}}
            
{
    public class {{name}}
    {
        private const string resourceName = "{{@namespace}}.{{name}}";
        private readonly Fluent.Net.MessageContext context;

        private const string FALLBACK = {{fallbackValue}};

        private {{name}}(Fluent.Net.MessageContext context)
        {
            this.context = context;
        }


        public static {{name}} GetContext(string? fluentFile = null, Fluent.Net.MessageContextOptions? options = null, CultureInfo? culture= null)
        {
            if(options== null)
                options = new Fluent.Net.MessageContextOptions();
            if(culture == null)
                culture = CultureInfo.CurrentUICulture;

            var ctx = new MessageContext(culture.Name, options);
            string correctResurce = null;
            if(fluentFile is null){
                correctResurce = FALLBACK;
            }
            var currentCulture = culture;
            while (correctResurce == null)
            {
                var pathWithoutExtension = System.IO.Path.GetFileNameWithoutExtension( fluentFile);
                if(pathWithoutExtension.EndsWith(currentCulture.Name)){
                    correctResurce = System.IO.File.ReadAllText(fluentFile);
                    break;
                }

                var pathWithLanguage = $"{pathWithoutExtension}{(string.IsNullOrEmpty(currentCulture.Name) ? "" : ".")}{currentCulture.Name}.ftl";
                if(System.IO.File.Exists(pathWithLanguage)){
                    correctResurce = System.IO.File.ReadAllText(pathWithLanguage);
                    break;
                }
                
                if (currentCulture.Equals(CultureInfo.InvariantCulture))
                    throw new FileNotFoundException("Resource not found",fluentFile);
                currentCulture = currentCulture.Parent;
            }

            FluentResource fluentResource;
            using (var reader = new StringReader(correctResurce))
            {
                fluentResource = FluentResource.FromReader(reader);
            }
                    
            ctx.AddResource(fluentResource);
            
            return new {{name}}(ctx);
        }
""");


        var enumerable = result.Body.OfType<Message>().ToArray();
        foreach (var message in enumerable) {
            var id = message.Id.Name;
            var comment = message.Comment;
            var propName = id.ToCharArray();
            propName[0] = char.ToUpperInvariant(propName[0]);
            while (propName.Contains('-')) {
                var index = Array.LastIndexOf(propName, '-');
                if (index + 1 < propName.Length) {
                    propName[index + 1] = char.ToUpperInvariant(propName[index + 1]);
                    propName[index] = (char)0;
                }
            }
            var prop = new string(propName);
            prop = prop.Replace(((char)0).ToString(), "");




            var variables = GetVariables(message.Value).Distinct().ToArray();

            var commentData = comment == null ? null as (string comment, (string name, Type type, string comment)[] parameter)? : GenerateDoc(variables, comment.Content);

            if (variables.Length > 0) {

                ComplexMessageStruct(stringBuilder, id, prop, variables, commentData);
                WriteComment(stringBuilder, commentData, false);
                GetComplexProperty(stringBuilder, prop);
            } else {
                SimpleProperty(stringBuilder, prop, id);

            }
        }

        stringBuilder.AppendLine("}\n}");
    }

    private static void WriteComment(StringBuilder stringBuilder, (string comment, (string name, Type type, string comment)[] parameter)? commentData, bool writeParameter) {
        if (commentData?.comment != null) {
            stringBuilder.AppendLine("/// <summary>");
            WritePrefixedLines(commentData.Value.comment);
            stringBuilder.AppendLine("/// </summary>");
        }

        if (writeParameter) {
            foreach (var parameter in commentData?.parameter ?? []) {
                stringBuilder.AppendLine($@"/// <param name=""{parameter.name}"">");
                WritePrefixedLines(parameter.comment);
                stringBuilder.AppendLine("/// </param>");
            }
        }

        void WritePrefixedLines(string str) {
            foreach (var item in str.Replace("\r\n", "\n").Split('\n'))
                stringBuilder.AppendLine($"/// {item}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="photoCount"></param>
    /// <param name="userGender"></param>
    /// <returns></returns>


    private static void SimpleProperty(StringBuilder stringBuilder, string propertyName, string messageId) {

        stringBuilder.AppendLine($"public string {ToPascalCase(propertyName)} => context.Format(context.GetMessage(\"{messageId}\"));");
    }

    private static void GetComplexProperty(StringBuilder stringBuilder, string propertyName) {
        stringBuilder.AppendLine($"    public Wrapper.{ToPascalCase(propertyName)}Wrapper {ToPascalCase(propertyName)} => new Wrapper.{ToPascalCase(propertyName)}Wrapper(context);");
    }

    private static (string comment, (string name, Type type, string comment)[] parameter) GenerateDoc(string[] variables, string comment) {
        if (String.IsNullOrWhiteSpace(comment))
            return (null, new (string name, Type type, string comment)[0]);

        var data = new List<(string name, Type type, StringBuilder comment)>();
        var normalComment = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(comment)) {
            var lines = comment.Replace("\r\n", "\n").Split('\n');
            Regex variableFormat = new Regex(@"^\$(?<name>[^ ]+)\s+(\((?<type>[^)]+)\))?\s*-\s*(?<rest>.*)$");

            foreach (var l in lines) {
                var match = variableFormat.Match(l);
                if (match.Success) {
                    var name = match.Groups["name"].Value;
                    if (variables.Contains(name)) {
                        var t = match.Groups["type"];

                        data.Add((name, GetTypeFromMatch(t), new StringBuilder(match.Groups["rest"].Value)));
                    }
                } else if (data.Any()) {
                    data[data.Count - 1].comment.AppendLine(l);
                } else {
                    normalComment.AppendLine(l);
                }
            }
        }
        return (normalComment.ToString(), data.Select(x => (x.name, x.type, x.comment.ToString())).ToArray());
    }

    private static Type GetTypeFromMatch(Group t) {
        if (!t.Success)
            return null;
        switch (t.Value.ToLower()) {
            case "string":
            case "text":
                return typeof(string);

            case "number":
                return typeof(double);

            case "int":
                return typeof(int);

            case "float":
                return typeof(float);

            case "double":
                return typeof(double);

            case "long":
                return typeof(long);

            default:
                return Type.GetType(t.Value, false) ?? typeof(object);
        }

    }

    private static void ComplexMessageStruct(StringBuilder stringBuilder, string messageId, string propertyName, string[] variables, (string comment, (string name, Type type, string comment)[] parameter)? commentData) {


        stringBuilder.AppendLine($@"public static partial class Wrapper{{
public struct {ToPascalCase(propertyName)}Wrapper
        {{
            private readonly MessageContext messageContext;
            public {ToPascalCase(propertyName)}Wrapper(MessageContext messageContext)
            {{
                this.messageContext = messageContext;
            }}");

        WriteComment(stringBuilder, commentData, true);
        stringBuilder.AppendLine($@"
            public string this[{string.Join(", ", variables.Select(x => {
            var type = commentData?.parameter.FirstOrDefault(y => x == y.name).type ?? typeof(object);
            return $"{type.FullName} {ToPascalCase(x)}";
        }))}]
            {{
                get
                {{
                    return this.messageContext.Format(this.messageContext.GetMessage(""{messageId}""), new Dictionary<string, object>{{{string.Join(", ", variables.Select(x => $@"{{""{x}"", {ToPascalCase(x)}}}"))}}});
                }}
            }}
        }}
}}");
    }

    //public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax Test(string name, string @namespace, TextReader ftl)
    //{
    //    var parser = new Fluent.Net.Parser(false);
    //    var result = parser.Parse(ftl);



    //    var classsDeclaration = GenerateClass(name, $"{@namespace}.{name}");

    //    foreach (var item in result.Body.OfType<Message>())
    //    {
    //        var id = item.Id.Name;
    //        var variables = GetVariables(item.Value).Distinct().ToArray();
    //        if (variables.Length > 0)
    //        {
    //            classsDeclaration = classsDeclaration.AddMembers(ComplexMessageStruct(id, id, variables), GetComplexProperty(id));
    //        }
    //        else
    //        {
    //            classsDeclaration = classsDeclaration.AddMembers(SimpleProperty(id, id));

    //        }
    //    }

    //    var element = GetNamespace(@namespace, classsDeclaration);
    //    return element;
    //}

    private static IEnumerable<string> GetVariables(SyntaxNode value) {
        switch (value) {
            case Pattern pattern:
                return pattern.Elements.SelectMany(GetVariables);
            case Placeable placeable:
                return GetVariables(placeable.Expression);
            case VariableReference variableReference:
                return new[] { variableReference.Id.Name };
            case SelectExpression selectExpression:
                return GetVariables(selectExpression.Selector)
                    .Concat(selectExpression.Variants.SelectMany(x => GetVariables(x.Value)));
            case Fluent.Net.Ast.CallExpression callExpression:
                return callExpression.Named.Select<NamedArgument, SyntaxNode>(x => x.Value).Concat(callExpression.Positional).SelectMany(GetVariables);

            default:
                return Enumerable.Empty<string>();
        }
    }

    private static string ToPascalCase(string name) {
        // find the words in current string, and capitalize the first letter of each word
        // split can be done by space, hyphen, underscore, or period
        return string.Join("", name.Split(new char[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }



}

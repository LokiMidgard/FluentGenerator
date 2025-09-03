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


    private readonly Regex variableFormat = new Regex(@"^\$(?<name>[^ ]+)\s+(\((?<type>[^)]+)\))?\s*-\s*(?<rest>.*)$");

    public void Initialize(GeneratorInitializationContext context) {
        // Register the attribute source
        //context.RegisterForPostInitialization((i) => {
        //    i.AddSource($"Attribute.g.cs", SourceGenerator.Helper.CopyCode.Copy.SourceGeneratorConfigurationGenerateConfigurationPropertiesAttribute);
        //    i.AddSource($"Exception.g.cs", SourceGenerator.Helper.CopyCode.Copy.SourceGeneratorConfigurationMissingConfigurationException);
        //});

    }


    public void Execute(GeneratorExecutionContext context) {

        var filesToConsume = context.AdditionalFiles.Where(x => Path.GetExtension(x.Path) == ".ftl").ToList();

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


    public string Generat(string rootNamespace, IList<AdditionalText> ftlFiles, CancellationToken cancellationToken) {
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

        Dictionary<string, string> additonalTypes = new();



        foreach (var f in ftlFiles) {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"// handling {f.Path}");
            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f.Path));
            var text = f.GetText(cancellationToken);

            var parent = rootNamespace;// + "." + Path.GetDirectoryName(f).Replace('\\', '.');
            parent = parent.TrimEnd('.');

            Test(stringBuilder, name, parent, text.ToString(), additonalTypes);
        }

        foreach (var item in additonalTypes.Values) {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(item);
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public void Test(StringBuilder stringBuilder, string name, string @namespace, string ftl, Dictionary<string, string> additonalTypes) {
        var parser = new Fluent.Net.Parser(false);
        using StringReader reader = new StringReader(ftl);
        var result = parser.Parse(reader);

        var quotes = "\"\"\"";
        while (ftl.Contains(quotes)) {
            quotes += "\"";
        }

        var fallbackValue = quotes + "\n" + ftl + "\n" + quotes;

        stringBuilder.AppendLine($$"""
            namespace {{@namespace}};
            

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
            string? correctResurce = null;
            if(fluentFile is null){
                correctResurce = FALLBACK;
            }
            var currentCulture = culture;
            while (correctResurce == null)
            {
                System.Diagnostics.Debug.Assert(fluentFile != null, "currentCulture != null");
                var pathWithoutExtension = System.IO.Path.GetFileNameWithoutExtension( fluentFile);
                if(pathWithoutExtension?.EndsWith(currentCulture.Name) ?? false){
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
            System.Diagnostics.Debug.Assert(correctResurce != null, "correctResurce != null");

            FluentResource fluentResource;
            using (var reader = new StringReader(correctResurce))
            {
                fluentResource = FluentResource.FromReader(reader);
            }
                    
            ctx.AddResource(fluentResource);
            
            return new {{name}}(ctx);
        }
""");


        var enumerable = result.Body.OfType<MessageTermBase>().ToDictionary(x => x.Id.Name);

        stringBuilder.AppendLine($"// message keys: {string.Join(", ", enumerable.Keys)}");




        foreach (var message in enumerable.Values.OfType<Message>()) {
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




            var variables = GetVariables(message, enumerable, message.Comment?.Content, additonalTypes).Distinct().ToArray();

            var commentData = comment == null ? null as (string comment, (string name, string comment)[] parameter)? : GenerateDoc(comment.Content);

            if (variables.Length > 0) {

                ComplexMessageStruct(stringBuilder, id, prop, variables, commentData);
                WriteComment(stringBuilder, commentData, false);
                GetComplexProperty(stringBuilder, prop);
            } else {
                SimpleProperty(stringBuilder, prop, id);

            }
        }

        stringBuilder.AppendLine("}\n");
    }

    private void WriteComment(StringBuilder stringBuilder, (string comment, (string name, string comment)[] parameter)? commentData, bool writeParameter) {
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
    ///
    private (string comment, (string name, string comment)[] parameter) GenerateDoc(string comment) {
        if (String.IsNullOrWhiteSpace(comment))
            return (null, new (string name, string comment)[0]);

        var data = new List<(string name, StringBuilder comment)>();
        var normalComment = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(comment)) {
            var lines = comment.Replace("\r\n", "\n").Split('\n');

            foreach (var l in lines) {
                var match = variableFormat.Match(l);
                if (match.Success) {
                    var name = match.Groups["name"].Value;
                    var t = match.Groups["type"];

                    data.Add((name, new StringBuilder(match.Groups["rest"].Value)));
                } else if (data.Any()) {
                    data[data.Count - 1].comment.AppendLine(l);
                } else {
                    normalComment.AppendLine(l);
                }
            }
        }
        return (normalComment.ToString(), data.Select(x => (x.name, x.comment.ToString())).ToArray());
    }


    private static (string type, Func<string,string>? transform) GetTypeFromMatch(string t, Dictionary<string, string> additonalTypes) {
        if (t is null)
            return default;
        if (t.Contains("|")) {
            var enumValues = t.Split('|').Select(x => x.Trim()).ToArray();
            var enumName = ToPascalCase(t); // as long as the ordeing is the same, the name will be the same


            additonalTypes[enumName] = $"public enum {enumName} {{ {string.Join(", ", enumValues)} }}";


            return (enumName, str => $"{str}.ToString()");

        }
        switch (t.ToLower()) {
            case "string":
            case "text":
                return (typeof(string).FullName, null);

            case "number":
                return (typeof(double).FullName, null);

            case "int":
                return (typeof(int).FullName, null);

            case "float":
                return (typeof(float).FullName, null);

            case "double":
                return (typeof(double).FullName, null);

            case "long":
                return (typeof(long).FullName, null);

            case "bool":
            case "boolean":
                return (typeof(bool).FullName, str=>$"{str}.ToString()");

            case "datetime":
                additonalTypes[typeof(DateTime).FullName] = """
                public class DateTimeConverter : FluentDateTime {
                    private readonly DateTime dateTime;
                    public DateTimeConverter(DateTime dateTime) : base(dateTime) {
                        this.dateTime = dateTime;
                    }
                    public override string Format(MessageContext ctx) {
                        return dateTime.ToString(ctx.Culture);
                    }
                    public override bool Match(MessageContext ctx, object obj) {
                        if (base.Match(ctx, obj)) {
                            return true;
                        } else if (obj is DateTimeConverter) {
                            return ((DateTimeConverter)obj).dateTime == dateTime;
                        }
                        return false;
                    }
                }
                """;


                return (typeof(DateTime).FullName, str => $"new Fluent.DateTimeOffsetConverter({str})");
            
            
            case "datetimeoffset":
                additonalTypes[typeof(DateTimeOffset).FullName] = """
                    public class DateTimeOffsetConverter : FluentType {
                        private readonly DateTimeOffset dateTimeOffset;
                        public DateTimeOffsetConverter(DateTimeOffset dateTimeOffset) : base(dateTimeOffset.ToString("o")) {
                            this.dateTimeOffset = dateTimeOffset;
                        }
                        public override string Format(MessageContext ctx) {
                            return dateTimeOffset.ToString(ctx.Culture);
                        }
                        public override bool Match(MessageContext ctx, object obj) {
                            if (obj is DateTimeOffsetConverter) {
                                return ((DateTimeOffsetConverter)obj).dateTimeOffset == dateTimeOffset;
                            }
                            return false;
                        }
                    }
                    """;

                return (typeof(DateTimeOffset).FullName, str => $"new Fluent.DateTimeOffsetConverter({str})");

            case "time":
                additonalTypes["System.TimeOnly"] = """
                    public class TimeConverter : FluentType {
                        private readonly TimeOnly time;
                        public TimeConverter(TimeOnly time) : base(time.ToString("o")) {
                            this.time = time;
                            ((IFluentType)this).Value = time.ToString();
                        }
                        public override string Format(MessageContext ctx) {
                            return time.ToString(ctx.Culture);
                        }
                        public override bool Match(MessageContext ctx, object obj) {
                            if (obj is TimeConverter) {
                                return ((TimeConverter)obj).time == time;
                            }
                            return false;
                        }
                    }
                    """;
                return ("System.TimeOnly", str=>$"new Fluent.TimeConverter({str})");

            case "date":
                additonalTypes["System.DateOnly"] = """
                    public class DateConverter : FluentType {
                        public readonly DateOnly date;
                        public DateConverter(DateOnly date) : base(date.ToString("o")) {
                            this.date = date;
                        }
                        public override string Format(MessageContext ctx) {
                            return date.ToString(ctx.Culture);
                        }

                        public override bool Match(MessageContext ctx, object obj) {
                            if (obj is DateConverter) {
                                return ((DateConverter)obj).date == date;
                            }
                            return false;
                        }
                    }
                    """;

                return ("System.DateOnly", str=>$"new Fluent.DateConverter({str})");

            default:
                return (t, str => $"{str} is Fluent.Net.IFluentType ? {str} : {str}.ToString()");
        }

    }

    private void ComplexMessageStruct(StringBuilder stringBuilder, string messageId, string propertyName, (string name, string type, Func<string, string>? transform)[] variables, (string comment, (string name, string comment)[] parameter)? commentData) {


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
            var type = x.type ?? typeof(object).FullName;
            return $"{type} {ToPascalCase(x.name)}";
        }))}]
            {{
                get
                {{
                    return this.messageContext.Format(this.messageContext.GetMessage(""{messageId}""), new Dictionary<string, object>{{{string.Join(", ", variables.Select(x => x.transform!=null ? $@"{{""{x.name}"", {x.transform(ToPascalCase(x.name))}}}" : $@"{{""{x.name}"", {ToPascalCase(x.name)}}}"))}}});
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

    private IEnumerable<(string name, string? type, Func<string, string>? transform)> GetVariables(SyntaxNode value, Dictionary<string, MessageTermBase> messages, string? comment, Dictionary<string, string> additonalTypes) {

        var lines = comment?.Replace("\r\n", "\n").Split('\n') ?? [];
        var knownTypes = lines.Select(l => {
            var match = variableFormat.Match(l);
            if (match.Success && match.Groups["type"].Success) {
                var name = match.Groups["name"].Value;
                var type = match.Groups["type"].Value;
                var (typeName, toString) = GetTypeFromMatch(type, additonalTypes);
                return (name, typeName, toString);
            }
            return null as (string name, string type, Func<string, string>? transform)?;
        })
            .Where(x => x.HasValue)
            .ToDictionary(x => x.Value.name, x => (x.Value.type, x.Value.transform));
        return GetVariables(value, messages, knownTypes, additonalTypes);
    }
    private IEnumerable<(string name, string? type, Func<string, string>? transform)> GetVariables(SyntaxNode value, Dictionary<string, MessageTermBase> messages, Dictionary<string, (string typeName, Func<string, string>? transform)> knownTypes, Dictionary<string, string> additonalTypes) {
        switch (value) {
            case Fluent.Net.Ast.MessageTermBase messageTerm:






                var variablesOfTerm = GetVariables(messageTerm.Value, messages, messageTerm.Comment?.Content, additonalTypes)
                    .Select(original => {

                        if (knownTypes.TryGetValue(original.name, out var type)) {
                            return (original.name, type.typeName, type.transform);
                        } else {
                            return original;
                        }
                    });


                return variablesOfTerm;
            case Pattern pattern:
                return pattern.Elements.SelectMany(x => GetVariables(x, messages, knownTypes, additonalTypes));
            case Placeable placeable:
                return GetVariables(placeable.Expression, messages, knownTypes, additonalTypes);
            case VariableReference variableReference:
                if (knownTypes.TryGetValue(variableReference.Id.Name, out var type)) {
                    return [(variableReference.Id.Name, type.typeName, type.transform)];
                } else {
                    return [(variableReference.Id.Name, null, null)];
                }
            case SelectExpression selectExpression:
                return GetVariables(selectExpression.Selector, messages, knownTypes, additonalTypes)
                    .Concat(selectExpression.Variants.SelectMany(x => GetVariables(x.Value, messages, knownTypes, additonalTypes)));
            case Fluent.Net.Ast.MessageTermReference mesageTermReference:
                var original = messages[mesageTermReference.Id.Name];
                var variables = GetVariables(original.Value, messages, original.Comment?.Content, additonalTypes);
                return variables;
            //case Fluent.Net.Ast.MessageReference messageReference:
            //    return GetVariables(messages[messageReference.Id.Name].Value, messages);
            case Fluent.Net.Ast.CallExpression callExpression:
                return callExpression.Named.Select<NamedArgument, SyntaxNode>(x => x.Value).Concat(callExpression.Positional).SelectMany(x => GetVariables(x, messages, knownTypes, additonalTypes));

            default:
                return [];
        }
    }

    private static string ToPascalCase(string name) {
        // find the words in current string, and capitalize the first letter of each word
        // split can be done by space, hyphen, underscore, or period
        return string.Join("", name.Split(new char[] { ' ', '-', '_', '.', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }



}

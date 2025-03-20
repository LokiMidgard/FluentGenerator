-privilegien-seite=https://hochschule-trier.de/

duration = 
  {$kasus ->
    [akkusativ] {-duration-akkusativ}
    *[dativ] {-duration-dativ}
  }

-duration-akkusativ = {$years ->
    [0] {""}
    [1] ein Jahr
    *[other] {$years} Jahre
  }{$years ->
    [0] {""}
    *[other] {$months ->
                [0] { $days ->
                      [0] {""}
                      *[other] {" "}und
                    }
                *[other]  { $days ->
                            [0] {" "}und
                            *[other] ,
                          }
              }
  }{$months ->
    [0] {""}
    [1] {""} einen Monat
    *[other] {""} {$months} Monate
  }{$months ->
    [0] {""}
    *[other] {$days ->
                [0] {""}
                *[other] {" "}und
              }
  }{$days ->
    [0] {""}
    [1] {""} einen Tag
    *[other] {""} {$days} Tage
  }
-duration-dativ = {$years ->
    [0] {""}
    [1] einem Jahr
    *[other] {$years} Jahren
  }{$years ->
    [0] {""}
    *[other] {$months ->
                [0] { $days ->
                      [0] {""}
                      *[other] {" "}und
                    }
                *[other]  { $days ->
                            [0] {" "}und
                            *[other] ,
                          }
              }
  }{$months ->
    [0] {""}
    [1] {""} einem Monat
    *[other] {""} {$months} Monaten
  }{$months ->
    [0] {""}
    *[other] {$days ->
                [0] {""}
                *[other] {" "}und
              }
  }{$days ->
    [0] {""}
    [1] {""} einem Tag
    *[other] {""} {$days} Tagen
  }

professori = 
  {$gender ->
    [male] Professor
    [female] Professorin
    *[other] Professor*in
  }

emeritori = 
  {$gender ->
    [male] Emeritus
    [female] Emerita
    *[other] Eseritierte Person
  }


personalverwaltungs-mail = {$location ->
 [trier] {""} (personalverwaltung@hochschule-trier.de)
 [ucb] {""} (personalverwaltung@umwelt-campus.de)
 *[other] {""}
 }

# Emeritus Review


#  ███████████                         ███                                                                                              
# ░░███░░░░░███                       ░░░                                                                                               
#  ░███    ░███   ██████  █████ █████ ████   ██████  █████ ███ █████    ████████   ██████  ████████   █████   ██████  ████████    █████ 
#  ░██████████   ███░░███░░███ ░░███ ░░███  ███░░███░░███ ░███░░███    ░░███░░███ ███░░███░░███░░███ ███░░   ███░░███░░███░░███  ███░░  
#  ░███░░░░░███ ░███████  ░███  ░███  ░███ ░███████  ░███ ░███ ░███     ░███ ░███░███████  ░███ ░░░ ░░█████ ░███ ░███ ░███ ░███ ░░█████ 
#  ░███    ░███ ░███░░░   ░░███ ███   ░███ ░███░░░   ░░███████████      ░███ ░███░███░░░   ░███      ░░░░███░███ ░███ ░███ ░███  ░░░░███
#  █████   █████░░██████   ░░█████    █████░░██████   ░░████░████       ░███████ ░░██████  █████     ██████ ░░██████  ████ █████ ██████ 
# ░░░░░   ░░░░░  ░░░░░░     ░░░░░    ░░░░░  ░░░░░░     ░░░░ ░░░░        ░███░░░   ░░░░░░  ░░░░░     ░░░░░░   ░░░░░░  ░░░░ ░░░░░ ░░░░░░  
#                                                                       ░███                                                            
#                                                                       █████                                                           
#                                                                      ░░░░░                                                            

# $firstname (string) - Vorname
review-emeritus-subject= Emeritus {$firstname} {$lastname} ({$person-id}) muss Überprüft werden
review-emeritus= 
  --- Überprüfung erforderlich ---

  Mitteilungsgrund:  Überprüfung eines Emeritus

  Vorname: {$firstname}
  Nachname:{$lastname}
  Ris-Id:  {$person-id}
  Link:    {$risAdminLink}

  Rolle:   {$role-description} ({$role-id})
  

  -----------------------------------------------------------------------------------------------------------------------
  RIS/IdM {$version}
  Diese Email wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten.

review-suspension-subject= Aussetzung des Cleanups für {$firstname} {$lastname} ({$person-id}/{$role-id}) muss Überprüft werden
review-suspension= 
  --- Überprüfung erforderlich ---

  Mitteilungsgrund:  Überprüfung der Aussetzung des automatischen Löschens für eine Rolle

  Vorname: {$firstname}
  Nachname:{$lastname}
  Ris-Id:  {$person-id}
  Link:    {$risAdminLink}

  Rolle:   {$role-description} ({$role-id})
  

  -----------------------------------------------------------------------------------------------------------------------
  RIS/IdM {$version}
  Diese Email wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten.


# Threshold mail
# params
#   {$personroleCount} (number) Die anzahl der nicht benachrichtgten rollen

threshold-met-subject=Threshold Erreicht für {$roleTypes}

threshold-met=
    Folgende RollenTypen werden nicht benachrichtigt: {$roleTypes}.
    Insgesamt werden {$personroleCount} nicht benachrichtigt.
    Dabei handelt es sich um:
    {$personroleIds}



#  ██████   █████           █████     ███     ██████   ███                      █████     ███                     
# ░░██████ ░░███           ░░███     ░░░     ███░░███ ░░░                      ░░███     ░░░                      
#  ░███░███ ░███   ██████  ███████   ████   ░███ ░░░  ████   ██████   ██████   ███████   ████   ██████  ████████  
#  ░███░░███░███  ███░░███░░░███░   ░░███  ███████   ░░███  ███░░███ ░░░░░███ ░░░███░   ░░███  ███░░███░░███░░███ 
#  ░███ ░░██████ ░███ ░███  ░███     ░███ ░░░███░     ░███ ░███ ░░░   ███████   ░███     ░███ ░███ ░███ ░███ ░███ 
#  ░███  ░░█████ ░███ ░███  ░███ ███ ░███   ░███      ░███ ░███  ███ ███░░███   ░███ ███ ░███ ░███ ░███ ░███ ░███ 
#  █████  ░░█████░░██████   ░░█████  █████  █████     █████░░██████ ░░████████  ░░█████  █████░░██████  ████ █████
# ░░░░░    ░░░░░  ░░░░░░     ░░░░░  ░░░░░  ░░░░░     ░░░░░  ░░░░░░   ░░░░░░░░    ░░░░░  ░░░░░  ░░░░░░  ░░░░ ░░░░░ 
                                                                                                                
                                                                                                                
                                                                                                                


# notifications
# for student, staff, lecture, guest
# parameter
# {$firstname} (text)
# {$lastname} (text)
# {$gender} (male|female|diverse|unknown)
# {$location} (TRIER|UCB|TRIER,UCB)
# {$isStudent} (bool)
# {$isStaff} (bool)
# {$isProf} (bool)
# {$id} (number) ris id
# {$willAccountDeleted} (bool)
# {$accountDeletitionDate} (date) the date when account will deleted
# {$roleNumber} (number) the number of roles
# {$roles} (text) a List generated by item generation (see below)
# {$lastDeletionDate} (date) the last deletion date in the list
# {$lastDueDate} (date) the last due date in the list
# {$isDeletionDateSame} (bool) true if all dates are the same
# {$isLastDueDateSame} (bool) true if all dates are the same
# {$version} (text) the version of ris
#
# item generation
#   all Parameters from above are availabel together with:
# parameter
#   {$item-name} (text)
#   {$item-shortname} (text)
#   {$item-deletionDate} (Date)
#   {$item-dueDate} (Date)
#   {$item-id} (number)

notification-student-role-list-seperator={", "} 
notification-student-role-list-seperator-last={" and "} 
notification-staff-role-list-seperator={", "} 
notification-staff-role-list-seperator-last={" and "} 
notification-lecturer-role-list-seperator={", "} 
notification-lecturer-role-list-seperator-last={" and "} 

notification-pir-role-list-seperator={", "} 
notification-pir-role-list-seperator-last={" and "} 

notification-guest-role-list-seperator={", "} 
notification-guest-role-list-seperator-last={" and "} 

notification-student-role-list-item=
  {$isDeletionDateSame ->
    [true] {$item-name}
    *[other] {$item-name} ({$item-deletionDate})
  }
notification-staff-role-list-item=
  {$isDeletionDateSame ->
    [true] {$item-name}
    *[other] {$item-name} ({$item-deletionDate})
  }
notification-lecturer-role-list-item=
  {$isDeletionDateSame ->
    [true] {$item-name}
    *[other] {$item-name} ({$item-deletionDate})
  }
notification-guest-role-list-item=
  {$isDeletionDateSame ->
    [true] {$item-name}
    *[other] {$item-name} ({$item-deletionDate})
  }
notification-pir-role-list-item=
  {$isDeletionDateSame ->
    [true] {$item-name}
    *[other] {$item-name} ({$item-deletionDate})
  }



notification-student-subject=
  {$willAccountDeleted ->
    [true] Ihre Hochschulkennung wird demnächst deaktiviert
    *[other] Ihr Studierendenstatus läuft demnächst ab
  }

notification-student=
 Hallo {$firstname} {$lastname},
 

 laut automatisierter Mitteilung aus dem Studienservice {COMPARE($lastDueDate, NOW()) ->
    [-1] ist
    [0] läuft
    *[1] wird
  } Ihre Immatrikulation für {$roleNumber ->
   [one] den Studiengang {$roles} 
   *[other] die Studiengänge {$roles}
 } {COMPARE($lastDueDate, NOW()) ->
    [-1] am {$lastDueDate} abgelaufen
    [0] heute ab
    *[1] am {$lastDueDate} ablaufen
  }.


 {$willAccountDeleted ->
                  [true]  Daher wird Ihre Hochschulkennung in {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate}, deaktiviert. Sie erhalten hierzu noch eine gesonderte E-Mail.
                  *[other] Sie verlieren in {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate}, die Berechtigungen, die mit Ihrem Studium verknüpft waren. Ihre Hochschulkennung bleibt jedoch zunächst erhalten. 
 }

 Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.
 
 Für Ihre Zukunft wünschen wir Ihnen alles Gute und viel Erfolg!
 
 Ihr IT-Helpdesk
 (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
 --
 Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
 Helpdesk: https://helpdesk.hochschule-trier.de
 Web: https://www.hochschule-trier.de/go/rz
 E-Mail: helpdesk@hochschule-trier.de
 Tel: +49 (0)651 8103 555
 
 IT-ServicePoint: Schneidershof, D03
 Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
 Fr.:       9:15 - 11:45 Uhr
 
 -----------------------------------------------------------------------------------------------------
 RIS/IdM {$version}  



notification-guest-subject=
   {$willAccountDeleted ->
    [true] Ihre Hochschulkennung wird demnächst deaktiviert
    *[other] Ihre Hochschulkennung wird demnächst ungültig
  }
notification-guest=
 Hallo {$firstname} {$lastname},
 
 die Gültigkeit Ihrer Hochschulkennung läuft demnächst ab.

  {$willAccountDeleted ->
    [true]  Daher wird Ihre Hochschulkennung in {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate}, deaktiviert. Sie erhalten hierzu noch eine gesonderte E-Mail.
    *[other]  Ihre Hochschulkennung bleibt jedoch erhalten, da Sie noch anderweitig an der Hochschule involviert sind.
  }

 Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.

 Mit freundlichen Grüßen
 
 Ihr IT-Helpdesk
 (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
 --
 Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
 Helpdesk: https://helpdesk.hochschule-trier.de
 Web: https://www.hochschule-trier.de/go/rz
 E-Mail: helpdesk@hochschule-trier.de
 Tel: +49 (0)651 8103 555
 
 IT-ServicePoint: Schneidershof, D03
 Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
 Fr.:       9:15 - 11:45 Uhr
 
 -----------------------------------------------------------------------------------------------------
 RIS/IdM {$version}  



notification-pir-subject=
   {$willAccountDeleted ->
    [true] Ihre Hochschulkennung wird demnächst deaktiviert
    *[other] Ihre Hochschulkennung wird demnächst ungültig
  }
notification-pir=
 Hallo {$firstname} {$lastname},
 
 die Gültigkeit Ihrer Hochschulkennung läuft demnächst ab.

  {$willAccountDeleted ->
    [true]  Daher wird Ihre Hochschulkennung in {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate}, deaktiviert. Sie erhalten hierzu noch eine gesonderte E-Mail.
    *[other]  Ihre Hochschulkennung bleibt jedoch erhalten, da Sie noch anderweitig an der Hochschule involviert sind.
  }

 Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.

 Mit freundlichen Grüßen
 
 Ihr IT-Helpdesk
 (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
 --
 Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
 Helpdesk: https://helpdesk.hochschule-trier.de
 Web: https://www.hochschule-trier.de/go/rz
 E-Mail: helpdesk@hochschule-trier.de
 Tel: +49 (0)651 8103 555
 
 IT-ServicePoint: Schneidershof, D03
 Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
 Fr.:       9:15 - 11:45 Uhr
 
 -----------------------------------------------------------------------------------------------------
 RIS/IdM {$version}  



notification-staff-subject=
   {$willAccountDeleted ->
    [true] Ihre Hochschulkennung wird demnächst deaktiviert
    *[other] Ihr Mitarbeiterstatus läuft demnächst ab
  }
notification-staff=
  Hallo {$firstname} {$lastname},

  laut automatisierter Mitteilung aus der Personalverwaltung {COMPARE($lastDueDate, NOW()) ->
    [-1] ist
    [0] läuft
    *[1] wird
  } Ihr Beschäftigungsverhältnis {COMPARE($lastDueDate, NOW()) ->
    [-1] am {$lastDueDate} abgelaufen
    [0] heute ab
    *[1] am {$lastDueDate} ablaufen
  }.
  

  {$isEmeriti ->
    [true] In {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate} werden wir in Absprache mit Ihrer Fachbereichsleitung Ihre Berechtigungen entsprechend Ihres Status als {professori} a.D. anpassen.
    *[false]   {$willAccountDeleted ->
                  [true]  { COMPARE($accountDeletitionDate, $lastDeletionDate) ->
                            [0]   { $isProf ->
                                    [true]  Wenn Sie nichts unternehmen, 
                                    *[other] Daher
                                  } wird Ihre Hochschulkennung in {TIMESPAN(NOW(), $lastDeletionDate,"Dativ")}, am {$lastDeletionDate}, deaktiviert. Sie erhalten hierzu noch eine gesonderte E-Mail.
                            *[other]  { $isProf ->
                                        [true]  Wenn Sie nichts unternehmen, 
                                        *[other] Daher
                                      } verlieren Sie die Berechtigungen, die mit Ihrer Beschäftigung verknüpft waren. Ihre Hochschulkennung bleibt jedoch bis zum {$accountDeletitionDate} ({TIMESPAN(NOW(), $accountDeletitionDate,"Akkusativ")}) erhalten.

                          }
                  *[other] Sie verlieren die Berechtigungen, die mit Ihrer Beschäftigung verknüpft waren. Ihre Hochschulkennung bleibt jedoch zunächst erhalten.
                           
                }
  } 


   {$isEmeriti ->
    [true] {""}
    *[other] {$isProf ->
                [true]  Als {professori} können Sie den Status "{professori} a.D" erhalten.
                        Ihre Hochschulkennung bleibt in diesem Fall dauerhaft bestehen, um beispielsweise weiterhin den E-Mailservice
                        nutzen zu können. In Absprache mit Ihrer Fachbereichsleitung werden Ihre Berechtigungen entsprechend angepasst.
                        
                        Falls dies Ihr Wunsch ist, antworten Sie bitte auf diese Mail.

                *[other] {""}
              }
  }

  Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.
  
 
  Mit freundlichen Grüßen
  
  Ihr IT-Helpdesk
  (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
  --
  Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
  Helpdesk: https://helpdesk.hochschule-trier.de
  Web: https://www.hochschule-trier.de/go/rz
  E-Mail: helpdesk@hochschule-trier.de
  Tel: +49 (0)651 8103 555
  
  IT-ServicePoint: Schneidershof, D03
  Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
  Fr.:       9:15 - 11:45 Uhr
  
  -----------------------------------------------------------------------------------------------------
  RIS/IdM {$version}  


notification-lecturer-subject=
   {$willAccountDeleted ->
    [true] Ihre Hochschulkennung wird demnächst deaktiviert
    *[other] Ihr Lehrbeauftragtenstatus läuft demnächst ab.
  }
notification-lecturer=
 Hallo {$firstname} {$lastname},
 
   {$willAccountDeleted ->
    [true] laut
    *[other] zusätzlich zu Ihrer Hochschulkennung besitzen Sie aktuell noch Berechtigungen Aufgrund einer Lehrauftragstätigkeit. Laut
   } automatisierter Mitteilung aus der Personalverwaltung {COMPARE($lastDueDate, NOW()) ->
    [-1] ist
    [0] läuft
    *[1] wird
  } Ihr Lehrbeauftragtenverhältnis {COMPARE($lastDueDate, NOW()) ->
    [-1] am {$lastDueDate} abgelaufen
    [0] heute ab
    *[1] am {$lastDueDate} ablaufen
  }. Sollten Sie bis zum {$lastDeletionDate} (in {TIMESPAN($lastDeletionDate)}) keinen neuen Lehrauftrag erhalten, {$willAccountDeleted ->
         [true]   wird Ihre Hochschulkennung zu diesem Zeitpunkt deaktiviert. Hierzu erhalten Sie einige Zeit vorher eine gesonderte E-Mail.
         *[other] verlieren Sie zu diesem Zeitpunkt die Berechtigungen die mit Ihrer Lehrauftragstätigkeit verknüpft waren. Ihre Hochschulkennung und sonstige damit verbundenen Berechtigungen bleiben jedoch erhalten.
     }

 Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.
 
 Mit freundlichen Grüßen
 
 Ihr IT-Helpdesk
 (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
 --
 Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
 Helpdesk: https://helpdesk.hochschule-trier.de
 Web: https://www.hochschule-trier.de/go/rz
 E-Mail: helpdesk@hochschule-trier.de
 Tel: +49 (0)651 8103 555
 
 IT-ServicePoint: Schneidershof, D03
 Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
 Fr.:       9:15 - 11:45 Uhr
 
 -----------------------------------------------------------------------------------------------------
 RIS/IdM {$version}  


# ====================================================================================================
#    █████████                                            █████       ██████   █████           █████     ███     ██████   ███                      █████     ███                     
#   ███░░░░░███                                          ░░███       ░░██████ ░░███           ░░███     ░░░     ███░░███ ░░░                      ░░███     ░░░                      
#  ░███    ░███   ██████   ██████  █████ ████ ████████   ███████      ░███░███ ░███   ██████  ███████   ████   ░███ ░░░  ████   ██████   ██████   ███████   ████   ██████  ████████  
#  ░███████████  ███░░███ ███░░███░░███ ░███ ░░███░░███ ░░░███░       ░███░░███░███  ███░░███░░░███░   ░░███  ███████   ░░███  ███░░███ ░░░░░███ ░░░███░   ░░███  ███░░███░░███░░███ 
#  ░███░░░░░███ ░███ ░░░ ░███ ░███ ░███ ░███  ░███ ░███   ░███        ░███ ░░██████ ░███ ░███  ░███     ░███ ░░░███░     ░███ ░███ ░░░   ███████   ░███     ░███ ░███ ░███ ░███ ░███ 
#  ░███    ░███ ░███  ███░███ ░███ ░███ ░███  ░███ ░███   ░███ ███    ░███  ░░█████ ░███ ░███  ░███ ███ ░███   ░███      ░███ ░███  ███ ███░░███   ░███ ███ ░███ ░███ ░███ ░███ ░███ 
#  █████   █████░░██████ ░░██████  ░░████████ ████ █████  ░░█████     █████  ░░█████░░██████   ░░█████  █████  █████     █████░░██████ ░░████████  ░░█████  █████░░██████  ████ █████
# ░░░░░   ░░░░░  ░░░░░░   ░░░░░░    ░░░░░░░░ ░░░░ ░░░░░    ░░░░░     ░░░░░    ░░░░░  ░░░░░░     ░░░░░  ░░░░░  ░░░░░     ░░░░░  ░░░░░░   ░░░░░░░░    ░░░░░  ░░░░░  ░░░░░░  ░░░░ ░░░░░ 
#                                                                                                                                                                                    
#                                                                                                                                                                                    
#                                                                                                                                                                                    
# ====================================================================================================


notification-account-subject=
  Ihre Hochschulkennung wird demnächst deaktiviert

notification-account=
 Hallo {$firstname} {$lastname},
 
 ihre Hochschulkennung für den Standort {$location} wird am {$accountDeletitionDate} deaktiviert. 30 Tage später wird die Hochschulkennung ohne weitere Benachrichtigung gelöscht.

 {$isEmeriti ->
    [true] {""}
    *[other] {$isProf ->
                [true]  Als {professori} können Sie den Status "{professori} a.D" erhalten.
                        Ihre Hochschulkennung bleibt in diesem Fall dauerhaft bestehen, um beispielsweise weiterhin den E-Mailservice
                        nutzen zu können. In Absprache mit Ihrer Fachbereichsleitung werden Ihre Berechtigungen entsprechend angepasst.
                        
                        Falls dies Ihr Wunsch ist, antworten Sie bitte auf diese Mail.
                *[other] {""}
              }
  }

 Falls Sie Fragen dazu haben, antworten Sie einfach auf diese Mail.

 Mit freundlichen Grüßen
 
 Ihr IT-Helpdesk
 (Diese E-Mail wurde automatisch generiert und digital signiert, um ihre Authentizität zu gewährleisten)
 --
 Rechenzentrum der Hochschule Trier am Standort Trier (RZ/HT)
 Helpdesk: https://helpdesk.hochschule-trier.de
 Web: https://www.hochschule-trier.de/go/rz
 E-Mail: helpdesk@hochschule-trier.de
 Tel: +49 (0)651 8103 555
 
 IT-ServicePoint: Schneidershof, D03
 Mo. - Do.: 9:15 - 11:45 Uhr und 13:45 - 16:00 Uhr
 Fr.:       9:15 - 11:45 Uhr
 
 -----------------------------------------------------------------------------------------------------
 RIS/IdM {$version}  













#  █████  █████                            █████             █████                                           █████     ███                         ██████   ██████            ███  ████ 
# ░░███  ░░███                            ░░███             ░░███                                           ░░███     ░░░                         ░░██████ ██████            ░░░  ░░███ 
#  ░███   ░███   █████   ██████  ████████  ░███  ████████   ███████    ██████  ████████   ██████    ██████  ███████   ████   ██████  ████████      ░███░█████░███   ██████   ████  ░███ 
#  ░███   ░███  ███░░   ███░░███░░███░░███ ░███ ░░███░░███ ░░░███░    ███░░███░░███░░███ ░░░░░███  ███░░███░░░███░   ░░███  ███░░███░░███░░███     ░███░░███ ░███  ░░░░░███ ░░███  ░███ 
#  ░███   ░███ ░░█████ ░███████  ░███ ░░░  ░███  ░███ ░███   ░███    ░███████  ░███ ░░░   ███████ ░███ ░░░   ░███     ░███ ░███ ░███ ░███ ░███     ░███ ░░░  ░███   ███████  ░███  ░███ 
#  ░███   ░███  ░░░░███░███░░░   ░███      ░███  ░███ ░███   ░███ ███░███░░░   ░███      ███░░███ ░███  ███  ░███ ███ ░███ ░███ ░███ ░███ ░███     ░███      ░███  ███░░███  ░███  ░███ 
#  ░░████████   ██████ ░░██████  █████     █████ ████ █████  ░░█████ ░░██████  █████    ░░████████░░██████   ░░█████  █████░░██████  ████ █████    █████     █████░░████████ █████ █████
#   ░░░░░░░░   ░░░░░░   ░░░░░░  ░░░░░     ░░░░░ ░░░░ ░░░░░    ░░░░░   ░░░░░░  ░░░░░      ░░░░░░░░  ░░░░░░     ░░░░░  ░░░░░  ░░░░░░  ░░░░ ░░░░░    ░░░░░     ░░░░░  ░░░░░░░░ ░░░░░ ░░░░░ 
                                                                                                                                                                                      
   
pending-user-interaction-subject=
  Unbehandelte Doubletten

pending-user-interaction=
  Hallo liebes Rechenzentrum,
  
  es gibt {$count ->
            [one] eine unbearbeitete Dublette.
            *[other] {$count} unbearbeitete Dubletten.
          }
 
  Einsehbar unter https://ris-play.hochschule-trier.de/
  
  Viele Grüße
  RIS/IdM {$version}  


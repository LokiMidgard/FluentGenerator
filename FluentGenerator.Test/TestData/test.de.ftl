-privilegien-seite=https://hochschule-trier.de/

# $kasus (akkusativ|dativ) - case
duration = 
  {$kasus ->
    [akkusativ] {-duration-akkusativ}
    *[dativ] {-duration-dativ}
  }

# $years (number) - years
# $months (number) - months
# $days (number) - days
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
# $years (number) - years
# $months (number) - months
# $days (number) - days
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

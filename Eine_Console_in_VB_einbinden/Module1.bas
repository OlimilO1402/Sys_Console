Attribute VB_Name = "Module1"
Option Explicit
Public Console As New Console

Public Sub Main()
  Call Console.wWrite("Gib ")
  Call Console.wWrite("ein Zeichen ")
  Call Console.WriteLine("ein!")
  Call Console.WriteLine("gefolgt von Enter:")
  Dim s As String
  s = Console.Read
  Call Console.WriteLine("Du hast das folgende Zeichen eingegeben: " & s)
  Call Console.WriteLine("Gib viele Zeichen ein!")
  Call Console.WriteLine("gefolgt von Enter:")
  s = Console.ReadLine
  Call Console.WriteLine("Du hast folgendes eingegeben: ")
  Call Console.WriteLine(s)
  Call Console.WriteLine("Starte cmd.exe")
  Dim cmd As String
  Dim rv As Double
  cmd = "cmd.exe"
  rv = Shell(cmd, vbNormalFocus)
  s = Environ("tmp")
  Call Console.WriteLine("Wechsle in des Verzeichnis: ")
  Call Console.WriteLine(s)
  Call ChDir(s)
  cmd = "cmd.exe"
  rv = Shell(cmd, vbNormalFocus)
  Do While StrComp(UCase$(s), "RAUS") <> 0
    s = Console.ReadLine
    If StrComp(s, "dir") = 0 Then
        cmd = "cmd.exe" & " dir"
        rv = Shell(cmd, vbNormalFocus)
        Debug.Print cmd
        cmd = vbNullString
    End If
  Loop
End Sub

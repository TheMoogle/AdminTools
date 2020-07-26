Imports System
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace AdminTools
    Public Module Extensions
        <Extension()>
        Public Sub InvokeStaticMethod(ByVal type As Type, ByVal methodName As String, ByVal param As Object())
            Dim flags = BindingFlags.Instance Or BindingFlags.InvokeMethod Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Public
            Dim info = type.GetMethod(methodName, flags)
            info?.Invoke(Nothing, param)
        End Sub
    End Module
End Namespace

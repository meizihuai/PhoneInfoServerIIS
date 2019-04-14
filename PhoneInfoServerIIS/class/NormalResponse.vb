Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Public Class NormalResponse
    Public result As Boolean
    Public msg As String
    Public errmsg As String
    Public data As Object

    Public Sub New()
    End Sub

    Public Sub New(ByVal result As Boolean, ByVal msg As String, ByVal errmsg As String, ByVal data As Object)
        Me.result = result
        Me.msg = msg
        Me.errmsg = errmsg
        Me.data = data
    End Sub

    Public Sub New(ByVal result As Boolean, ByVal msg As String)
        Me.result = result
        Me.msg = msg
        Me.errmsg = ""
        Me.data = ""
    End Sub

    Public Sub New(ByVal msg As String)
        Me.result = False

        If msg = "success" Then
            Me.result = True
        End If

        Me.msg = msg
        Me.errmsg = ""
        Me.data = ""
    End Sub
End Class

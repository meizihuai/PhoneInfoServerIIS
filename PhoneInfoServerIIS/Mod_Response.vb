Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Sockets
Module Mod_Response
    Structure PostStu
        Dim func As String
        Dim data As Object
    End Structure

    Public Sub myResponse(ByVal context As HttpContext, ByVal np As NormalResponse)
        Try
            If IsNothing(context) Then Return
            If IsNothing(np) Then Return
            myResponse(context, JsonConvert.SerializeObject(np))
        Catch ex As Exception
            'MsgBox(ex.ToString)
        End Try
    End Sub
    Public Sub myResponse(ByVal context As HttpContext, ByVal msg As String)
        If IsNothing(context) Then Return
        If context.Response.IsClientConnected = False Then Return
        Try
            context.Response.StatusCode = 200
            '  context.Response.Headers.Add("Access-Control-Allow-Origin", "*")
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
            context.Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with,Content-Type")

            'context.Response.AddHeader("Access-Control-Allow-Origin", "*")
            'context.Response.Write(msg)
            'context.Response.End()

            Dim w As New StreamWriter(context.Response.OutputStream, Encoding.UTF8)
            w.Write(msg)
            w.Close()

        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Sub
End Module

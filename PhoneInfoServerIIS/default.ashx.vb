Imports System.Web
Imports System.Web.Services
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Math
Imports System.Net.Sockets
Imports System.Net
Imports System.Net.HttpListener
Imports System.Data
Imports System.Threading
Imports System.Threading.Thread
Imports System
Imports System.Int32
Imports System.BitConverter
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Xml
Imports System.Reflection
Public Class _default1
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        HandleHTTP(context)
        'context.Response.ContentType = "text/plain"
        'context.Response.Write("Hello World!")

    End Sub
    Private Sub HandleHTTP(ByVal context As HttpContext)
        Dim method As String = Context.Request.HttpMethod.ToLower
        If method.ToLower = "post" Then
            HandlePost(context) '处理post请求
            Return
        End If
        If method.ToLower = "get" Then
            Dim func As String = context.Request.QueryString("func")
            Dim token As String = context.Request.QueryString("token")
            If isNeedCheckToken And NeedCheckTokenFuncs.Contains(func) Then
                If IsNothing(token) Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
                If token = "" Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
                If CheckToken(token) = False Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
            End If
            Try
                Dim n As New HTTPHandle()
                Dim t As Type = n.GetType()
                Dim obj As Object = Activator.CreateInstance(t)
                Dim mf As MethodInfo = t.GetMethod("Handle_" & func)
                Dim ok As Object = mf.Invoke(obj, New Object() {context})
                If IsNothing(ok) Then
                    myResponse(context, New NormalResponse(False, "您的请求GET func出现未预料的错误"))
                    Return
                End If
                Dim np As NormalResponse = CType(ok, NormalResponse)
                If IsNothing(np) = False Then
                    myResponse(context, np)
                    Return
                End If
            Catch ex As Exception
                myResponse(context, New NormalResponse(False, "您的请求GET func出现了错误,func=" & func & "," & ex.Message, ex.ToString, context.Request.Url.PathAndQuery))
                Return
            End Try
            myResponse(context, New NormalResponse(False, "您的请求GET func不在受理范围内,func=" & func ,"", context.Request.Url.PathAndQuery))
                Return
        End If
        myResponse(context, New NormalResponse(False, "您的请求不在受理范围内", "", context.Request.Url.PathAndQuery))
    End Sub


    Sub HandlePost(ByVal context As HttpContext)
       
        Dim sr As New StreamReader(context.Request.InputStream, Encoding.UTF8)
        Dim body As String = sr.ReadToEnd
        Dim func As String = ""
        Try
            Dim ps As PostStu = JsonConvert.DeserializeObject(body, GetType(PostStu))
            func = ps.func
            Dim token As String = ps.token
            If isNeedCheckToken And NeedCheckTokenFuncs.Contains(func) Then
                If IsNothing(token) Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
                If token = "" Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
                If CheckToken(token) = False Then
                    myResponse(context, New NormalResponse(False, "token无效"))
                    Return
                End If
            End If
            Try
                Dim n As New HTTPHandle()
                Dim t As Type = n.GetType()
                Dim obj As Object = Activator.CreateInstance(t)
                Dim mf As MethodInfo = t.GetMethod("Handle_" & func)
                Dim ok As Object = mf.Invoke(obj, New Object() {context, ps.data, ps.token})
                Dim np As NormalResponse = CType(ok, NormalResponse)
                If IsNothing(np) = False Then
                    myResponse(context, np)
                    Return
                End If
            Catch ex As Exception
                'myResponse(context, New NormalResponse(False, ex.ToString, "", context.Request.Url.PathAndQuery))
                'Return
            End Try
        Catch ex As Exception

        End Try
        myResponse(context, New NormalResponse(False, "您的请求POST func不在受理范围内,func=" & func, "", context.Request.Url.PathAndQuery))
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
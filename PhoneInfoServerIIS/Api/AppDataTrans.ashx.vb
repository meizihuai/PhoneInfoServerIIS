Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class AppDataTrans
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        HandleHttp(context)
    End Sub
    Structure AppDataTransInfo
        Dim type As String
        Dim content As String
        Dim fileName As String
    End Structure
    Private Sub HandleHttp(context As HttpContext)
        Dim method As String = context.Request.HttpMethod.ToLower
        If method <> "post" Then
            myResponse(context, New NormalResponse(False, "不支持get请求"))
            Return
        End If
        HandlePost(context)
    End Sub
    Private Sub HandlePost(ByVal context As HttpContext)
        Dim sr As New StreamReader(context.Request.InputStream, Encoding.UTF8)
        Dim body As String = sr.ReadToEnd
        Dim func As String = ""
        Try
            Dim ps As PostStu = JsonConvert.DeserializeObject(body, GetType(PostStu))
            func = ps.func
            Dim data As String = ps.data.ToString
            If func = "UploadAppDataTrans" Then
                HandleAppDataTrans(context, data)
                Return
            End If
        Catch ex As Exception

        End Try
        myResponse(context, New NormalResponse(False, "您的请求POST func不在受理范围内", "", context.Request.Url.PathAndQuery))
    End Sub
    Private Sub HandleAppDataTrans(context As HttpContext, data As String)
        Dim apd As AppDataTransInfo
        Try
            apd = JsonConvert.DeserializeObject(data, GetType(AppDataTransInfo))
        Catch ex As Exception
            myResponse(context, New NormalResponse(False, "AppDataTransInfo格式非法"))
            Return
        End Try
        If apd.type = "file" Then
            Dim json As String = ""
            Dim by() As Byte = Convert.FromBase64String(apd.content)
            by = Decompress(by)
            json = Encoding.Default.GetString(by)
            Dim virtualPath As String = "AppDataTrans"
            Dim rootPath As String = System.Web.HttpContext.Current.Server.MapPath("~/" & virtualPath & "/")
            Dim filePath As String = rootPath & apd.fileName
            If File.Exists(filePath) Then File.Delete(filePath)
            File.WriteAllText(filePath, json)
            myResponse(context, New NormalResponse(True, "处理成功"))
            Return
        End If
        myResponse(context, New NormalResponse(False, "您的请求AppDataTransInfo type不在受理范围内", "", context.Request.Url.PathAndQuery))
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
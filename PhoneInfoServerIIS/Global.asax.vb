Imports System.Web.SessionState

Public Class Global_asax
    Inherits System.Web.HttpApplication



    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' 应用程序启动时激发

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' 会话启动时激发
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' 每个请求开始时激发
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' 尝试验证用户身份时激发
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' 发生错误时激发
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' 会话结束时激发
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' 应用程序结束时激发
    End Sub

End Class
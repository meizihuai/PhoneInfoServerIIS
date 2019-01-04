Public Class NormalResponse
    Public result As Boolean
    Public msg As String
    Public errmsg As String
    Public data As Object
    Sub New(ByVal _result As Boolean, ByVal _msg As String, ByVal _errmsg As String, ByVal _data As Object) '基本构造函数() As _errmsg,string
        result = _result
        msg = _msg
        errmsg = _errmsg
        data = _data
    End Sub
    Sub New(ByVal _result As Boolean, ByVal _msg As String) '重载构造函数，为了方便写new,很多时候，只需要一个结果和一个参数() As _result,string
        result = _result
        msg = _msg
        errmsg = ""
        data = ""
    End Sub
End Class

Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Npgsql
Public Class PgSQLHelper
    Private ConnectSQL As String = ""
    Sub New(ip As String, port As Integer, databaseName As String, usr As String, pwd As String)
        ConnectSQL = "Server={0};port={1};Database={2};User Id={3};Password={4};timeout=1024;"
        ConnectSQL = String.Format(ConnectSQL, ip, port, databaseName, usr, pwd)
    End Sub
    Public Function SQLGetDT(ByVal CmdString As String) As DataTable
        Dim dt As New DataTable
        Try
            Dim SQL As New NpgsqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As NpgsqlCommand = New NpgsqlCommand(CmdString, SQL)
            SQLCommand.CommandTimeout = 900000
            Dim SQLDataAdapter As New NpgsqlDataAdapter(SQLCommand)
            SQLDataAdapter.Fill(dt)
            SQL.Close()
            If IsNothing(dt) Then dt = New DataTable
            Return dt
        Catch ex As Exception
            File.WriteAllText("d:\err.txt", ex.ToString)
            '  MsgBox(ex.ToString)
            Return dt
        End Try
    End Function
    Public Function SQLCmd(ByVal CmdString As String) As String
        Try
            Dim SQL As New NpgsqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As NpgsqlCommand = New NpgsqlCommand(CmdString, SQL)

            Dim ResultRowInt As Integer = SQLCommand.ExecuteNonQuery()
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function SQLCMDList(ByVal sqllist As List(Of String)) As String
        If IsNothing(sqllist) Then Return "sqllist is null"
        If sqllist.Count = 0 Then Return "sqllist.count=0"
        Try
            Dim SQL As New NpgsqlConnection(ConnectSQL)
            SQL.Open()
            Dim tx As NpgsqlTransaction = SQL.BeginTransaction
            Dim SQLCommand As NpgsqlCommand = New NpgsqlCommand()
            SQLCommand.Connection = SQL
            SQLCommand.Transaction = tx
            Dim count As Integer = sqllist.Count
            For i = 0 To count - 1
                SQLCommand.CommandText = sqllist(i)
                SQLCommand.ExecuteNonQuery()
                If i > 0 And (i Mod 1000 = 0 Or i = count - 1) Then
                    tx.Commit()
                    tx = SQL.BeginTransaction
                End If
            Next
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function SQLCMDList(ByVal connString As String, ByVal sqllist As List(Of String)) As String
        If IsNothing(sqllist) Then Return "sqllist is null"
        If sqllist.Count = 0 Then Return "sqllist.count=0"
        Try
            Dim SQL As New NpgsqlConnection(connString)
            SQL.Open()
            Dim tx As NpgsqlTransaction = SQL.BeginTransaction
            Dim SQLCommand As NpgsqlCommand = New NpgsqlCommand()
            SQLCommand.Connection = SQL
            SQLCommand.Transaction = tx
            Dim count As Integer = sqllist.Count
            For i = 0 To count - 1
                SQLCommand.CommandText = sqllist(i)
                SQLCommand.ExecuteNonQuery()
                If i > 0 And (i Mod 500 = 0 Or i = count - 1) Then
                    tx.Commit()
                    tx = SQL.BeginTransaction
                End If
            Next
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function SQLInfo(ByVal CmdString As String) As String
        Try
            Dim SQL As New NpgsqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As NpgsqlCommand = New NpgsqlCommand(CmdString, SQL)
            Dim str As String = SQLCommand.ExecuteScalar.ToString
            SQL.Close()
            Return str
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function DT2SQL(ByVal dataTableName As String, ByVal dt As DataTable) As String
        Dim errSql As String = ""
        Try
            If IsNothing(dt) Then Return "dt=null"
            Dim keys As String = ""
            Dim values As String = ""
            For Each c As DataColumn In dt.Columns
                keys = keys & c.ColumnName & ","
            Next
            keys = keys.Substring(0, keys.Length - 1)
            Dim errMsgList As New List(Of String)
            Dim sql As String = "insert into " & dataTableName & "(" & keys & ") values ({0})"
            Dim sqlList As New List(Of String)
            Dim isFirst As Boolean = True
            For Each row As DataRow In dt.Rows
                values = ""
                For i = 0 To dt.Columns.Count - 1
                    Dim v As String = row(i).ToString
                    v = v.Replace("'", Chr(34))
                    values = values & "'" & v & "',"
                Next
                values = values.Substring(0, values.Length - 1)
                Dim newsql As String = String.Format(sql, values)
                sqlList.Add(newsql)
                If isFirst Then
                    errSql = newsql
                    isFirst = False
                End If
            Next
            dt = Nothing
            Dim result As String = SQLCMDList(sqlList)
            If result = "success" Then
                Return result
            Else
                Return result & vbCrLf & errSql
            End If
            '   Return SQLCMDList(sqlList)
            'If failCount = 0 Then
            '    Return "success"
            'Else
            '    Return "成功数量：" & successCount & ",失败数量：" & failCount & "," & JsonConvert.SerializeObject(errMsgList)
            'End If

        Catch ex As Exception
            dt = Nothing
            Return ex.Message & vbCrLf & errSql
        End Try
    End Function
End Class

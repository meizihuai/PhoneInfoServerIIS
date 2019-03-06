Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports System.Net
Imports System.IO.Compression
Imports System.Data
Imports Oracle
Imports Oracle.ManagedDataAccess
Imports Oracle.ManagedDataAccess.Client
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class OracleHelper
    Dim NKConnectString As String = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=111.53.74.132)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=oss)));Persist Security Info=True;User ID=npo;Password=Smart9080;"
    Sub New(ip As String, port As Integer, seviceName As String, usr As String, pwd As String)
        NKConnectString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));Persist Security Info=True;User ID={3};Password={4};"
        NKConnectString = String.Format(NKConnectString, New String() {ip, port, seviceName, usr, pwd})
    End Sub
    Structure OracleParaList
            Dim dataList As List(Of String)
        End Structure
    Public Function SqlCMDListQuickByPara(databaseName As String, dt As DataTable) As String
        Return SubSqlCMDListQuickByPara(databaseName, dt)
    End Function
    Private Function SubSqlCMDListQuickByPara(databaseName As String, ByVal dt As DataTable) As String
        If IsNothing(dt) Then Return "dt=null"
        Dim conn As New OracleConnection(NKConnectString)
        Try
            conn.Open()
            Dim sql As String = "insert into " & databaseName & " ({0}) values ({1})"
            Dim measTypes As String = ""
            Dim measValues As String = ""
            For Each c As DataColumn In dt.Columns
                measTypes = measTypes & c.ColumnName & ","
                measValues = measValues & ":" & c.ColumnName & ","
            Next
            measTypes = measTypes.Substring(0, measTypes.Length - 1)
            measValues = measValues.Substring(0, measValues.Length - 1)
            sql = String.Format(sql, measTypes, measValues)
            Dim sumCount As Integer = dt.Rows.Count
            Dim readIndex As Integer = 0
            While True
                Dim perCount As Integer = 500
                Dim restCount As Integer = sumCount - readIndex
                If perCount > restCount Then
                    perCount = restCount
                End If
                Dim cmd As New OracleCommand()
                cmd.Connection = conn
                cmd.ArrayBindCount = perCount
                cmd.CommandText = sql
                Dim arrayCount As Integer = perCount
                Dim fatherList As New List(Of OracleParaList)
                For i = 0 To dt.Columns.Count - 1
                    Dim itm As New OracleParaList
                    itm.dataList = New List(Of String)
                    For j = readIndex To perCount + readIndex - 1
                        Dim itmRow As DataRow = dt.Rows(j)
                        Dim valueItm As String = itmRow(i).ToString
                        itm.dataList.Add(valueItm)
                    Next
                    fatherList.Add(itm)
                Next
                For i = 0 To dt.Columns.Count - 1
                    Dim PARM As New OracleParameter(dt.Columns(i).ColumnName, OracleDbType.Varchar2)
                    PARM.Direction = ParameterDirection.Input
                    PARM.Value = fatherList(i).dataList.ToArray
                    cmd.Parameters.Add(PARM)
                Next
                cmd.ExecuteNonQuery()
                readIndex = readIndex + perCount
                If readIndex >= sumCount Then
                    Exit While
                End If
            End While
            conn.Close()
            Return "success"
        Catch ex As Exception
            ' MsgBox(ex.ToString)
            'File.WriteAllText("d:\oraerr.txt", ex.ToString)
            conn.Close()
            Return ex.ToString
        End Try
    End Function
    Public Function SQLGetFirstRowCell(sql As String) As String
        Try
            Dim dt As DataTable = SqlGetDT(sql)
            If IsNothing(dt) Then Return ""
            If dt.Rows.Count = 0 Then Return ""
            If IsNothing(dt.Rows(0)(0)) Then Return ""
            If IsDBNull(dt.Rows(0)(0)) Then Return ""
            Return dt.Rows(0)(0)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function SQLInfo(ByVal CmdString As String) As String


        Dim SQL As New OracleConnection(NKConnectString)
        Try
            SQL.Open()
            Dim SQLCommand As OracleCommand = New OracleCommand(CmdString, SQL)
            Dim obj As Object = SQLCommand.ExecuteScalar
            Dim str As String = "success"
            If IsNothing(obj) = False Then
                If IsDBNull(obj) = False Then
                    str = obj.ToString
                End If
            End If
            SQL.Close()
            Return str
        Catch ex As Exception
            SQL.Close()
            Return ex.Message
        End Try
    End Function
    Public Function SqlCMD(ByVal sql As String) As String
        Dim conn As New OracleConnection(NKConnectString)
        Try

            conn.Open()
            Dim cmd As New OracleCommand(sql, conn)
            cmd.CommandTimeout = 99999999
            cmd.ExecuteNonQuery()
            conn.Close()
            Return "success"
        Catch ex As Exception
            ' File.WriteAllText("d:\oraerrCmd.txt", ex.ToString & vbCrLf & sql)
            conn.Close()

            Return ex.Message
        End Try
    End Function
    Public Function SqlCMDList(ByVal sqlList As List(Of String)) As String
        If IsNothing(sqlList) Then Return "sqlList=null"
        Dim conn As New OracleConnection(NKConnectString)
        Try
            conn.Open()
            Dim str As String = ""
            For Each sq In sqlList
                str = str & sq & ";"
            Next
            str = "begin" & vbCrLf & str & " " & vbCrLf & "end;"
            Dim cmd As New OracleCommand(str, conn)
            cmd.CommandTimeout = 99999999
            cmd.ExecuteNonQuery()
            conn.Close()
            Return "success"
        Catch ex As Exception
            conn.Close()
            Return ex.ToString
        End Try
    End Function

    Public Function SqlGetDT(ByVal sql As String) As DataTable
        Dim dt As New DataTable
        Dim conn As New OracleConnection(NKConnectString)
        Try
            conn.Open()
            Dim cmd As New OracleCommand(sql, conn)
            cmd.CommandTimeout = 99999999
            Dim sda As New OracleDataAdapter(cmd)
            sda.Fill(dt)
            conn.Close()
            Return dt
        Catch ex As Exception
            File.WriteAllText("d:\oraerrGet.txt", ex.ToString & vbCrLf & sql)
            conn.Close()
            Return dt
        End Try
    End Function
    Public Function SqlIsIn(ByVal sql As String) As Boolean
        Dim dt As DataTable = SqlGetDT(sql)
        If IsNothing(dt) Then Return False
        If dt.Rows.Count = 0 Then Return False
        Return True
    End Function

    Public Function GetOraTableColumns(tableName As String) As String()
        Try
            Dim sql As String = "select COLUMN_NAME from user_tab_columns where table_name ='" & tableName.ToUpper & "'"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return Nothing
            If dt.Rows.Count = 0 Then Return Nothing
            Dim list As New List(Of String)
            For Each row As DataRow In dt.Rows
                If IsNothing(row(0)) = False Then
                    list.Add(row(0).ToString)
                End If
            Next
            Return list.ToArray
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function GetOraTableColumnsOnDt(tableName As String, Optional isRemoveId As Boolean = True) As DataTable
        Try
            Dim cols() As String = GetOraTableColumns(tableName)
            If IsNothing(cols) Then Return Nothing
            If cols.Length = 0 Then Return Nothing
            Dim dt As New DataTable
            For Each col In cols
                If isRemoveId Then
                    If col = "ID" Then
                        Continue For
                    End If
                End If
                dt.Columns.Add(col)
            Next
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class

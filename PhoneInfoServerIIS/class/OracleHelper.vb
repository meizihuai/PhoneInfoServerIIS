Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Oracle
Imports Oracle.ManagedDataAccess
Imports Oracle.ManagedDataAccess.Client
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.IO

Public Class OracleHelper
    Private NKConnectString As String = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=111.53.74.132)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=oss)));Persist Security Info=True;User ID=npo;Password=Smart9080;"

    Public Sub New(ByVal ip As String, ByVal port As Integer, ByVal seviceName As String, ByVal usr As String, ByVal pwd As String)
        NKConnectString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));Persist Security Info=True;User ID={3};Password={4};"
        NKConnectString = String.Format(NKConnectString, New String() {ip, port & "", seviceName, usr, pwd})
    End Sub

    Structure OracleParaList
        Public dataList As List(Of String)
    End Structure

    Public Function SqlCMDListQuickByPara(ByVal databaseName As String, ByVal dt As DataTable) As String
        Return SubSqlCMDListQuickByPara(databaseName, dt)
    End Function

    Private Function SubSqlCMDListQuickByPara(ByVal databaseName As String, ByVal dt As DataTable) As String
        If dt Is Nothing Then Return "dt=null"

        Using conn As OracleConnection = New OracleConnection(NKConnectString)

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
                    If perCount > restCount Then perCount = restCount
                    Dim cmd As OracleCommand = New OracleCommand()
                    cmd.Connection = conn
                    cmd.ArrayBindCount = perCount
                    cmd.CommandText = sql
                    Dim arrayCount As Integer = perCount
                    Dim fatherList As List(Of OracleParaList) = New List(Of OracleParaList)()

                    For i = 0 To dt.Columns.Count - 1
                        Dim itm As OracleParaList = New OracleParaList()
                        itm.dataList = New List(Of String)()

                        For j = readIndex To perCount + readIndex - 1
                            Dim itmRow As DataRow = dt.Rows(j)
                            Dim valueItm As String = itmRow(i).ToString()
                            itm.dataList.Add(valueItm)
                        Next

                        fatherList.Add(itm)
                    Next

                    For i As Integer = 0 To dt.Columns.Count - 1
                        Dim PARM As OracleParameter = New OracleParameter(dt.Columns(i).ColumnName, OracleDbType.Varchar2)
                        PARM.Direction = ParameterDirection.Input
                        PARM.Value = fatherList(i).dataList.ToArray()
                        cmd.Parameters.Add(PARM)
                    Next

                    cmd.ExecuteNonQuery()
                    readIndex = readIndex + perCount
                    If readIndex >= sumCount Then Exit While
                End While

                conn.Close()
                Return "success"
            Catch ex As Exception
                conn.Close()
                Return ex.ToString()
            End Try
        End Using
    End Function

    Public Function SQLGetFirstRowCell(ByVal sql As String) As String
        Try
            Dim dt As DataTable = SqlGetDT(sql)
            If dt Is Nothing Then Return ""
            If dt.Rows.Count = 0 Then Return ""
            Dim str As String = dt.Rows(0)(0).ToString()
            If str Is Nothing Then Return ""
            If str = DBNull.Value.ToString() Then Return ""
            Return str
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function SQLInfo(ByVal CmdString As String) As String
        Using conn As OracleConnection = New OracleConnection(NKConnectString)

            Try
                conn.Open()
                Dim SQLCommand As OracleCommand = New OracleCommand(CmdString, conn)
                Dim obj As Object = SQLCommand.ExecuteScalar()
                Dim str As String = ""

                If obj IsNot Nothing AndAlso Not (TypeOf obj Is DBNull) Then
                    str = obj.ToString()
                End If

                conn.Close()
                Return str
            Catch ex As Exception
                conn.Close()
                Return ex.Message
            End Try
        End Using
    End Function

    Public Function SqlCMD(ByVal sql As String) As String
        Using conn As OracleConnection = New OracleConnection(NKConnectString)

            Try
                conn.Open()
                Dim cmd As OracleCommand = New OracleCommand(sql, conn)
                cmd.CommandTimeout = 99999999
                cmd.ExecuteNonQuery()
                conn.Close()
                Return "success"
            Catch ex As Exception
                conn.Close()
                Return ex.Message
            End Try
        End Using
    End Function

    Public Function SqlCMDList(ByVal sqlList As List(Of String)) As String
        If sqlList Is Nothing Then Return "sqlList=null"

        Using conn As OracleConnection = New OracleConnection(NKConnectString)

            Try
                conn.Open()
                Dim str As String = ""

                For Each sq In sqlList
                    str = str & sq & ";"
                Next

                str = "begin" & Environment.NewLine & str & " " & Environment.NewLine & "end;"
                Dim cmd As OracleCommand = New OracleCommand(str, conn)
                cmd.CommandTimeout = 99999999
                cmd.ExecuteNonQuery()
                conn.Close()
                Return "success"
            Catch ex As Exception
                conn.Close()
                Return ex.ToString()
            End Try
        End Using
    End Function

    Public Function SqlGetDT(ByVal sql As String) As DataTable
        Dim dt As DataTable = New DataTable()

        Using conn As OracleConnection = New OracleConnection(NKConnectString)

            Try
                conn.Open()
                Dim cmd As OracleCommand = New OracleCommand(sql, conn)
                cmd.CommandTimeout = 99999999
                Dim sda As OracleDataAdapter = New OracleDataAdapter(cmd)
                sda.Fill(dt)
                conn.Close()
                Return dt
            Catch ex As Exception
                conn.Close()
                Return dt
            End Try
        End Using
    End Function

    Public Function SqlIsIn(ByVal sql As String) As Boolean
        Dim dt As DataTable = SqlGetDT(sql)
        If dt Is Nothing Then Return False
        If dt.Rows.Count = 0 Then Return False
        Return True
    End Function

    Public Function GetOraTableColumns(ByVal tableName As String) As String()
        Try
            Dim sql As String = "select COLUMN_NAME from user_tab_columns where table_name ='" & tableName.ToUpper() & "'"
            Dim dt As DataTable = SqlGetDT(sql)
            If dt Is Nothing Then Return Nothing
            If dt.Rows.Count = 0 Then Return Nothing
            Dim list As List(Of String) = New List(Of String)()

            For Each row As DataRow In dt.Rows
                If row(0) IsNot Nothing Then list.Add(row(0).ToString())
            Next

            Return list.ToArray()
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetOraTableColumnsOnDt(ByVal tableName As String, ByVal Optional isRemoveId As Boolean = True) As DataTable
        Try
            Dim cols As String() = GetOraTableColumns(tableName)
            If cols IsNot Nothing Then Return Nothing
            If cols.Length = 0 Then Return Nothing
            Dim dt As DataTable = New DataTable()

            For Each col In cols

                If isRemoveId Then
                    If col = "ID" Then Continue For
                End If

                dt.Columns.Add(col)
            Next

            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function InsertByDik(ByVal tableName As String, ByVal dik As Dictionary(Of String, Object)) As NormalResponse
        Try

            If dik Is Nothing Then
                Return New NormalResponse(False, "写入数据库字段为空")
            End If

            Dim sql As String = "insert into " & tableName & "({0}) values ({1})"
            Dim keys As String = ""

            For Each itm As String In dik.Keys
                keys = keys & "," & itm
            Next

            keys = keys.Substring(1, keys.Length - 1)
            Dim values As String = ""

            For Each itm As Object In dik.Values
                Dim tmp As String = ""

                If itm IsNot Nothing Then
                    tmp = itm.ToString()
                End If

                values = values & ",'" & tmp & "'"
            Next

            values = values.Substring(1, values.Length - 1)
            sql = String.Format(sql, keys, values)
            Dim result As String = SqlCMD(sql)

            If result = "success" Then
                Return New NormalResponse(True, result)
            Else
                Return New NormalResponse(False, result, "", sql)
            End If

        Catch e As Exception
            Return New NormalResponse(False, e.ToString())
        End Try
    End Function

    Public Function UpdateByDik(ByVal tableName As String, ByVal dik As Dictionary(Of String, Object), ByVal id As Integer, ByVal Optional where As String = "") As NormalResponse
        Try
            Dim sql As String = ""

            If id > 0 Then
                sql = "update " & tableName & " set " & "{0} where id=" & id & (If(where = "", "", " and " & where))
            Else
                sql = "update " & tableName & " set " & "{0} " & (If(where = "", "", " where " & where))
            End If

            Dim sets As String = ""

            For Each itm In dik
                Dim tmp As String = ""

                If itm.Value IsNot Nothing Then
                    tmp = itm.Value.ToString()
                End If

                sets = sets & "," & itm.Key.ToString() & "='" & tmp & "'"
            Next

            sets = sets.Substring(1, sets.Length - 1)
            sql = String.Format(sql, sets)
            Dim result As String = SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, result)
            Else
                Return New NormalResponse(False, result, "", sql)
            End If
        Catch e As Exception
            ' File.WriteAllText("d:\454656.txt", e.ToString())
            Return New NormalResponse(False, e.ToString())
        End Try
    End Function
End Class


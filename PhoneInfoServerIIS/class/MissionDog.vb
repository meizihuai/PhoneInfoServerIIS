Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Sockets
Imports System.Web
Imports System.Threading
Imports System.Threading.Thread
Public Class MissionDog
    Private Shared mThread As Thread
    Private Shared sleepSecond As Integer = 5
    '说明 app_mission_table isClosed =-2(新建)  -1(未开启)  0(正常)  1(结束）
    Public Shared Sub StartWatching()
        StopWatching()
        Try
            mThread = New Thread(AddressOf Watching)
            mThread.Start()
        Catch ex As Exception

        End Try
    End Sub
    Public Shared Sub StopWatching()
        If IsNothing(mThread) = False Then
            Try
                mThread.Abort()
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Shared Sub Watching()
        While True
            Try
                WatchingWork()
            Catch ex As Exception

            End Try
            Sleep(1000 * sleepSecond)
        End While
    End Sub
    Private Shared Sub WatchingWork()
        Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim lastDateTime As String = Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "update app_mission_table set isClosed=1,status='结束' where endTime<='" & time & "' and isClosed=0"
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='未开启',isClosed=-1 where  startTime>'" & time & "'"
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='正常',isClosed=0 where startTime<='" & time & "' and endTime>='" & time & "'"
        ORALocalhost.SqlCMD(sql)
        sql = "select imei from app_mission_table where isClosed=0"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) = False Then
            If dt.Rows.Count > 0 Then
                Dim imeiList As New List(Of String)
                For Each row As DataRow In dt.Rows
                    Dim imei As String = row("IMEI").ToString
                    If IsNothing(imei) = False Then
                        If imei <> "" Then
                            imeiList.Add(imei)
                        End If
                    End If
                Next
                If imeiList.Count > 0 Then
                    ''将设备库更改设备是否繁忙 isbusy
                    sql = "select imei from deviceTable"
                    Dim imeiDt As DataTable = ORALocalhost.SqlGetDT(sql)
                    If IsNothing(imeiDt) = False Then
                        If imeiDt.Rows.Count > 0 Then
                            For Each row In imeiDt.Rows
                                Dim imei As String = row("IMEI").ToString
                                If imeiList.Contains(imei) Then
                                    sql = "update deviceTable set isBusy=1 where imei='" & imei & "' and isBusy=0"
                                    ORALocalhost.SqlCMD(sql)
                                Else
                                    sql = "update deviceTable set isBusy=0 where imei='" & imei & "' and isBusy=1"
                                    ORALocalhost.SqlCMD(sql)
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        End If
        '监控数据
        sql = "update app_mission_table set lastDateTime=dateTime where lastDateTime is null"
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='异常' where isClosed=0 and lastDateTime<='" & lastDateTime & "'"
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='正常' where isClosed=0 and lastDateTime>'" & lastDateTime & "'"
        ORALocalhost.SqlCMD(sql)

    End Sub
End Class

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
Imports System.Web
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.IO.Compression

Public Class GetNewPointByPhoneModelHelper
    Public phoneModel As String
    Private context As HttpContext
    Sub New(phoneModel As String, context As HttpContext)
        Me.phoneModel = phoneModel
        Me.context = context
    End Sub
    Structure PIInfo
        Dim Time As String
        Dim phoneModel As String
        Dim RSRP As Double
        Dim lon As Double
        Dim lat As Double
    End Structure
    Public Sub Reply(RSRP As Double, time As String, lon As Double, lat As Double)
        Dim th As New Thread(Sub()
                                 Try
                                     If IsNothing(context) Then Return
                                     If context.Response.IsClientConnected = False Then Return
                                     Dim pi As New PIInfo
                                     pi.Time = time
                                     pi.phoneModel = phoneModel
                                     pi.RSRP = RSRP
                                     pi.lon = lon
                                     pi.lat = lat
                                     Dim np As New NormalResponse(True, "", "", pi)
                                     myResponse(context, np)
                                 Catch ex As Exception

                                 End Try
                             End Sub)
        th.Start()
    End Sub
End Class

﻿
Public Class PhoneInfo
    Public AID As String
    Public RID As String
    Public DATETIME As String
    Public businessType As String
    Public apkName As String
    Public phoneModel As String
    Public phoneName As String
    Public phoneOS As String
    Public phonePRODUCT As String
    Public carrier As String
    Public IMSI As String
    Public IMEI As String
    Public RSRP As Double
    Public SINR As Double
    Public RSRQ As Double
    Public TAC As Integer
    Public PCI As Integer
    Public EARFCN As Integer

    Public CI As Integer
    Public eNodeBId As Integer
    Public cellId As Integer
    Public netType As String
    Public sigNalType As String
    Public sigNalInfo As String
    Public lon As Double
    Public lat As Double
    Public accuracy As Double
    Public altitude As Double
    Public gpsSpeed As Double
    Public satelliteCount As Integer
    Public address As String
    Public apkVersion As String
    Public ISUPLOADDATATIMELY As Integer

    Public MNC As Integer
    Public wifi_SSID As String
    Public wifi_MAC As String
    Public PING_AVG_RTT As Single
    Public FREQ As Double
    Public cpu As String
    Public ADJ_SIGNAL As String
    Public neighbourList As List(Of Neighbour)
    Public Adj_ECI1 As Integer
    Public Adj_RSRP1 As Integer
    Public Adj_SINR1 As Integer
    Public isScreenOn As Integer
    Public isGPSOpen As Integer
    Public PHONE_ELECTRIC As Integer
    Public PHONE_SCREEN_BRIGHTNESS As Integer
    Public xyZaSpeed As XYZaSpeedInfo

    Public province As String
    Public city As String
    Public district As String
    Public DetailAddress As String
    Public bdlon As Double
    Public bdlat As Double
    Public gdlon As Double
    Public gdlat As Double

    Public VMOS As Integer
    Public HTTP_URL As String
    Public HTTP_RESPONSE_TIME As Long
    Public HTTP_BUFFERSIZE As Long
End Class

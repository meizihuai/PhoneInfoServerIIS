Public Class QoEVideoInfo
    Public DATETIME As String    '/*数据的入库时间*/
    Public NET_TYPE As String    'netType
    Public BUSINESS_TYPE As String    '业务类型，Ex：流媒体、直播、FTP
    Public VIDEO_BUFFER_INIT_TIME As Long    '初始缓冲时延
    Public VIDEO_BUFFER_TOTAL_TIME As Long    '视频总的下载时长
    Public VIDEO_STALL_NUM As Integer    '卡顿次数
    Public VIDEO_STALL_TOTAL_TIME As Long    '卡顿总时长
    Public VIDEO_STALL_DURATION_PROPORTION As Double    '卡顿时延占比
    Public VIDEO_LOAD_SCORE As Integer    '初始加载评分
    Public VIDEO_STALL_SCORE As Integer    '卡顿评分
    Public VIDEO_BUFFER_TOTAL_SCORE As Integer    '缓冲时延评分
    Public USER_SCORE As Integer    '用户分值
    Public VMOS As Integer    'VMOS
    Public PACKET_LOSS As Long    '丢包数
    Public ELOAD As Integer    '/*用户对视频播放等待时间的评分(5：无法察觉到缓冲，4：缓冲时间很短，3：缓冲时间长度一般，2：缓冲时间较长，1：缓冲时间过长无法容忍)*/
    Public ESTALL As Integer    '/*用户对流畅度的评分(5:毫无卡顿，4：略有卡顿但不影响观看，3：有卡顿对观看造成一定影响，2：有卡顿对观看造成较大影响，1：卡顿过多无法容忍)*/
    Public EVMOS As Integer    '/*用户对整体视频服务的综合评分(5:非常好，4：良好，3：一般，2：较差，1：无法容忍)*/
    Public ELIGHT As Integer    '/*环境光照对视频观看的影响程度(5：无影响，4：较小影响，3：有一定影响，2：较大影响，1：极大影响）*/
    Public ESTATE As Integer    '/*用户对运动状态的反馈(:4：静止不动，3：偶尔走动，2：持续走动，1：交通工具上)*/
    Public CARRIER As String    '/*运营商名称*/
    Public PLMN As Integer    '/*公共陆地移动网络*/
    Public MCC As Integer    '/*移动国家码*/
    Public MNC As Integer    '/*移动网络号码*/
    Public TAC As Integer    'tac
    Public ECI As Integer    'ECI
    Public ENODEBID As Integer    'enodebid
    Public CELLID As Integer    'cellid
    Public SIGNAL_STRENGTH As Integer    'RSRP
    Public SINR As Integer    'SINR
    Public PHONE_MODEL As String    '/*手机型号*/
    Public OPERATING_SYSTEM As String    '/*操作系统*/
    Public UDID As String    '/*移动设备国际身份码*/
    Public IMEI As String    'IMEI
    Public IMSI As String    '/*国际移动用户识别码*/
    Public USER_TEL As String    '用户号码
    Public PHONE_PLACE_STATE As Integer    '/*手机放置状态,1表示竖屏,2表示横屏*/
    Public COUNTRY As String    '/*国家/地区*/
    Public PROVINCE As String    '/*省份*/
    Public CITY As String    '/*城市*/
    Public ADDRESS As String    '/*地址*/
    Public PHONE_ELECTRIC_START As Integer    '/*开始播放时的手机电量百分比*/
    Public PHONE_ELECTRIC_END As Integer    '/*播放结束时的手机电量百分比*/
    Public SCREEN_RESOLUTION_LONG As Integer    '/*屏幕分辨率(长)*/
    Public SCREEN_RESOLUTION_WIDTH As Integer    '/*屏幕分辨率(宽)*/
    Public LIGHT_INTENSITY As Integer    '/*手机环境光照强度*/
    Public PHONE_SCREEN_BRIGHTNESS As Integer    '/*手机屏幕亮度*/
    Public HTTP_RESPONSE_TIME As Long    'http响应时间
    Public PING_AVG_RTT As Long    '/*Ping
    Public VIDEO_CLARITY As String    '视频清晰度
    Public VIDEO_CODING_FORMAT As String    '/*视频编码格式,如h.264*/
    Public VIDEO_BITRATE As Integer    '视频比特率
    Public FPS As Integer    '帧率
    Public VIDEO_TOTAL_TIME As Long    '视频总时长
    Public VIDEO_PLAY_TOTAL_TIME As Long    '/*视频播放时长=结束播放的时间点-点击播放的时间点(秒)*/
    Public VIDEO_PEAK_DOWNLOAD_SPEED As Long    '/*初始缓冲阶段的峰值速率，单位kb/s*/
    Public APP_PREPARED_TIME As Long    '手机UI加载播放器插件的准备工作时间
    Public BVRATE As Double    'BVRate
    Public STARTTIME As String    '/*视频开始播放的时间*/
    Public FILE_SIZE As Long    '文件大小
    Public FILE_NAME As String    '文件名称
    Public FILE_SERVER_LOCATION As String    '/*视频源服务器的实际地理位置*/
    Public FILE_SERVER_IP As String    '服务器IP
    Public UE_INTERNAL_IP As String    'UE
    Public ENVIRONMENTAL_NOISE As String    '环境噪声
    Public VIDEO_AVERAGE_PEAK_RATE As Long    '/*视频平均下载速率=总下载量/视频播放时长(kb/s)*/
    Public CELL_SIGNAL_STRENGTHList As List(Of Integer)    '按0.5s采集一次，保存后集中上报
    Public ACCELEROMETER_DATAList As List(Of XYZaSpeedInfo)  '/*重力感应数据=X/Y/Z轴的加速度
    Public INSTAN_DOWNLOAD_SPEEDList As List(Of Long)    '/*全程瞬时下载速率=每3s的下载量(kb)*/
    Public VIDEO_ALL_PEAK_RATEList As List(Of Long)   '/*全程阶段的峰值速率，下载量每秒（kb/s）*/
    Public GPSPointList As List(Of GPSPoint)    '/*GPS经度*/
    Public SIGNALList As List(Of String)   '信号汇总信息（按GPS的5个时间点来取）
    Public ADJList As List(Of ADJInfo)   '邻区ECELLID
    Public STALLlist As List(Of STALLInfo)    '卡顿信息
    Public ACCMIN As Integer    '最小接入电平
    Public USERSCENE As String    '/*用户场景*/
    Public MOVE_SPEED As Long    '手机移动速度
    Public ISPLAYCOMPLETED As Integer    '是否播放完成
    Public LOCALDATASAVETIME As String    '本地文件保存时间，延时上报的用
    Public ISUPLOADDATATIMELY As Integer    '记录是测试完了就及时上报了，还是延时上报的
    Public TASKNAME As String    '测试任务（包括测试间隔、测试文件、时间区间等）
    Public RECFILE As String    '录屏文件
    Public APKVERSION As String    'APP版本
    Public SATELLITECOUNT As Integer    '卫星数量
    Public ISOUTSIDE As Integer    '是否室外
    Public DISTRICT As String    '
    Public BDLON As Double    '
    Public BDLAT As Double    '
    Public GDLON As Double    '
    Public GDLAT As Double    '
    Public ACCURACY As Double    '
    Public ALTITUDE As Double    '
    Public GPSSPEED As Double    '
    Public BUSINESSTYPE As String    '
    Public APKNAME As String    '


    Public wifi_SSID As String
    Public wifi_MAC As String

    Public FREQ As Double
    Public cpu As String
    Public ADJ_SIGNAL As String
    Public Adj_ECI1 As Integer
    Public Adj_RSRP1 As Integer
    Public Adj_SINR1 As Integer
    Public isScreenOn As Integer

    Public pi As PhoneInfo    '
    Sub New()

    End Sub
    Public Class GPSPoint
        Public LONGITUDE As Double
        Public LATITUDE As Double
    End Class
    Public Class ADJInfo
        Public ECI As Double
        Public RSRP As Double
    End Class
    Public Class STALLInfo
        Public POINT As Integer
        Public TIME As Integer
    End Class
End Class

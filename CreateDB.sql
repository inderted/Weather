Create database WeatherDB
On primary
(
    --配置主数据文件的选项
	name='WeatherDB',--主数据文件的逻辑名称
	filename='D:\WeatherDB.mdf',--主数据文件的实际保存路径
	size=5MB,--主数据文件的初始大小
	maxsize=150MB,--主数据文件的最大容量限制
	filegrowth=20%--主数据文件文件增长率
)
use WeatherDB
IF NOT EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[WeatherData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
Begin
    Create table WeatherData
    (
    Id int identity(1,1) primary key,--创建主键从1开始自增1
	City NVARCHAR(100) NOT null,
	Date datetime not null,
	WeatherType nvarchar(100) NOT null,
	DayTemperature nvarchar(100) not null,
	NightTemperature NVARCHAR(100) not null,
	WindType NVARCHAR(100) not null,
	WindSpeed NVARCHAR(100) not null
    )

End 

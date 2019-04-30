Create database WeatherDB
On primary
(
    --�����������ļ���ѡ��
	name='WeatherDB',--�������ļ����߼�����
	filename='D:\WeatherDB.mdf',--�������ļ���ʵ�ʱ���·��
	size=5MB,--�������ļ��ĳ�ʼ��С
	maxsize=150MB,--�������ļ��������������
	filegrowth=20%--�������ļ��ļ�������
)
use WeatherDB
IF NOT EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[WeatherData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
Begin
    Create table WeatherData
    (
    Id int identity(1,1) primary key,--����������1��ʼ����1
	City NVARCHAR(100) NOT null,
	Date datetime not null,
	WeatherType nvarchar(100) NOT null,
	DayTemperature nvarchar(100) not null,
	NightTemperature NVARCHAR(100) not null,
	WindType NVARCHAR(100) not null,
	WindSpeed NVARCHAR(100) not null
    )

End 

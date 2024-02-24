CREATE DATABASE	QLPHHT
go
use QLPHHT
go
CREATE TABLE PhienHop(
						IDPhienHop int identity primary key,
						TenPhienHop nvarchar(50) not null,
						Mota nvarchar(max) null,
						HinhThuc nvarchar(50) not null,
						NgayBatDau date not null,
						NgayKetThuc date not null,
						ChieuDoc int null,
						ChieuNgang int null,
						SoDay int null,
						SoHangMoiDay int null,
						SoGheMoiHang varchar(50) null
						)
go
CREATE TABLE ThongTinGhe(
							STT int null,
							IDGhe varchar(20),
							TenGhe nvarchar(20) not null,
							HoTen nvarchar(50) null,
							ChucDanh nvarchar(50) null,
							ChucVu nvarchar(50) null,
							DoUuTien int null,
							IDPhienHop int not null,
							constraint fk_PH_TTG foreign key (IDPhienHop) references PhienHop(IDPhienHop),
							constraint pk_idghe_idph primary key (IDGhe, IDPhienHop)
							)




								


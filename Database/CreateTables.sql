-- ============================================================================
-- HotelBookingDb schema
-- Table/column names match what Data/DataContextEF.cs, Auth/AuthController.cs,
-- Rooms/RoomController.cs and Data/HotelBookingRepository.cs already query.
-- Safe to re-run: each table is dropped and recreated.
-- ============================================================================

IF DB_ID(N'HotelBookingDb') IS NULL
BEGIN
    CREATE DATABASE HotelBookingDb;
END
GO

USE HotelBookingDb;
GO

-- Drop in dependency order (children first)
IF OBJECT_ID(N'dbo.RoomBookings', N'U') IS NOT NULL DROP TABLE dbo.RoomBookings;
IF OBJECT_ID(N'dbo.Auth', N'U') IS NOT NULL DROP TABLE dbo.Auth;
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID(N'dbo.Rooms', N'U') IS NOT NULL DROP TABLE dbo.Rooms;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID(N'dbo.Status', N'U') IS NOT NULL DROP TABLE dbo.Status;
GO

-- ----------------------------------------------------------------------------
-- Roles  (mirrors Enums/Roles.cs — Id must match the enum's underlying int)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Roles
(
    Id       INT          NOT NULL PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

INSERT INTO dbo.Roles (Id, RoleName) VALUES
    (1, N'Clerk'),
    (2, N'Manager');
GO

-- ----------------------------------------------------------------------------
-- Status  (mirrors Enums/BookingStatus.cs — Id must match the enum's int)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Status
(
    Id         INT          NOT NULL PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);
GO

INSERT INTO dbo.Status (Id, StatusName) VALUES
    (0, N'Booked'),
    (1, N'CheckedIn'),
    (2, N'CheckedOut'),
    (3, N'Canceled');
GO

-- ----------------------------------------------------------------------------
-- Users  (columns match AuthController's INSERT INTO Users(...))
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Users
(
    Id        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FirstName NVARCHAR(100)     NOT NULL,
    LastName  NVARCHAR(100)     NOT NULL,
    Email     NVARCHAR(256)     NOT NULL UNIQUE,
    RoleId    INT               NOT NULL,
    Active    BIT               NOT NULL DEFAULT (1),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (Id)
);
GO

-- ----------------------------------------------------------------------------
-- Auth  (credentials store queried/inserted by AuthController; no FK to Users
-- since Register() inserts the Auth row before the matching Users row exists)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Auth
(
    Email        NVARCHAR(256)   NOT NULL PRIMARY KEY,
    PasswordHash VARBINARY(256)  NOT NULL,
    PasswordSalt VARBINARY(256)  NOT NULL
);
GO

-- ----------------------------------------------------------------------------
-- Rooms  (columns match Rooms/Room.cs and RoomController's raw SQL)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Rooms
(
    Id               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Number           NVARCHAR(20)      NOT NULL,
    AdultsCapacity   INT               NOT NULL,
    ChildrenCapacity INT               NULL,
    Price            DECIMAL(10,2)     NOT NULL
);
GO

-- ----------------------------------------------------------------------------
-- Bookings  (columns match Booking/Booking.cs)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Bookings
(
    Id               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    GuestFirstName   NVARCHAR(100)     NOT NULL,
    GuestLastName    NVARCHAR(100)     NOT NULL,
    CheckInDate      DATETIME2         NOT NULL,
    CheckOutDate     DATETIME2         NOT NULL,
    NumberOfAdults   INT               NOT NULL,
    NumberOfChildren INT               NULL
);
GO

-- ----------------------------------------------------------------------------
-- RoomBookings  (join table; composite key matches DataContextEF's
-- HasKey(rb => new { rb.RoomId, rb.BookingId }))
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.RoomBookings
(
    RoomId    INT NOT NULL,
    BookingId INT NOT NULL,
    Status    INT NOT NULL,
    CONSTRAINT PK_RoomBookings PRIMARY KEY (RoomId, BookingId),
    CONSTRAINT FK_RoomBookings_Rooms FOREIGN KEY (RoomId) REFERENCES dbo.Rooms (Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoomBookings_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings (Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoomBookings_Status FOREIGN KEY (Status) REFERENCES dbo.Status (Id)
);
GO

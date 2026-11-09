/*
 CLOUD 5IVE - Bursary Management System
 Database: CloudFiveBursaryDB
 Based on the supplied ApplicationDbContext and model classes.
 Creates schema only: no user accounts, passwords, or sample records.
 For the exact production schema, the project's EF Core migrations remain authoritative.
 Use this script OR EF Core migrations for initial creation, not both on the same empty database.
*/
IF DB_ID(N'CloudFiveBursaryDB') IS NULL CREATE DATABASE [CloudFiveBursaryDB];
GO
USE [CloudFiveBursaryDB];
GO

/* ASP.NET Core Identity */
CREATE TABLE dbo.AspNetRoles (
 Id nvarchar(450) NOT NULL CONSTRAINT PK_AspNetRoles PRIMARY KEY,
 Name nvarchar(256) NULL, NormalizedName nvarchar(256) NULL, ConcurrencyStamp nvarchar(max) NULL
);
CREATE TABLE dbo.AspNetUsers (
 Id nvarchar(450) NOT NULL CONSTRAINT PK_AspNetUsers PRIMARY KEY,
 UserName nvarchar(256) NULL, NormalizedUserName nvarchar(256) NULL,
 Email nvarchar(256) NULL, NormalizedEmail nvarchar(256) NULL,
 EmailConfirmed bit NOT NULL, PasswordHash nvarchar(max) NULL,
 SecurityStamp nvarchar(max) NULL, ConcurrencyStamp nvarchar(max) NULL,
 PhoneNumber nvarchar(max) NULL, PhoneNumberConfirmed bit NOT NULL,
 TwoFactorEnabled bit NOT NULL, LockoutEnd datetimeoffset(7) NULL,
 LockoutEnabled bit NOT NULL, AccessFailedCount int NOT NULL
);
CREATE TABLE dbo.AspNetRoleClaims (
 Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY,
 RoleId nvarchar(450) NOT NULL, ClaimType nvarchar(max) NULL, ClaimValue nvarchar(max) NULL,
 CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);
CREATE TABLE dbo.AspNetUserClaims (
 Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AspNetUserClaims PRIMARY KEY,
 UserId nvarchar(450) NOT NULL, ClaimType nvarchar(max) NULL, ClaimValue nvarchar(max) NULL,
 CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
CREATE TABLE dbo.AspNetUserLogins (
 LoginProvider nvarchar(450) NOT NULL, ProviderKey nvarchar(450) NOT NULL,
 ProviderDisplayName nvarchar(max) NULL, UserId nvarchar(450) NOT NULL,
 CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
 CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
CREATE TABLE dbo.AspNetUserRoles (
 UserId nvarchar(450) NOT NULL, RoleId nvarchar(450) NOT NULL,
 CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
 CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE,
 CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);
CREATE TABLE dbo.AspNetUserTokens (
 UserId nvarchar(450) NOT NULL, LoginProvider nvarchar(450) NOT NULL, Name nvarchar(450) NOT NULL, Value nvarchar(max) NULL,
 CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
 CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
GO

/* Application tables */
CREATE TABLE dbo.Applicants (
 ApplicantId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Applicants PRIMARY KEY,
 UserId nvarchar(450) NOT NULL, FirstName nvarchar(100) NOT NULL, LastName nvarchar(100) NOT NULL,
 IDNumber nvarchar(20) NOT NULL, Email nvarchar(max) NOT NULL, PhoneNumber nvarchar(max) NOT NULL,
 Address nvarchar(250) NULL, InstitutionName nvarchar(150) NULL, CourseName nvarchar(150) NULL,
 YearOfStudy nvarchar(30) NULL, DateCreated datetime2(7) NOT NULL,
 CONSTRAINT FK_Applicants_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
);
CREATE TABLE dbo.BursaryProgrammes (
 BursaryProgrammeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BursaryProgrammes PRIMARY KEY,
 ProgrammeName nvarchar(150) NOT NULL, Description nvarchar(1000) NOT NULL,
 EligibilityCriteria nvarchar(1000) NOT NULL, FundingAmount decimal(18,2) NOT NULL,
 ApplicationOpeningDate datetime2(7) NOT NULL, ApplicationClosingDate datetime2(7) NOT NULL,
 ProgrammeStatus nvarchar(20) NOT NULL, DateCreated datetime2(7) NOT NULL
);
CREATE TABLE dbo.BursaryApplications (
 BursaryApplicationId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BursaryApplications PRIMARY KEY,
 ReferenceNumber nvarchar(30) NOT NULL, ApplicantId int NOT NULL, BursaryProgrammeId int NOT NULL,
 DateSubmitted datetime2(7) NOT NULL, ApplicationStatus nvarchar(30) NOT NULL,
 AdministratorComments nvarchar(1000) NULL,
 CONSTRAINT FK_BursaryApplications_Applicants_ApplicantId FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(ApplicantId),
 CONSTRAINT FK_BursaryApplications_BursaryProgrammes_BursaryProgrammeId FOREIGN KEY (BursaryProgrammeId) REFERENCES dbo.BursaryProgrammes(BursaryProgrammeId)
);
CREATE TABLE dbo.Beneficiaries (
 BeneficiaryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Beneficiaries PRIMARY KEY,
 BursaryApplicationId int NOT NULL, DateAdded datetime2(7) NOT NULL,
 BeneficiaryStatus nvarchar(30) NOT NULL, FundingAmount decimal(18,2) NOT NULL,
 CONSTRAINT FK_Beneficiaries_BursaryApplications_BursaryApplicationId FOREIGN KEY (BursaryApplicationId) REFERENCES dbo.BursaryApplications(BursaryApplicationId)
);
CREATE TABLE dbo.Payments (
 PaymentId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
 BeneficiaryId int NOT NULL, PaymentReference nvarchar(50) NOT NULL,
 PaymentAmount decimal(18,2) NOT NULL, PaymentDate datetime2(7) NOT NULL,
 PaymentStatus nvarchar(20) NOT NULL, Description nvarchar(500) NULL,
 CONSTRAINT FK_Payments_Beneficiaries_BeneficiaryId FOREIGN KEY (BeneficiaryId) REFERENCES dbo.Beneficiaries(BeneficiaryId)
);
CREATE TABLE dbo.ApplicantDocuments (
 ApplicantDocumentId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ApplicantDocuments PRIMARY KEY,
 ApplicantId int NOT NULL, DocumentName nvarchar(255) NOT NULL, StoredFileName nvarchar(255) NOT NULL,
 DocumentType nvarchar(100) NOT NULL, ContentType nvarchar(100) NOT NULL, FileSize bigint NOT NULL,
 DateUploaded datetime2(7) NOT NULL, DocumentStatus nvarchar(30) NOT NULL, AdminComment nvarchar(1000) NULL,
 CONSTRAINT FK_ApplicantDocuments_Applicants_ApplicantId FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(ApplicantId)
);
CREATE TABLE dbo.Notifications (
 NotificationId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notifications PRIMARY KEY,
 ApplicantId int NOT NULL, Title nvarchar(150) NOT NULL, Message nvarchar(1000) NOT NULL,
 NotificationType nvarchar(50) NOT NULL, IsRead bit NOT NULL, DateCreated datetime2(7) NOT NULL,
 ActionUrl nvarchar(255) NULL,
 CONSTRAINT FK_Notifications_Applicants_ApplicantId FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(ApplicantId)
);
CREATE TABLE dbo.ActivityLogs (
 ActivityLogId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ActivityLogs PRIMARY KEY,
 UserId nvarchar(max) NOT NULL, UserEmail nvarchar(256) NOT NULL, UserRole nvarchar(50) NOT NULL,
 Action nvarchar(100) NOT NULL, Description nvarchar(1000) NOT NULL,
 EntityType nvarchar(100) NULL, EntityId int NULL, DateCreated datetime2(7) NOT NULL
);
GO

/* Identity indexes */
CREATE INDEX IX_AspNetRoleClaims_RoleId ON dbo.AspNetRoleClaims(RoleId);
CREATE UNIQUE INDEX RoleNameIndex ON dbo.AspNetRoles(NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE INDEX IX_AspNetUserClaims_UserId ON dbo.AspNetUserClaims(UserId);
CREATE INDEX IX_AspNetUserLogins_UserId ON dbo.AspNetUserLogins(UserId);
CREATE INDEX IX_AspNetUserRoles_RoleId ON dbo.AspNetUserRoles(RoleId);
CREATE INDEX EmailIndex ON dbo.AspNetUsers(NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON dbo.AspNetUsers(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;

/* Application indexes configured in ApplicationDbContext */
CREATE UNIQUE INDEX IX_Applicants_UserId ON dbo.Applicants(UserId);
CREATE UNIQUE INDEX IX_Applicants_IDNumber ON dbo.Applicants(IDNumber);
CREATE INDEX IX_BursaryApplications_ApplicantId ON dbo.BursaryApplications(ApplicantId);
CREATE INDEX IX_BursaryApplications_BursaryProgrammeId ON dbo.BursaryApplications(BursaryProgrammeId);
CREATE UNIQUE INDEX IX_Beneficiaries_BursaryApplicationId ON dbo.Beneficiaries(BursaryApplicationId);
CREATE INDEX IX_Payments_BeneficiaryId ON dbo.Payments(BeneficiaryId);
CREATE UNIQUE INDEX IX_Payments_PaymentReference ON dbo.Payments(PaymentReference);
CREATE INDEX IX_ApplicantDocuments_ApplicantId ON dbo.ApplicantDocuments(ApplicantId);
CREATE INDEX IX_Notifications_ApplicantId_IsRead ON dbo.Notifications(ApplicantId, IsRead);
CREATE INDEX IX_ActivityLogs_UserId ON dbo.ActivityLogs(UserId);
CREATE INDEX IX_ActivityLogs_UserRole ON dbo.ActivityLogs(UserRole);
CREATE INDEX IX_ActivityLogs_DateCreated ON dbo.ActivityLogs(DateCreated);
GO
PRINT 'CloudFiveBursaryDB schema created successfully.';
GO

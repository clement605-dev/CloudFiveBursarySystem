# CLOUD 5IVE – Bursary Management System

## 1. Project Overview

CLOUD 5IVE is a Bursary Management System developed to simplify and improve the management of bursary applications.

The system allows applicants to register, manage their profiles, view available bursary programmes, submit applications and upload supporting documents.

Administrators can manage bursary programmes, review applications, verify documents, manage beneficiaries, record payments, generate reports and monitor system activities.

## 2. Technologies Used

The project was developed using the following technologies:

| Technology | Purpose |
|---|---|
| C# | Main backend programming language |
| ASP.NET Core MVC (.NET 8) | Web application framework |
| HTML5 | Page structure |
| CSS3 | Styling |
| JavaScript | Frontend functionality |
| Bootstrap | Responsive interface design |
| Microsoft SQL Server | Database management |
| Entity Framework Core | Database operations |
| ASP.NET Core Identity | User registration and authentication |
| Visual Studio 2022 | Development environment |
| GitHub | Source code management and collaboration |

## 3. Software Requirements

Before running the project, ensure that your computer has the following installed:

1. Visual Studio 2022.
2. .NET 8 SDK.
3. ASP.NET and web development workload in Visual Studio.
4. Microsoft SQL Server 2022 Express or another compatible SQL Server instance.
5. SQL Server Management Studio (SSMS) – recommended for viewing and managing the database.
6. Git – for downloading and updating the project.

## 4. Downloading the Project from GitHub

Follow these steps:

1. Open the project repository:

   https://github.com/clement605-dev/CloudFiveBursarySystem

2. Click the green **Code** button.
3. Select **Download ZIP**, or clone the repository using Git.
4. If you downloaded the ZIP file, extract it.
5. Open the extracted project folder.
6. Locate `CloudFiveBursarySystem.sln`.
7. Double-click the solution file to open the project in Visual Studio 2022.

**Important:** Open the `.sln` file, not an individual C# file.

## 5. Setting Up the Database

The project uses Microsoft SQL Server.

Each team member must create their own local database.

### Step 1: Install and Start SQL Server

Make sure SQL Server is installed and running on your computer.

### Step 2: Configure the Connection String

Open the project in Visual Studio.

Locate:

`appsettings.json`

Find the `DefaultConnection` setting.

Update it to match your own SQL Server instance.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CloudFiveBursaryDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Replace `YOUR_SERVER_NAME` with your own SQL Server instance.

For example:

```text
Server=DESKTOP-8HV2L54\SQLEXPRESS
```

Do not copy another team member's computer name unless it is also your SQL Server instance.

### Step 3: Create the Database

The database can be created using Entity Framework Core migrations.

In Visual Studio:

1. Open **Tools**.
2. Select **NuGet Package Manager**.
3. Open **Package Manager Console**.
4. Make sure the project containing `ApplicationDbContext` is selected as the Default Project.
5. Run:

```powershell
Update-Database
```

This applies the existing migrations and creates the database tables.

Do not create a second set of migrations unnecessarily.

## 6. Running the Application

After configuring the database:

1. Open the solution in Visual Studio 2022.
2. Allow NuGet packages to restore.
3. Build the solution.
4. Confirm that there are no build errors.
5. Select the HTTPS launch profile.
6. Click the green **Run** button.

The application should open in your default browser.

## 7. Main System Modules

The system includes the following modules:

### Applicant Modules
- User registration and login.
- Applicant dashboard.
- Personal and academic profile management.
- Available bursary programmes.
- Bursary application submission.
- Supporting document uploads.
- Application tracking.
- Notifications.

### Administrator Modules
- Administrator dashboard.
- Bursary programme management.
- Applicant management.
- Application review and approval.
- Document verification.
- Beneficiary management.
- Payment record management.
- Report generation.
- Activity Logs.

## 8. Important Database Information

The database is named:

`CloudFiveBursaryDB`

The project uses Entity Framework Core and SQL Server.

Database migrations are included in the project under:

`Data/Migrations`

These migrations are used to create and update the database structure.

**Important:** GitHub stores the source code and migration files, not automatically the records inside each member's local database.

Each team member will have their own local database unless a shared database is configured separately.

## 9. Applicant Document Storage

Uploaded applicant documents are stored locally in the application's private storage directory:

`App_Data/ApplicantDocuments/`

These files are not automatically transferred to other team members through GitHub.

Use sample or test documents when demonstrating the system. Do not upload real applicant documents, identity documents or other private information to the public repository.

## 10. Working with GitHub

To receive the latest changes from another team member:

1. Open the project in Visual Studio.
2. Open the Git menu or Git Changes panel.
3. Select **Pull** to download the latest committed changes.

Before making changes, always pull the latest version.

After completing your assigned work:

1. Build and test your changes.
2. Open Git Changes.
3. Enter a meaningful commit message.
4. Commit your changes.
5. Push them to GitHub.

Avoid editing the same files simultaneously without communicating with the team.

## 11. Troubleshooting

### Database connection error

Check that:
- SQL Server is installed and running.
- Your connection string contains the correct server name.
- Windows Authentication is configured correctly.

### Database or table does not exist

Open Package Manager Console and run:

```powershell
Update-Database
```

### Build errors

Try:
- Restoring NuGet packages.
- Checking that the .NET 8 SDK is installed.
- Building the solution again.

### Project does not open

Make sure you opened:

`CloudFiveBursarySystem.sln`

rather than an individual source-code file.

## 12. Project Repository

GitHub Repository:

https://github.com/clement605-dev/CloudFiveBursarySystem

**Project Name:** CLOUD 5IVE – Bursary Management System

**Backend:** C# / ASP.NET Core MVC

**Database:** Microsoft SQL Server

**Development Environment:** Visual Studio 2022

**Framework:** .NET 8

---

**CLOUD 5IVE – Bursary Management System**

Developed as a collaborative academic group project.

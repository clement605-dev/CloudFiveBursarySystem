# CLOUD 5IVE – Bursary Management System

## 1. Project Overview

CLOUD 5IVE is a web-based Bursary Management System developed to simplify and improve the management of bursary applications.

The system provides a centralised platform where students can register, manage their profiles, view available bursary programmes, submit applications and upload supporting documents.

Administrators can manage bursary programmes, review applications, verify submitted documents, manage beneficiaries, record payment information and monitor system activities.

The project was developed as part of an academic group project to demonstrate the practical application of software engineering, database management, web development and system security.

## 2. Main Features

### Applicant Features
- Applicant registration and login.
- Applicant profile management.
- View available bursary programmes.
- Submit bursary applications.
- Upload supporting documents.
- View application statuses.
- Receive in-system notifications.
- View application-related information.

### Administrator Features
- Secure administrator login.
- Administrator dashboard.
- Manage bursary programmes.
- View and review applications.
- Approve or reject applications.
- Request additional information.
- Verify applicant documents.
- Register successful applicants as beneficiaries.
- Manage beneficiary records.
- Record and monitor payment information.
- Generate reports.
- Monitor administrator and applicant activities.

## 3. Technologies Used

The system was developed using the following technologies:

| Technology | Purpose |
|---|---|
| C# | Backend programming |
| ASP.NET Core MVC (.NET 8) | Web application framework |
| HTML5 | Web page structure |
| CSS3 | Styling and layout |
| JavaScript | Client-side functionality |
| Bootstrap | Responsive user interface |
| Microsoft SQL Server 2022 Express | Database management |
| Entity Framework Core | Database access and management |
| ASP.NET Core Identity | Authentication and role management |
| Visual Studio 2022 | Development environment |

## 4. System Architecture

The application follows the Model-View-Controller (MVC) architectural pattern.

- **Model:** Represents the application's data and business entities.
- **View:** Provides the user interface displayed to applicants and administrators.
- **Controller:** Handles user requests, application processes and communication between models and views.

Entity Framework Core is used to communicate with the SQL Server database.

## 5. Database Information

Database name:

`CloudFiveBursaryDB`

The database contains the following main application tables:

- Applicants
- BursaryProgrammes
- BursaryApplications
- Beneficiaries
- Payments
- ApplicantDocuments
- Notifications
- ActivityLogs

The system also uses ASP.NET Core Identity tables for managing user accounts, authentication and roles.

### Database Setup

A database setup script is available in:

`CloudFiveBursarySystem/Database/CloudFiveBursaryDB.sql`

To create the database:

1. Download or clone this repository.
2. Open Microsoft SQL Server Management Studio (SSMS).
3. Connect to your local SQL Server instance.
4. Open the `CloudFiveBursaryDB.sql` file.
5. Execute the script.
6. Confirm that the `CloudFiveBursaryDB` database has been created.

**Important:** The SQL script creates the database structure. It does not include existing user accounts, administrator credentials or sample applicant records.

## 6. Requirements

Before running the application, ensure that you have installed:

- Visual Studio 2022.
- .NET 8 SDK.
- Microsoft SQL Server 2022 Express.
- SQL Server Management Studio (SSMS).

## 7. Installation and Configuration

### Step 1: Clone the Repository

Open a terminal and execute:

`git clone https://github.com/clement605-dev/CloudFiveBursarySystem.git`

### Step 2: Open the Project

Open the downloaded solution file:

`CloudFiveBursarySystem.sln`

Use Visual Studio 2022.

### Step 3: Configure the Database Connection

Open `appsettings.json` and update the database connection string to match your local SQL Server instance.

Example:

`Server=YOUR-PC-NAME\\SQLEXPRESS;Database=CloudFiveBursaryDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

Replace `YOUR-PC-NAME` with your computer's actual SQL Server instance name.

### Step 4: Restore Dependencies

In Visual Studio, restore the NuGet packages required by the project.

### Step 5: Run the Application

Select the project as the startup project and press:

`F5`

The application should build and launch in your browser.

## 8. User Roles

The system supports two main user roles:

**Applicant**
- Registers and manages their account.
- Submits bursary applications.
- Uploads supporting documents.
- Monitors application progress.

**Administrator**
- Manages bursary programmes.
- Reviews applications and supporting documents.
- Manages beneficiaries and payment records.
- Monitors system activities and reports.

The system automatically creates the Applicant and Administrator roles when it starts if they do not already exist.

Administrator login accounts must be created and configured separately.

## 9. Document Storage

Applicant documents are stored separately from the SQL Server database.

The system uses a private application storage directory:

`App_Data/ApplicantDocuments/`

The database stores the document information and file metadata.

## 10. Security Features

The application incorporates several security measures, including:

- ASP.NET Core Identity authentication.
- Role-based access control.
- Password requirements.
- Server-side validation.
- Anti-forgery token validation for protected form submissions.
- Applicant document ownership checks.
- Restricted document storage outside the public web directory.
- Activity logging.

## 11. Project Repository

GitHub Repository:

https://github.com/clement605-dev/CloudFiveBursarySystem

## 12. Development Team

**CLOUD 5IVE**

This project was developed collaboratively by a team of six students as part of an academic software development project.

## 13. Conclusion

CLOUD 5IVE demonstrates how a web-based information system can improve the administration of bursary programmes by providing a centralised platform for application processing, document management, beneficiary administration and reporting.

The project provided practical experience in software development, database design, system integration, testing and teamwork.

\# StudentOperations



\## 1. Project Overview



StudentOperations is a .NET 8 console application for managing student registration and enrollment requests. It uses Entity Framework Core, SQL Server LocalDB, and Hangfire to process enrollment-related background jobs.



\## 2. Technologies Used



\- C#

\- .NET 8

\- Entity Framework Core

\- SQL Server LocalDB

\- Hangfire

\- PowerShell



\## 3. Project Structure



\- \*\*StudentOperations.Domain\*\* — Contains student and enrollment-related entities.

\- \*\*StudentOperations.Application\*\* — Contains the enrollment service interface and background job definitions.

\- \*\*StudentOperations.Infrastructure\*\* — Contains the database context, migrations, and enrollment service implementation.

\- \*\*StudentOperations.Console\*\* — Contains the console menu, application startup, and configuration.



\## 4. Database



The application uses two SQL Server LocalDB databases:



1\. `StudentOperationsDb` — Stores student records, enrollment requests, and processing logs.

2\. `StudentOperationsHangfireDb` — Stores Hangfire background job information.



The database connection strings are configured in `StudentOperations.Console/appsettings.json`.



\## 5. Prerequisites



\- Windows

\- .NET 8 SDK

\- SQL Server LocalDB

\- `sqlcmd` command-line utility

\- Entity Framework Core CLI (`dotnet-ef`) version 8



\## 6. Setup Instructions



Open PowerShell in the solution folder.



\### Step 1: Start LocalDB



```powershell

sqllocaldb start MSSQLLocalDB

```



\### Step 2: Restore the projects



```powershell

dotnet restore

```



\### Step 3: Build the solution



```powershell

dotnet build

```



\### Step 4: Apply the Entity Framework migration



```powershell

dotnet ef database update --project StudentOperations.Infrastructure --startup-project StudentOperations.Console

```



\### Step 5: Create the Hangfire database



```powershell

sqlcmd -S "(localdb)\\MSSQLLocalDB" -E -Q "IF DB\_ID(N'StudentOperationsHangfireDb') IS NULL CREATE DATABASE StudentOperationsHangfireDb;"

```



Hangfire installs its required SQL objects when the application starts.



\### Step 6: Run the application



```powershell

dotnet run --project StudentOperations.Console

```



\## 7. Application Features



1\. Register a student.

2\. Create an enrollment request.

3\. Schedule an immediate background job.

4\. Schedule a delayed background job.

5\. Execute a continuation job.

6\. Queue a batch job.

7\. Display processing history.

8\. Run a recurring job that checks pending enrollment requests.



\## 8. Migration Files



The migration files are located in:



`StudentOperations.Infrastructure/Data/Migrations/`



They include the initial database migration, its designer file, and the model snapshot.



\## 9. Screenshots



The `Screenshots` folder contains evidence of the working application and database records.



The screenshots should include:



\- Application menu

\- Student registration

\- Enrollment request

\- Processing history

\- Student database records

\- Enrollment request database records

\- Processing log records

\- Hangfire database tables



\## 10. Conclusion



The project demonstrates a multi-project .NET application using Entity Framework Core for data persistence and Hangfire for background job scheduling and execution.


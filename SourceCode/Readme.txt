SE LAB ASSIGNMENT 2 - B2B WHOLESALE ORDER MANAGEMENT SYSTEM
==========================================================
Project: B2BWholesaleManagement.sln
Repository name: SE-Lab-A2-B2B-Wholesale-Management
GitHub URL: [PASTE THE ACTUAL REPOSITORY LINK]
Video URL (3-5 minutes): [PASTE THE ACTUAL DEMO LINK]
Group: [GROUP NUMBER]
Members and contribution percentages (must add to 100%):
[STUDENT ID] - [FULL NAME] - [FEATURES/TASKS] - [PERCENT]
[STUDENT ID] - [FULL NAME] - [FEATURES/TASKS] - [PERCENT]
[STUDENT ID] - [FULL NAME] - [FEATURES/TASKS] - [PERCENT]
(Add up to 5 members, as needed.)

1. PROJECT MODULES
------------------
src/Wholesale.Core: OrderRules (validation, arithmetic, lockout threshold)
src/Wholesale.Data: EF Core SQL Server entities, DbContext, AuthService, OrderService, demo seeder
src/Wholesale.Web: ASP.NET Core MVC Option C Agent Portal
src/Wholesale.Desktop: Windows Forms Admin Back-Office / PDF Shipping Manifest
tests/Wholesale.Tests: xUnit business tests and SQLite in-memory transaction tests
database/SchemaAndSeed.sql: full OPTIONAL SQL Server database + sample data script
database/IntegrityAndSeedChecks.sql: query evidence for row counts, integrity and dashboards
docs/: report, worksheet, Git plan and video guidance

2. REQUIREMENTS
---------------
Windows 10 or Windows 11 (WinForms requires Windows), Visual Studio 2022 17.8+ or newer
.NET 8 SDK; ASP.NET Core development workload and .NET desktop development workload
SQL Server Express LocalDB (installed with Visual Studio) OR SQL Server Express/Developer
Internet connectivity for the first NuGet restore and optional Bootstrap CDN

3. RECOMMENDED SETUP (EF CORE CODE FIRST, OPTION C)
----------------------------------------------------
A) Ensure SQL Server LocalDB is installed; optional check: sqllocaldb info MSSQLLocalDB
B) Edit BOTH config files if using a different SQL instance:
   src/Wholesale.Web/appsettings.json -> ConnectionStrings:WholesaleDatabase
   src/Wholesale.Desktop/App.config -> connectionStrings/WholesaleDatabase
   For LocalDB keep the provided trusted Windows authentication connection strings.
   NEVER commit a real DB password; environment variable `ConnectionStrings__WholesaleDatabase`
   overrides the Web application's JSON setting. For desktop, use secure local configuration.
C) In terminal from the project root:
   dotnet restore B2BWholesaleManagement.sln
   dotnet run --project src/Wholesale.Web/Wholesale.Web.csproj --launch-profile Wholesale.Web
D) Open https://localhost:7148 (or http://localhost:5148 if HTTPS is unavailable).
E) On FIRST launch, EF Core EnsureCreated creates the five tables and DemoSeeder adds
   20 Items, 20 Agents, 20 Orders, 28 OrderDetails, 20 Users with hashed demo passwords.
   Repeated launches do not reinsert the original sample rows.
F) Run the desktop app on Windows:
   dotnet run --project src/Wholesale.Desktop/Wholesale.Desktop.csproj
   Or in Visual Studio right-click Wholesale.Desktop -> Set as Startup Project -> F5.
G) Run the Web and Desktop apps simultaneously to demonstrate shared data.

Alternative SQL script setup: create a fresh database in SSMS, execute
   database/SchemaAndSeed.sql
ONLY if you are NOT using step E to create the database. It creates the same five
logical entities with 20/20/20/28/20 records and ASP.NET Identity PBKDF2 V3 hashes.
Running BOTH methods against the same populated database is not necessary.

4. DEMO ACCOUNTS (FOR LOCAL TEST ONLY)
-------------------------------------
WinForms: username admin    password Admin@12345!  role Admin
WinForms: username admin02  password Admin@12345!  role Admin
Web:      username agent01 password Agent@12345!  role Agent
Web:      username agent02 password Agent@12345!  role Agent
Web:      other agent03..agent18 use Agent@12345!
Accounts lock after 5 failed passwords, for demonstration. Delete demo credentials
and replace with individual strong secrets before any non-local deployment.

5. RUN UNIT TESTS
-----------------
Run: dotnet test B2BWholesaleManagement.sln
Tests use a temporary SQLite in-memory DB; no local SQL Server is needed for tests.
The suite covers quantity validation, duplicate items, stock overselling,
financial totals, lockout, database transaction commit/rollback, and agent isolation.
Capture real Test Explorer or terminal results. Do not report PASS until run.

6. WHAT TO DEMONSTRATE
----------------------
Web: role-restricted login; catalog; multi-line order, stock updates;
     insufficient stock blocks order; My Orders; invoice; responsive mobile layout.
WinForms: Admin login; Item CRUD; Agent CRUD; Order status update;
          Shipping Manifest saved to PDF; dashboard best-sellers/low-stock/revenue.
Database: five tables, foreign keys, 15-30 rows per table, data checks.
Testing: dotnet test output and a failing rollback scenario prevented.
Teamwork: REAL branches and merged pull requests with each member's contribution.

7. SECURITY / DATABASE NOTES
----------------------------
- All passwords stored using ASP.NET Identity PasswordHasher PBKDF2 V3; not plaintext.
- Web portal authorizes Role=Agent and uses a signed cookie with AgentId claim.
- Server recomputes price from DB, ignores any modified price in the form.
- Every order writes master, details, stock in ONE DB transaction.
- Atomic conditional stock UPDATE ensures StockQuantity does not drop below zero.
- Agent cannot see orders belonging to another agent.
- Item and Agent with historical references cannot be deleted (RESTRICT FK).
- OrderDetails delete with Order master (CASCADE FK).
- Schema auto-creation uses EnsureCreated for a clean educational demo database.
  If you later add migrations, drop/recreate this DEMO DB or switch to a dedicated
  EF migration workflow; EnsureCreated does not maintain a migrations history.
- Do not expose SQL Server or these demo accounts on a public Internet server.

8. SUBMISSION CHECKLIST
-----------------------
[ ] Report.docx completed with real owner, student IDs, screenshots and test results
[ ] SourceCode.zip (this project; NO bin or obj folders) including .sql
[ ] Readme.txt updated with GitHub link, 3-5min demo link, DB instructions, % per member
[ ] Team workbook contains verified real member assignments, branches, PR links
[ ] Upload as ONE archive and name as the LMS screenshot requests:
    StudentID1_StudentFullName_StudentID2_StudentFullName.zip
[ ] Confirm the correct semester assignment version with your teacher.

9. HONEST VERIFICATION STATUS
-----------------------------
The project was generated and checked structurally in an environment WITHOUT
.NET SDK, SQL Server, or Windows Forms runtime. Compilation, actual app screenshots,
PDF export, and unit-test PASS status must be verified on a Windows machine with
Visual Studio/.NET 8 and SQL Server. Report placeholders explicitly mark this.

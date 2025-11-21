# PROG6212 Part3

## College Claims System

A comprehensive web application for managing academic claims within a college environment. This system facilitates the submission, verification, approval, and tracking of teaching claims through a role-based workflow.

## Overview

The College Claims System is an ASP.NET Core MVC application that streamlines the claims management process for educational institutions. It provides separate interfaces for different user roles including Lecturers, Coordinators, Managers, and HR staff, each with specific permissions and functionalities.

## System Requirements

- **.NET 7.0 SDK** or later
- **Web browser** (Chrome, Firefox, Edge, or Safari)
- **Internet connection** for CDN resources (Bootstrap, Font Awesome)

## Installation & Setup

### Prerequisites
1. Install [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Clone or download the project files
3. Ensure all required NuGet packages are restored

### Running the Application
1. Open the project in Visual Studio 2022 or VS Code
2. Restore NuGet packages: `dotnet restore`
3. Build the solution: `dotnet build`
4. Run the application: `dotnet run`
5. Open your browser and navigate to: `https://localhost:7000` (or the port shown in terminal)

## User Roles and Login Details

### Lecturer
- **Email:** zaara.salie@prog.ac.za
- **Password:** lecturer123
- **Permissions:** Submit claims, View personal claims history

### Coordinator
- **Email:** thabo.mokoena@college.ac.za
- **Password:** coordinator123
- **Permissions:** Verify/Reject submitted claims, View all claims

### Manager
- **Email:** naledi.jacobs@college.ac.za
- **Password:** manager123
- **Permissions:** Approve verified claims, View dashboard and reports

### HR
- **Email:** hr@college.ac.za
- **Password:** hr123
- **Permissions:** Full system access, User management, Analytics and reporting

## How to Use the Application

### For Lecturers:
1. **Login** using lecturer credentials
2. **Navigate to "Submit Claim"** to create new claims
3. **Fill in claim details:** Month, course information, hours worked, hourly rate
4. **Upload supporting documents** (PDF, DOCX, XLSX up to 5MB)
5. **Submit claim** for verification
6. **View claim status** in "My Claims" section

### For Coordinators:
1. **Login** using coordinator credentials
2. **Access "Verify Claims"** to view submitted claims
3. **Review claim details** and download supporting documents
4. **Verify or Reject** claims based on validation
5. **Monitor claim workflow** through status updates

### For Managers:
1. **Login** using manager credentials
2. **View "Dashboard"** for system overview and statistics
3. **Access "Approve Claims"** to review verified claims
4. **Approve or Reject** claims for final processing
5. **Generate reports** for financial oversight

### For HR:
1. **Login** using HR credentials
2. **Access comprehensive dashboard** with system analytics
3. **Manage users** through "Manage Users" section
4. **View all claims** across the system
5. **Generate summary reports** for management
6. **Add/edit/delete users** from all roles

## Key Features

### Claim Management
- Multi-step approval workflow (Submitted → Verified → Approved/Rejected)
- File upload with encryption
- Automatic total amount calculation
- Status tracking and history

### User Management
- Role-based access control
- Secure password hashing
- Comprehensive user profiles
- Department and faculty assignment

### Reporting & Analytics
- Lecturer performance summaries
- Financial reporting
- Approval rate tracking
- Department-wise analysis

### Security Features
- Session-based authentication
- Password hashing using SHA256
- Role-based authorization
- Input validation and sanitization

## File Upload Specifications

- **Supported formats:** PDF, DOCX, XLSX
- **Maximum file size:** 5MB
- **Automatic encryption** for secure storage
- **Download functionality** for authorized users

## Business Rules

### Claim Validation
- Hours worked: 0.5 - 200 hours
- Hourly rate: R1 - R1000
- Monthly earnings limit: R80,000
- Part-time hours limit: 160 hours per claim

### Workflow Rules
- Only Coordinators can verify claims
- Only Managers can approve verified claims
- HR has read-only access to all claims
- Lecturers can only view their own claims

## Troubleshooting

### Common Issues:

1. **File upload fails:**
   - Check file size (max 5MB)
   - Ensure file type is supported
   - Verify internet connection

2. **Login issues:**
   - Verify username and password
   - Check role assignment
   - Clear browser cache if needed

3. **Calculation errors:**
   - Ensure hours and rate are within valid ranges
   - Refresh the page and re-enter values

### Support:
For technical support, contact the system administrator or refer to the application logs.

## References

Microsoft (2023) ASP.NET Core documentation, Available at: https://learn.microsoft.com/en-us/aspnet/core/ (Accessed: 15 January 2024).

Microsoft (2023) Entity Framework Core, Available at: https://learn.microsoft.com/en-us/ef/core/ (Accessed: 15 January 2024).

Bootstrap Team (2023) Bootstrap 5.3 documentation, Available at: https://getbootstrap.com/docs/5.3/ (Accessed: 15 January 2024).

Font Awesome (2023) Font Awesome icons, Available at: https://fontawesome.com/icons (Accessed: 15 January 2024).

jQuery Foundation (2023) jQuery API documentation, Available at: https://api.jquery.com/ (Accessed: 15 January 2024).

Chart.js Team (2023) Chart.js documentation, Available at: https://www.chartjs.org/docs/latest/ (Accessed: 15 January 2024).

ECMA International (2023) ECMAScript 2023 language specification, Available at: https://www.ecma-international.org/publications-and-standards/standards/ecma-262/ (Accessed: 15 January 2024).

World Wide Web Consortium (2023) HTML Living Standard, Available at: https://html.spec.whatwg.org/ (Accessed: 15 January 2024).

World Wide Web Consortium (2023) CSS Snapshot 2023, Available at: https://www.w3.org/TR/css-2023/ (Accessed: 15 January 2024).

xUnit.net (2023) xUnit.net documentation, Available at: https://xunit.net/ (Accessed: 15 January 2024).

Microsoft (2023) Unit testing in .NET, Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/ (Accessed: 15 January 2024).

OWASP Foundation (2023) OWASP Cheat Sheet Series, Available at: https://cheatsheetseries.owasp.org/ (Accessed: 15 January 2024).

Microsoft (2023) ASP.NET Core security documentation, Available at: https://learn.microsoft.com/en-us/aspnet/core/security/ (Accessed: 15 January 2024).

Microsoft (2023) Visual Studio 2022 documentation, Available at: https://learn.microsoft.com/en-us/visualstudio/ide/ (Accessed: 15 January 2024).

.NET Foundation (2023) .NET 7 SDK, Available at: https://dotnet.microsoft.com/en-us/download/dotnet/7.0 (Accessed: 15 January 2024).

Microsoft (2023) ASP.NET Core MVC pattern, Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview (Accessed: 15 January 2024).

Gamma, E. et al. (1994) Design patterns: elements of reusable object-oriented software, Boston: Addison-Wesley.

Martin, R.C. (2008) Clean code: a handbook of agile software craftsmanship, Upper Saddle River: Prentice Hall.

Fowler, M. (2002) Patterns of enterprise application architecture, Boston: Addison-Wesley.

Beck, K. (2002) Test-driven development: by example, Boston: Addison-Wesley.

Evans, E. (2003) Domain-driven design: tackling complexity in the heart of software, Boston: Addison-Wesley.

Internet Engineering Task Force (2023) HTTP/1.1 specification (RFC 9112), Available at: https://httpwg.org/specs/rfc9112.html (Accessed: 15 January 2024).

World Wide Web Consortium (2023) Web Content Accessibility Guidelines (WCAG) 2.2, Available at: https://www.w3.org/TR/WCAG22/ (Accessed: 15 January 2024).

Information Commissioner's Office (2023) Guide to the UK General Data Protection Regulation (UK GDPR), Available at: https://ico.org.uk/for-organisations/guide-to-data-protection/guide-to-the-general-data-protection-regulation-gdpr/ (Accessed: 15 January 2024).

European Union (2016) General Data Protection Regulation (GDPR), Official Journal of the European Union, L119/1.

Microsoft (2023) C# coding conventions, Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions (Accessed: 15 January 2024).

GitHub (2023) GitHub Flow, Available at: https://docs.github.com/en-get-started/using-github/github-flow (Accessed: 15 January 2024).

Newtonsoft (2023) Json.NET documentation, Available at: https://www.newtonsoft.com/json (Accessed: 15 January 2024).

Moq Project (2023) Moq documentation, Available at: https://github.com/moq/moq (Accessed: 15 January 2024).

University of South Africa (2023) PROG6212: Programming course materials, Pretoria: UNISA.

Stack Overflow (2023) Stack Overflow community knowledge base, Available at: https://stackoverflow.com/ (Accessed: 15 January 2024).

Chacon, S. and Straub, B. (2014) Pro Git, 2nd edn, New York: Apress.

Git (2023) Git documentation, Available at: https://git-scm.com/doc (Accessed: 15 January 2024).

## Project Links

GitHub Repository: https://github.com/ZSalie/PROG6212_PART3_St10455456_POE.git

YouTube Demonstration: [Insert YouTube Link Here]


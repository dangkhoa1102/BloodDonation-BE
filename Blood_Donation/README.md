# Blood Donation System

This is a Blood Donation System built using ASP.NET Core Web API with Entity Framework Core.

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server
- Visual Studio or any other IDE

## Database Setup

1. Open SQL Server Management Studio
2. Create a new database named `BloodDonationSupport`
3. Configure the connection string in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BloodDonationSupport;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Running the Application

### Using Visual Studio

1. Open the solution file `Blood_Donation.sln` in Visual Studio
2. Right-click on the `Blood_Donation_System` project and select "Set as Startup Project"
3. Press F5 or click the Run button

### Using Command Line

1. Navigate to the project directory:

```
cd Blood_Donation_System
```

2. Create the database (first time only):

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3. Run the application:

```
dotnet run
```

4. The API will be available at:
   - https://localhost:5001
   - http://localhost:5000

## API Documentation

The API documentation is available using Swagger:

- When running locally: https://localhost:5001/swagger

## Features

- User management
- Donor registration and management
- Blood donation tracking
- Blood type compatibility
- Medical facility management
- And more...

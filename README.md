# ImoSphere

## User Manual for ImoSphere

### Introduction


### Prerequisites
- .NET SDK installed on your system.
- A supported database (e.g., SQL Server, MySQL, PostgreSQL) set up and running.
- Connection string details for the database.

### Installation
1. Clone the repository:
    ```bash
    git clone <repository-url>
    ```
2. Navigate to the project directory:
    ```bash
    cd <project-directory>
    ```
3. Restore dependencies:
    ```bash
    dotnet restore
    ```
4. Add the migrations:
    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

### Configuration
1. Open the `appsettings.json` file.
2. Verify the `ConnectionStrings` section has the database connection details:
    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "Data Source=ImoSphereDb.db"
        }
    }
    ```

### Running the Application
1. Build the project:
    ```bash
    dotnet build
    ```
2. Run the application:
    ```bash
    dotnet run
    ```
or just:
    ```
    dotnet watch
    ```

### Features
- **Database Integration**: Perform CRUD operations on the database.
- **User Authentication**: Secure login and registration system.
- **Reporting**: Generate and export reports.

### Troubleshooting
- **Database Connection Issues**: Verify the connection string in `appsettings.json`.
- **Missing Dependencies**: Run `dotnet restore` to install missing packages.
- **Runtime Errors**: Check the logs for detailed error messages.

## Team ImoSphere
- Alexandre Miguel, 202200037
    -
    - Database
    - Controlers and Migrations
    - Debug

- Bruna Rossa, 202200603
    -
    - App Design
    - Views and Models
    - Debug

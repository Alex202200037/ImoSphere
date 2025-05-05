# ImoSphere

## User Manual for ImoSphere

### Introduction


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

### Configuration
1. Open the `appsettings.json` file.
2. Ensure the `ConnectionStrings` section has the database connection details:
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
### Users:
1. Admin user:
    - **Email**: admin@imosphere.com
    - **Password**: Admin@123
    - ***Can manage users, properties and messages.***
2. Seller:
    - **Email**: seller@imosphere.com
    - **Password**: Seller@123
    - ***Can manage properties only.***
2. Regular user:
    - **Email**: user@imosphere.com
    - **Password**: User@123
    - ***Can only view properties.***

## Team ImoSphere
- Alexandre Miguel, 202200037
    -
    - Email: 202200037@estudante.ips.pt

- Bruna Rossa, 202200603
    -
    - Email: 202200603@estudante.ips.pt

# ImoSphere

## User Manual for ImoSphere

### Introduction
O projeto ImoSphere consiste numa plataforma web desenvolvida com o objetivo de facilitar a mediação imobiliária digital, permitindo a navegação, visualização e gestão de propriedades por diferentes tipos de utilizadores, de acordo com os respetivos níveis de acesso. A aplicação foi desenvolvida em ambiente ASP.NET MVC com integração de base de dados SQLite para autenticação e gestão de conteúdos.

A plataforma distingue-se por oferecer experiências adaptadas a quatro tipos de utilizadores:

Utilizador Não Registado (Convidado): Pode explorar o site e aceder a páginas informativas como a página inicial, “Sobre Nós”, “Serviços”, “Contactos” e a listagem de propriedades. Contudo, não tem acesso aos detalhes completos das propriedades nem pode interagir com funcionalidades mais avançadas.
Utilizador Registado (User): Após efetuar o registo e login, ganha acesso a funcionalidades adicionais, como o sistema de mensagens (simulado) com outros utilizadores, mantendo ainda acesso a todas as funcionalidades do convidado.
Vendedor (Seller): Tem a possibilidade de criar, editar e gerir as suas propriedades, com validações e notificações de sucesso. Partilha com os utilizadores registados o acesso ao sistema de mensagens.
Administrador (Admin): Possui acesso total à plataforma. Para além das funcionalidades dos Sellers, o Administrador pode gerir todos os utilizadores (criar, editar, eliminar) e controlar as mensagens enviadas através do formulário de contacto, incluindo marcar mensagens como lidas e eliminá-las. Tem ainda permissões para eliminar propriedades de qualquer utilizador.
A interface foi concebida de forma intuitiva, com base em boas práticas de usabilidade, e com uma clara distinção entre as permissões de cada tipo de utilizador. A navegação é apoiada por menus consistentes, mensagens de validação e confirmação, bem como feedback visual sobre todas as ações realizadas.

Este relatório detalha o comportamento da aplicação conforme o tipo de utilizador, suportado por capturas de ecrã das principais funcionalidades e páginas.

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
3. Ensure the MS EFC is installed:
    ```
    dotnet add package Microsoft.EntityFrameworkCore
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

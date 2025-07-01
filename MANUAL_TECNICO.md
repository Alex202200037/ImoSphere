# Manual Técnico — ImoSphere

## Arquitetura
- ASP.NET Core MVC
- Entity Framework Core (ORM)
- Base de dados SQLite (local)

## Estrutura de Pastas
- `Models/` — Modelos de dados
- `Controllers/` — Controladores MVC
- `Views/` — Vistas Razor
- `Data/` — Contexto EF Core

## Configuração
- Connection string definida em `appsettings.json`.
- Para migrar a base de dados:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

## Desenvolvimento
- Para adicionar novos modelos, cria uma classe em `Models/` e adiciona ao contexto.
- Para criar controladores, usa scaffolding ou cria manualmente em `Controllers/`.
- As views são ficheiros `.cshtml` em `Views/`.

## Dependências
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.AspNetCore.Mvc

## Notas
- O projeto está preparado para evoluir para outros SGBD alterando a connection string e o provider EF Core. 
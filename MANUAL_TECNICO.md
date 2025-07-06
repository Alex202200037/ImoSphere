# 🔧 Manual Técnico — ImoSphere

> **Documentação técnica completa para desenvolvedores e administradores de sistema**

## 📋 Índice

- [🏗️ Arquitetura](#️-arquitetura)
- [🗄️ Base de Dados](#️-base-de-dados)
- [📁 Estrutura do Projeto](#-estrutura-do-projeto)
- [🔧 Configuração](#-configuração)
- [🚀 Desenvolvimento](#-desenvolvimento)
- [🔌 APIs e Endpoints](#-apis-e-endpoints)
- [🔐 Autenticação e Autorização](#-autenticação-e-autorização)
- [📊 Migrações](#-migrações)
- [🐛 Troubleshooting](#-troubleshooting)
- [📦 Deploy](#-deploy)

## 🏗️ Arquitetura

### Stack Tecnológico
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Base de Dados**: SQLite (desenvolvimento) / SQL Server (produção)
- **Autenticação**: ASP.NET Core Identity
- **Comunicação Real-time**: SignalR
- **Frontend**: Bootstrap 5.3, jQuery, Font Awesome
- **Mapas**: Leaflet.js + OpenStreetMap

### Padrões Arquiteturais
- **MVC (Model-View-Controller)**: Separação clara de responsabilidades
- **Repository Pattern**: Abstração da camada de dados
- **Dependency Injection**: Injeção de dependências nativa do ASP.NET Core
- **Identity Framework**: Gestão de utilizadores e roles

## 🗄️ Base de Dados

### Esquema Principal

#### Tabelas de Utilizadores (Identity)
```sql
-- Tabelas padrão do ASP.NET Core Identity
AspNetUsers          -- Utilizadores do sistema
AspNetRoles          -- Roles (SuperAdmin, Admin, Comercial, User)
AspNetUserRoles      -- Relação utilizador-role
AspNetUserClaims     -- Claims dos utilizadores
AspNetUserLogins     -- Logins externos
AspNetUserTokens     -- Tokens de autenticação
AspNetRoleClaims     -- Claims das roles
```

#### Tabelas de Negócio
```sql
Agencies             -- Agências imobiliárias
AgencyUsers          -- Relação utilizador-agência
Properties           -- Propriedades imobiliárias
PropertyImages       -- Imagens das propriedades
Messages             -- Sistema de mensagens
```

### Relacionamentos
- **AgencyUsers**: N:N entre Users e Agencies
- **Properties**: N:1 com Agencies e Users (CreatedBy)
- **PropertyImages**: N:1 com Properties
- **Messages**: N:1 com Users (Sender/Receiver)

### Índices Recomendados
```sql
-- Performance para consultas frequentes
CREATE INDEX IX_Properties_AgencyId ON Properties(AgencyId);
CREATE INDEX IX_Properties_CreatedByUserId ON Properties(CreatedByUserId);
CREATE INDEX IX_Properties_Location ON Properties(Location);
CREATE INDEX IX_PropertyImages_PropertyId ON PropertyImages(PropertyId);
```

## 📁 Estrutura do Projeto

```
ImoSphere/
├── Controllers/           # Controladores MVC
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── ChatController.cs
│   ├── HomeController.cs
│   └── PropertyController.cs
├── Data/                  # Camada de dados
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Models/                # Modelos de dados
│   ├── Agency.cs
│   ├── AgencyUser.cs
│   ├── ApplicationUser.cs
│   ├── Property.cs
│   ├── PropertyImage.cs
│   └── ViewModels/
├── Views/                 # Vistas Razor
│   ├── Account/
│   ├── Admin/
│   ├── Chat/
│   ├── Home/
│   ├── Properties/
│   └── Shared/
├── wwwroot/              # Ficheiros estáticos
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/
├── Migrations/           # Migrações EF Core
├── Program.cs           # Ponto de entrada
└── appsettings.json     # Configuração
```

## 🔧 Configuração

### Ficheiros de Configuração

#### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ImoSphereDb.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### Variáveis de Ambiente
```bash
# Desenvolvimento
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Data Source=ImoSphereDb.db

# Produção
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...
```

## 🚀 Desenvolvimento

### Pré-requisitos
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code
- SQLite (desenvolvimento) ou SQL Server (produção)

### Comandos Essenciais
```bash
# Restaurar dependências
dotnet restore

# Compilar projeto
dotnet build

# Executar em modo desenvolvimento
dotnet run

# Executar com auto-reload
dotnet watch

# Executar testes
dotnet test
```

### Adicionar Novas Funcionalidades

#### 1. Criar Modelo
```csharp
// Models/NovoModelo.cs
public class NovoModelo
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataCriacao { get; set; }
}
```

#### 2. Adicionar ao Contexto
```csharp
// Data/ApplicationDbContext.cs
public DbSet<NovoModelo> NovoModelos { get; set; }
```

#### 3. Criar Migração
```bash
dotnet ef migrations add AddNovoModelo
dotnet ef database update
```

#### 4. Criar Controller
```csharp
// Controllers/NovoModeloController.cs
public class NovoModeloController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public NovoModeloController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        return View(await _context.NovoModelos.ToListAsync());
    }
}
```

#### 5. Criar Views
```bash
# Scaffold automático (se aplicável)
dotnet aspnet-codegenerator controller -name NovoModeloController -m NovoModelo -dc ApplicationDbContext --relativeFolderPath Controllers
```

## 🔌 APIs e Endpoints

### Endpoints Principais

#### Propriedades
```
GET    /Properties              # Listar propriedades
GET    /Properties/{id}         # Detalhes da propriedade
POST   /Properties              # Criar propriedade
PUT    /Properties/{id}         # Atualizar propriedade
DELETE /Properties/{id}         # Eliminar propriedade
```

#### Utilizadores
```
GET    /Admin/Users             # Listar utilizadores (Admin)
POST   /Admin/CreateUser        # Criar utilizador
PUT    /Admin/EditUser/{id}     # Editar utilizador
DELETE /Admin/DeleteUser/{id}   # Eliminar utilizador
```

#### Chat
```
GET    /Chat                    # Interface de chat
POST   /Chat/SendMessage        # Enviar mensagem
GET    /Chat/GetMessages        # Obter mensagens
```

### APIs AJAX
```javascript
// Exemplo: Obter comerciais por agência
fetch('/Properties/GetComerciaisByAgency?agencyId=' + agencyId)
    .then(response => response.json())
    .then(data => {
        // Processar dados
    });

// Exemplo: Verificar dependências de utilizador
fetch('/Admin/CheckUserDependencies?id=' + userId)
    .then(response => response.json())
    .then(data => {
        // Processar resposta
    });
```

## 🔐 Autenticação e Autorização

### Roles e Permissões
```csharp
// Roles definidas no sistema
"SuperAdmin"  // Controlo total
"Admin"       // Gestão de utilizadores e propriedades
"Comercial"   // Gestão de propriedades próprias
"User"        // Visualização e chat
```

### Autorização em Controllers
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Apenas admins podem aceder
}

[Authorize]
public class PropertyController : Controller
{
    // Utilizadores autenticados
}
```

### Autorização em Views
```razor
@if (User.IsInRole("Admin"))
{
    <button>Admin Action</button>
}

@if (User.Identity.IsAuthenticated)
{
    <span>Bem-vindo, @User.Identity.Name!</span>
}
```

## 📊 Migrações

### Comandos de Migração
```bash
# Criar nova migração
dotnet ef migrations add NomeDaMigracao

# Aplicar migrações pendentes
dotnet ef database update

# Reverter última migração
dotnet ef database update NomeDaMigracaoAnterior

# Gerar script SQL
dotnet ef migrations script

# Remover última migração
dotnet ef migrations remove
```

### Seed Data
```csharp
// Data/SeedData.cs
public static async Task Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
{
    // Verificar se já existem dados
    if (context.Properties.Any())
        return;
    
    // Criar dados iniciais
    // ...
}
```

## 🐛 Troubleshooting

### Problemas Comuns

#### 1. Erro de Base de Dados
```
SQLite Error 1: 'no such table: Agencies'
```
**Solução:**
```bash
dotnet ef database update
```

#### 2. Erro de Migração
```
table "AspNetRoles" already exists
```
**Solução:**
```bash
# Eliminar base de dados e recriar
rm ImoSphereDb.db*
dotnet ef database update
```

#### 3. Erro de Dependências
```
Failed to resolve service for type 'ApplicationDbContext'
```
**Solução:** Verificar registo no `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

#### 4. Erro de Permissões
```
Access denied for user
```
**Solução:** Verificar connection string e permissões de utilizador.

### Logs e Debugging
```csharp
// Habilitar logs detalhados
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

## 📦 Deploy

### Preparação para Produção
1. **Alterar base de dados** para SQL Server ou PostgreSQL
2. **Configurar HTTPS** e certificados SSL
3. **Definir variáveis de ambiente** de produção
4. **Configurar logging** para ficheiro ou serviço externo
5. **Otimizar performance** (caching, CDN, etc.)

### Docker (Opcional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ImoSphere.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImoSphere.dll"]
```

### Comandos de Deploy
```bash
# Publicar para produção
dotnet publish -c Release -o ./publish

# Executar em produção
dotnet ImoSphere.dll --environment Production
```

---

## 📞 Suporte

Para questões técnicas ou problemas:
- **Emails**: 202200037@estudante.ips.pt, 202200603@estudante.ips.pt
- **Equipa**: Alexandre Miguel, Bruna Rossa
- **Instituição**: ESTSetúbal - IPS

---

*Manual Técnico v2.0 - ImoSphere* 
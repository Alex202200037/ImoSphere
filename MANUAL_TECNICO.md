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

## 🏗️ Arquitetura

### Stack Tecnológico
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Base de Dados**: SQLite (desenvolvimento) / SQL Server (produção)
- **Autenticação**: ASP.NET Core Identity
- **Comunicação Real-time**: SignalR
- **Frontend**: Bootstrap 5.3, jQuery, Font Awesome
- **Design Responsivo**: Interface moderna, com modo claro (fundo branco, texto preto, cartões brancos, roxo claro, header/footer escuros) e modo escuro (fundo escuro, texto claro, cartões escuros). O utilizador pode alternar facilmente entre os modos, garantindo sempre excelente contraste, acessibilidade e conforto visual.
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
AgencyUsers          -- Relação utilizador-agência com hierarquia
Properties           -- Propriedades imobiliárias
PropertyImages       -- Imagens das propriedades
Favorites            -- Sistema de favoritos (User-Property)
ChatConversations    -- Conversas de chat
ChatMessages         -- Mensagens de chat
Messages             -- Sistema de mensagens de contacto
```

### Relacionamentos
- **AgencyUsers**: N:N entre Users e Agencies com hierarquia (AdminId)
- **Properties**: N:1 com Agencies e Users (CreatedBy)
- **PropertyImages**: N:1 com Properties
- **Favorites**: N:N entre Users e Properties
- **ChatConversations**: N:1 com Properties, Users (como User e Comercial)
- **ChatMessages**: N:1 com ChatConversations e Users (Sender)
- **Messages**: Sistema de contacto independente

### DER e MER do Sistema

O diagrama seguinte representa as principais entidades do sistema e os seus relacionamentos:
![Diagrama Entidade-Relacionamento ImoSphere](./wwwroot/images/Manuals/DER.png)

**Entidades e Relações:**
- **ApplicationUser**: Utilizador do sistema (herda de IdentityUser)
- **Agency**: Agência imobiliária
- **Property**: Imóvel
- **PropertyImage**: Imagem de imóvel
- **Favorite**: Favorito (ligação entre utilizador e imóvel)
- **AgencyUser**: Relação N:N entre utilizador e agência com hierarquia
- **ChatConversation**: Conversa de chat por propriedade
- **ChatMessage**: Mensagem individual de chat

**Relações principais:**
- Uma agência tem vários imóveis e vários utilizadores (AgencyUser)
- Um imóvel pertence a uma agência e pode ter várias imagens
- Um imóvel é criado por um utilizador
- Um utilizador pode ter vários favoritos (imóveis)
- Um favorito liga um utilizador a um imóvel
- Uma conversa de chat está associada a uma propriedade específica
- Uma mensagem pertence a uma conversa e tem um remetente

## 📁 Estrutura do Projeto

```
ImoSphere/
├── bin/                     # Ficheiros de build/output
├── Controllers/             # Controladores MVC
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── ChatController.cs
│   ├── FavoriteController.cs
│   ├── HomeController.cs
│   ├── LanguageController.cs
│   ├── PropertyController.cs
├── Data/                    # Camada de dados
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Migrations/              # Migrações EF Core
├── Models/                  # Modelos de domínio e ViewModels
│   ├── Agency.cs
│   ├── AgencyUser.cs
│   ├── ApplicationUser.cs
│   ├── ChatConversation.cs
│   ├── ChatMessage.cs
│   ├── ChatViewModel.cs
│   ├── EditUserView.cs
│   ├── ErrorViewModel.cs
│   ├── Favorite.cs
│   ├── LoginViewModel.cs
│   ├── MarkAsReadRequest.cs
│   ├── Message.cs
│   ├── Property.cs
│   ├── PropertyFilterViewModel.cs
│   ├── PropertyImage.cs
│   ├── RegisterViewModel.cs
│   ├── UserHierarchyViewModel.cs
│   ├── UserWithRolesViewModel.cs
├── obj/                     # Ficheiros temporários de build
├── Properties/              # Configurações do projeto (ex: launchSettings)
├── Resources/               # Ficheiros de localização (resx)
├── Views/                   # Vistas Razor
│   ├── Account/
│   ├── Admin/
│   ├── Chat/
│   ├── Home/
│   ├── Properties/
│   └── Shared/
├── wwwroot/                 # Ficheiros estáticos
│   ├── css/
│   ├── images/
│   ├── js/
│   └── lib/
├── .gitignore
├── appsettings.json
├── appsettings.Development.json
├── ImoSphere.csproj
├── ImoSphere.sln
├── MANUAL_TECNICO.md
├── MANUAL_UTILIZADOR.md
├── Program.cs
├── README.md
├── UserManual_ImoSphere.pdf
└── ImoSphereDb.db
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
```

### Scripts de Execução
```bash
# Executar projeto (modo normal)
./run.sh

# Executar projeto (hot reload)
./watch.sh

# Executar testes
./test.sh
```

## 🧪 Testes

### Estrutura de Testes
```
Tests/
├── ImoSphere.Tests.csproj    # Projeto de testes
├── TestBase.cs                # Classe base para testes
├── Controllers/               # Testes dos controllers
│   ├── HomeControllerTests.cs
│   └── PropertyControllerTests.cs
└── Data/                      # Testes da base de dados
    └── ApplicationDbContextTests.cs
```

### Execução de Testes
```bash
# Compilar testes
dotnet build Tests/ImoSphere.Tests.csproj

# Executar todos os testes
dotnet test Tests/ImoSphere.Tests.csproj

# Executar testes específicos
dotnet test Tests/ImoSphere.Tests.csproj --filter "FullyQualifiedName~HomeController"

# Executar com relatório de cobertura
dotnet test Tests/ImoSphere.Tests.csproj --collect:"XPlat Code Coverage"
```

### Tipos de Testes Implementados

#### Testes de Controllers
- **HomeController**: Testes de filtros de propriedades
- **PropertyController**: Testes CRUD de propriedades

#### Testes de Base de Dados
- **ApplicationDbContext**: Testes de relacionamentos e operações CRUD

## 🔌 APIs e Endpoints

### Controllers Principais

#### HomeController
```csharp
// GET: /Home/Index
// GET: /Home/Properties (com filtros)
// GET: /Home/Perfil (perfil do utilizador)
// POST: /Home/SubmitContactForm
// GET: /Home/ContactUsMessages (SuperAdmin)
```

#### PropertyController
```csharp
// GET: /Properties/Index
// GET: /Properties/Details/{id}
// GET: /Properties/Create
// POST: /Properties/Create
// GET: /Properties/Edit/{id}
// POST: /Properties/Edit/{id}
// POST: /Properties/Delete/{id}
```

#### FavoriteController
```csharp
// POST: /Favorite/ToggleFavorite
// GET: /Favorite/GetFavorites
// POST: /Favorite/RemoveFavorite
// GET: /Favorite/CheckFavorite
```

#### ChatController
```csharp
// GET: /Chat/Index
// GET: /Chat/Conversations
// POST: /Chat/SendMessage
// POST: /Chat/MarkAsRead
```

#### AdminController
```csharp
// GET: /Admin/Users
// GET: /Admin/CreateUser
// POST: /Admin/CreateUser
// GET: /Admin/EditUser/{id}
// POST: /Admin/EditUser/{id}
```

#### AccountController
```csharp
// GET: /Account/Login
// POST: /Account/Login
// GET: /Account/Register
// POST: /Account/Register
// POST: /Account/Logout
```

### Endpoints JSON (APIs)

#### Sistema de Favoritos
```javascript
// Toggle favorite
POST /Favorite/ToggleFavorite
Body: { "propertyId": 123 }

// Remove favorite
POST /Favorite/RemoveFavorite
Body: { "favoriteId": 456 }

// Check if favorite
GET /Favorite/CheckFavorite?propertyId=123
```

#### Sistema de Chat
```javascript
// Send message
POST /Chat/SendMessage
Body: { "conversationId": 789, "text": "Hello" }

// Mark as read
POST /Chat/MarkAsRead
Body: { "conversationId": 789 }
```

## 🔐 Autenticação e Autorização

### Roles e Permissões

#### SuperAdmin
- **Acesso total** a todas as funcionalidades
- **Gestão de agências** e administradores
- **Relatórios globais** e estatísticas
- **Gestão de mensagens** de contacto

#### Admin
- **Gestão de utilizadores** da agência
- **Supervisão de comerciais** da agência
- **Gestão de propriedades** da agência
- **Relatórios da agência**

#### Comercial
- **Criação e edição** de propriedades próprias
- **Upload de imagens** para propriedades
- **Chat com clientes** sobre propriedades
- **Gestão de listagens** pessoais

#### User
- **Visualização** de propriedades
- **Sistema de favoritos**
- **Chat com comerciais**
- **Perfil pessoal**

### Políticas de Segurança

#### Validação de Dados
- **ModelState Validation**: Validação automática de formulários
- **Anti-forgery Tokens**: Proteção CSRF em formulários
- **Input Sanitization**: Limpeza de dados de entrada

#### Autorização por Controller
```csharp
[Authorize]                    // Requer autenticação
[Authorize(Roles = "Admin")]   // Requer role específica
[AllowAnonymous]               // Permite acesso anónimo
```

## 📊 Migrações

### Estrutura de Migrações
```
Migrations/
├── 20250707191146_InitialCreate.cs
├── 20250707191146_InitialCreate.Designer.cs
├── 20250707214638_AddFavoritesTable.cs
├── 20250707214638_AddFavoritesTable.Designer.cs
└── ApplicationDbContextModelSnapshot.cs
```

### Comandos de Migração
```bash
# Criar nova migração
dotnet ef migrations add NomeDaMigracao

# Aplicar migrações
dotnet ef database update

# Reverter migração
dotnet ef database update NomeDaMigracaoAnterior

# Remover migração
dotnet ef migrations remove
```

### Migrações Principais

#### InitialCreate
- Criação das tabelas base do sistema
- Tabelas Identity (AspNetUsers, etc.)
- Tabelas de negócio (Agencies, Properties, etc.)

#### AddFavoritesTable
- Adição do sistema de favoritos
- Tabela Favorites com relacionamentos

## 🐛 Troubleshooting

### Problemas Comuns

#### Erro de Base de Dados
```bash
# Reset da base de dados
rm ImoSphereDb.db
dotnet ef database update
```

#### Erro de Compilação
```bash
# Limpar e restaurar
dotnet clean
dotnet restore
dotnet build
```

#### Erro de Dependências
```bash
# Atualizar pacotes
dotnet list package --outdated
dotnet add package NomeDoPacote --version NovaVersao
```

#### Problemas de Chat (SignalR)
- Verificar se o SignalR está configurado no Program.cs
- Confirmar se o hub está registado corretamente
- Verificar logs do browser para erros de JavaScript

#### Problemas de Favoritos
- Verificar se o FavoriteController está a aceitar JSON corretamente
- Confirmar se o JavaScript está a enviar os dados no formato correto
- Verificar logs do servidor para erros de binding

### Logs e Debugging

#### Configuração de Logs
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ImoSphere.Controllers": "Debug"
    }
  }
}
```

#### Debug de JavaScript
```javascript
// No browser console
console.log('Debug info:', data);
```

#### Debug de C#
```csharp
// No controller
Console.WriteLine($"[DEBUG] Info: {data}");
```

### Performance

#### Otimizações de Base de Dados
- **Include()**: Carregar dados relacionados
- **AsNoTracking()**: Para consultas só de leitura
- **Pagination**: Limitar resultados grandes

#### Otimizações de Frontend
- **Lazy Loading**: Carregar imagens sob demanda
- **Minificação**: CSS e JS minificados
- **Caching**: Cache de recursos estáticos

---

## 📄 Licença

Este projeto está licenciado sob a Licença MIT.

## 👥 Equipa

- **Alexandre Miguel** - Desenvolvimento Backend e Frontend
- **Bruna Rossa** - Desenvolvimento Backend e Frontend
- **Unidade Curricular** - Programação Visual
- **Docente** - José Cordeiro

---

**ImoSphere** - Transformando a mediação imobiliária digital 🏠✨ 
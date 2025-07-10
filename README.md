# 🏠 ImoSphere - Plataforma de Mediação Imobiliária Digital

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-green.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-yellow.svg)](https://www.sqlite.org/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> **Plataforma web completa para mediação imobiliária com gestão de propriedades, utilizadores hierárquicos, sistema de favoritos e comunicação integrada em tempo real.**

[📖 Manual do Utilizador](/MANUAL_UTILIZADOR.md) | [📖 Manual do Utilizador (PDF)](/UserManual_ImoSphere.pdf) | [🔧 Manual Técnico](/MANUAL_TECNICO.md)

## 📋 Índice

- [🏢 Sobre o Projeto](#-sobre-o-projeto)
- [✨ Funcionalidades](#-funcionalidades)
- [🛠️ Tecnologias](#️-tecnologias)
- [🚀 Instalação](#-instalação)
- [⚙️ Configuração](#️-configuração)
- [👥 Tipos de Utilizador](#-tipos-de-utilizador)
- [🔑 Contas de Teste](#-contas-de-teste)
- [📱 Screenshots](#-screenshots)
- [👨‍💻 Equipa](#-equipa)

## 🏢 Sobre o Projeto

A **ImoSphere** é uma plataforma web desenvolvida em **ASP.NET Core MVC** que facilita a mediação imobiliária digital. A aplicação oferece uma experiência completa para navegação, visualização e gestão de propriedades, com diferentes níveis de acesso baseados no tipo de utilizador e hierarquia organizacional.

### 🎯 Objetivos

- **Gestão de Propriedades**: Criação, edição e eliminação de listagens imobiliárias
- **Sistema de Utilizadores Hierárquico**: Gestão de agências, administradores e comerciais
- **Sistema de Favoritos**: Utilizadores podem marcar propriedades como favoritas
- **Comunicação Integrada**: Sistema de chat em tempo real com SignalR
- **Interface Responsiva**: Design moderno com modo claro/escuro
- **Gestão de Agências**: Suporte a múltiplas agências imobiliárias

## ✨ Funcionalidades

### 🔍 Exploração de Propriedades
- **Listagem Completa**: Visualização de todas as propriedades disponíveis
- **Filtros Avançados**: Por preço, localização, quartos, área, agência, etc.
- **Detalhes Completos**: Informações detalhadas com galeria de imagens
- **Mapa Interativo**: Localização geográfica com OpenStreetMap
- **Sistema de Favoritos**: Marcar/desmarcar propriedades como favoritas

### 👤 Sistema de Utilizadores Hierárquico
- **Registo e Login**: Sistema de autenticação seguro
- **Hierarquia de Agências**: SuperAdmin → Admin → Comercial → User
- **Gestão de Utilizadores**: Criação, edição e eliminação por nível
- **Perfis Personalizados**: Diferentes níveis de acesso por agência
- **Sistema de Favoritos**: Gestão de propriedades favoritas por utilizador

### 💬 Comunicação em Tempo Real
- **Chat com SignalR**: Sistema de mensagens em tempo real
- **Conversas por Propriedade**: Chat específico por imóvel
- **Notificações**: Badge de mensagens não lidas
- **Formulário de Contacto**: Comunicação direta com a plataforma
- **Gestão de Mensagens**: Marcação como lidas e eliminação

### 🏗️ Gestão de Conteúdo
- **Upload de Imagens**: Suporte a múltiplas imagens por propriedade
- **Validação de Dados**: Verificação automática de formulários
- **Notificações**: Feedback visual para todas as ações
- **Auto-save**: Guarda automática de rascunhos
- **Gestão de Favoritos**: Interface para gerir propriedades favoritas

### 🎨 Interface Moderna
- **Design Responsivo**: Adaptável a todos os dispositivos
- **Modo Claro/Escuro**: Alternância entre temas
- **Animações**: Transições suaves e feedback visual
- **Acessibilidade**: Contraste e navegação otimizados

## 🛠️ Tecnologias

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core** - ORM para base de dados
- **SQLite** - Base de dados relacional
- **SignalR** - Comunicação em tempo real
- **Identity Framework** - Autenticação e autorização

### Frontend
- **Bootstrap 5.3** - Framework CSS responsivo
- **jQuery** - Manipulação do DOM
- **Font Awesome** - Ícones
- **Leaflet.js** - Mapas interativos
- **OpenStreetMap** - Serviço de mapas
- **SignalR Client** - Comunicação em tempo real

### Ferramentas de Desenvolvimento
- **Visual Studio 2022** / **VS Code**
- **Git** - Controlo de versões
- **Entity Framework Tools** - Migrações de base de dados

## 🚀 Instalação

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Passos de Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/Alex202200037/ImoSphere.git
   cd ImoSphere
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Configurar a base de dados**
   ```bash
   dotnet ef database update
   ```

4. **Executar a aplicação**
   ```bash
   # Modo normal
   dotnet run
   
   # Modo desenvolvimento (hot reload)
   dotnet watch
   
   # Usar scripts (se disponíveis)
   ./run.sh      # Modo normal
   ./watch.sh    # Hot reload
   ```

5. **Aceder à aplicação**
   ```
   https://localhost:5151
   ```

## ⚙️ Configuração

### Base de Dados
A aplicação utiliza SQLite por padrão. O ficheiro `appsettings.json` contém a configuração:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ImoSphereDb.db"
  }
}
```

### Variáveis de Ambiente
Para produção, configure as seguintes variáveis:
- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ConnectionStrings__DefaultConnection`: String de conexão da base de dados

### Migrações
Para atualizar o esquema da base de dados:
```bash
dotnet ef migrations add NomeDaMigracao
dotnet ef database update
```

### 🧪 Testes
A aplicação inclui uma suite completa de testes unitários:

```bash
# Executar todos os testes
dotnet test Tests/ImoSphere.Tests.csproj

# Executar testes específicos
dotnet test Tests/ImoSphere.Tests.csproj --filter "FullyQualifiedName~HomeController"

# Usar script de testes
./test.sh
```

**Tipos de Testes:**
- **Controllers**: Testes de endpoints e lógica de negócio
- **Base de Dados**: Testes de relacionamentos e operações CRUD
- **Autenticação**: Mock de utilizadores e verificação de roles

## 👥 Tipos de Utilizador

### 🎯 Utilizador Não Registado (Convidado)
- ✅ Explorar propriedades
- ✅ Aceder a páginas informativas
- ✅ Ver listagens básicas
- ❌ Detalhes completos de propriedades
- ❌ Sistema de mensagens
- ❌ Sistema de favoritos

### 👤 Utilizador Registado (User)
- ✅ Todas as funcionalidades do convidado
- ✅ Detalhes completos de propriedades
- ✅ Sistema de mensagens e chat
- ✅ Sistema de favoritos
- ✅ Perfil pessoal
- ❌ Gestão de propriedades

### 🏠 Comercial (Comercial)
- ✅ Todas as funcionalidades do User
- ✅ Criar propriedades
- ✅ Editar propriedades próprias
- ✅ Upload de imagens
- ✅ Gestão de listagens pessoais
- ✅ Chat com clientes

### 👨‍💼 Administrador (Admin)
- ✅ Todas as funcionalidades do Comercial
- ✅ Gestão de utilizadores da agência
- ✅ Gestão de todas as propriedades da agência
- ✅ Sistema de mensagens administrativo
- ✅ Supervisão de comerciais
- ✅ Relatórios da agência

### 🔧 Super Administrador (SuperAdmin)
- ✅ Todas as funcionalidades do Admin
- ✅ Gestão de todas as agências
- ✅ Criação de administradores
- ✅ Controlo total da plataforma
- ✅ Relatórios globais
- ✅ Gestão de mensagens de contacto

### 🔑 Contas de Teste

#### 👑 Super Administrador
| Email                             | Password      |
|-----------------------------------|---------------|
| **imosphere.admin@imosphere.com** | Imosphere@123 |

#### 👨‍💼 Administradores por Agência
| Agência           | Email                          | Password   |
|-------------------|------------------------------- |----------- |
| **ERA**           | jguilherme.era@imosphere.com   | Admin@123  |
| **REMAX**         | candrade.remax@imosphere.com   | Admin@123  |
| **Century21**     | mramos.century21@imosphere.com | Admin@123  |
| **KW**            | vnunes.kw@imosphere.com        | Admin@123  |
| **Fine and Country** | afaria.fac@imosphere.com    | Admin@123  |

#### 🏠 Comerciais por Agência
| Agência           | Email                            | Password      |
|-------------------|--------------------------------- |-------------- |
| **ERA**           | tsilva.era@imosphere.com         | Comercial@123 |
| **ERA**           | mlopes.era@imosphere.com         | Comercial@123 |
| **ERA**           | rcosta.era@imosphere.com         | Comercial@123 |
| **REMAX**         | apereira.remax@imosphere.com     | Comercial@123 |
| **REMAX**         | psousa.remax@imosphere.com       | Comercial@123 |
| **Century21**     | smartins.century21@imosphere.com | Comercial@123 |
| **Century21**     | balves.century21@imosphere.com   | Comercial@123 |
| **KW**            | rpinto.kw@imosphere.com          | Comercial@123 |
| **KW**            | hcruz.kw@imosphere.com           | Comercial@123 |
| **Fine and Country** | pdias.fac@imosphere.com       | Comercial@123 |
| **Fine and Country** | lamaral.fac@imosphere.com     | Comercial@123 |

#### 👤 Utilizador Regular
| Email                  | Password   |
|------------------------|----------- |
| **user@imosphere.com** | User@123   |

## 📱 Screenshots

### Página Inicial
![Página Inicial](wwwroot/images/Manuals/screenshots/homepage.png)

### Listagem de Propriedades
![Propriedades](wwwroot/images/Manuals/screenshots/properties.png)

### Sistema de Chat
![Chat](wwwroot/images/Manuals/screenshots/chat.png)

### Criação de Propriedades
![Criar Propriedade](wwwroot/images/Manuals/screenshots/create-property.png)

## 👨‍💻 Equipa

- **Alexandre Miguel e Bruna Rossa** - Desenvolvimento
- **Unidade Curricular** - Programação Visual
- **Docente** - José Cordeiro

---

**ImoSphere** - Transformando a mediação imobiliária digital 🏠✨

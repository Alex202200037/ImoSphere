# 👥 Manual do Utilizador — ImoSphere

> **Guia completo para utilizadores da plataforma de mediação imobiliária digital**

## 📋 Índice

- [🏠 Sobre o ImoSphere](#-sobre-o-imosphere)
- [🚀 Primeiros Passos](#-primeiros-passos)
- [👤 Tipos de Utilizador](#-tipos-de-utilizador)
- [🔍 Explorar Propriedades](#-explorar-propriedades)
- [❤️ Sistema de Favoritos](#️-sistema-de-favoritos)
- [🏗️ Gestão de Propriedades](#️-gestão-de-propriedades)
- [👥 Gestão de Utilizadores](#-gestão-de-utilizadores)
- [💬 Sistema de Comunicação](#-sistema-de-comunicação)
- [⚙️ Configurações e Perfil](#️-configurações-e-perfil)
- [❓ FAQ](#-faq)
- [📞 Suporte](#-suporte)

## 🏠 Sobre o ImoSphere

O **ImoSphere** é uma plataforma web moderna e intuitiva para mediação imobiliária digital. Permite a gestão completa de propriedades, utilizadores hierárquicos e comunicação entre todos os intervenientes no processo imobiliário.

### 🎯 Principais Funcionalidades
- **Gestão de Propriedades**: Criação, edição e eliminação de listagens
- **Sistema de Utilizadores Hierárquico**: SuperAdmin → Admin → Comercial → User
- **Sistema de Favoritos**: Marcar propriedades como favoritas
- **Comunicação Integrada**: Chat em tempo real com SignalR
- **Mapas Interativos**: Localização geográfica das propriedades
- **Upload de Imagens**: Galeria de fotos para cada propriedade
- **Filtros Avançados**: Pesquisa por múltiplos critérios

## 🚀 Primeiros Passos

### 1. Aceder à Aplicação
```bash
# No terminal, navegar para a pasta do projeto
cd ImoSphere

# Opção 1: Executar a aplicação (modo normal)
dotnet run

# Opção 2: Executar com hot reload (recomendado para desenvolvimento)
dotnet watch

# Opção 3: Usar scripts (se disponíveis)
./run.sh      # Modo normal
./watch.sh    # Hot reload

# Abrir no browser
https://localhost:5151
```

### 2. Primeiro Login
1. **Clicar** em "Login" no menu superior
2. **Inserir** as credenciais fornecidas
3. **Clicar** em "Sign In"

### 3. Navegação Básica
- **Menu Superior**: Acesso rápido a todas as funcionalidades
- **Breadcrumbs**: Indicam a localização atual
- **Notificações**: Feedback visual para todas as ações
- **Modo Claro/Escuro**: Alternar tema no topo da página

## 👤 Tipos de Utilizador

### 🎯 Utilizador Não Registado (Convidado)
**Funcionalidades Disponíveis:**
- ✅ Explorar propriedades na página inicial
- ✅ Ver listagens básicas de propriedades
- ✅ Aceder a páginas informativas (Sobre Nós, Serviços, Contactos)
- ❌ Detalhes completos de propriedades
- ❌ Sistema de mensagens
- ❌ Sistema de favoritos

**Como Utilizar:**
1. Navegar pela página inicial
2. Clicar em "Browse Properties" para ver propriedades
3. Utilizar filtros básicos de pesquisa

### 👤 Utilizador Registado (User)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do convidado
- ✅ Detalhes completos de propriedades
- ✅ Sistema de mensagens e chat
- ✅ Sistema de favoritos
- ✅ Perfil pessoal
- ❌ Gestão de propriedades

**Como Utilizar:**
1. **Registar conta** ou fazer login
2. **Explorar propriedades** com acesso completo
3. **Marcar favoritos** com o botão de coração
4. **Utilizar o chat** para comunicar com outros utilizadores
5. **Gerir perfil** pessoal

### 🏠 Comercial (Comercial)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do User
- ✅ Criar propriedades
- ✅ Editar propriedades próprias
- ✅ Upload de imagens
- ✅ Gestão de listagens pessoais
- ✅ Chat com clientes

**Como Utilizar:**
1. **Fazer login** com credenciais de comercial
2. **Criar propriedades** através do botão "Adicionar Propriedade"
3. **Gerir propriedades** na secção "Minhas Propriedades"
4. **Upload de imagens** durante a criação/edição
5. **Responder a mensagens** de clientes

### 👨‍💼 Administrador (Admin)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do Comercial
- ✅ Gestão de utilizadores da agência
- ✅ Gestão de todas as propriedades da agência
- ✅ Sistema de mensagens administrativo
- ✅ Supervisão de comerciais
- ✅ Relatórios da agência

**Como Utilizar:**
1. **Aceder ao painel administrativo**
2. **Gerir utilizadores** da agência
3. **Supervisionar propriedades** de todos os comerciais
4. **Responder a mensagens** de contacto
5. **Criar novos comerciais** e administradores

### 🔧 Super Administrador (SuperAdmin)
**Funcionalidades Disponíveis:**
- ✅ Controlo total da plataforma
- ✅ Gestão de todas as agências
- ✅ Criação de administradores
- ✅ Configurações globais
- ✅ Relatórios completos
- ✅ Gestão de mensagens de contacto

**Como Utilizar:**
1. **Aceder ao painel de controlo global**
2. **Gerir agências** e administradores
3. **Configurar** parâmetros do sistema
4. **Monitorizar** toda a atividade

## 🔍 Explorar Propriedades

### Listagem de Propriedades
1. **Clicar** em "Properties" no menu
2. **Utilizar filtros** para refinar a pesquisa:
   - **Preço**: Mínimo e máximo
   - **Localização**: Cidade ou região
   - **Quartos**: Número de quartos
   - **Área**: Metros quadrados
   - **Agência**: Filtrar por agência específica

### Detalhes de Propriedade
1. **Clicar** numa propriedade da listagem
2. **Ver informações completas**:
   - Descrição detalhada
   - Galeria de imagens
   - Características técnicas
   - Localização no mapa
   - Informações de contacto

### Filtros Avançados
- **Ordenação**: Por preço, área, data, etc.
- **Vista de mapa**: Visualizar propriedades geograficamente
- **Favoritos**: Marcar propriedades de interesse

## ❤️ Sistema de Favoritos

### Marcar como Favorito
1. **Navegar** para uma propriedade
2. **Clicar** no botão de coração (❤️)
3. **O coração fica vermelho** quando marcado como favorito
4. **Clicar novamente** para remover dos favoritos

### Gerir Favoritos
1. **Aceder ao perfil** pessoal
2. **Ver secção "Favoritos"**
3. **Visualizar** todas as propriedades favoritas
4. **Remover favoritos** com o botão de toggle

### Funcionalidades dos Favoritos
- **Contador**: Número total de favoritos no perfil
- **Grid responsivo**: 3 propriedades por linha em desktop
- **Remoção instantânea**: Sem necessidade de refresh
- **Animações**: Transições suaves ao adicionar/remover

## 🏗️ Gestão de Propriedades

### Criar Nova Propriedade
1. **Clicar** em "Adicionar Propriedade"
2. **Preencher formulário**:
   - **Informações básicas**: Nome, descrição, preço
   - **Características**: Quartos, WC, área, ano construção
   - **Localização**: Endereço e coordenadas GPS
   - **Imagens**: Upload de múltiplas fotos
3. **Clicar** em "Adicionar Propriedade"

### Editar Propriedade
1. **Aceder** aos detalhes da propriedade
2. **Clicar** em "Editar"
3. **Modificar** campos necessários
4. **Guardar** alterações

### Eliminar Propriedade
1. **Aceder** aos detalhes da propriedade
2. **Clicar** em "Eliminar"
3. **Confirmar** a ação

### Upload de Imagens
- **Formatos aceites**: JPG, PNG, GIF
- **Tamanho máximo**: 5MB por imagem
- **Número**: Múltiplas imagens por propriedade
- **Destaque**: Primeira imagem será a principal

## 👥 Gestão de Utilizadores

### Criar Utilizador (Admin/SuperAdmin)
1. **Aceder** ao painel administrativo
2. **Clicar** em "Criar Utilizador"
3. **Preencher** informações:
   - Email e username
   - Role (Comercial, Admin, etc.)
   - Agência (se aplicável)
4. **Definir** password inicial
5. **Criar** utilizador

### Editar Utilizador
1. **Selecionar** utilizador da lista
2. **Clicar** em "Editar"
3. **Modificar** informações necessárias
4. **Guardar** alterações

### Eliminar Utilizador
1. **Selecionar** utilizador da lista
2. **Clicar** em "Eliminar"
3. **Verificar** dependências (propriedades, mensagens)
4. **Transferir** dependências se necessário
5. **Confirmar** eliminação

## 💬 Sistema de Comunicação

### Chat em Tempo Real
1. **Aceder** à secção "Chat"
2. **Selecionar** utilizador para conversar
3. **Escrever** mensagem
4. **Enviar** com Enter ou botão

### Conversas por Propriedade
1. **Aceder** aos detalhes de uma propriedade
2. **Clicar** em "Contactar" se disponível
3. **Iniciar** conversa sobre a propriedade específica
4. **Receber** notificações de novas mensagens

### Mensagens de Contacto
1. **Aceder** à página "Contactos"
2. **Preencher** formulário:
   - Nome e email
   - Assunto
   - Mensagem
3. **Enviar** mensagem

### Gestão de Mensagens (Admin)
1. **Aceder** ao painel administrativo
2. **Ver** lista de mensagens recebidas
3. **Marcar** como lida
4. **Responder** ou eliminar

### Notificações
- **Badge de mensagens**: Indicador de mensagens não lidas
- **Notificações em tempo real**: Atualizações automáticas
- **Som de notificação**: Alertas sonoros (se configurado)

## ⚙️ Configurações e Perfil

### Editar Perfil Pessoal
1. **Clicar** no nome de utilizador no menu
2. **Selecionar** "Perfil"
3. **Modificar** informações pessoais
4. **Alterar** password se necessário
5. **Guardar** alterações

### Configurações de Notificações
- **Email**: Receber notificações por email
- **Sistema**: Notificações na aplicação
- **Chat**: Alertas de mensagens

### Preferências de Visualização
- **Tema**: Claro (fundo branco, texto preto, cartões brancos, roxo claro, header/footer escuros) ou Escuro (fundo escuro, texto claro, cartões escuros). O utilizador pode alternar facilmente entre os modos no topo do site, garantindo sempre excelente contraste e conforto visual.
- **Idiomas**: Português (padrão), inglês e espanhol
- **Densidade**: Compacto/normal

### Perfil por Tipo de Utilizador

#### 👤 Perfil de User
- **Favoritos**: Lista de propriedades favoritas
- **Informações pessoais**: Dados básicos
- **Histórico**: Atividade recente

#### 🏠 Perfil de Comercial
- **Propriedades criadas**: Lista de imóveis próprios
- **Estatísticas**: Número de propriedades
- **Chat**: Conversas com clientes

#### 👨‍💼 Perfil de Admin
- **Gestão de utilizadores**: Lista de comerciais supervisionados
- **Estatísticas da agência**: Dados da agência
- **Relatórios**: Informações de performance

#### 🔧 Perfil de SuperAdmin
- **Gestão global**: Todas as agências
- **Estatísticas globais**: Dados de toda a plataforma
- **Relatórios completos**: Análises detalhadas

## ❓ FAQ

### Como marcar uma propriedade como favorita?
Clica no botão de coração (❤️) na listagem ou detalhes da propriedade. O coração fica vermelho quando marcado como favorito.

### Como remover um favorito?
Clica novamente no coração vermelho ou vai ao perfil e remove da lista de favoritos.

### Como aceder ao chat?
Faz login e clica em "Chat" no menu. Podes conversar com outros utilizadores em tempo real.

### Como criar uma propriedade?
Se és Comercial, Admin ou SuperAdmin, clica em "Adicionar Propriedade" e preenche o formulário.

### Como alterar o tema (claro/escuro)?
Clica no botão de alternar tema no topo da página.

### Como filtrar propriedades?
Na página de propriedades, utiliza os filtros por preço, localização, quartos, etc.

### Como contactar um comercial?
Nos detalhes da propriedade, clica em "Contactar" se disponível.

### Como ver as minhas propriedades favoritas?
Acede ao teu perfil e vê a secção "Favoritos".

## 📞 Suporte

### Contactos
- **Email**: 202200037@estudantes.ips.pt, 202200603@estudantes.ips.pt

### Recursos de Ajuda
- **Manual Técnico**: Para administradores e desenvolvedores
- **FAQ**: Perguntas frequentes
- **Tutoriais**: Guias passo-a-passo

### Reportar Problemas
1. **Descrever** o problema detalhadamente
2. **Incluir** screenshots se possível
3. **Especificar** o tipo de utilizador
4. **Mencionar** o browser utilizado

---

**ImoSphere** - Transformando a mediação imobiliária digital 🏠✨ 
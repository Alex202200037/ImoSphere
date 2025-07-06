# 👥 Manual do Utilizador — ImoSphere

> **Guia completo para utilizadores da plataforma de mediação imobiliária digital**

## 📋 Índice

- [🏠 Sobre o ImoSphere](#-sobre-o-imosphere)
- [🚀 Primeiros Passos](#-primeiros-passos)
- [👤 Tipos de Utilizador](#-tipos-de-utilizador)
- [🔍 Explorar Propriedades](#-explorar-propriedades)
- [🏗️ Gestão de Propriedades](#️-gestão-de-propriedades)
- [👥 Gestão de Utilizadores](#-gestão-de-utilizadores)
- [💬 Sistema de Comunicação](#-sistema-de-comunicação)
- [⚙️ Configurações e Perfil](#️-configurações-e-perfil)
- [❓ FAQ](#-faq)
- [📞 Suporte](#-suporte)

## 🏠 Sobre o ImoSphere

O **ImoSphere** é uma plataforma web moderna e intuitiva para mediação imobiliária digital. Permite a gestão completa de propriedades, utilizadores e comunicação entre todos os intervenientes no processo imobiliário.

### 🎯 Principais Funcionalidades
- **Gestão de Propriedades**: Criação, edição e eliminação de listagens
- **Sistema de Utilizadores**: Hierarquia de permissões por agência
- **Comunicação Integrada**: Chat em tempo real e sistema de mensagens
- **Mapas Interativos**: Localização geográfica das propriedades
- **Upload de Imagens**: Galeria de fotos para cada propriedade
- **Filtros Avançados**: Pesquisa por múltiplos critérios

## 🚀 Primeiros Passos

### 1. Aceder à Aplicação
```bash
# No terminal, navegar para a pasta do projeto
cd ImoSphere

# Executar a aplicação
dotnet run

# Abrir no browser
https://localhost:5001
```

### 2. Primeiro Login
1. **Clicar** em "Login" no menu superior
2. **Inserir** as credenciais fornecidas
3. **Clicar** em "Sign In"

### 3. Navegação Básica
- **Menu Superior**: Acesso rápido a todas as funcionalidades
- **Breadcrumbs**: Indicam a localização atual
- **Notificações**: Feedback visual para todas as ações

## 👤 Tipos de Utilizador

### 🎯 Utilizador Não Registado (Convidado)
**Funcionalidades Disponíveis:**
- ✅ Explorar propriedades na página inicial
- ✅ Ver listagens básicas de propriedades
- ✅ Aceder a páginas informativas (Sobre Nós, Serviços, Contactos)
- ❌ Detalhes completos de propriedades
- ❌ Sistema de mensagens

**Como Utilizar:**
1. Navegar pela página inicial
2. Clicar em "Browse Properties" para ver propriedades
3. Utilizar filtros básicos de pesquisa

### 👤 Utilizador Registado (User)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do convidado
- ✅ Detalhes completos de propriedades
- ✅ Sistema de mensagens e chat
- ✅ Perfil pessoal
- ❌ Gestão de propriedades

**Como Utilizar:**
1. **Registar conta** ou fazer login
2. **Explorar propriedades** com acesso completo
3. **Utilizar o chat** para comunicar com outros utilizadores
4. **Gerir perfil** pessoal

### 🏠 Comercial (Seller)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do User
- ✅ Criar propriedades
- ✅ Editar propriedades próprias
- ✅ Upload de imagens
- ✅ Gestão de listagens pessoais

**Como Utilizar:**
1. **Fazer login** com credenciais de comercial
2. **Criar propriedades** através do botão "Adicionar Propriedade"
3. **Gerir propriedades** na secção "Minhas Propriedades"
4. **Upload de imagens** durante a criação/edição

### 👨‍💼 Administrador (Admin)
**Funcionalidades Disponíveis:**
- ✅ Todas as funcionalidades do Comercial
- ✅ Gestão de utilizadores da agência
- ✅ Gestão de todas as propriedades da agência
- ✅ Sistema de mensagens administrativo
- ✅ Relatórios e estatísticas

**Como Utilizar:**
1. **Aceder ao painel administrativo**
2. **Gerir utilizadores** da agência
3. **Supervisionar propriedades** de todos os comerciais
4. **Responder a mensagens** de contacto

### 🔧 Super Administrador (SuperAdmin)
**Funcionalidades Disponíveis:**
- ✅ Controlo total da plataforma
- ✅ Gestão de todas as agências
- ✅ Criação de administradores
- ✅ Configurações globais
- ✅ Relatórios completos

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
- **Tema**: Claro/escuro
- **Idioma**: Português (padrão)
- **Densidade**: Compacto/normal

## ❓ FAQ

### 🔐 Autenticação
**Q: Esqueci-me da password. O que faço?**
A: Contacte o administrador da sua agência para reset da password.

**Q: Posso alterar o meu email?**
A: Sim, através do perfil pessoal ou contactando o administrador.

### 🏠 Propriedades
**Q: Quantas imagens posso adicionar por propriedade?**
A: Não há limite, mas recomendamos máximo 10 imagens de qualidade.

**Q: Posso editar propriedades de outros utilizadores?**
A: Apenas administradores podem editar propriedades de outros utilizadores.

**Q: Como funciona o sistema de coordenadas GPS?**
A: As coordenadas são automaticamente obtidas ao inserir o endereço, ou podem ser definidas manualmente no mapa.

### 💬 Comunicação
**Q: O chat funciona em tempo real?**
A: Sim, utiliza SignalR para comunicação instantânea.

**Q: Posso enviar ficheiros pelo chat?**
A: Atualmente apenas texto é suportado no chat.

### 🗺️ Mapas
**Q: Que serviço de mapas é utilizado?**
A: OpenStreetMap, um serviço gratuito e de código aberto.

**Q: Posso adicionar coordenadas manualmente?**
A: Sim, através do botão "Escolher no mapa" no formulário de propriedades.

## 📞 Suporte

### Contactos de Suporte
- **Emails**: 202200037@estudante.ips.pt, 202200603@estudante.ips.pt
- **Equipa**: Alexandre Miguel, Bruna Rossa
- **Instituição**: ESTSetúbal - IPS

### Horário de Suporte
- **Segunda a Sexta**: 9:00 - 18:00
- **Fim de semana**: Suporte por email

### Informações Úteis
- **Versão**: ImoSphere v2.0
- **Navegadores Suportados**: Chrome, Firefox, Safari, Edge
- **Dispositivos**: Desktop

---

## 🎯 Dicas de Utilização

### Para Comerciais
- **Mantenha as propriedades atualizadas** com informações precisas
- **Use imagens de qualidade** para atrair mais interesse
- **Responda rapidamente** às mensagens dos clientes
- **Utilize descrições detalhadas** para destacar características únicas

### Para Administradores
- **Monitore regularmente** a atividade dos comerciais
- **Verifique as mensagens** de contacto diariamente
- **Mantenha a base de dados** de utilizadores atualizada
- **Analise os relatórios** para identificar tendências

### Para Utilizadores
- **Utilize os filtros** para encontrar propriedades específicas
- **Guarde propriedades** de interesse para consulta posterior
- **Contacte os comerciais** para mais informações
- **Partilhe feedback** sobre a plataforma

---

*Manual do Utilizador v2.0 - ImoSphere* 
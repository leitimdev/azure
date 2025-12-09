# Repositório Azure - Projetos e Laboratórios

Este repositório contém diversos projetos e laboratórios voltados para o desenvolvimento e estudo de serviços Azure e ASP.NET Core.

## 📁 Projetos

### 1. AZ204-Labs

Conjunto de laboratórios práticos para a certificação AZ-204 (Developing Solutions for Microsoft Azure).

#### **cosmosdb**
Aplicação console em C# para gerenciamento de dados no Azure Cosmos DB.

**Funcionalidades:**
- Conexão com Azure Cosmos DB
- Criação de databases e containers
- Operações CRUD (Create, Read, Update, Delete)
- Gerenciamento de produtos
- Menu interativo para operações

**Stack Tecnológica:**
- .NET 10.0
- Azure Cosmos DB SDK (v3.35.4)
- Newtonsoft.Json
- C#

---

#### **lab03**
Aplicação console para trabalhar com Azure Blob Storage.

**Funcionalidades:**
- Criação de containers no Azure Blob Storage
- Upload de arquivos (blobs)
- Listagem de blobs
- Download de arquivos
- Exclusão de containers
- Menu interativo para gerenciamento

**Stack Tecnológica:**
- .NET 10.0
- Azure Storage Blobs SDK (v12.19.1)
- C#

---

### 2. Netflix API

API RESTful desenvolvida em ASP.NET Core que fornece dados de previsão do tempo.

**Funcionalidades:**
- Endpoint para obter previsões meteorológicas
- Arquitetura em camadas (Controllers, Services, Models)
- Suporte a OpenAPI/Swagger
- Injeção de dependências

**Endpoints:**
- `GET /weatherforecast` - Retorna previsões para os próximos 5 dias

**Stack Tecnológica:**
- .NET 6.0
- ASP.NET Core Web API
- OpenAPI (Swagger)
- Newtonsoft.Json
- C#

**Estrutura do Projeto:**
```
netflix-api/
├── Controllers/          # Controladores da API
├── Services/            # Lógica de negócio
├── Models/              # Modelos de dados
├── Properties/          # Configurações de inicialização
└── appsettings.json     # Configurações da aplicação
```

---

## 🚀 Como Executar os Projetos

### Pré-requisitos
- .NET SDK 6.0 ou superior
- Conta Azure (para os laboratórios AZ-204)
- Visual Studio 2022, VS Code ou Rider

### AZ204-Labs - CosmosDB
```bash
cd az204-labs/cosmosdb
dotnet restore
dotnet run
```

**Importante:** Configure as credenciais do Azure Cosmos DB no arquivo `Program.cs`:
- `EndpointUri`
- `PrimaryKey`

### AZ204-Labs - Lab03 (Blob Storage)
```bash
cd az204-labs/lab03
dotnet restore
dotnet run
```

**Importante:** Configure a connection string do Azure Storage Account no arquivo `Program.cs`.

### Netflix API
```bash
cd netflix-api
dotnet restore
dotnet run
```

Acesse a API em: `https://localhost:5001/weatherforecast`

---

## 📚 Recursos de Aprendizado

- [Documentação Azure Cosmos DB](https://docs.microsoft.com/azure/cosmos-db/)
- [Documentação Azure Blob Storage](https://docs.microsoft.com/azure/storage/blobs/)
- [Certificação AZ-204](https://docs.microsoft.com/learn/certifications/exams/az-204)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)

---

## 📝 Notas

- Os projetos AZ-204 são laboratórios práticos para estudo da certificação Azure Developer
- Todas as credenciais devem ser configuradas antes de executar os projetos
- A Netflix API é um projeto de exemplo para demonstrar conceitos de Web API

---

## 📄 Licença

Este repositório contém projetos de estudo e laboratórios práticos.

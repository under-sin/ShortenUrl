# Projeto de Testes - ShortenUrl

Este projeto contém testes unitários e de integração para a aplicação ShortenUrl.

## Estrutura de Pastas

```
ShortenUrl.Tests/
├── Unit/
│   └── Application/
│       └── ShortenUrlServiceTests.cs       # Testes unitários do serviço de encurtamento
│
└── Integration/
    ├── IntegrationTestWebAppFactory.cs      # Factory customizada para testes de integração
    └── ShortenUrlControllerIntegrationTests.cs  # Testes de integração da API
```

## Testes Unitários

### Unit/Application/ShortenUrlServiceTests.cs

Testes unitários para o `ShortenUrlService` usando Moq e Shouldly:

- ✅ `ShortenAsync_ShouldReturnShortCode_WhenUrlIsValid` - Valida geração de código curto
- ✅ `ShortenAsync_ShouldThrowException_WhenRepositoryFails` - Valida tratamento de erros
- ✅ `GetOriginalUrlAsync_ShouldReturnOriginalUrl_WhenShortCodeExists` - Valida recuperação de URL
- ✅ `GetOriginalUrlAsync_ShouldReturnNull_WhenShortCodeDoesNotExist` - Valida URL não encontrada
- ✅ `GetOriginalUrlAsync_ShouldReturnNull_WhenShortCodeIsNullOrEmpty` - Valida entrada inválida

**Tecnologias:**
- xUnit
- Moq (para mocks de dependências)
- Shouldly (para assertions fluentes)

## Testes de Integração

### Integration/ShortenUrlControllerIntegrationTests.cs

Testes end-to-end da API usando WebApplicationFactory e Testcontainers:

- ✅ `ShortenUrl_ShouldReturnCreated_WhenRequestIsValid` - POST válido retorna 201
- ✅ `ShortenUrl_ShouldReturnBadRequest_WhenUrlIsEmpty` - POST com URL vazia retorna 400
- ✅ `RedirectToOriginalUrl_ShouldReturnRedirect_WhenShortCodeExists` - GET retorna 302
- ✅ `RedirectToOriginalUrl_ShouldReturnNotFound_WhenShortCodeDoesNotExist` - GET retorna 404
- ✅ `ShortenUrl_ShouldGenerateDifferentCodes_ForDifferentUrls` - Valida unicidade de códigos
- ✅ `FullWorkflow_ShouldWork_EndToEnd` - Testa fluxo completo (criar + redirecionar)

**Tecnologias:**
- xUnit
- Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
- Testcontainers.PostgreSql (banco de dados em container Docker)
- Shouldly (para assertions fluentes)

### Como Funciona a Integração

A classe `IntegrationTestWebAppFactory` configura:
1. Container PostgreSQL usando Testcontainers
2. Aplicação ASP.NET Core com configurações de teste
3. Migrations automáticas do banco de dados
4. Limpeza automática após os testes

## Executando os Testes

### Todos os testes:
```bash
dotnet test
```

### Apenas testes unitários:
```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

### Apenas testes de integração:
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

### Com cobertura de código:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Requisitos

- **.NET 8.0 SDK**
- **Docker** (para testes de integração com Testcontainers)
  - ⚠️ **Importante**: Os testes de integração requerem que o Docker esteja rodando
  - Se o Docker não estiver disponível, execute apenas os testes unitários

## Pacotes Utilizados

- **xUnit** - Framework de testes
- **Moq** - Biblioteca de mocking
- **Shouldly** - Assertions fluentes
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração para APIs
- **Testcontainers.PostgreSql** - Containers PostgreSQL para testes
- **Microsoft.Extensions.Configuration** - Configuração para testes

## Boas Práticas Implementadas

1. ✅ **Separação de testes** - Unitários e de integração em pastas separadas
2. ✅ **Padrão AAA** - Arrange-Act-Assert em todos os testes
3. ✅ **Nomes descritivos** - Seguem padrão `MethodName_Should_When`
4. ✅ **Isolamento** - Cada teste é independente com banco in-memory
5. ✅ **Testes rápidos** - SQLite in-memory sem necessidade de Docker
6. ✅ **Mocks apropriados** - Redis mockado para testes determinísticos
7. ✅ **Assertions fluentes** - Shouldly para melhor legibilidade
8. ✅ **Factory pattern** - Reutilização da configuração de testes de integração


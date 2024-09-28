# VendaAPI

VendaAPI é um sistema de gerenciamento de vendas desenvolvido com .NET 8, que utiliza Entity Framework Core para persistência em MySQL e Azure Service Bus para publicação de eventos. Este repositório contém a API de vendas, incluindo lógica de negócios encapsulada nas entidades de domínio, e um sistema robusto de testes automatizados.

## Funcionalidades

- **Gerenciamento de Vendas**: Adicionar, atualizar, buscar e remover vendas.
- **Cálculo Automático**: Cálculo automático de valor total da venda e descontos.
- **Publicação de Eventos**: Publicação de eventos no Azure Service Bus para acompanhamento em tempo real.
- **Testes Automatizados**: Cobertura de testes unitários e de integração com xUnit e Moq, incluindo integração com Azure Service Bus e MySQL.

## Tecnologias Utilizadas

- **.NET 8**
- **Entity Framework Core**
- **MySQL**
- **Azure Service Bus**
- **AutoMapper**
- **Serilog**
- **xUnit e Moq** (para testes automatizados)
- **Testcontainers** (para integração com MySQL em testes)

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (necessário para testes de integração com MySQL)
- [Azure Service Bus](https://azure.microsoft.com/en-us/services/service-bus/) (necessário para testes de integração com o Service Bus)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (ou superior) com a carga de trabalho de desenvolvimento .NET instalada

## Configuração do Projeto

### 1. Clonar o Repositório

Clone o repositório para sua máquina local:

```bash
[git clone https://github.com/seu-usuario/API-Vendas.git](https://github.com/Wellington-Paulo-dias/API-Vendas.git)
````

### 2. Configuração do Banco de Dados

Crie um banco de dados MySQL e configure a string de conexão em appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=VendaApiDb;User=root;Password=your_password;",
    "ServiceBusConnection": "Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your_shared_access_key;"
  }
}
```
### 3. Criar um Serviço Azure Service Bus

Para que a aplicação funcione corretamente, é necessário criar um namespace do Azure Service Bus no portal do Azure. Após a criação, configure uma fila chamada sbq-queue-vendas. Substitua a string de conexão do Service Bus gerada pelo Azure no arquivo appsettings.json e no arquivo de testes ServiceBusTests.cs.

### 4. Aplicar Migrações

Aplique as migrações para criar as tabelas no banco de dados:

```bash
dotnet ef database update --project Vendas.Data
```

### 5.Executando a Aplicação

```bash
dotnet run --project Vendas.API
```
A API estará disponível em https://localhost:5001.

### 6. Endpoints

### Vendas

* GET /api/vendas/{id}: Obter venda por ID.
* GET /api/vendas/: Obter todas as vendas.
* POST /api/vendas/: Adicionar uma nova venda.
* PUT /api/vendas/{id}: Atualizar uma venda existente.
* DELETE /api/vendas/{id}: Remover uma venda.

Exemplo de Requisição para Adicionar uma Venda

```json
POST /api/vendas/
{
    "dataVenda": "2024-09-27T00:00:00Z",
    "cliente": "João da Silva",
    "filial": "Filial Centro",
    "itens": [
        {
            "produto": "Produto A",
            "quantidade": 2,
            "valorUnitario": 100.00,
            "desconto": 10.00
        },
        {
            "produto": "Produto B",
            "quantidade": 1,
            "valorUnitario": 200.00,
            "desconto": 20.00
        }
    ]
}
```

### Exemplo de Resposta

```json
{
    "id": "b1c3a4d6-e3d6-4b12-8ec7-14c3a4d8b7a7",
    "dataVenda": "2024-09-27T00:00:00Z",
    "cliente": "João da Silva",
    "filial": "Filial Centro",
    "cancelado": false,
    "itens": [
        {
            "id": "f1c3a4d6-e3d6-4b12-8ec7-14c3a4d8b7a7",
            "produto": "Produto A",
            "quantidade": 2,
            "valorUnitario": 100.00,
            "desconto": 10.00,
            "valorTotal": 180.00
        },
        {
            "id": "g1c3a4d6-e3d6-4b12-8ec7-14c3a4d8b7a7",
            "produto": "Produto B",
            "quantidade": 1,
            "valorUnitario": 200.00,
            "desconto": 20.00,
            "valorTotal": 180.00
        }
    ],
    "valorTotal": 360.00,
    "descontoTotal": 30.00
}

```

### 7. Testes do Projeto de Vendas


## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (necessário para testes de integração com MySQL)
- [Azure Service Bus](https://azure.microsoft.com/en-us/services/service-bus/) (necessário para testes de integração com o Service Bus)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (ou superior) com a carga de trabalho de desenvolvimento .NET instalada

## Estrutura dos Testes

### Testes de Unidade

Os testes de unidade estão localizados no arquivo `VendaServiceTests.cs`. Eles testam a lógica de negócios do serviço de vendas (`VendaService`) usando mocks para dependências como repositórios e publicadores de eventos.

### Recursos Úteis

### Instalação do Docker no Windows

Para instalar o Docker no Windows, siga o guia oficial:
[Instalar Docker Desktop no Windows](https://docs.docker.com/desktop/install/windows-install/)

### Configuração do Azure Service Bus

Para configurar o Azure Service Bus, siga o guia rápido oficial:
[Guia Rápido do Azure Service Bus](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-quickstart-portal)

### Testes de Integração

Os testes de integração estão localizados nos arquivos `VendaApiIntegrationTests.cs` e `ServiceBusTests.cs`.

- `VendaApiIntegrationTests.cs`: Testa a integração do serviço de vendas com um banco de dados MySQL usando contêineres Docker.
- `ServiceBusTests.cs`: Testa a integração do serviço de vendas com o Azure Service Bus.

## Configuração e Execução dos Testes

Os testes de integração estão localizados nos arquivos `VendaApiIntegrationTests.cs` e `ServiceBusTests.cs`.

- `VendaApiIntegrationTests.cs`: Testa a integração do serviço de vendas com um banco de dados MySQL usando contêineres Docker.
- `ServiceBusTests.cs`: Testa a integração do serviço de vendas com o Azure Service Bus.

## Configuração e Execução dos Testes

### 1. Configurar Variáveis de Ambiente

Certifique-se de configurar as seguintes variáveis de ambiente para os testes de integração com o Azure Service Bus:

```bash
export ServiceBusConnectionString="sua-string-de-conexao-do-service-bus" export TopicName="sbq-quee-vendas"
```
### Detalhamento
1.	export ServiceBusConnectionString="sua-string-de-conexao-do-service-bus":

* Este comando define uma variável de ambiente chamada ServiceBusConnectionString.
*	O valor desta variável é "sua-string-de-conexao-do-service-bus", que deve ser substituído pela string de conexão real do seu Azure Service Bus.
*	A string de conexão é usada para autenticar e conectar-se ao Azure Service Bus.

2.	export TopicName="sbq-quee-vendas":

*	Este comando define uma variável de ambiente chamada TopicName.
*	O valor desta variável é "sbq-quee-vendas", que deve ser substituído pelo nome real do tópico que você está usando no Azure Service Bus.
*	O nome do tópico é usado para especificar o tópico no qual as mensagens serão publicadas ou do qual serão consumidas.
Propósito

Essas variáveis de ambiente são geralmente usadas em testes de integração ou em configurações de aplicativos para fornecer informações de configuração sensíveis, como strings de conexão e nomes de tópicos, sem codificá-las diretamente no código-fonte. Isso melhora a segurança e facilita a configuração em diferentes ambientes (desenvolvimento, teste, produção).


### 3. Abrir o Projeto no Visual Studio

1. Abra o Visual Studio.
2. Selecione **File > Open > Project/Solution**.
3. Navegue até o diretório onde você clonou o repositório e selecione o arquivo `.sln`.

### 4. Executar os Testes de Unidade

Para executar os testes de unidade no Visual Studio:

1. Abra o **Test Explorer** (menu **Test > Test Explorer**).
2. No **Test Explorer**, você verá uma lista de todos os testes.
3. Filtre os testes para exibir apenas os testes de unidade (`VendaServiceTests`).
4. Clique com o botão direito em `VendaServiceTests` e selecione **Run Selected Tests**.

### 5. Executar os Testes de Integração

#### Testes de Integração com MySQL

Os testes de integração com MySQL usam contêineres Docker para criar um banco de dados temporário. Certifique-se de que o Docker está instalado e em execução.

Para executar os testes de integração com MySQL no Visual Studio:

1. No **Test Explorer**, filtre os testes para exibir apenas os testes de integração com MySQL (`VendaApiIntegrationTests`).
2. Clique com o botão direito em `VendaApiIntegrationTests` e selecione **Run Selected Tests**.

#### Testes de Integração com Azure Service Bus

Para executar os testes de integração com o Azure Service Bus no Visual Studio:

1. No **Test Explorer**, filtre os testes para exibir apenas os testes de integração com o Azure Service Bus (`ServiceBusTests`).
2. Clique com o botão direito em `ServiceBusTests` e selecione **Run Selected Tests**.

### Detalhes dos Testes

### VendaServiceTests.cs

- **ObterPorIdAsync_DeveRetornarVendaDTO_QuandoVendaExistir**: Testa se o método `ObterPorIdAsync` retorna uma venda quando ela existe.
- **ObterPorIdAsync_DeveRetornarNull_QuandoVendaNaoExistir**: Testa se o método `ObterPorIdAsync` retorna `null` quando a venda não existe.
- **ObterTodasAsync_DeveRetornarListaDeVendaDTOs**: Testa se o método `ObterTodasAsync` retorna uma lista de vendas.
- **AdicionarAsync_DeveAdicionarVendaEPublicarEvento**: Testa se o método `AdicionarAsync` adiciona uma venda e publica um evento.
- **AtualizarAsync_DeveAtualizarVendaEPublicarEvento**: Testa se o método `AtualizarAsync` atualiza uma venda e publica um evento.
- **RemoverAsync_DeveRemoverVendaEPublicarEvento_QuandoVendaExistir**: Testa se o método `RemoverAsync` remove uma venda e publica um evento quando a venda existe.
- **RemoverAsync_NaoDevePublicarEvento_QuandoVendaNaoExistir**: Testa se o método `RemoverAsync` não publica um evento quando a venda não existe.

### VendaApiIntegrationTests.cs

- **DeveInserirEVenderProdutoCorretamente**: Testa se uma venda é inserida corretamente no banco de dados MySQL.

### ServiceBusTests.cs

- **DevePublicarEventoCompraCriadaNoServiceBus**: Testa se um evento de compra criada é publicado corretamente no Azure Service Bus.

## Contribuição

Sinta-se à vontade para abrir issues e pull requests para melhorias e correções.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).



















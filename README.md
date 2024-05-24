# Cadastro de Contatos

Bem-vindo ao repositório do RockHub! desenvolvido em C# com ASP.NET Core utilizando Blazor para a interface do usuário. Este projeto é uma aplicação web destinada a gerenciar artistas e músicas.
O projeto é destinado a ser um exemplo básico de uma aplicação web utilizando ASP.Net Core e pode servir como base para projetos mais complexos. 

## Funcionalidades

- Gerenciamento de Artistas: Adicionar, editar, visualizar e excluir informações sobre artistas musicais.
- Gerenciamento de Músicas: Adicionar, editar, visualizar e excluir informações sobre músicas, incluindo nome, ano de lançamento e gêneros associados.
- Gerenciamento de Albuns: Adicionar, editar, visualizar e excluir informações sobre albuns.
- Operações CRUD: Implementação de operações básicas de criação, leitura, atualização e exclusão para artistas e músicas.


## Tecnologias Utilizadas

- Blazor WebAssembly: Framework para desenvolvimento de interfaces de usuário interativas baseadas em WebAssembly.
- Entity Framework Core: Biblioteca para mapeamento objeto-relacional (ORM) e acesso a banco de dados.
- SQL Server: Banco de dados utilizado para armazenamento de dados.
- C#: Linguagem de programação utilizada no desenvolvimento da aplicação.

## Pré-requisitos

- [.NET Core SDK](https://dotnet.microsoft.com/download) instalado.
- [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/) para desenvolvimento (opcional).
- Banco de dados SQL Server ou outro banco de dados compatível.

## Configuração do Projeto

1. Clone este repositório em sua máquina local.
2. Configure a conexão com o banco de dados no arquivo `RockHub.API>appsettings.json` para API e `RockHub.Shared.Dados>RockHubContext.cs`.
3. Abra o projeto em sua IDE preferida.
4. Execute as migrações para criar o banco de dados.
5. Inicie o aplicativo.

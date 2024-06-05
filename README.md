# RockHub

Bem-vindo! Este projeto foi desenvolvido em C# utilizando ASP.NET Core e Blazor para a interface do usuário. 
Ele consiste em uma aplicação web integrada com uma API WEB, destinada ao gerenciamento de artistas, músicas, gêneros musicais e Albuns.

## Descrição

O RockHub é uma aplicação web construída para demonstrar uma solução completa de gerenciamento musical. 
A aplicação inclui um frontend desenvolvido com Blazor e um backend robusto utilizando ASP.NET Core com uma API WEB. 
Este projeto serve como um exemplo prático e pode ser usado como base para desenvolvimentos mais complexos.

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

using Microsoft.AspNetCore.Mvc;
using RockHub.API.Requests;
using RockHub.API.Response;
using RockHub.Shared.Dados.Banco;
using RockHub.Shared.Modelos.Modelos;

namespace RockHub.API.Endpoints;

public static class GeneroExtensions
{

    public static void AddEndPointGeneros(this WebApplication app)
    {
        var grupoBuilder = app.MapGroup("generos")
            .RequireAuthorization()
            .WithTags("Gêneros");

        #region Endpoint Gênero musical

        /// <summary>
        /// Endpoint para adicionar um novo gênero.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos gêneros.</param>
        /// <param name="generoReq">Dados do gênero a ser criado.</param>
        /// <returns>Retorna 200 (OK) se o gênero for adicionado com sucesso.</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL. Ele converte os dados da requisição em uma entidade de gênero e a adiciona ao banco de dados.</remarks>
        grupoBuilder.MapPost("", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoReq) =>
        {
            dal.Adicionar(RequestToEntity(generoReq));
        });

        /// <summary>
        /// Endpoint para obter uma lista de gêneros.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos gêneros.</param>
        /// <returns>Retorna uma lista de gêneros.</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL e retorna a lista de gêneros disponíveis.</remarks>
        grupoBuilder.MapGet("", ([FromServices] DAL<Genero> dal) =>
        {
            return EntityListToResponseList(dal.Listar());
        });

        /// <summary>
        /// Endpoint para obter um gênero pelo nome.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos gêneros.</param>
        /// <param name="nome">Nome do gênero a ser buscado.</param>
        /// <returns>Retorna os dados do gênero se encontrado. Se o gênero não for encontrado, retorna 404 (Not Found).</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL. Ele busca um gênero pelo nome fornecido, realizando uma busca case-insensitive.</remarks>
        grupoBuilder.MapGet("{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
        {
            var genero = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (genero is not null)
            {
                var response = EntityToResponse(genero!);
                return Results.Ok(response);
            }
            return Results.NotFound("Gênero não encontrado.");
        });

        /// <summary>
        /// Endpoint para excluir um gênero pelo ID.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos gêneros.</param>
        /// <param name="id">ID do gênero a ser excluído.</param>
        /// <returns>Retorna 204 (No Content) se o gênero for excluído com sucesso. Se o gênero não for encontrado, retorna 404 (Not Found).</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter a instância de DAL e exclui o gênero correspondente ao ID fornecido.</remarks>
        grupoBuilder.MapDelete("{id}", ([FromServices] DAL<Genero> dal, int id) =>
        {
            var genero = dal.RecuperarPor(a => a.Id == id);
            if (genero is null)
            {
                return Results.NotFound("Gênero para exclusão não encontrado.");
            }
            dal.Deletar(genero);
            return Results.NoContent();
        });
    }
    #endregion

    private static Genero RequestToEntity(GeneroRequest generoRequest)
    {
        return new Genero() { Nome = generoRequest.Nome, Descricao = generoRequest.Descricao };
    }

    private static ICollection<GeneroResponse> EntityListToResponseList(IEnumerable<Genero> generos)
    {
        return generos.Select(a => EntityToResponse(a)).ToList();
    }

    private static GeneroResponse EntityToResponse(Genero genero)
    {
        return new GeneroResponse(genero.Id, genero.Nome!, genero.Descricao!);
    }
}

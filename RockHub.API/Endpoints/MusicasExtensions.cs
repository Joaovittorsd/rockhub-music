using Microsoft.AspNetCore.Mvc;
using RockHub.API.Requests;
using RockHub.API.Response;
using RockHub.Shared.Dados.Banco;
using RockHub.Shared.Modelos.Modelos;

namespace RockHub.API.Endpoints;

public static class MusicasExtensions
{
    public static void AddEndPointsMusicas(this WebApplication app)
    {
        #region Endpoint Músicas

        /// <summary>
        /// Endpoint para obter uma lista de músicas.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados das músicas.</param>
        /// <returns>Retorna uma lista de músicas. Se não houver músicas, retorna 404 (Not Found).</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter a instância de DAL e retorna a lista de músicas disponíveis.</remarks>
        app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
        {
            var musicaList = dal.Listar();
            if (musicaList is null)
            {
                return Results.NotFound();
            }
            var musicaListResponse = EntityListToResponseList(musicaList);
            return Results.Ok(musicaListResponse);
        });

        /// <summary>
        /// Endpoint para obter uma música pelo nome.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados das músicas.</param>
        /// <param name="nome">Nome da música a ser buscada.</param>
        /// <returns>Retorna os dados da música se encontrada. Se a música não for encontrada, retorna 404 (Not Found).</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter a instância de DAL e busca uma música pelo nome fornecido, realizando uma busca case-insensitive.</remarks>
        app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
        {
            var musica = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (musica is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(EntityToResponse(musica));
        });

        /// <summary>
        /// Endpoint para adicionar uma nova música.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados das músicas.</param>
        /// <param name="dalGenero">Instância de DAL para acessar os dados dos gêneros.</param>
        /// <param name="musicaRequest">Dados da música a ser criada.</param>
        /// <returns>Retorna 200 (OK) se a música for adicionada com sucesso.</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter as instâncias de DAL e converte os dados da requisição em uma entidade de música para adicionar ao banco de dados.</remarks>
        app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
        {
            var musica = new Musica(musicaRequest.nome)
            {
                ArtistaId = musicaRequest.ArtistaId,
                AnoLancamento = musicaRequest.anoLancamento,
                Generos = musicaRequest.Generos is not null ? GeneroRequestConverter(musicaRequest.Generos, dalGenero) :
                new List<Genero>()

            };
            dal.Adicionar(musica);
            return Results.Ok();
        });

        /// <summary>
        /// Endpoint para excluir uma música pelo ID.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados das músicas.</param>
        /// <param name="id">ID da música a ser excluída.</param>
        /// <returns>Retorna 204 (No Content) se a música for excluída com sucesso. Se a música não for encontrada, retorna 404 (Not Found).</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter a instância de DAL e exclui a música correspondente ao ID fornecido.</remarks>
        app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
        {
            var musica = dal.RecuperarPor(a => a.Id == id);
            if (musica is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(musica);
            return Results.NoContent();
        });

        /// <summary>
        /// Endpoint para atualizar uma música existente.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados das músicas.</param>
        /// <param name="musicaRequestEdit">Dados da música a ser atualizada, incluindo o ID da música.</param>
        /// <returns>Retorna 200 (OK) se a música for atualizada com sucesso. Se a música não for encontrada, retorna 404 (Not Found).</returns>
        /// <remarks>Este método utiliza injeção de dependência para obter a instância de DAL. Ele atualiza o nome e o ano de lançamento da música existente com os novos dados fornecidos.</remarks>
        app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequestEdit) =>
        {
            var musicaAAtualizar = dal.RecuperarPor(a => a.Id == musicaRequestEdit.Id);
            if (musicaAAtualizar is null)
            {
                return Results.NotFound();
            }
            musicaAAtualizar.Nome = musicaRequestEdit.nome;
            musicaAAtualizar.AnoLancamento = musicaRequestEdit.anoLancamento;

            dal.Atualizar(musicaAAtualizar);
            return Results.Ok();
        });
        #endregion
    }

    private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {
        var listaDeGeneros = new List<Genero>();
        foreach (var item in generos)
        {
            var entity = RequestToEntity(item);
            var genero = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(item.Nome.ToUpper()));
            if (genero is not null)
            {
                listaDeGeneros.Add(genero);
            }
            else
            {
                listaDeGeneros.Add(entity);
            }
        }
        return listaDeGeneros;
    }

    private static Genero RequestToEntity(GeneroRequest genero)
    {
        return new Genero() { Nome = genero.Nome, Descricao = genero.Descricao };
    }

    private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
    {
        return musicaList.Select(a => EntityToResponse(a)).ToList();
    }

    private static MusicaResponse EntityToResponse(Musica musica)
    {
        return new MusicaResponse(musica.Id, musica.Nome!, musica.Artista!.Id, musica.Artista.Nome, musica.AnoLancamento);
    }
}

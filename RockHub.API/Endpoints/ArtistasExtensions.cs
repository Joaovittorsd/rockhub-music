using Microsoft.AspNetCore.Mvc;
using RockHub.API.Requests;
using RockHub.API.Response;
using RockHub.Shared.Dados.Banco;
using RockHub.Shared.Dados.Modelos;
using RockHub.Shared.Modelos.Modelos;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

namespace RockHub.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndPointsArtistas(this WebApplication app)
    {
        var grupoBuilder = app.MapGroup("artistas")
            .RequireAuthorization()
            .WithTags("Artistas");

        #region Endpoint Artistas
        /// <summary>
        /// Endpoint para obter uma lista de artistas.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos artistas.</param>
        /// <returns>Retorna uma lista de artistas. Se não houver artistas, retorna 404 (Not Found).</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL.</remarks>
        grupoBuilder.MapGet("", ([FromServices] DAL<Artista> dal) =>
        {
            var listaDeArtistas = dal.Listar();
            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var listaDeArtistaResponse = EntityListToResponseList(listaDeArtistas);
            return Results.Ok(listaDeArtistaResponse);
        });

        /// <summary>
        /// Endpoint para obter um artista pelo nome.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="nome">Nome do artista a ser buscado.</param>
        /// <returns>Retorna os dados do artista. Se o artista não for encontrado, retorna 404 (Not Found).</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL e faz uma busca case-insensitive pelo nome do artista.</remarks>
        grupoBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artista = dal.RecuperarPor(a => a.Nome.Trim().Equals(HttpUtility.UrlDecode(nome), StringComparison.OrdinalIgnoreCase));
            if (artista is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(EntityToResponse(artista));

        });

        /// <summary>
        /// Endpoint para criar um novo artista.
        /// </summary>
        /// <param name="env">Instância de IHostEnvironment para acessar informações do ambiente de hospedagem.</param>
        /// <param name="dal">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="artistaRequest">Dados do artista a ser criado.</param>
        /// <returns>Retorna 200 (OK) se o artista for criado com sucesso.</returns>
        /// <remarks>Este método usa injeção de dependência para obter as instâncias de IHostEnvironment e DAL. Ele salva a foto do perfil do artista em uma pasta em wwwroot>FotoPerfil e adiciona o novo artista ao banco de dados.</remarks>
        grupoBuilder.MapPost("", async ([FromServices] IHostEnvironment env, [FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        {
            var nome = artistaRequest.nome.Trim();
            var imagemArtista = DateTime.Now.ToString("ddMMyyyyhhss") + "." + nome.Replace("/", "-") + ".jpeg";

            var path = Path.Combine(env.ContentRootPath,
                      "wwwroot", "FotoPerfil", imagemArtista);

            using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequest.fotoPerfil!));
            using FileStream fs = new(path, FileMode.Create);
            await ms.CopyToAsync(fs);

            var artista = new Artista(artistaRequest.nome, artistaRequest.bio)
            {
                FotoPerfil = $"/FotoPerfil/{imagemArtista}"
            };

            dal.Adicionar(artista);
            return Results.Ok();
        });

        /// <summary>
        /// Endpoint para deletar um artista pelo ID.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="id">ID do artista a ser deletado.</param>
        /// <returns>Retorna 204 (No Content) se o artista for deletado com sucesso. Se o artista não for encontrado, retorna 404 (Not Found).</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL e busca o artista pelo ID fornecido.</remarks>
        grupoBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) =>
        {
            var artista = dal.RecuperarPor(a => a.Id == id);
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();

        });

        /// <summary>
        /// Endpoint para atualizar um artista existente.
        /// </summary>
        /// <param name="dal">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="artistaRequestEdit">Dados do artista a ser atualizado, incluindo o ID do artista.</param>
        /// <returns>Retorna 200 (OK) se o artista for atualizado com sucesso. Se o artista não for encontrado, retorna 404 (Not Found).</returns>
        /// <remarks>Este método usa injeção de dependência para obter a instância de DAL. Ele atualiza o nome e a biografia do artista existente com os novos dados fornecidos.</remarks>
        grupoBuilder.MapPut("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) =>
        {
            var artistaAAtualizar = dal.RecuperarPor(a => a.Id == artistaRequestEdit.Id);
            if (artistaAAtualizar is null)
            {
                return Results.NotFound();
            }
            artistaAAtualizar.Nome = artistaRequestEdit.nome;
            artistaAAtualizar.Bio = artistaRequestEdit.bio;
            dal.Atualizar(artistaAAtualizar);
            return Results.Ok();
        });


        /// <summary>
        /// Endpoint para adicionar ou atualizar a avaliação de um artista.
        /// </summary>
        /// <param name="context">Instância de HttpContext para acessar informações da requisição e do usuário.</param>
        /// <param name="request">Dados da avaliação do artista, incluindo o ID do artista e a nota.</param>
        /// <param name="dalArtista">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="dalPessoa">Instância de DAL para acessar os dados das pessoas com acesso.</param>
        /// <returns>
        /// Retorna 201 (Created) se a avaliação for adicionada ou atualizada com sucesso.
        /// Se o artista não for encontrado, retorna 404 (Not Found).
        /// Se a pessoa não estiver conectada, lança uma InvalidOperationException.
        /// </returns>
        /// <remarks>
        /// Este método utiliza injeção de dependência para obter as instâncias de DAL para artistas e pessoas.
        /// Ele verifica se o artista existe e se a pessoa está conectada através do e-mail nos claims do usuário.
        /// Se uma avaliação para o artista e a pessoa já existe, ela é atualizada; caso contrário, uma nova avaliação é adicionada.
        /// </remarks>
        grupoBuilder.MapPost("avaliacao", (
            HttpContext context,
            [FromBody] AvaliacaoArtistaRequest request,
            [FromServices] DAL<Artista> dalArtista,
            [FromServices] DAL<PessoaComAcesso> dalPessoa
            ) =>
        {
            var artista = dalArtista.RecuperarPor(a => a.Id == request.ArtistaId);
            if (artista is null) return Results.NotFound();

            var email = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                ?? throw new InvalidOperationException("Pessoa não está conectada");

            var pessoa = dalPessoa
                .RecuperarPor(p => p.Email!.Equals(email))
                ?? throw new InvalidOperationException("Pessoa não está conectada");

            var avaliacao = artista.Avaliacoes
                .FirstOrDefault(a => a.ArtistaId == artista.Id && a.PessoaId == pessoa.Id);

            if (avaliacao is null)
            {
                artista.AdicionarNota(pessoa.Id, request.Nota);
            }
            else
            {
                avaliacao.Nota = request.Nota;
            }

            dalArtista.Atualizar(artista);

            return Results.Created();
        });

        /// <summary>
        /// Endpoint para recuperar a avaliação de um artista específica para a pessoa logada.
        /// </summary>
        /// <param name="id">ID do artista cujas avaliações serão recuperadas.</param>
        /// <param name="context">Instância de HttpContext para acessar informações da requisição e do usuário.</param>
        /// <param name="dalArtista">Instância de DAL para acessar os dados dos artistas.</param>
        /// <param name="dalPessoa">Instância de DAL para acessar os dados das pessoas com acesso.</param>
        /// <returns>
        /// Retorna 200 (OK) com a avaliação do artista.
        /// Se o artista não for encontrado, retorna 404 (Not Found).
        /// Se a pessoa não estiver conectada ou seu e-mail não for encontrado, lança uma InvalidOperationException.
        /// </returns>
        /// <remarks>
        /// Este método utiliza injeção de dependência para obter as instâncias de DAL para artistas e pessoas.
        /// Ele verifica se o artista existe e se a pessoa está conectada através do e-mail nos claims do usuário.
        /// Se uma avaliação para o artista e a pessoa já existe, ela é retornada; caso contrário, é retornada uma avaliação com nota zero.
        /// </remarks>
        grupoBuilder.MapGet("{id}/avaliacao", (
            int id, 
            HttpContext context, 
            [FromServices] DAL<Artista> dalArtista, 
            [FromServices] DAL<PessoaComAcesso> dalPessoa) =>
        {
            var artista = dalArtista.RecuperarPor(a => a.Id == id);
            if (artista is null) return Results.NotFound();

            var email = context.User.Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value
                ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada");

            var pessoa = dalPessoa.RecuperarPor(p => p.Email!.Equals(email))
                ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada");

            var avaliacao = artista
                .Avaliacoes
                .FirstOrDefault(a => a.ArtistaId == id && a.PessoaId == pessoa.Id);

            if (avaliacao is null) return Results.Ok(new AvaliacaoArtistaResponse(id, 0));
            else return Results.Ok(new AvaliacaoArtistaResponse(id, avaliacao.Nota));
        });
        #endregion
    }

    private static string TransformarNomeArtista(string nome)
    {
        string nomeSemEspeciais = Regex.Replace(nome.ToUpper(), "[^A-Z0-9]", "");
        string nomeTransformado = nomeSemEspeciais.Replace(" ", "TICAR");
        return nomeTransformado;
    }

    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil)
        {
            Classificacao = artista
                .Avaliacoes
                .Select(a => a.Nota)
                .DefaultIfEmpty(0)
                .Average()
        };
    }
}

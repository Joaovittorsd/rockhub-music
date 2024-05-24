using System.ComponentModel.DataAnnotations;

namespace RockHub.Web.Requests;
public record ArtistaRequest([Required] string nome, [Required] string bio, string? fotoPerfil);


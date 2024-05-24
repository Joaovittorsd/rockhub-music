using System.ComponentModel.DataAnnotations;

namespace RockHub.API.Requests;
public record ArtistaRequest([Required] string nome, [Required] string bio, string? fotoPerfil);


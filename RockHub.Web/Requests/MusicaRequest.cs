using System.ComponentModel.DataAnnotations;

namespace RockHub.Web.Requests;

public record MusicaRequest([Required] string nome, [Required] int ArtistaId, int? anoLancamento, ICollection<GeneroRequest> Generos = null);


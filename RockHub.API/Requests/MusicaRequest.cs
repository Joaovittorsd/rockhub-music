using System.ComponentModel.DataAnnotations;

namespace RockHub.API.Requests;

public record MusicaRequest([Required] string nome, [Required] int ArtistaId, int anoLancamento, ICollection<GeneroRequest> Generos = null);


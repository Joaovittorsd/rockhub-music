using RockHub.Shared.Dados.Banco;
using RockHub.Shared.Modelos.Modelos;

namespace RockHub.Menus;

internal class MenuSair : Menu
{
    public override void Executar(DAL<Artista> artistaDAL)
    {
        Console.WriteLine("Tchau tchau :)");
    }
}

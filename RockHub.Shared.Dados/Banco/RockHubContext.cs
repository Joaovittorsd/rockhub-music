using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RockHub.Shared.Dados.Modelos;
using RockHub.Shared.Modelos.Modelos;

namespace RockHub.Shared.Dados.Banco;
public class RockHubContext : IdentityDbContext<PessoaComAcesso, PerfilDeAcesso, int>
{
    public DbSet<Artista> Artistas { get; set; }
    public DbSet<Musica> Musicas { get; set; }
    public DbSet<Genero> Generos { get; set; }

    private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ScreenSoundV0;Integrated Security=True;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    public RockHubContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        optionsBuilder
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Musica>()
            .HasMany(c => c.Generos)
            .WithMany(c => c.Musicas);

        modelBuilder.Entity<Artista>()
            .HasMany(a => a.Musicas)
            .WithOne(m => m.Artista)
            .HasForeignKey(a => a.ArtistaId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}

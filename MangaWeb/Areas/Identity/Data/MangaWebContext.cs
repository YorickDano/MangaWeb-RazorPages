using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace MangaWeb.Data;

public class MangaWebContext : IdentityDbContext<MangaWebUser>
{
    public MangaWebContext(DbContextOptions<MangaWebContext> options)
        : base(options)
    {
    }
    public DbSet<Manga> Manga { get; set; } = default!;
    public DbSet<FullManga> FullMangas { get; set; } = default!;    

    protected override void OnModelCreating(ModelBuilder builder)
    {    
        base.OnModelCreating(builder);
        builder.Entity<Manga>()
            .Property(e => e.AnimeImagesUrls)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        builder.Entity<Manga>()
            .Property(e => e.HentaiImagesUrls)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        builder.Entity<FullManga>()
           .Property(e => e.Geners)
           .HasConversion(
               v => string.Join(',', v),
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<FullManga>()
           .Property(e => e.Autors)
           .HasConversion(
               v => string.Join(',', v),
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<FullManga>()
            .HasMany(x => x.Characters)
            .WithOne(i => i.FullManga)
            .OnDelete(DeleteBehavior.Cascade);


    }
}

using MangaWeb.Models;
using MangaWeb.Models.ForumModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Areas.Identity.Data;

public class MangaWebContext : IdentityDbContext<MangaWebUser>
{
    public MangaWebContext(DbContextOptions<MangaWebContext> options)
        : base(options)
    {
    }
    public DbSet<Manga> Manga { get; set; } = default!;
    public DbSet<MangaCharacter> MangaCharacter { get; set; } = default!;
    public DbSet<Comment> Comments { get; set; } = default!;
    public DbSet<Topic> Topics { get; set; } = default!;
    public DbSet<Post> Posts { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Manga>()
           .Property(e => e.Genres)
           .HasConversion(
               v => string.Join(',', v),
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<Manga>()
          .Property(e => e.ReadLinks)
          .HasConversion(
              v => string.Join('>', v),
              v => v.Split('>', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<Manga>()
           .Property(e => e.Authors)
           .HasConversion(
               v => string.Join(',', v),
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<Manga>()
            .HasMany(x => x.Characters)
            .WithOne(i => i.Manga)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MangaRead>()
            .HasMany(x => x.Pages)
            .WithOne(i => i.MangaRead)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MangaCharacter>()
         .Property(e => e.ImagesUrls)
         .HasConversion(
             v => string.Join(',', v),
             v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Entity<MangaWebUser>()
            .Property(e => e.FavoriteManga)
            .HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(x => Convert.ToInt32(x)).ToList());
        builder.Entity<MangaWebUser>()
           .Property(e => e.CreatedManga)
           .HasConversion(
           v => string.Join(',', v),
           v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => Convert.ToInt32(x)).ToList());
    }
}

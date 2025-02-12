using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Data
{
    public class AppDbTournamentContext : DbContext
    {
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Game> Games { get; set; }
        public AppDbTournamentContext(DbContextOptions<AppDbTournamentContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tournament>().ToTable("tournaments");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Game>().ToTable("games");

            modelBuilder.Entity<Tournament>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Tournament>().Property(t => t.IdGame).HasColumnName("id_game");
            modelBuilder.Entity<Tournament>().Property(t => t.IdCategory).HasColumnName("id_category");
            modelBuilder.Entity<Tournament>().Property(t => t.IsPaid).HasColumnName("is_paid");
            modelBuilder.Entity<Tournament>().Property(t => t.CreatedAt).HasColumnName("created_at");


            modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("name");
            modelBuilder.Entity<Category>().Property(c => c.Alias).HasColumnName("alias");
            modelBuilder.Entity<Category>().Property(c => c.Code).HasColumnName("code");

            modelBuilder.Entity<Game>().Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Game>().Property(g => g.Name).HasColumnName("name");
            modelBuilder.Entity<Game>().Property(g => g.Players).HasColumnName("players");


            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tournaments)
                .HasForeignKey(t => t.IdCategory)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Game)
                .WithMany(g => g.Tournaments)
                .HasForeignKey(t => t.IdGame)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = 1,
                Name = "Carreras",
                Code= "0235",
                Alias= "Racing"
            });

            modelBuilder.Entity<Game>().HasData(new Game {Id=1, Name = "Need For Speed", Players = 10 });
        }
    }
}

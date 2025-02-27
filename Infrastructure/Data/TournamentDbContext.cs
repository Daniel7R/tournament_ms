using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Infrastructure.Data.Converters;

namespace TournamentMS.Infrastructure.Data
{
    public class TournamentDbContext : DbContext
    {
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<TournamentUserRole> UserRoles { get; set; }
        public DbSet<Prizes> Prizes { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<Matches> Matches { get; set; }
        public DbSet<TeamsMatches> TeamsMatches { get; set; }
        public DbSet<TeamsMembers> TeamsMembers { get; set; }
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tournament>().ToTable("tournaments");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Game>().ToTable("games");
            modelBuilder.Entity<Matches>().ToTable("matches");
            modelBuilder.Entity<Prizes>().ToTable("prizes");
            modelBuilder.Entity<Teams>().ToTable("teams");
            modelBuilder.Entity<TeamsMatches>().ToTable("teams_matches");
            modelBuilder.Entity<TeamsMembers>().ToTable("teams_members");
            modelBuilder.Entity<TournamentUserRole>().ToTable("tournaments_users_roles");

            modelBuilder.Entity<Tournament>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Tournament>().Property(t => t.Name).HasColumnName("name");
            modelBuilder.Entity<Tournament>().Property(t => t.Description).HasColumnName("description");
            modelBuilder.Entity<Tournament>().Property(t => t.IdCategory).HasColumnName("id_category");
            modelBuilder.Entity<Tournament>().Property(t => t.IdGame).HasColumnName("id_game");
            modelBuilder.Entity<Tournament>().Property(t => t.IsFree).HasColumnName("is_free");
            modelBuilder.Entity<Tournament>().Property(t => t.IdOrganizer).HasColumnName("id_organizer");
            modelBuilder.Entity<Tournament>().Property(t => t.StartDate).HasColumnName("start_date");
            modelBuilder.Entity<Tournament>().Property(t => t.EndDate).HasColumnName("end_date");
            modelBuilder.Entity<Tournament>().Property(t => t.IdTeamWinnerTournament).HasColumnName("id_team_winner_tournmanent");
            modelBuilder.Entity<Tournament>().Property(t => t.Status).HasColumnName("tournament_status").HasConversion(new EnumToStringConverter<TournamentStatus>());
            modelBuilder.Entity<Tournament>().Property(t => t.IdPrize).HasColumnName("id_prize");

            modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("name");
            modelBuilder.Entity<Category>().Property(c => c.Alias).HasColumnName("alias");
            modelBuilder.Entity<Category>().Property(c => c.Code).HasColumnName("code");

            modelBuilder.Entity<Game>().Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Game>().Property(g => g.Name).HasColumnName("name");
            modelBuilder.Entity<Game>().Property(g => g.Players).HasColumnName("players");

            modelBuilder.Entity<TournamentUserRole>()
                .HasKey(tur => new { tur.IdUser, tur.IdTournament, tur.IdRole });
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdTournament).HasColumnName("id_tournament");
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdUser).HasColumnName("id_user");
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdRole).HasColumnName("id_role");
            modelBuilder.Entity<TournamentUserRole>()
                .HasOne<Tournament>()
                .WithMany(t => t.UsersTournamentRole)
                .HasForeignKey(tur =>  tur.IdTournament);

            modelBuilder.Entity<Matches>().HasKey(m => m.Id);
            modelBuilder.Entity<Matches>().Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Matches>().Property(m => m.Status).HasColumnName("status");
            modelBuilder.Entity<Matches>().Property(m => m.IdTournament).HasColumnName("id_tournament");


            // <---To make sure does not repeat category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Alias)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Code)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
            // --->
            modelBuilder.Entity<Matches>()
                .HasOne(m => m.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.IdTournament)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prizes>()
                .HasOne(p => p.Tournament)
                .WithOne(t => t.Prize)
                .HasForeignKey<Tournament>(t => t.IdPrize);


            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tournaments)
                .HasForeignKey(t => t.IdCategory)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Prize)
                .WithOne(p => p.Tournament)
                .HasForeignKey<Prizes>(p => p.Id)
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
                Alias= "Racing",
                LimitParticipant= 20
            });
            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = 2,
                Name = "Estrategia",
                Code = "0236",
                Alias = "Strategy",
                LimitParticipant = 60
            });

            modelBuilder.Entity<Game>().HasData(new Game {Id=1, Name = "Need For Speed", Players = 10, IsCooperative= false, MaxTeams=10, MaxPlayersPerTeam=1 });
            modelBuilder.Entity<Game>().HasData(new Game { Id = 2, Name = "League Of Legends", Players = 10, IsCooperative = true, MaxTeams = 2, MaxPlayersPerTeam = 5 });
        }
    }
}

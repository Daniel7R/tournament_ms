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

            modelBuilder.Entity<Teams>()
                .Property(t => t.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Teams>()
                .Property(t => t.IdTournament)
                .HasColumnName("id_tournament");
            modelBuilder.Entity<Teams>()
                .Property(t => t.Name)
                .HasColumnName("name");
            modelBuilder.Entity<Teams>().Property(t => t.CurrentMembers).HasColumnName("current_members");
            modelBuilder.Entity<Teams>().Property(t => t.MaxMembers).HasColumnName("max_members");
            modelBuilder.Entity<Teams>()
                .Property(t => t.IsFull)
                .HasColumnName("is_full");

            modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("name");
            modelBuilder.Entity<Category>().Property(c => c.Alias).HasColumnName("alias");
            modelBuilder.Entity<Category>().Property(c => c.Code).HasColumnName("code");

            modelBuilder.Entity<Game>().Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Game>().Property(g => g.Name).HasColumnName("name");
            modelBuilder.Entity<Game>().Property(g => g.Players).HasColumnName("players");

            modelBuilder.Entity<Prizes>().Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            //modelBuilder.Entity<Prizes>().Property(p => p.IdTournament).HasColumnName("id_tournament");
            modelBuilder.Entity<Prizes>().Property(p => p.Description).HasColumnName("description");
            modelBuilder.Entity<Prizes>().Property(p => p.Total).HasColumnName("total");

            modelBuilder.Entity<TeamsMembers>().Property(tm => tm.IdTeam).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<TeamsMembers>().Property(tm => tm.IdTeam).HasColumnName("id_team");
            modelBuilder.Entity<TeamsMembers>().Property(tm => tm.IdUser).HasColumnName("id_user");

            modelBuilder.Entity<TeamsMatches>().Property(tm => tm.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<TeamsMatches>().Property(tm => tm.IdTeam).HasColumnName("id_team");
            modelBuilder.Entity<TeamsMatches>().Property(tm => tm.IdMatch).HasColumnName("id_match");

            // modelBuilder.Entity<TournamentUserRole>()
            //     .HasKey(tur => new { tur.IdUser, tur.IdTournament, tur.Role, tur.IdMatch });
            modelBuilder.Entity<TournamentUserRole>()
                .HasIndex(tur => new { tur.IdUser, tur.IdTournament })
                .IsUnique()
                .HasFilter("id_tournament IS NOT NULL");
            modelBuilder.Entity<TournamentUserRole>()
                .HasIndex(tur => new { tur.IdUser, tur.IdMatch })
                .IsUnique()
                .HasFilter("id_match IS NOT NULL"); 
            modelBuilder.Entity<TournamentUserRole>().Property(t  => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdTournament).HasColumnName("id_tournament");
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdMatch).HasColumnName("id_match");
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.IdUser).HasColumnName("id_user");
            modelBuilder.Entity<TournamentUserRole>().Property(t => t.Role).HasColumnName("role").HasConversion<string>();
            modelBuilder.Entity<TournamentUserRole>()
                .HasOne(t => t.Tournament)
                .WithMany(t => t.UsersTournamentRole)
                .HasForeignKey(tur => tur.IdTournament)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            modelBuilder.Entity<TournamentUserRole>()
                .HasOne(t => t.Match)
                .WithMany()
                .HasForeignKey(tur => tur.IdMatch)
                .OnDelete(DeleteBehavior.SetNull).IsRequired(false);

            modelBuilder.Entity<Matches>().HasKey(m => m.Id);
            modelBuilder.Entity<Matches>().Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Matches>().Property(m => m.Status).HasColumnName("status").HasConversion<string>();
            modelBuilder.Entity<Matches>().Property(m => m.IdTournament).HasColumnName("id_tournament");
            //modelBuilder.Entity<Matches>().Property(m => m.IdStream).HasColumnName("id_stream");
            modelBuilder.Entity<Matches>().Property(m => m.Name).HasColumnName("name");
            modelBuilder.Entity<Matches>().Property(m => m.IdTeamWinner).HasColumnName("id_team_winner");
            modelBuilder.Entity<Matches>().Property(m => m.Date).HasColumnName("date");

            modelBuilder.Entity<Matches>()
                .HasOne(m => m.TeamWinner)
                .WithMany()
                .HasForeignKey(m => m.IdTeamWinner)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TeamsMembers>()
                .HasOne(tm => tm.Team)
                .WithMany(tm => tm.Members)
                .HasForeignKey(tm => tm.IdTeam);

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

            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Prize)
                .WithOne(p => p.Tournament)
                .HasForeignKey<Tournament>(p => p.IdPrize);


            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tournaments)
                .HasForeignKey(t => t.IdCategory)
                .OnDelete(DeleteBehavior.Cascade);
            /*modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Prize)
                .WithOne(p => p.Tournament)
                .HasForeignKey<Prizes>(p => p.IdTournament)
                .OnDelete(DeleteBehavior.Cascade);
            */
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Game)
                .WithMany(g => g.Tournaments)
                .HasForeignKey(t => t.IdGame)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamsMatches>()
                .HasOne(tm => tm.Team)
                .WithMany(m => m.TeamsMatches)
                .HasForeignKey(tm => tm.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TeamsMatches>()
                .HasOne(tm => tm.Match)
                .WithMany(tm => tm.TeamsMatches)
                .HasForeignKey(tm => tm.IdMatch).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Matches>()
                .HasOne(m => m.TeamWinner)
                .WithMany()
                .HasForeignKey(m => m.IdTeamWinner)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = 1,
                Name = "Carreras",
                Code = "0235",
                Alias = "Racing",
                LimitParticipant = 10
            });
            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = 2,
                Name = "Estrategia",
                Code = "0236",
                Alias = "Strategy",
                LimitParticipant = 10
            });

            modelBuilder.Entity<Game>().HasData(new Game { Id = 1, Name = "Need For Speed", Players = 10, IsCooperative = false, MaxTeams = 10, MaxPlayersPerTeam = 1 });
            modelBuilder.Entity<Game>().HasData(new Game { Id = 2, Name = "League Of Legends", Players = 10, IsCooperative = true, MaxTeams = 2, MaxPlayersPerTeam = 5 });
        }
    }
}

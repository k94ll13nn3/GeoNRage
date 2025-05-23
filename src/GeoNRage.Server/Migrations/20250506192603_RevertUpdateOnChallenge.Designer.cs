﻿// <auto-generated />
using System;
using GeoNRage.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeoNRage.Server.Migrations
{
    [DbContext(typeof(GeoNRageDbContext))]
    [Migration("20250506192603_RevertUpdateOnChallenge")]
    partial class RevertUpdateOnChallenge
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_general_ci")
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4", DelegationModes.ApplyToAll);
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("GeoNRage.Server.Entities.Challenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("CreatorId")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("GeoGuessrId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("MapId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("TimeLimit")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("GameId");

                    b.HasIndex("GeoGuessrId")
                        .IsUnique();

                    b.HasIndex("MapId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Date = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Default Game - do not use!"
                        });
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Location", b =>
                {
                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("int");

                    b.Property<string>("AdministrativeAreaLevel1")
                        .HasColumnType("longtext");

                    b.Property<string>("AdministrativeAreaLevel2")
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .HasColumnType("longtext");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Locality")
                        .HasColumnType("longtext");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("ChallengeId", "RoundNumber");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Map", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("GeoGuessrName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("longtext")
                        .HasDefaultValue("N/A");

                    b.Property<bool>("IsMapForGame")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Maps");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("AssociatedPlayerId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("GeoGuessrName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("longtext")
                        .HasDefaultValue("N/A");

                    b.Property<string>("IconUrl")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("longtext")
                        .HasDefaultValue("/img/icon.png");

                    b.Property<DateTime?>("LastGeoGuessrUpdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("longtext")
                        .HasDefaultValue("Newbie");

                    b.HasKey("Id");

                    b.HasIndex("AssociatedPlayerId")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.PlayerGuess", b =>
                {
                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("int");

                    b.Property<double?>("Distance")
                        .HasColumnType("double");

                    b.Property<int?>("Score")
                        .HasColumnType("int");

                    b.Property<int?>("Steps")
                        .HasColumnType("int");

                    b.Property<int?>("Time")
                        .HasColumnType("int");

                    b.Property<bool>("TimedOut")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("TimedOutWithGuess")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("ChallengeId", "PlayerId", "RoundNumber");

                    b.ToTable("PlayerGuesses");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.PlayerScore", b =>
                {
                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("ChallengeId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerScores");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PlayerId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("PlayerId")
                        .IsUnique();

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FriendlyName")
                        .HasColumnType("longtext");

                    b.Property<string>("Xml")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Challenge", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.Player", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.HasOne("GeoNRage.Server.Entities.Game", "Game")
                        .WithMany("Challenges")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoNRage.Server.Entities.Map", "Map")
                        .WithMany("Challenges")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Game");

                    b.Navigation("Map");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Location", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.Challenge", "Challenge")
                        .WithMany("Locations")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Challenge");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Player", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.Player", "AssociatedPlayer")
                        .WithOne()
                        .HasForeignKey("GeoNRage.Server.Entities.Player", "AssociatedPlayerId");

                    b.Navigation("AssociatedPlayer");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.PlayerGuess", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.PlayerScore", "PlayerScore")
                        .WithMany("PlayerGuesses")
                        .HasForeignKey("ChallengeId", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlayerScore");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.PlayerScore", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.Challenge", "Challenge")
                        .WithMany("PlayerScores")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoNRage.Server.Entities.Player", "Player")
                        .WithMany("PlayerScores")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Challenge");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.User", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.Player", "Player")
                        .WithOne()
                        .HasForeignKey("GeoNRage.Server.Entities.User", "PlayerId");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.UserRole", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoNRage.Server.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GeoNRage.Server.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Challenge", b =>
                {
                    b.Navigation("Locations");

                    b.Navigation("PlayerScores");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Game", b =>
                {
                    b.Navigation("Challenges");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Map", b =>
                {
                    b.Navigation("Challenges");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.Player", b =>
                {
                    b.Navigation("PlayerScores");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.PlayerScore", b =>
                {
                    b.Navigation("PlayerGuesses");
                });

            modelBuilder.Entity("GeoNRage.Server.Entities.User", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}

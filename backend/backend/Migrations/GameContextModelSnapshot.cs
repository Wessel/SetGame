﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend.Models;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(GameContext))]
    partial class GameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("backend.Models.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.PrimitiveCollection<int[]>("Deck")
                        .HasColumnType("integer[]");

                    b.Property<int>("Fails")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.PrimitiveCollection<int[]>("Found")
                        .HasColumnType("integer[]");

                    b.PrimitiveCollection<int[]>("Hand")
                        .HasColumnType("integer[]");

                    b.Property<int>("Hints")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });
#pragma warning restore 612, 618
        }
    }
}

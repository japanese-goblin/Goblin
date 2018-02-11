﻿// <auto-generated />
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Goblin.Migrations.Main
{
    [DbContext(typeof(MainContext))]
    partial class MainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Goblin.Models.Remind", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text");

                    b.Property<long>("Timestamp");

                    b.Property<int>("VkID");

                    b.HasKey("ID");

                    b.ToTable("Reminds");
                });

            modelBuilder.Entity("Goblin.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<int>("CityNumber")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<short>("Group")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue((short)0);

                    b.Property<bool>("Schedule")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<int>("Vk");

                    b.Property<bool>("Weather")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.HasKey("ID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

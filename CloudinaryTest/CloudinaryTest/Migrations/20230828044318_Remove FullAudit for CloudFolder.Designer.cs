﻿// <auto-generated />
using System;
using CloudinaryTest.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CloudinaryTest.Migrations
{
    [DbContext(typeof(CloudDBContext))]
    [Migration("20230828044318_Remove FullAudit for CloudFolder")]
    partial class RemoveFullAuditforCloudFolder
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CloudinaryTest.Entities.CloudFile", b =>
                {
                    b.Property<long>("FolderId")
                        .HasColumnType("bigint");

                    b.Property<string>("FolderPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Format")
                        .HasColumnType("int");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOverride")
                        .HasColumnType("bit");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FolderId");

                    b.ToTable("CloudFiles");
                });

            modelBuilder.Entity("CloudinaryTest.Entities.CloudFolder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long?>("CloudFolderId")
                        .HasColumnType("bigint");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CombineName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLeaf")
                        .HasColumnType("bit");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CloudFolderId");

                    b.ToTable("CloudFolders");
                });

            modelBuilder.Entity("CloudinaryTest.Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("CloudinaryTest.Entities.CloudFile", b =>
                {
                    b.HasOne("CloudinaryTest.Entities.CloudFolder", "CloudFolder")
                        .WithMany("CloudFiles")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CloudFolder");
                });

            modelBuilder.Entity("CloudinaryTest.Entities.CloudFolder", b =>
                {
                    b.HasOne("CloudinaryTest.Entities.CloudFolder", null)
                        .WithMany("CloudFolders")
                        .HasForeignKey("CloudFolderId");
                });

            modelBuilder.Entity("CloudinaryTest.Entities.CloudFolder", b =>
                {
                    b.Navigation("CloudFiles");

                    b.Navigation("CloudFolders");
                });
#pragma warning restore 612, 618
        }
    }
}

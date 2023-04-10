﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mientreno.Server.Helpers;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230410073249_ejercicios_basica")]
    partial class ejercicios_basica
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mientreno.Server.Models.Categoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Categoria");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Contrato", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AlumnoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EntrenadorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("FechaAsignacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaDesasignacion")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AlumnoId");

                    b.HasIndex("EntrenadorId");

                    b.ToTable("Asignaciones");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Cuestionario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("AlturaCm")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("ContratoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Edad")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaFormalizacion")
                        .HasColumnType("datetime2");

                    b.Property<byte>("FrecuenciaCardiacaReposo")
                        .HasColumnType("tinyint");

                    b.Property<float>("MasaKilogramos")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.ToTable("Cuestionarios");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Ejercicio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("Dificultad")
                        .HasColumnType("tinyint");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("VideoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Ejercicio");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Sesion", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaExpiracion")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Invalidada")
                        .HasColumnType("bit");

                    b.Property<Guid>("UsuarioId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SessionId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Sesiones");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Usuario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaEliminacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");

                    b.HasDiscriminator<string>("Tipo").HasValue("Usuario");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Mientreno.Server.Models.Alumno", b =>
                {
                    b.HasBaseType("Mientreno.Server.Models.Usuario");

                    b.HasDiscriminator().HasValue("Alumno");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Entrenador", b =>
                {
                    b.HasBaseType("Mientreno.Server.Models.Usuario");

                    b.HasDiscriminator().HasValue("Entrenador");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Categoria", b =>
                {
                    b.HasOne("Mientreno.Server.Models.Entrenador", "Owner")
                        .WithMany("Categorias")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Contrato", b =>
                {
                    b.HasOne("Mientreno.Server.Models.Alumno", "Alumno")
                        .WithMany("Asignaciones")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Mientreno.Server.Models.Entrenador", "Entrenador")
                        .WithMany("Asignaciones")
                        .HasForeignKey("EntrenadorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Alumno");

                    b.Navigation("Entrenador");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Cuestionario", b =>
                {
                    b.HasOne("Mientreno.Server.Models.Contrato", "Contrato")
                        .WithMany("Cuestionarios")
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("Mientreno.Server.Models.Habitos", "Habitos", b1 =>
                        {
                            b1.Property<Guid>("CuestionarioId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Bebedor")
                                .HasColumnType("int");

                            b1.Property<int>("Deporte")
                                .HasColumnType("int");

                            b1.Property<int>("Fumador")
                                .HasColumnType("int");

                            b1.Property<string>("OtrasDrogas")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Profesion")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Sueño")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CuestionarioId");

                            b1.ToTable("Cuestionarios");

                            b1.WithOwner()
                                .HasForeignKey("CuestionarioId");
                        });

                    b.OwnsOne("Mientreno.Server.Models.Perimetros", "Perimetros", b1 =>
                        {
                            b1.Property<Guid>("CuestionarioId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte>("Abd1")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("AbdominalOmbligo")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("Cadera")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("Cintura")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("CuadricepsMaximo")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("CuadricepsMinimo")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("Gastrocnemio")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("PectoralExpiracion")
                                .HasColumnType("tinyint");

                            b1.Property<byte>("PectoralInspiracion")
                                .HasColumnType("tinyint");

                            b1.HasKey("CuestionarioId");

                            b1.ToTable("Cuestionarios");

                            b1.WithOwner()
                                .HasForeignKey("CuestionarioId");
                        });

                    b.Navigation("Contrato");

                    b.Navigation("Habitos")
                        .IsRequired();

                    b.Navigation("Perimetros")
                        .IsRequired();
                });

            modelBuilder.Entity("Mientreno.Server.Models.Ejercicio", b =>
                {
                    b.HasOne("Mientreno.Server.Models.Categoria", "Categoria")
                        .WithMany("Ejercicios")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Mientreno.Server.Models.Entrenador", "Owner")
                        .WithMany("Ejercicios")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Sesion", b =>
                {
                    b.HasOne("Mientreno.Server.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Usuario", b =>
                {
                    b.OwnsOne("Mientreno.Server.Models.Credenciales", "Credenciales", b1 =>
                        {
                            b1.Property<Guid>("UsuarioId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CodigoVerificacionEmail")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Contraseña")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool>("EmailVerificado")
                                .HasColumnType("bit");

                            b1.Property<bool>("MfaHabilitado")
                                .HasColumnType("bit");

                            b1.Property<bool>("MfaVerificado")
                                .HasColumnType("bit");

                            b1.Property<bool>("RequiereCambioContraseña")
                                .HasColumnType("bit");

                            b1.Property<string>("SemillaMfa")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UsuarioId");

                            b1.ToTable("Usuarios");

                            b1.WithOwner()
                                .HasForeignKey("UsuarioId");
                        });

                    b.Navigation("Credenciales")
                        .IsRequired();
                });

            modelBuilder.Entity("Mientreno.Server.Models.Categoria", b =>
                {
                    b.Navigation("Ejercicios");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Contrato", b =>
                {
                    b.Navigation("Cuestionarios");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Alumno", b =>
                {
                    b.Navigation("Asignaciones");
                });

            modelBuilder.Entity("Mientreno.Server.Models.Entrenador", b =>
                {
                    b.Navigation("Asignaciones");

                    b.Navigation("Categorias");

                    b.Navigation("Ejercicios");
                });
#pragma warning restore 612, 618
        }
    }
}
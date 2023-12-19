﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MunicipalityRegistry.Projections.Integration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    [DbContext(typeof(IntegrationContext))]
    partial class IntegrationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates.ProjectionStateItem", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("DesiredState")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("DesiredStateChangedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<long>("Position")
                        .HasColumnType("bigint");

                    b.HasKey("Name");

                    b.ToTable("ProjectionStates", "integration");
                });

            modelBuilder.Entity("MunicipalityRegistry.Projections.Integration.MunicipalityLatestItem", b =>
                {
                    b.Property<Guid>("MunicipalityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("municipality_id");

                    b.Property<bool?>("FacilityLanguageDutch")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_dutch");

                    b.Property<bool?>("FacilityLanguageEnglish")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_english");

                    b.Property<bool?>("FacilityLanguageFrench")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_french");

                    b.Property<bool?>("FacilityLanguageGerman")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_german");

                    b.Property<long>("IdempotenceKey")
                        .HasColumnType("bigint")
                        .HasColumnName("idempotence_key");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("NameDutch")
                        .HasColumnType("text")
                        .HasColumnName("name_dutch");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("text")
                        .HasColumnName("name_english");

                    b.Property<string>("NameFrench")
                        .HasColumnType("text")
                        .HasColumnName("name_french");

                    b.Property<string>("NameGerman")
                        .HasColumnType("text")
                        .HasColumnName("name_german");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("namespace");

                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character(5)")
                        .HasColumnName("nis_code")
                        .IsFixedLength();

                    b.Property<bool?>("OfficialLanguageDutch")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_dutch");

                    b.Property<bool?>("OfficialLanguageEnglish")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_english");

                    b.Property<bool?>("OfficialLanguageFrench")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_french");

                    b.Property<bool?>("OfficialLanguageGerman")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_german");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("puri_id");

                    b.Property<string>("Status")
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("VersionAsString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("version_as_string");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("version_timestamp");

                    b.HasKey("MunicipalityId");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("NameDutch");

                    b.HasIndex("NameEnglish");

                    b.HasIndex("NameFrench");

                    b.HasIndex("NameGerman");

                    b.HasIndex("NisCode");

                    b.HasIndex("Status");

                    b.ToTable("municipality_latest_items", "integration");
                });

            modelBuilder.Entity("MunicipalityRegistry.Projections.Integration.MunicipalityVersion", b =>
                {
                    b.Property<long>("Position")
                        .HasColumnType("bigint")
                        .HasColumnName("position");

                    b.Property<bool?>("FacilityLanguageDutch")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_dutch");

                    b.Property<bool?>("FacilityLanguageEnglish")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_english");

                    b.Property<bool?>("FacilityLanguageFrench")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_french");

                    b.Property<bool?>("FacilityLanguageGerman")
                        .HasColumnType("boolean")
                        .HasColumnName("facility_language_german");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<Guid>("MunicipalityId")
                        .HasColumnType("uuid")
                        .HasColumnName("municipality_id");

                    b.Property<string>("NameDutch")
                        .HasColumnType("text")
                        .HasColumnName("name_dutch");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("text")
                        .HasColumnName("name_english");

                    b.Property<string>("NameFrench")
                        .HasColumnType("text")
                        .HasColumnName("name_french");

                    b.Property<string>("NameGerman")
                        .HasColumnType("text")
                        .HasColumnName("name_german");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("namespace");

                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character(5)")
                        .HasColumnName("nis_code")
                        .IsFixedLength();

                    b.Property<bool?>("OfficialLanguageDutch")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_dutch");

                    b.Property<bool?>("OfficialLanguageEnglish")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_english");

                    b.Property<bool?>("OfficialLanguageFrench")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_french");

                    b.Property<bool?>("OfficialLanguageGerman")
                        .HasColumnType("boolean")
                        .HasColumnName("official_language_german");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("puri_id");

                    b.Property<string>("Status")
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("VersionAsString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("version_as_string");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("version_timestamp");

                    b.HasKey("Position");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("MunicipalityId");

                    b.HasIndex("NameDutch");

                    b.HasIndex("NameEnglish");

                    b.HasIndex("NameFrench");

                    b.HasIndex("NameGerman");

                    b.HasIndex("NisCode");

                    b.HasIndex("Status");

                    b.ToTable("municipality_versions", "integration");
                });
#pragma warning restore 612, 618
        }
    }
}

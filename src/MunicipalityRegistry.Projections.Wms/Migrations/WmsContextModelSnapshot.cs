﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MunicipalityRegistry.Projections.Wms;

namespace MunicipalityRegistry.Projections.Wms.Migrations
{
    [DbContext(typeof(WmsContext))]
    partial class WmsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates.ProjectionStateItem", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DesiredState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("DesiredStateChangedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Position")
                        .HasColumnType("bigint");

                    b.HasKey("Name")
                        .IsClustered();

                    b.ToTable("ProjectionStates", "wms.municipality");
                });

            modelBuilder.Entity("MunicipalityRegistry.Projections.Wms.Municipality.MunicipalityHelper", b =>
                {
                    b.Property<Guid?>("MunicipalityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FacilitiesLanguagesAsString")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("FacilitiesLanguages");

                    b.Property<string>("NameDutch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFrench")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameGerman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NisCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OfficialLanguagesAsString")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("OfficialLanguages");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("VersionTimestamp");

                    b.HasKey("MunicipalityId")
                        .IsClustered(false);

                    b.HasIndex("NisCode")
                        .IsClustered();

                    b.ToTable("MunicipalityHelper", "wms.municipality");
                });
#pragma warning restore 612, 618
        }
    }
}

namespace MunicipalityRegistry.Api.Import
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MunicipalityRegistry.Infrastructure;
    using Newtonsoft.Json;

    public class MunicipalityMerger
    {
        public Guid MunicipalityId { get; set; }
        private string MunicipalityIdsToMergeWithAsString { get; set; }
        public int Year { get; set; }

        public IEnumerable<Guid> MunicipalityIdsToMergeWith
        {
            get => GetMunicipalityIdsToMergeWith();
            set => MunicipalityIdsToMergeWithAsString = JsonConvert.SerializeObject(value);
        }

        private List<Guid> GetMunicipalityIdsToMergeWith() =>
            string.IsNullOrEmpty(MunicipalityIdsToMergeWithAsString)
                ? []
                : JsonConvert.DeserializeObject<List<Guid>>(MunicipalityIdsToMergeWithAsString) ?? [];

        public Guid NewMunicipalityId { get; set; }

        public MunicipalityMerger(
            int year,
            Guid municipalityId,
            IEnumerable<Guid> municipalityIdsToMergeWith,
            Guid newMunicipalityId)
        {
            Year = year;
            MunicipalityId = municipalityId;
            MunicipalityIdsToMergeWith = municipalityIdsToMergeWith;
            NewMunicipalityId = newMunicipalityId;
        }

        private MunicipalityMerger()
        { }
    }

    public class MunicipalityMergerConfiguration : IEntityTypeConfiguration<MunicipalityMerger>
    {
        private const string TableName = "MunicipalityMergers";

        public void Configure(EntityTypeBuilder<MunicipalityMerger> b)
        {
            b.ToTable(TableName, Schema.Import)
                .HasKey(x => new { x.MunicipalityId, x.Year })
                .IsClustered(false);

            b.Ignore(x => x.MunicipalityIdsToMergeWith);

            b.Property("MunicipalityIdsToMergeWithAsString")
                .HasColumnName("MunicipalityIdsToMergeWith")
                .IsRequired();

            b.HasIndex(x => x.Year)
                .IsClustered();
        }
    }
}

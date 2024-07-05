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

        public IEnumerable<string> MunicipalityIdsToMergeWith
        {
            get => GetMunicipalityIdsToMergeWith();
            set => MunicipalityIdsToMergeWithAsString = JsonConvert.SerializeObject(value);
        }

        private List<string> GetMunicipalityIdsToMergeWith()
        {
            return string.IsNullOrEmpty(MunicipalityIdsToMergeWithAsString)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(MunicipalityIdsToMergeWithAsString);
        }

        public MunicipalityId NewMunicipalityId { get; set; }

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
        }
    }
}

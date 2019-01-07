namespace MunicipalityRegistry.Api.Legacy.Municipality.Query
{
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Microsoft.EntityFrameworkCore;
    using NodaTime;
    using Projections.Legacy;
    using Projections.Legacy.MunicipalitySyndication;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class MunicipalitySyndicationQueryResult
    {
        public bool ContainsDetails { get; }

        public Guid? MunicipalityId { get; }
        public long Position { get; }
        public string ChangeType { get; }
        public string NisCode { get; }
        public string DefaultName { get; }
        public Instant RecordCreatedAt { get; }
        public Instant LastChangedOn { get; }
        public MunicipalityStatus? Status { get; }
        public string NameDutch { get; }
        public string NameFrench { get; }
        public string NameGerman { get; }
        public string NameEnglish { get; }
        public Organisation? Organisation { get; }
        public Plan? Plan { get; }

        public MunicipalitySyndicationQueryResult(
            Guid? municipalityId,
            long position,
            string nisCode,
            string changeType,
            Instant recordCreatedAt,
            Instant lastChangedOn,
            Organisation? organisation,
            Plan? plan)
        {
            ContainsDetails = false;

            MunicipalityId = municipalityId;
            Position = position;
            NisCode = nisCode;
            ChangeType = changeType;
            RecordCreatedAt = recordCreatedAt;
            LastChangedOn = lastChangedOn;
            Organisation = organisation;
            Plan = plan;
        }

        public MunicipalitySyndicationQueryResult(
            Guid? municipalityId,
            long position,
            string nisCode,
            string changeType,
            string defaultName,
            Instant recordCreatedAt,
            Instant lastChangedOn,
            MunicipalityStatus? status,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            Organisation? organisation,
            Plan? plan) :
            this(
                municipalityId,
                position,
                nisCode,
                changeType,
                recordCreatedAt,
                lastChangedOn,
                organisation,
                plan)
        {
            ContainsDetails = true;

            DefaultName = defaultName;
            Status = status;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NameEnglish = nameEnglish;
        }
    }

    public class MunicipalitySyndicationQuery : Query<MunicipalitySyndicationItem, MunicipalitySyndicationFilter, MunicipalitySyndicationQueryResult>
    {
        private readonly LegacyContext _context;
        private readonly bool _embed;

        protected override ISorting Sorting => new MunicipalitySorting();

        public MunicipalitySyndicationQuery(LegacyContext context, bool embed)
        {
            _context = context;
            _embed = embed;
        }

        protected override Expression<Func<MunicipalitySyndicationItem, MunicipalitySyndicationQueryResult>> Transformation => _embed
            ? (Expression<Func<MunicipalitySyndicationItem, MunicipalitySyndicationQueryResult>>)(x =>
               new MunicipalitySyndicationQueryResult(
                   x.MunicipalityId,
                   x.Position,
                   x.NisCode,
                   x.ChangeType,
                   x.DefaultName,
                   x.RecordCreatedAt,
                   x.LastChangedOn,
                   x.Status,
                   x.NameDutch,
                   x.NameFrench,
                   x.NameGerman,
                   x.NameEnglish,
                   x.Organisation,
                   x.Plan))
            : x =>
                new MunicipalitySyndicationQueryResult(
                    x.MunicipalityId,
                    x.Position,
                    x.NisCode,
                    x.ChangeType,
                    x.RecordCreatedAt,
                    x.LastChangedOn,
                    x.Organisation,
                    x.Plan);

        protected override IQueryable<MunicipalitySyndicationItem> Filter(FilteringHeader<MunicipalitySyndicationFilter> filtering)
        {
            var municipalities = _context
                .MunicipalitySyndication
                .AsNoTracking();

            if (!filtering.ShouldFilter)
                return municipalities;

            if (filtering.Filter.Position.HasValue)
                municipalities = municipalities.Where(m => m.Position >= filtering.Filter.Position);

            return municipalities;
        }

        internal class MunicipalitySorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(MunicipalitySyndicationItem.Position)
            };

            public SortingHeader DefaultSortingHeader { get; } = new SortingHeader(nameof(MunicipalitySyndicationItem.Position), SortOrder.Ascending);
        }
    }

    public class MunicipalitySyndicationFilter
    {
        public long? Position { get; set; }
    }
}

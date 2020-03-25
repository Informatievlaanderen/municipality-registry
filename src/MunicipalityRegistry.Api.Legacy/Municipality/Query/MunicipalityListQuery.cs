namespace MunicipalityRegistry.Api.Legacy.Municipality.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Convertors;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;
    using Projections.Legacy.MunicipalityList;

    public class MunicipalityListQuery :
        Query<MunicipalityListItem, MunicipalityListFilter, MunicipalityListItem>
    {
        private readonly LegacyContext _context;

        protected override ISorting Sorting => new MunicipalityListSorting();

        public MunicipalityListQuery(LegacyContext context) => _context = context;

        protected override IQueryable<MunicipalityListItem> Filter(FilteringHeader<MunicipalityListFilter> filtering)
        {
            var municipalities = _context
                .MunicipalityList
                .OrderBy(x => x.NisCode)
                .AsNoTracking();

            if (!filtering.ShouldFilter)
                return municipalities;

            if (!string.IsNullOrEmpty(filtering.Filter.NisCode))
                municipalities = municipalities.Where(m => m.NisCode.Contains(filtering.Filter.NisCode));

            if (!string.IsNullOrEmpty(filtering.Filter.NameDutch))
                municipalities = municipalities.Where(m => m.NameDutch.Contains(filtering.Filter.NameDutch));

            if (!string.IsNullOrEmpty(filtering.Filter.NameEnglish))
                municipalities = municipalities.Where(m => m.NameEnglish.Contains(filtering.Filter.NameEnglish));

            if (!string.IsNullOrEmpty(filtering.Filter.NameFrench))
                municipalities = municipalities.Where(m => m.NameFrench.Contains(filtering.Filter.NameFrench));

            if (!string.IsNullOrEmpty(filtering.Filter.NameGerman))
                municipalities = municipalities.Where(m => m.NameGerman.Contains(filtering.Filter.NameGerman));

            if (!string.IsNullOrEmpty(filtering.Filter.Status))
            {
                if (Enum.TryParse(typeof(GemeenteStatus), filtering.Filter.Status, true, out var status))
                {
                    var municipalityStatus = ((GemeenteStatus) status).ConvertFromGemeenteStatus();
                    municipalities = municipalities.Where(m => m.Status.HasValue && m.Status.Value == municipalityStatus);
                }
                else
                    //have to filter on EF cannot return new List<>().AsQueryable() cause non-EF provider does not support .CountAsync()
                    municipalities = municipalities.Where(m => m.Status.HasValue && (int)m.Status.Value == -1);
            }

            return municipalities;
        }
    }

    public class MunicipalityListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(MunicipalityListItem.NisCode),
            nameof(MunicipalityListItem.DefaultName),
            nameof(MunicipalityListItem.NameDutch),
            nameof(MunicipalityListItem.NameEnglish),
            nameof(MunicipalityListItem.NameFrench),
            nameof(MunicipalityListItem.NameGerman)
        };

        public SortingHeader DefaultSortingHeader { get; } = new SortingHeader(nameof(MunicipalityListItem.NisCode), SortOrder.Ascending);
    }

    public class MunicipalityListFilter
    {
        public string NisCode { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }
        public string Status { get; set; }
    }
}

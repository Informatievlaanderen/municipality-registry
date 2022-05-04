namespace MunicipalityRegistry.Api.Legacy.Municipality.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Convertors;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;
    using Projections.Legacy.MunicipalityList;

    public class MunicipalityListQuery :
        Query<MunicipalityListItem, MunicipalityListFilter, MunicipalityListItem>
    {
        private readonly LegacyContext _context;

        protected override ISorting Sorting => new MunicipalityListSorting();

        public MunicipalityListQuery(LegacyContext context)
        {
            _context = context;
        }

        protected override IQueryable<MunicipalityListItem> Filter(FilteringHeader<MunicipalityListFilter> filtering)
        {
            var municipalities = _context
                .MunicipalityList
                .OrderBy(x => x.NisCode)
                .AsNoTracking();

            if (!filtering.ShouldFilter)
            {
                return municipalities;
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NisCode))
            {
                municipalities = municipalities.Where(m => m.NisCode != null && m.NisCode.Contains(filtering.Filter.NisCode));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameDutch))
            {
                municipalities = municipalities.Where(m => m.NameDutch != null && m.NameDutch.Contains(filtering.Filter.NameDutch));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameEnglish))
            {
                municipalities = municipalities.Where(m => m.NameEnglish != null && m.NameEnglish.Contains(filtering.Filter.NameEnglish));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameFrench))
            {
                municipalities = municipalities.Where(m => m.NameFrench != null && m.NameFrench.Contains(filtering.Filter.NameFrench));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameGerman))
            {
                municipalities = municipalities.Where(m => m.NameGerman != null && m.NameGerman.Contains(filtering.Filter.NameGerman));
            }

            var filterMunicipalityName = filtering.Filter.MunicipalityName.RemoveDiacritics();
            if (!string.IsNullOrEmpty(filtering.Filter.MunicipalityName))
            {
                municipalities = municipalities
                    .Where(x => x.NameDutchSearch == filterMunicipalityName
                        || x.NameFrenchSearch == filterMunicipalityName
                        || x.NameEnglishSearch == filterMunicipalityName
                        || x.NameGermanSearch == filterMunicipalityName);
            }

            if (!string.IsNullOrEmpty(filtering.Filter.Status))
            {
                if (Enum.TryParse(typeof(GemeenteStatus), filtering.Filter.Status, true, out var status))
                {
                    if (status == null)
                    {
                        throw new InvalidOperationException($"{nameof(status)} is null");
                    }
                    var municipalityStatus = ((GemeenteStatus)status).ConvertFromGemeenteStatus();
                    municipalities = municipalities.Where(m => m.Status.HasValue && m.Status.Value == municipalityStatus);
                }
                else
                //have to filter on EF cannot return new List<>().AsQueryable() cause non-EF provider does not support .CountAsync()
                {
                    municipalities = municipalities.Where(m => m.Status.HasValue && (int)m.Status.Value == -1);
                }
            }

            // this should be the last filter item processed because it implicitly realizes a list
            if (filtering.Filter.IsFlemishRegion)
            {
                municipalities = municipalities.FlemishMunicipalities();
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
        public string NisCode { get; set; } = string.Empty;
        public string MunicipalityName { get; set; } = string.Empty;
        public string NameDutch { get; set; } = string.Empty;
        public string NameFrench { get; set; } = string.Empty;
        public string NameGerman { get; set; } = string.Empty;
        public string NameEnglish { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsFlemishRegion { get; set; } = false;
    }
}

namespace MunicipalityRegistry.Api.Legacy.Municipality.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Bosa;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;
    using Projections.Legacy.MunicipalityName;
    using Requests;

    public class MunicipalityBosaQueryResult
    {
        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }
        public DateTimeOffset Version { get; set; }

        public MunicipalityBosaQueryResult(
            Guid municipalityId,
            string nisCode,
            DateTimeOffset version,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Version = version;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NameEnglish = nameEnglish;
        }

        public string GetNameValueByLanguage(Language language)
        {
            switch (language)
            {
                default:
                case Language.Dutch:
                    return NameDutch;

                case Language.French:
                    return NameFrench;

                case Language.German:
                    return NameGerman;

                case Language.English:
                    return NameEnglish;
            }
        }
    }

    public class MunicipalityBosaQuery : Query<MunicipalityName, MunicipalityNameFilter, MunicipalityBosaQueryResult>
    {
        private readonly LegacyContext _context;

        protected override ISorting Sorting => new MunicipalityNameSorting();

        public MunicipalityBosaQuery(LegacyContext context) => _context = context;

        protected override Expression<Func<MunicipalityName, MunicipalityBosaQueryResult>> Transformation =>
            x => new MunicipalityBosaQueryResult(
                x.MunicipalityId,
                x.NisCode,
                x.VersionTimestamp.ToBelgianDateTimeOffset(),
                x.NameDutch,
                x.NameFrench,
                x.NameGerman,
                x.NameEnglish);

        protected override IQueryable<MunicipalityName> Filter(FilteringHeader<MunicipalityNameFilter> filtering)
        {
            var municipalities = _context
                .MunicipalityName
                .AsNoTracking()
                .Where(m => m.IsFlemishRegion);

            if (!filtering.ShouldFilter)
                return municipalities;

            if (!string.IsNullOrEmpty(filtering.Filter.NisCode))
                municipalities = municipalities.Where(m => m.NisCode == filtering.Filter.NisCode);

            if (filtering.Filter.Version.HasValue)
                municipalities = municipalities.Where(m => m.VersionTimestamp.ToDateTimeOffset() == filtering.Filter.Version);

            if (string.IsNullOrEmpty(filtering.Filter.Name))
            {
                if (filtering.Filter.Language.HasValue)
                    municipalities = ApplyLanguageFilter(municipalities, filtering.Filter.Language.Value);

                return municipalities;
            }

            municipalities = CompareByCompareType(municipalities,
                filtering.Filter.Name.SanitizeForBosaSearch(),
                filtering.Filter.Language,
                filtering.Filter.IsContainsFilter);

            return municipalities;
        }

        private static IQueryable<MunicipalityName> ApplyLanguageFilter(
            IQueryable<MunicipalityName> query,
            Language language)
        {
            switch (language)
            {
                default:
                case Language.Dutch:
                    return query.Where(m => m.NameDutchSearch != null);

                case Language.French:
                    return query.Where(m => m.NameFrenchSearch != null);

                case Language.German:
                    return query.Where(m => m.NameGermanSearch != null);

                case Language.English:
                    return query.Where(m => m.NameEnglishSearch != null);
            }
        }

        private static IQueryable<MunicipalityName> CompareByCompareType(
            IQueryable<MunicipalityName> query,
            string searchValue,
            Language? language,
            bool isContainsFilter)
        {
            if (!language.HasValue)
            {
                return isContainsFilter
                    ? query.Where(i =>
                        i.NameDutchSearch.Contains(searchValue) ||
                        i.NameFrenchSearch.Contains(searchValue) ||
                        i.NameGermanSearch.Contains(searchValue) ||
                        i.NameEnglishSearch.Contains(searchValue))
                    : query.Where(i =>
                        i.NameDutch.Equals(searchValue) ||
                        i.NameFrench.Equals(searchValue) ||
                        i.NameGerman.Equals(searchValue) ||
                        i.NameEnglish.Equals(searchValue));
            }

            switch (language.Value)
            {
                default:
                case Language.Dutch:
                    return isContainsFilter
                        ? query.Where(i => i.NameDutchSearch.Contains(searchValue))
                        : query.Where(i => i.NameDutch.Equals(searchValue));

                case Language.French:
                    return isContainsFilter
                        ? query.Where(i => i.NameFrenchSearch.Contains(searchValue))
                        : query.Where(i => i.NameFrench.Equals(searchValue));

                case Language.German:
                    return isContainsFilter
                        ? query.Where(i => i.NameGermanSearch.Contains(searchValue))
                        : query.Where(i => i.NameGerman.Equals(searchValue));

                case Language.English:
                    return isContainsFilter
                        ? query.Where(i => i.NameEnglishSearch.Contains(searchValue))
                        : query.Where(i => i.NameEnglish.Equals(searchValue));
            }
        }
    }

    internal class MunicipalityNameSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(MunicipalityName.NisCode)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(MunicipalityName.NisCode), SortOrder.Ascending);
    }

    public class MunicipalityNameFilter
    {
        public string NisCode { get; set; }
        public DateTimeOffset? Version { get; set; }
        public string Name { get; set; }
        public Language? Language { get; set; }
        public bool IsContainsFilter { get; set; }

        public MunicipalityNameFilter(BosaMunicipalityRequest request)
        {
            NisCode = request.GemeenteCode?.ObjectId;
            Version = request.GemeenteCode?.VersieId;
            Name = request.Gemeentenaam?.Spelling;
            Language = (Language?) request.Gemeentenaam?.Taal;
            IsContainsFilter = (request.Gemeentenaam?.SearchType ?? BosaSearchType.Bevat) == BosaSearchType.Bevat;
        }
    }
}

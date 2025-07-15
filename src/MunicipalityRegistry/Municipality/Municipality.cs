namespace MunicipalityRegistry.Municipality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Events;
    using Exceptions;
    using NetTopologySuite.Geometries;

    public sealed partial class Municipality : AggregateRootEntity
    {
        public static readonly Func<Municipality> Factory = () => new Municipality();

        public static Municipality Register(
            MunicipalityId id,
            NisCode nisCode,
            IEnumerable<Language> officialLanguages,
            IEnumerable<Language> facilityLanguages,
            IEnumerable<MunicipalityName> names,
            ExtendedWkbGeometry geometry)
        {
            if (!officialLanguages.Any())
                throw new NoOfficialLanguagesException();

            var municipalityNames = names as MunicipalityName[] ?? names.ToArray();

            if (municipalityNames.Length == 0)
                throw new NoNameException("At least one name is required.");

            var duplicateLanguages = municipalityNames
                .GroupBy(x => x.Language)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            if (duplicateLanguages.Any())
                throw new DuplicateLanguageException($"Cannot give a municipality multiple names for the same language: {string.Join(", ", duplicateLanguages)}");

            var municipality = Factory();
            municipality.ApplyChange(new MunicipalityWasRegistered(id, nisCode));

            foreach (var language in officialLanguages.Distinct())
                municipality.ApplyChange(new MunicipalityOfficialLanguageWasAdded(id, language));

            foreach (var facilityLanguage in facilityLanguages.Distinct())
                municipality.ApplyChange(new MunicipalityFacilityLanguageWasAdded(id, facilityLanguage));

            foreach (var name in municipalityNames)
                municipality.Name(name);

            municipality.Draw(geometry);

            return municipality;
        }

        public void Merge(
            IEnumerable<MunicipalityId> municipalityIdsToMergeWithWith,
            IEnumerable<NisCode> nisCodesToMergeWith,
            MunicipalityId newMunicipalityId,
            NisCode newNisCode)
        {
            if (IsMerged)
                return;

            if (!IsCurrent)
                throw new MunicipalityHasInvalidStatusException();

            if (MunicipalityId == newMunicipalityId)
                throw new CannotMergeMunicipalityWithSelfException();

            ApplyChange(new MunicipalityWasMerged(
                MunicipalityId,
                NisCode,
                municipalityIdsToMergeWithWith,
                nisCodesToMergeWith,
                newMunicipalityId,
                newNisCode));
        }

        public void Activate()
        {
            if (Status == MunicipalityStatus.Current)
                return;

            if (Status == MunicipalityStatus.Retired)
                throw new MunicipalityHasInvalidStatusException();

            ApplyChange(new MunicipalityBecameCurrent(MunicipalityId));
        }

        public void Draw(ExtendedWkbGeometry geometry)
        {
            GuardPolygon(GeometryConfiguration.CreateWkbReader().Read(geometry));
            if(geometry.ToString() == Geometry?.ToString())
                return;

            if(Geometry is null)
                ApplyChange(new MunicipalityWasDrawn(MunicipalityId, geometry));
            else
                ApplyChange(new MunicipalityGeometryWasCorrected(MunicipalityId, geometry));
        }

        public void Remove()
        {
            if (IsRemoved)
                return;

            ApplyChange(new MunicipalityWasRemoved(MunicipalityId, NisCode));
        }

        private static void GuardPolygon(Geometry? geometry)
        {
            if (geometry is Polygon
                && geometry.SRID == ExtendedWkbGeometry.SridLambert72
                && GeometryValidator.IsValid(geometry))
            {
                return;
            }

            if (geometry is MultiPolygon multiPolygon
                && multiPolygon.SRID == ExtendedWkbGeometry.SridLambert72
                && multiPolygon.Geometries.All(GeometryValidator.IsValid))
            {
                return;
            }

            throw new InvalidPolygonException();
        }
    }
}

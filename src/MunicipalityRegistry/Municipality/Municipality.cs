namespace MunicipalityRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Events;
    using GeoAPI.Geometries;
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using NetTopologySuite.IO;
    using NodaTime;
    using System;
    using Version = Version;

    public partial class Municipality : AggregateRootEntity
    {
        public static readonly Func<Municipality> Factory = () => new Municipality();

        private bool IsRetired => _status == MunicipalityStatus.Retired;
        private bool IsCurrent => _status == MunicipalityStatus.Current;

        public static Municipality Register(MunicipalityId id, NisCode nisCode)
        {
            var municipality = Factory();
            municipality.ApplyChange(new MunicipalityWasRegistered(id, nisCode));
            return municipality;
        }

        public void DefineNisCode(NisCode nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
                throw new NoNisCodeException("Cannot clear NisCode of a municipality.");

            ApplyChange(new MunicipalityNisCodeWasDefined(_municipalityId, nisCode));
        }

        public void Name(MunicipalityName name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NoNameException("Cannot give a municipality an empty name.");

            ApplyChange(new MunicipalityWasNamed(_municipalityId, name));
        }

        public void ClearName(Language language)
        {
            ApplyChange(new MunicipalityNameWasCleared(_municipalityId, language));
        }

        public void DefinePrimaryLanguage(Language language)
        {
            ApplyChange(new MunicipalityPrimaryLanguageWasDefined(_municipalityId, language));
        }

        public void ClearPrimaryLanguage()
        {
            ApplyChange(new MunicipalityPrimaryLanguageWasCleared(_municipalityId));
        }

        public void DefineSecondaryLanguage(Language language)
        {
            ApplyChange(new MunicipalitySecondaryLanguageWasDefined(_municipalityId, language));
        }

        public void ClearSecondaryLanguage()
        {
            ApplyChange(new MunicipalitySecondaryLanguageWasCleared(_municipalityId));
        }

        public void Draw(ExtendedWkbGeometry ewkb)
        {
            ApplyChange(new MunicipalityWasDrawn(_municipalityId, ewkb));
        }

        public void ClearGeometry()
        {
            ApplyChange(new MunicipalityGeometryWasCleared(_municipalityId));
        }

        public void Retire(RetirementDate date)
        {
            ApplyChange(new MunicipalityWasRetired(_municipalityId, date));
        }

        public void ImportFromCrab(
            CrabMunicipalityId crabMunicipalityId,
            NisCode nisCode,
            CrabLanguage? primaryLanguage,
            CrabLanguage? secondaryLanguage,
            NumberOfFlags numberOfFlags,
            CrabLifetime crabLifetime,
            WkbGeometry geometry,
            CrabTimestamp crabTimestamp,
            CrabOperator crabOperator,
            CrabModification? crabModification,
            CrabOrganisation? crabOrganisation)
        {
            var endTime = crabLifetime?.EndDateTime;

            CheckChangedNisCode(nisCode, crabModification);
            CheckChangedPrimaryLanguage(primaryLanguage?.ToLanguage(), crabModification);
            CheckChangedSecondaryLanguage(secondaryLanguage?.ToLanguage(), crabModification);
            CheckChangedGeometry(geometry == null ? null : geometry, crabModification);
            CheckChangedStatus(endTime, crabModification);

            // Legacy Event
            ApplyChange(new MunicipalityWasImportedFromCrab(
                crabMunicipalityId,
                nisCode,
                primaryLanguage,
                secondaryLanguage,
                numberOfFlags,
                crabLifetime,
                geometry,
                crabTimestamp,
                crabOperator,
                crabModification,
                crabOrganisation));
        }

        public void ImportNameFromCrab(
            CrabMunicipalityId crabMunicipalityId,
            CrabMunicipalityNameId crabMunicipalityNameId,
            CrabMunicipalityName municipalityName,
            CrabLifetime lifetime,
            CrabTimestamp timestamp,
            CrabOperator @operator,
            CrabModification? modification,
            CrabOrganisation? organisation)
        {
            CheckChangedName(municipalityName.Language?.ToLanguage(), municipalityName.Name, modification);

            // Legacy Event
            ApplyChange(new MunicipalityNameWasImportedFromCrab(
                crabMunicipalityId,
                crabMunicipalityNameId,
                municipalityName,
                lifetime,
                timestamp,
                @operator,
                modification,
                organisation));
        }

        private void CheckChangedNisCode(NisCode nisCode, CrabModification? crabModification)
        {
            if (_nisCode == nisCode)
                return;

            if (nisCode == null)
                throw new NoNisCodeException("Cannot clear NisCode of a municipality.");

            if (crabModification == CrabModification.Correction)
                ApplyChange(new MunicipalityNisCodeWasCorrected(_municipalityId, nisCode));
            else
                DefineNisCode(nisCode);
        }

        private void CheckChangedPrimaryLanguage(Language? primaryLanguage, CrabModification? crabModification)
        {
            if (_primaryLanguage == primaryLanguage)
                return;

            if (primaryLanguage.HasValue)
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(new MunicipalityPrimaryLanguageWasCorrected(_municipalityId, primaryLanguage.Value));
                else
                    DefinePrimaryLanguage(primaryLanguage.Value);
            }
            else
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(new MunicipalityPrimaryLanguageWasCorrectedToCleared(_municipalityId));
                else
                    ClearPrimaryLanguage();
            }
        }

        private void CheckChangedSecondaryLanguage(Language? secondaryLanguage, CrabModification? crabModification)
        {
            if (_secondaryLanguage == secondaryLanguage)
                return;

            if (secondaryLanguage.HasValue)
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(new MunicipalitySecondaryLanguageWasCorrected(_municipalityId, secondaryLanguage.Value));
                else
                    DefineSecondaryLanguage(secondaryLanguage.Value);
            }
            else
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(new MunicipalitySecondaryLanguageWasCorrectedToCleared(_municipalityId));
                else
                    ClearSecondaryLanguage();
            }
        }

        private void CheckChangedGeometry(byte[] wkb, CrabModification? modification)
        {
            var ewkb = CreateEWkb(wkb);

            if (_geometry == ewkb)
                return;

            if (ewkb != null)
            {
                if (modification == CrabModification.Correction)
                    ApplyChange(new MunicipalityGeometryWasCorrected(_municipalityId, ewkb));
                else
                    Draw(ewkb);
            }
            else
            {
                if (modification == CrabModification.Correction)
                    ApplyChange(new MunicipalityGeometryWasCorrectedToCleared(_municipalityId));
                else
                    ClearGeometry();
            }
        }

        private static ExtendedWkbGeometry CreateEWkb(byte[] wkb)
        {
            if (wkb == null)
                return null;

            var geometry = new WKBReader(
                new NtsGeometryServices(
                    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XY),
                    new PrecisionModel(PrecisionModels.Floating),
                    SpatialReferenceSystemId.Lambert72)).Read(wkb);

            geometry.SRID = SpatialReferenceSystemId.Lambert72;

            var wkbWriter = new WKBWriter { Strict = false, HandleSRID = true };
            return new ExtendedWkbGeometry(wkbWriter.Write(geometry));
        }

        private void CheckChangedStatus(LocalDateTime? endTime, CrabModification? crabModification)
        {
            if (!IsRetired && endTime != null)
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(
                        new MunicipalityWasCorrectedToRetired(
                            _municipalityId,
                            new RetirementDate(endTime.Value.ToCrabInstant())));
                else
                    ApplyChange(
                        new MunicipalityWasRetired(
                            _municipalityId,
                            new RetirementDate(endTime.Value.ToCrabInstant())));
            }
            else if (!IsCurrent && endTime == null)
            {
                if (crabModification == CrabModification.Correction)
                    ApplyChange(new MunicipalityWasCorrectedToCurrent(_municipalityId));
                else
                    ApplyChange(new MunicipalityBecameCurrent(_municipalityId));
            }
        }

        private void CheckChangedName(Language? language, string name, CrabModification? modification)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NoNameException("Cannot give a municipality an empty name.");

            // TODO: Wat als language null is? Primary language?
            if (!language.HasValue)
                return;

            var newMunicipalityName = new MunicipalityName(name, language.Value);

            // We dont have this language yet, and it is actually filled in
            // We dont care about empty languages which we dont know yet anyway
            if (!_names.ContainsKey(language.Value) && !string.IsNullOrWhiteSpace(name))
            {
                if (modification == CrabModification.Correction)
                    ApplyChange(new MunicipalityNameWasCorrected(_municipalityId, newMunicipalityName));
                else
                    Name(newMunicipalityName);
            }

            // We already have this language, and it got cleared
            else if (_names.ContainsKey(language.Value) && string.IsNullOrWhiteSpace(name))
            {
                if (modification == CrabModification.Correction)
                    ApplyChange(new MunicipalityNameWasCorrectedToCleared(_municipalityId, language.Value));
                else
                    ClearName(language.Value);
            }

            // We already have this language, and it got changed
            else if (_names.ContainsKey(language.Value) && _names[language.Value] != newMunicipalityName)
            {
                if (modification == CrabModification.Correction)
                    ApplyChange(new MunicipalityNameWasCorrected(_municipalityId, newMunicipalityName));
                else
                    Name(newMunicipalityName);
            }

            // We dont care if it didnt change
        }
    }
}

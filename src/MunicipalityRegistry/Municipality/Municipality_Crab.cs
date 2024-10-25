namespace MunicipalityRegistry.Municipality
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Events;
    using Exceptions;
    using NodaTime;

    public sealed partial class Municipality
    {
        public static Municipality Register(MunicipalityId id, NisCode nisCode)
        {
            var municipality = Factory();
            municipality.ApplyChange(new MunicipalityWasRegistered(id, nisCode));
            return municipality;
        }

        public void DefineNisCode(NisCode nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
            {
                throw new NoNisCodeException("Cannot clear NisCode of a municipality.");
            }

            ApplyChange(new MunicipalityNisCodeWasDefined(MunicipalityId, nisCode));
        }

        public void Name(MunicipalityName name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NoNameException("Cannot give a municipality an empty name.");
            }

            ApplyChange(new MunicipalityWasNamed(MunicipalityId, name));
        }

        public void ClearName(Language language)
        {
            ApplyChange(new MunicipalityNameWasCleared(MunicipalityId, language));
        }

        public void ClearGeometry()
        {
            ApplyChange(new MunicipalityGeometryWasCleared(MunicipalityId));
        }

        public void Retire(RetirementDate date)
        {
            ApplyChange(new MunicipalityWasRetired(MunicipalityId, date));
        }

        public void ImportFromCrab(
            CrabMunicipalityId crabMunicipalityId,
            NisCode nisCode,
            CrabLanguage? primaryLanguage,
            CrabLanguage? secondaryLanguage,
            CrabLanguage? facilityLanguage,
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
            CheckChangedOfficialLanguages(new[] { primaryLanguage?.ToLanguage(), secondaryLanguage?.ToLanguage() });
            CheckChangedFacilityLanguages(facilityLanguage?.ToLanguage());
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

        private void CheckChangedFacilityLanguages(Language? facilityLanguage)
        {
            if (facilityLanguage.HasValue && !_facilitiesLanguages.Contains(facilityLanguage.Value))
            {
                ApplyChange(new MunicipalityFacilityLanguageWasAdded(MunicipalityId, facilityLanguage.Value));
            }

            if (!facilityLanguage.HasValue && _facilitiesLanguages.Any())
            {
                ApplyChange(new MunicipalityFacilityLanguageWasRemoved(MunicipalityId, _facilitiesLanguages.Single()));
            }
            else if (facilityLanguage.HasValue && _facilitiesLanguages.Count > 1)
            {
                ApplyChange(new MunicipalityFacilityLanguageWasRemoved(MunicipalityId, _facilitiesLanguages.Single(x => x != facilityLanguage.Value)));
            }
        }

        private void CheckChangedOfficialLanguages(IEnumerable<Language?> officialLanguages)
        {
            var languages = officialLanguages
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            foreach (var language in languages)
            {
                if (!_officialLanguages.Contains(language))
                {
                    ApplyChange(new MunicipalityOfficialLanguageWasAdded(MunicipalityId, language));
                }
            }

            var languagesToBeRemoved = _officialLanguages.Except(languages).ToList();

            foreach (var language in languagesToBeRemoved)
            {
                if (!languages.Contains(language))
                {
                    ApplyChange(new MunicipalityOfficialLanguageWasRemoved(MunicipalityId, language));
                }
            }
        }

        private void CheckChangedNisCode(NisCode? nisCode, CrabModification? crabModification)
        {
            if (nisCode == null)
            {
                throw new NoNisCodeException("Cannot clear NisCode of a municipality.");
            }

            if (NisCode == nisCode)
            {
                return;
            }

            if (crabModification == CrabModification.Correction)
            {
                ApplyChange(new MunicipalityNisCodeWasCorrected(MunicipalityId, nisCode));
            }
            else
            {
                DefineNisCode(nisCode);
            }
        }

        private void CheckChangedGeometry(byte[] wkb, CrabModification? modification)
        {
            var ewkb = ExtendedWkbGeometry.CreateEWkb(wkb);

            if (Geometry == ewkb)
            {
                return;
            }

            if (ewkb != null)
            {
                if (modification == CrabModification.Correction)
                {
                    ApplyChange(new MunicipalityGeometryWasCorrected(MunicipalityId, ewkb));
                }
                else
                {
                    Draw(ewkb);
                }
            }
            else
            {
                if (modification == CrabModification.Correction)
                {
                    ApplyChange(new MunicipalityGeometryWasCorrectedToCleared(MunicipalityId));
                }
                else
                {
                    ClearGeometry();
                }
            }
        }

        private void CheckChangedStatus(LocalDateTime? endTime, CrabModification? crabModification)
        {
            if (!IsRetired && endTime != null)
            {
                if (crabModification == CrabModification.Correction)
                {
                    ApplyChange(
                        new MunicipalityWasCorrectedToRetired(
                            MunicipalityId,
                            new RetirementDate(endTime.Value.ToCrabInstant())));
                }
                else
                {
                    ApplyChange(
                        new MunicipalityWasRetired(
                            MunicipalityId,
                            new RetirementDate(endTime.Value.ToCrabInstant())));
                }
            }
            else if (!IsCurrent && endTime == null)
            {
                if (crabModification == CrabModification.Correction)
                {
                    ApplyChange(new MunicipalityWasCorrectedToCurrent(MunicipalityId));
                }
                else
                {
                    ApplyChange(new MunicipalityBecameCurrent(MunicipalityId));
                }
            }
        }

        private void CheckChangedName(Language? language, string name, CrabModification? modification)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NoNameException("Cannot give a municipality an empty name.");
            }

            // TODO: Wat als language null is? Primary language?
            if (!language.HasValue)
            {
                return;
            }

            var newMunicipalityName = new MunicipalityName(name, language.Value);

            // We dont have this language yet, and it is actually filled in
            // We dont care about empty languages which we dont know yet anyway
            if (!_names.ContainsKey(language.Value))
            {
                if (modification == CrabModification.Correction && !string.IsNullOrWhiteSpace(name))
                {
                    ApplyChange(new MunicipalityNameWasCorrected(MunicipalityId, newMunicipalityName));
                }
                else
                {
                    Name(newMunicipalityName);
                }
            }

            // We already have this language, and it got cleared
            else if (_names.ContainsKey(language.Value) && string.IsNullOrWhiteSpace(name))
            {
                if (modification == CrabModification.Correction)
                {
                    ApplyChange(new MunicipalityNameWasCorrectedToCleared(MunicipalityId, language.Value));
                }
                else
                {
                    ClearName(language.Value);
                }
            }

            // We already have this language, and it got changed
            else if (_names.ContainsKey(language.Value) && _names[language.Value] != newMunicipalityName)
            {
                if (modification == CrabModification.Correction)
                {
                    ApplyChange(new MunicipalityNameWasCorrected(MunicipalityId, newMunicipalityName));
                }
                else
                {
                    Name(newMunicipalityName);
                }
            }

            // We dont care if it didnt change
        }
    }
}

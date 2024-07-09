namespace MunicipalityRegistry.Municipality
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using Events;

    public partial class Municipality
    {
        public MunicipalityId MunicipalityId { get; private set; }
        public NisCode NisCode { get; private set; }
        public MunicipalityStatus? Status { get; private set; }
        private RetirementDate _retiredDate;

        private readonly Dictionary<Language, MunicipalityName> _names
            = new Dictionary<Language, MunicipalityName>();

        private readonly List<Language> _officialLanguages = new List<Language>();
        private readonly List<Language> _facilitiesLanguages = new List<Language>();

        public IReadOnlyCollection<MunicipalityName> Names => _names.Values.ToList().AsReadOnly();
        public IReadOnlyCollection<Language> OfficialLanguages => _officialLanguages.AsReadOnly();
        public IReadOnlyCollection<Language> FacilitiesLanguages => _facilitiesLanguages.AsReadOnly();

        public ExtendedWkbGeometry? Geometry { get; private set; }

        private bool IsRetired => Status == MunicipalityStatus.Retired;
        private bool IsCurrent => Status == MunicipalityStatus.Current;
        public bool IsMerged { get; private set; }

        public Modification LastModificationBasedOnCrab { get; private set; }

        private Municipality()
        {
            Register<MunicipalityWasRegistered>(When);
            Register<MunicipalityNisCodeWasDefined>(When);
            Register<MunicipalityNisCodeWasCorrected>(When);

            Register<MunicipalityWasNamed>(When);
            Register<MunicipalityNameWasCorrected>(When);
            Register<MunicipalityNameWasCleared>(When);
            Register<MunicipalityNameWasCorrectedToCleared>(When);

            Register<MunicipalityOfficialLanguageWasAdded>(When);
            Register<MunicipalityOfficialLanguageWasRemoved>(When);

            Register<MunicipalityFacilityLanguageWasAdded>(When);
            Register<MunicipalityFacilityLanguageWasRemoved>(When);

            Register<MunicipalityWasDrawn>(When);
            Register<MunicipalityGeometryWasCorrected>(When);
            Register<MunicipalityGeometryWasCleared>(When);
            Register<MunicipalityGeometryWasCorrectedToCleared>(When);

            Register<MunicipalityBecameCurrent>(When);
            Register<MunicipalityWasCorrectedToCurrent>(When);
            Register<MunicipalityWasRetired>(When);
            Register<MunicipalityWasCorrectedToRetired>(When);

            Register<MunicipalityWasImportedFromCrab>(@event => WhenCrabEventApplied());
            Register<MunicipalityNameWasImportedFromCrab>(@event => WhenCrabEventApplied());

            Register<MunicipalityWasMerged>(When);
        }

        private void When(MunicipalityWasMerged @event)
        {
            Status = MunicipalityStatus.Retired;
            IsMerged = true;
        }

        private void WhenCrabEventApplied()
        {
            switch (LastModificationBasedOnCrab)
            {
                case Modification.Unknown:
                    LastModificationBasedOnCrab = Modification.Insert;
                    break;

                case Modification.Insert:
                    LastModificationBasedOnCrab = Modification.Update;
                    break;
            }
        }

        private void When(MunicipalityWasRegistered @event)
        {
            MunicipalityId = new MunicipalityId(@event.MunicipalityId);
            NisCode = new NisCode(@event.NisCode);
            Status = MunicipalityStatus.Proposed;
        }

        private void When(MunicipalityNisCodeWasDefined @event)
        {
            NisCode = new NisCode(@event.NisCode);
        }

        private void When(MunicipalityNisCodeWasCorrected @event)
        {
            NisCode = new NisCode(@event.NisCode);
        }

        private void When(MunicipalityWasNamed @event)
        {
            _names[@event.Language] = new MunicipalityName(@event.Name, @event.Language);
        }

        private void When(MunicipalityNameWasCorrected @event)
        {
            _names[@event.Language] = new MunicipalityName(@event.Name, @event.Language);
        }

        private void When(MunicipalityNameWasCleared @event)
        {
            _names.Remove(@event.Language);
        }

        private void When(MunicipalityNameWasCorrectedToCleared @event)
        {
            _names.Remove(@event.Language);
        }

        private void When(MunicipalityOfficialLanguageWasAdded @event)
        {
            _officialLanguages.Add(@event.Language);
        }

        private void When(MunicipalityOfficialLanguageWasRemoved @event)
        {
            _officialLanguages.Remove(@event.Language);
        }

        private void When(MunicipalityFacilityLanguageWasAdded @event)
        {
            _facilitiesLanguages.Add(@event.Language);
        }

        private void When(MunicipalityFacilityLanguageWasRemoved @event)
        {
            _facilitiesLanguages.Remove(@event.Language);
        }

        private void When(MunicipalityWasDrawn @event)
        {
            Geometry = new ExtendedWkbGeometry(@event.ExtendedWkbGeometry.ToByteArray());
        }

        private void When(MunicipalityGeometryWasCorrected @event)
        {
            Geometry = new ExtendedWkbGeometry(@event.ExtendedWkbGeometry.ToByteArray());
        }

        private void When(MunicipalityGeometryWasCleared @event)
        {
            Geometry = null;
        }

        private void When(MunicipalityGeometryWasCorrectedToCleared @event)
        {
            Geometry = null;
        }

        private void When(MunicipalityBecameCurrent @event)
        {
            Status = MunicipalityStatus.Current;
        }

        private void When(MunicipalityWasRetired @event)
        {
            Status = MunicipalityStatus.Retired;
            _retiredDate = new RetirementDate(@event.RetirementDate);
        }

        private void When(MunicipalityWasCorrectedToRetired @event)
        {
            Status = MunicipalityStatus.Retired;
            _retiredDate = new RetirementDate(@event.RetirementDate);
        }

        private void When(MunicipalityWasCorrectedToCurrent @event)
        {
            Status = MunicipalityStatus.Current;
        }
    }
}

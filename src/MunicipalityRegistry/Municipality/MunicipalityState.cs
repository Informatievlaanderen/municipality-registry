namespace MunicipalityRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using Events;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public partial class Municipality
    {
        private MunicipalityId _municipalityId;
        private NisCode _nisCode;
        private MunicipalityStatus? _status;
        private RetirementDate _retiredDate;

        private Language? _primaryLanguage;
        private Language? _secondaryLanguage;

        private readonly Dictionary<Language, MunicipalityName> _names
            = new Dictionary<Language, MunicipalityName>();

        private ExtendedWkbGeometry _geometry;

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

            Register<MunicipalityPrimaryLanguageWasDefined>(When);
            Register<MunicipalityPrimaryLanguageWasCorrected>(When);
            Register<MunicipalityPrimaryLanguageWasCleared>(When);
            Register<MunicipalityPrimaryLanguageWasCorrectedToCleared>(When);

            Register<MunicipalitySecondaryLanguageWasDefined>(When);
            Register<MunicipalitySecondaryLanguageWasCorrected>(When);
            Register<MunicipalitySecondaryLanguageWasCleared>(When);
            Register<MunicipalitySecondaryLanguageWasCorrectedToCleared>(When);

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
        }

        private void WhenCrabEventApplied()
        {
            if (LastModificationBasedOnCrab == Modification.Unknown)
                LastModificationBasedOnCrab = Modification.Insert;
            else if (LastModificationBasedOnCrab == Modification.Insert)
                LastModificationBasedOnCrab = Modification.Update;
        }

        private void When(MunicipalityWasRegistered @event)
        {
            _municipalityId = new MunicipalityId(@event.MunicipalityId);
            _nisCode = new NisCode(@event.NisCode);
        }

        private void When(MunicipalityNisCodeWasDefined @event)
        {
            _nisCode = new NisCode(@event.NisCode);
        }

        private void When(MunicipalityNisCodeWasCorrected @event)
        {
            _nisCode = new NisCode(@event.NisCode);
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

        private void When(MunicipalityPrimaryLanguageWasDefined @event)
        {
            _primaryLanguage = @event.Language;
        }

        private void When(MunicipalityPrimaryLanguageWasCorrected @event)
        {
            _primaryLanguage = @event.Language;
        }

        private void When(MunicipalityPrimaryLanguageWasCleared @event)
        {
            _primaryLanguage = null;
        }

        private void When(MunicipalityPrimaryLanguageWasCorrectedToCleared @event)
        {
            _primaryLanguage = null;
        }

        private void When(MunicipalitySecondaryLanguageWasDefined @event)
        {
            _secondaryLanguage = @event.Language;
        }

        private void When(MunicipalitySecondaryLanguageWasCorrected @event)
        {
            _secondaryLanguage = @event.Language;
        }

        private void When(MunicipalitySecondaryLanguageWasCleared @event)
        {
            _secondaryLanguage = null;
        }

        private void When(MunicipalitySecondaryLanguageWasCorrectedToCleared @event)
        {
            _secondaryLanguage = null;
        }

        private void When(MunicipalityWasDrawn @event)
        {
            _geometry = new ExtendedWkbGeometry(@event.ExtendedWkbGeometry.ToByteArray());
        }

        private void When(MunicipalityGeometryWasCorrected @event)
        {
            _geometry = new ExtendedWkbGeometry(@event.ExtendedWkbGeometry.ToByteArray());
        }

        private void When(MunicipalityGeometryWasCleared @event)
        {
            _geometry = null;
        }

        private void When(MunicipalityGeometryWasCorrectedToCleared @event)
        {
            _geometry = null;
        }

        private void When(MunicipalityBecameCurrent @event)
        {
            _status = MunicipalityStatus.Current;
        }

        private void When(MunicipalityWasRetired @event)
        {
            _status = MunicipalityStatus.Retired;
            _retiredDate = new RetirementDate(@event.RetirementDate);
        }

        private void When(MunicipalityWasCorrectedToRetired @event)
        {
            _status = MunicipalityStatus.Retired;
            _retiredDate = new RetirementDate(@event.RetirementDate);
        }

        private void When(MunicipalityWasCorrectedToCurrent @event)
        {
            _status = MunicipalityStatus.Current;
        }
    }
}

namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using Newtonsoft.Json;

    [EventName("MunicipalityGeometryWasCorrected")]
    [EventDescription("De grenzen van de gemeente werden gecorrigeerd.")]
    public class MunicipalityGeometryWasCorrected : IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }
        
        [EventPropertyDescription("Extended WKB-voorstelling van de gemeentegrenzen.")]
        [EventPropertyDescription("...")]
        public string ExtendedWkbGeometry { get; }
        
        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityGeometryWasCorrected(
            MunicipalityId municipalityId,
            ExtendedWkbGeometry ewkb)
        {
            MunicipalityId = municipalityId;
            ExtendedWkbGeometry = ewkb.ToString();
        }

        [JsonConstructor]
        private MunicipalityGeometryWasCorrected(
            Guid municipalityId,
            string extendedWkbGeometry,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                new ExtendedWkbGeometry(extendedWkbGeometry.ToByteArray())) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}

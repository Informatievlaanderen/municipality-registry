namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using Newtonsoft.Json;

    [EventName("MunicipalityGeometryWasCorrected")]
    [EventDescription("De geometrie voor de gemeentegrenzen werd gecorrigeerd.")]
    public class MunicipalityGeometryWasCorrected : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string ExtendedWkbGeometry { get; }
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

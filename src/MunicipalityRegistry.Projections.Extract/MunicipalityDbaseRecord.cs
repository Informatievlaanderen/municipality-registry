namespace MunicipalityRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class MunicipalityDbaseRecord : DbaseRecord
    {
        public static readonly MunicipalityDbaseSchema Schema = new MunicipalityDbaseSchema();

        public DbaseString id { get; }
        public DbaseString gemeenteid { get; }
        public DbaseDateTime versie { get; }
        public DbaseString gemeentenm { get; }
        public DbaseString status { get; }

        public MunicipalityDbaseRecord()
        {
            id = new DbaseString(Schema.id);
            gemeenteid = new DbaseString(Schema.gemeenteid);
            versie = new DbaseDateTime(Schema.versie);
            gemeentenm = new DbaseString(Schema.gemeentenm);
            status = new DbaseString(Schema.status);

            Values = new DbaseFieldValue[]
            {
                id,
                gemeenteid,
                versie,
                gemeentenm,
                status
            };
        }
    }
}

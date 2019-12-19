namespace MunicipalityRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class MunicipalityDbaseRecord : DbaseRecord
    {
        public static readonly MunicipalityDbaseSchema Schema = new MunicipalityDbaseSchema();

        public DbaseCharacter id { get; }
        public DbaseCharacter gemeenteid { get; }
        public DbaseCharacter versieid { get; }
        public DbaseCharacter gemeentenm { get; }
        public DbaseCharacter status { get; }

        public MunicipalityDbaseRecord()
        {
            id = new DbaseCharacter(Schema.id);
            gemeenteid = new DbaseCharacter(Schema.gemeenteid);
            versieid = new DbaseCharacter(Schema.versieid);
            gemeentenm = new DbaseCharacter(Schema.gemeentenm);
            status = new DbaseCharacter(Schema.status);

            Values = new DbaseFieldValue[]
            {
                id,
                gemeenteid,
                versieid,
                gemeentenm,
                status
            };
        }
    }
}

namespace MunicipalityRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class MunicipalityDbaseSchema : DbaseSchema
    {
        public DbaseField id => Fields[0];
        public DbaseField gemeenteid => Fields[1];
        public DbaseField versieid => Fields[2];
        public DbaseField gemeentenm => Fields[3];
        public DbaseField status => Fields[4];

        public MunicipalityDbaseSchema() => Fields = new[]
        {
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(id)), new DbaseFieldLength(50)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(gemeenteid)), new DbaseFieldLength(5)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(versieid)), new DbaseFieldLength(25)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(gemeentenm)), new DbaseFieldLength(40)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(status)), new DbaseFieldLength(50))
        };
    }
}

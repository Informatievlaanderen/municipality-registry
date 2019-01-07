namespace MunicipalityRegistry.Projections.Extract
{
    using System;

    public class MunicipalityExtractItem
    {
        public Guid? MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public byte[] DbaseRecord { get; set; }
    }
}

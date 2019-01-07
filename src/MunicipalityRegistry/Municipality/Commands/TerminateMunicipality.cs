namespace MunicipalityRegistry.Municipality.Commands
{
    public class TerminateMunicipality
    {
        public NisCode NisCode { get; }

        public TerminationDate Date { get; }

        public TerminateMunicipality(
            NisCode nisCode,
            TerminationDate date)
        {
            NisCode = nisCode;
            Date = date;
        }
    }
}

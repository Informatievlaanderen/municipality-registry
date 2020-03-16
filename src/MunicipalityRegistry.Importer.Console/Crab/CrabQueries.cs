namespace MunicipalityRegistry.Importer.Console.Crab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aiv.Vbr.CentraalBeheer.Crab.Entity;
    using Aiv.Vbr.CrabModel;

    public static class CrabQueries
    {
        private static readonly string DeletedBewerking = CrabBewerking.Verwijdering.Code;

        public static List<int> GetChangedGemeenteIdsBetween(DateTime since, DateTime until, Func<CRABEntities> crabEntitiesFactory)
        {
            var gemeenteIds = new List<int>();

            using (var crabEntities = crabEntitiesFactory())
            {
                crabEntities
                    .tblGemeente
                    .Where(x => x.beginTijd > since && x.beginTijd <= until)
                    .Select(x => x.gemeenteId)
                    .AddRangeTo(gemeenteIds);

                crabEntities
                    .tblGemeente_hist
                    .Where(x => x.beginTijd > since && x.beginTijd <= until)
                    .Select(x => x.gemeenteId.Value)
                    .AddRangeTo(gemeenteIds);

                crabEntities
                    .tblGemeente_hist
                    .Where(x => x.eindTijd > since && x.eindTijd <= until && x.eindBewerking == DeletedBewerking)
                    .Select(x => x.gemeenteId.Value)
                    .AddRangeTo(gemeenteIds);

                crabEntities
                    .tblGemeenteNaam
                    .Where(x => x.beginTijd > since && x.beginTijd <= until)
                    .Select(x => x.gemeenteId)
                    .AddRangeTo(gemeenteIds);

                crabEntities
                    .tblGemeenteNaam_hist
                    .Where(x => x.beginTijd > since && x.beginTijd <= until)
                    .Select(x => x.gemeenteId.Value)
                    .AddRangeTo(gemeenteIds);

                crabEntities
                    .tblGemeenteNaam_hist
                    .Where(x => x.eindTijd > since && x.eindTijd <= until && x.eindBewerking == DeletedBewerking)
                    .Select(x => x.gemeenteId.Value)
                    .AddRangeTo(gemeenteIds);
            }

            return gemeenteIds
                .Distinct()
                .ToList();
        }
    }

    public static class AddRangeToExtension
    {
        public static void AddRangeTo<T>(this IQueryable<T> collection, List<T> list) => list.AddRange(collection);
    }
}

namespace MunicipalityRegistry.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aiv.Vbr.CentraalBeheer.Crab.CrabHist;
    using Aiv.Vbr.CentraalBeheer.Crab.Entity;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Generate;
    using Crab;
    using Municipality.Commands.Crab;

    internal class MunicipalityCommandGenerator : ICommandGenerator<int>
    {
        public string Name => GetType().Name;

        public IEnumerable<int> GetChangedKeys(DateTime from, DateTime until) =>
            CrabQueries.GetChangedGemeenteIdsBetween(from, until);

        public IEnumerable<dynamic> GenerateInitCommandsFor(int key, DateTime from, DateTime until) =>
            CreateCommands(key, from, until);

        public IEnumerable<dynamic> GenerateUpdateCommandsFor(int key, DateTime from, DateTime until) =>
            CreateCommands(key, from, until);

        private static List<dynamic> CreateCommands(int gemeenteId, DateTime from, DateTime until)
        {
            var importGemeenteCommands = new List<ImportMunicipalityFromCrab>();
            var importGemeenteHistCommands = new List<ImportMunicipalityFromCrab>();
            var importGemeenteNaamCommands = new List<ImportMunicipalityNameFromCrab>();
            var importGemeenteNaamHistCommands = new List<ImportMunicipalityNameFromCrab>();

            using (var crabEntities = new CRABEntities())
            {
                var gemeente = GemeenteQueries.GetTblGemeentesByGemeenteIds(crabEntities, new HashSet<int> { gemeenteId }).SingleOrDefault();
                var gemeenteHists = GemeenteQueries.GetTblGemeenteHistByGemeenteByGemeente(crabEntities, gemeente);
                var namen = GemeenteQueries.GetTblGemeenteNamenById(crabEntities, gemeente.gemeenteId);
                var namenHists = GemeenteQueries.GetTblGemeenteNamenHistById(crabEntities, gemeente.gemeenteId);

                importGemeenteHistCommands.AddRange(MunicipalityMappings.GetCommandsFromGemeenteHist(gemeenteHists).ToList());
                importGemeenteCommands.Add(MunicipalityMappings.GetCommandFromGemeente(gemeente));

                importGemeenteNaamHistCommands.AddRange(MunicipalityMappings.GetCommandsFromGemeenteNaamHist(namenHists).ToList());
                importGemeenteNaamCommands.AddRange(MunicipalityMappings.GetCommandsFromGemeenteNaam(namen).ToList());
            }

            var firstCommand = importGemeenteCommands
                .Concat(importGemeenteHistCommands)
                .OrderBy(x => x.Timestamp)
                .First();

            var allCommands = importGemeenteHistCommands.Select(x => Tuple.Create<dynamic, int>(x, 0))
                .Concat(importGemeenteCommands.Select(x => Tuple.Create<dynamic, int>(x, 1)))
                .Concat(importGemeenteNaamHistCommands.Select(x => Tuple.Create<dynamic, int>(x, 2)))
                .Concat(importGemeenteNaamCommands.Select(x => Tuple.Create<dynamic, int>(x, 3)));

            return new[] { firstCommand }.Concat(allCommands
                .Where(x => x.Item1.Timestamp > from.ToCrabInstant() && x.Item1.Timestamp <= until.ToCrabInstant() && !x.Item1.Equals(firstCommand))
                .OrderBy(x => x.Item1.Timestamp)
                .ThenBy(x => x.Item2)
                .Select(x => x.Item1))
                .ToList();
        }
    }
}

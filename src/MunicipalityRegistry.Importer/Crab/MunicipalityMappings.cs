namespace MunicipalityRegistry.Importer.Crab
{
    using Aiv.Vbr.CentraalBeheer.Crab.Entity;
    using Aiv.Vbr.CrabModel;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Municipality.Commands.Crab;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CrabLifetime = Be.Vlaanderen.Basisregisters.Crab.CrabLifetime;

    internal class MunicipalityMappings
    {
        internal static List<string> NisCodesWith2OfficialLanguages = new List<string>
        {
            "21001",
            "21002",
            "21003",
            "21004",
            "21005",
            "21006",
            "21007",
            "21008",
            "21009",
            "21010",
            "21011",
            "21012",
            "21013",
            "21014",
            "21015",
            "21016",
            "21017",
            "21018",
            "21019",
        };

        public static ImportMunicipalityFromCrab GetCommandFromGemeente(tblGemeente gemeente)
        {
            MapLogging.Log(".");

            var secondaryLanguage = ParseLanguage(gemeente.TaalcodeTweedeTaal);
            var facilityLanguage = ParseLanguage(gemeente.TaalcodeTweedeTaal);

            if (NisCodesWith2OfficialLanguages.Contains(gemeente.NISCode))
                facilityLanguage = null;
            else
                secondaryLanguage = null;

            return new ImportMunicipalityFromCrab(
                new NisCode(gemeente.NISCode),
                ParseLanguage(gemeente.Taalcode),
                secondaryLanguage,
                facilityLanguage,
                gemeente.Geometry != null
                    ? new WkbGeometry(gemeente.Geometry.WKB)
                    : null,
                gemeente.aantal_vlaggen.HasValue
                    ? new NumberOfFlags(gemeente.aantal_vlaggen.Value)
                    : null,
                new CrabLifetime(
                    gemeente.beginDatum.ToCrabLocalDateTime(),
                    gemeente.eindDatum?.ToCrabLocalDateTime()),
                new CrabTimestamp(gemeente.CrabTimestamp.ToCrabInstant()),
                new CrabOperator(gemeente.Operator),
                ParseBewerking(gemeente.Bewerking),
                ParseOrganisatie(gemeente.Organisatie),
                new CrabMunicipalityId(gemeente.gemeenteId));
        }

        public static IEnumerable<ImportMunicipalityFromCrab> GetCommandsFromGemeenteHist(
            IEnumerable<tblGemeente_hist> gemeenteHists)
        {
            return gemeenteHists
                .OrderBy(g => g.CrabTimestamp)
                .Select(gemeenteHist =>
                {
                    MapLogging.Log(".");

                    var secondaryLanguage = ParseLanguage(gemeenteHist.TaalcodeTweedeTaal);
                    var facilityLanguage = ParseLanguage(gemeenteHist.TaalcodeTweedeTaal);

                    if (NisCodesWith2OfficialLanguages.Contains(gemeenteHist.NISCode))
                        facilityLanguage = null;
                    else
                        secondaryLanguage = null;

                    return new ImportMunicipalityFromCrab(
                        new NisCode(gemeenteHist.NISCode),
                        ParseLanguage(gemeenteHist.Taalcode),
                        secondaryLanguage,
                        facilityLanguage,
                        gemeenteHist.Geometry != null
                            ? new WkbGeometry(gemeenteHist.Geometry.WKB)
                            : null,
                        gemeenteHist.aantal_vlaggen.HasValue
                            ? new NumberOfFlags(gemeenteHist.aantal_vlaggen.Value)
                            : null,
                        new CrabLifetime(
                            gemeenteHist.beginDatum?.ToCrabLocalDateTime(),
                            gemeenteHist.eindDatum?.ToCrabLocalDateTime()),
                        new CrabTimestamp(gemeenteHist.CrabTimestamp.ToCrabInstant()),
                        new CrabOperator(gemeenteHist.Operator),
                        ParseBewerking(gemeenteHist.Bewerking),
                        ParseOrganisatie(gemeenteHist.Organisatie),
                        gemeenteHist.gemeenteId.HasValue
                            ? new CrabMunicipalityId(gemeenteHist.gemeenteId.Value)
                            : null);
                });
        }

        public static IEnumerable<ImportMunicipalityNameFromCrab> GetCommandsFromGemeenteNaam(
            IEnumerable<tblGemeenteNaam> namen)
        {
            return namen
                .OrderBy(n => n.CrabTimestamp)
                .Select(naam =>
                {
                    MapLogging.Log(".");

                    return new ImportMunicipalityNameFromCrab(
                        new CrabMunicipalityId(naam.gemeenteId),
                        new CrabMunicipalityName(naam.Naam, ParseLanguage(naam.Taalcode)),
                        new CrabLifetime(
                            naam.beginDatum.ToCrabLocalDateTime(),
                            naam.eindDatum?.ToCrabLocalDateTime()),
                        new CrabTimestamp(naam.CrabTimestamp.ToCrabInstant()),
                        new CrabOperator(naam.Operator),
                        ParseBewerking(naam.Bewerking),
                        ParseOrganisatie(naam.Organisatie),
                        new CrabMunicipalityNameId(naam.gemeenteNaamId));
                });
        }

        public static IEnumerable<ImportMunicipalityNameFromCrab> GetCommandsFromGemeenteNaamHist(
            IEnumerable<tblGemeenteNaam_hist> namenHists)
        {
            return namenHists
                .OrderBy(nh => nh.CrabTimestamp)
                .Where(nh => nh.gemeenteId.HasValue)
                .Select(naamHist =>
                {
                    MapLogging.Log(".");

                    return new ImportMunicipalityNameFromCrab(
                        new CrabMunicipalityId(naamHist.gemeenteId.Value),
                        new CrabMunicipalityName(naamHist.Naam, ParseLanguage(naamHist.Taalcode)),
                        new CrabLifetime(
                            naamHist.beginDatum?.ToCrabLocalDateTime(),
                            naamHist.eindDatum?.ToCrabLocalDateTime()),
                        new CrabTimestamp(naamHist.CrabTimestamp.ToCrabInstant()),
                        new CrabOperator(naamHist.Operator),
                        ParseBewerking(naamHist.Bewerking),
                        ParseOrganisatie(naamHist.Organisatie),
                        naamHist.gemeenteNaamId.HasValue
                            ? new CrabMunicipalityNameId(naamHist.gemeenteNaamId.Value)
                            : null);
                });
        }

        private static CrabLanguage? ParseLanguage(string taalCode)
        {
            if (string.IsNullOrWhiteSpace(taalCode))
                return null;

            switch (taalCode.ToLowerInvariant())
            {
                case "nl": return CrabLanguage.Dutch;
                case "fr": return CrabLanguage.French;
                case "de": return CrabLanguage.German;
                case "en": return CrabLanguage.English;
            }

            throw new Exception($"Onbekende taalcode {taalCode}.");
        }

        private static CrabModification? ParseBewerking(CrabBewerking bewerking)
        {
            if (bewerking == null)
                return null;

            if (bewerking.Code == CrabBewerking.Invoer.Code)
                return CrabModification.Insert;

            if (bewerking.Code == CrabBewerking.Correctie.Code)
                return CrabModification.Correction;

            if (bewerking.Code == CrabBewerking.Historering.Code)
                return CrabModification.Historize;

            if (bewerking.Code == CrabBewerking.Verwijdering.Code)
                return CrabModification.Delete;

            throw new Exception($"Onbekende bewerking {bewerking.Code}");
        }

        private static CrabOrganisation? ParseOrganisatie(CrabOrganisatieEnum organisatie)
        {
            if (organisatie == null)
                return null;

            if (CrabOrganisatieEnum.AKRED.Code == organisatie.Code)
                return CrabOrganisation.Akred;

            if (CrabOrganisatieEnum.Andere.Code == organisatie.Code)
                return CrabOrganisation.Other;

            if (CrabOrganisatieEnum.DePost.Code == organisatie.Code)
                return CrabOrganisation.DePost;

            if (CrabOrganisatieEnum.Gemeente.Code == organisatie.Code)
                return CrabOrganisation.Municipality;

            if (CrabOrganisatieEnum.NGI.Code == organisatie.Code)
                return CrabOrganisation.Ngi;

            if (CrabOrganisatieEnum.NavTeq.Code == organisatie.Code)
                return CrabOrganisation.NavTeq;

            if (CrabOrganisatieEnum.Rijksregister.Code == organisatie.Code)
                return CrabOrganisation.NationalRegister;

            if (CrabOrganisatieEnum.TeleAtlas.Code == organisatie.Code)
                return CrabOrganisation.TeleAtlas;

            if (CrabOrganisatieEnum.VKBO.Code == organisatie.Code)
                return CrabOrganisation.Vkbo;

            if (CrabOrganisatieEnum.VLM.Code == organisatie.Code)
                return CrabOrganisation.Vlm;

            throw new Exception($"Onbekende organisatie {organisatie.Code}");
        }
    }
}

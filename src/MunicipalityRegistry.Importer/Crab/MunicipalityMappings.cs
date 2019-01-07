namespace MunicipalityRegistry.Importer.Crab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aiv.Vbr.CentraalBeheer.Crab.Entity;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Aiv.Vbr.CrabModel;
    using Municipality.Commands.Crab;
    using CrabLifetime = Be.Vlaanderen.Basisregisters.Crab.CrabLifetime;

    internal class MunicipalityMappings
    {
        public static ImportMunicipalityFromCrab GetCommandFromGemeente(tblGemeente gemeente)
        {
            MapLogging.Log(".");

            return new ImportMunicipalityFromCrab(
                new NisCode(gemeente.NISCode),
                ParseLanguage(gemeente.Taalcode),
                ParseLanguage(gemeente.TaalcodeTweedeTaal),
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

                    return new ImportMunicipalityFromCrab(
                        new NisCode(gemeenteHist.NISCode),
                        ParseLanguage(gemeenteHist.Taalcode),
                        ParseLanguage(gemeenteHist.TaalcodeTweedeTaal),
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

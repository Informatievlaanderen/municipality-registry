namespace MunicipalityRegistry.Tests.AggregateTests.WhenRegisteringMunicipality
{
    using System.Collections.Generic;
    using System.Linq;
    using Municipality.Commands;

    public static class RegisterMunicipalityExensions
    {
        public static RegisterMunicipality WithOfficialLanguages(this RegisterMunicipality command, Language[] languages)
        {
            return new RegisterMunicipality(
                command.MunicipalityId,
                command.NisCode,
                languages.ToList(),
                command.OfficialLanguages,
                command.Names,
                command.Geometry,
                command.Provenance);
        }

        public static RegisterMunicipality WithFacilityLanguages(this RegisterMunicipality command, Language[] languages)
        {
            return new RegisterMunicipality(
                command.MunicipalityId,
                command.NisCode,
                command.OfficialLanguages,
                languages.ToList(),
                command.Names,
                command.Geometry,
                command.Provenance);
        }

        public static RegisterMunicipality WithNames(this RegisterMunicipality command, List<KeyValuePair<Language, string>> names)
        {
            return new RegisterMunicipality(
                command.MunicipalityId,
                command.NisCode,
                command.OfficialLanguages,
                command.FacilityLanguages,
                names.Select(n => new MunicipalityName(n.Value, n.Key)).ToList(),
                command.Geometry,
                command.Provenance);
        }

        public static RegisterMunicipality WithGeometry(this RegisterMunicipality command, ExtendedWkbGeometry geometry)
        {
            return new RegisterMunicipality(
                command.MunicipalityId,
                command.NisCode,
                command.OfficialLanguages,
                command.FacilityLanguages,
                command.Names,
                geometry,
                command.Provenance);
        }
    }
}

namespace MunicipalityRegistry.Tests.ImportApi.Merger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api.Import.Merger;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using Projections.Legacy.MunicipalityDetail;
    using Xunit;

    public class ProposeMergerRequestValidatorTests
    {
        private readonly ProposeMergerRequestValidator _validator;
        private readonly FakeLegacyContext _fakeLegacyContext;

        public ProposeMergerRequestValidatorTests()
        {
            _fakeLegacyContext = new FakeLegacyContextFactory().CreateDbContext();
            _validator = new ProposeMergerRequestValidator(_fakeLegacyContext);
        }

        [Fact]
        public void When_nis_code_is_empty_then_validation_fails()
        {
            var request = new ProposeMergerRequest { NisCode = string.Empty, MergerOf = [] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Where(x => x.PropertyName == nameof(request.NisCode)).Should().HaveCount(2);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456")]
        public void When_nis_code_is_invalid_then_validation_fails(string nisCode)
        {
            var request = new ProposeMergerRequest { NisCode = nisCode, MergerOf = [] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.NisCode));
        }

        [Fact]
        public void When_nis_code_is_valid_and_propose_municipality_is_null_and_is_not_in_legacy_context_then_validation_fails()
        {
            var request = new ProposeMergerRequest { NisCode = "12345", MergerOf = [] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.NisCode));
        }

        [Fact]
        public void When_propose_municipality_is_not_null_and_official_languages_is_empty_then_validation_fails()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "67890", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergerRequest { NisCode = "12345", ProposeMunicipality = new ProposeMergerMunicipalityRequest { OfficialLanguages = [] }, MergerOf = ["67890"] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == $"{nameof(request.ProposeMunicipality)}.{nameof(request.ProposeMunicipality.OfficialLanguages)}");
        }

        [Fact]
        public void When_propose_municipality_is_not_null_and_official_languages_contains_language_that_has_no_name_then_validation_fails()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "67890", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergerRequest { NisCode = "12345", ProposeMunicipality = new ProposeMergerMunicipalityRequest { OfficialLanguages = [Taal.EN], Names = [] }, MergerOf = ["67890"] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == $"{nameof(request.ProposeMunicipality)}.{nameof(request.ProposeMunicipality.OfficialLanguages)}[0]");
        }

        [Fact]
        public void When_propose_municipality_is_not_null_and_facilities_languages_contains_language_that_is_also_official_then_validation_fails()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "67890", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergerRequest
            {
                NisCode = "12345",
                ProposeMunicipality = new ProposeMergerMunicipalityRequest
                {
                    OfficialLanguages = [Taal.NL],
                    FacilitiesLanguages = [Taal.NL],
                    Names = new Dictionary<Taal, string>
                    {
                        { Taal.NL, "name" }
                    }
                }, MergerOf = ["67890"]
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == $"{nameof(request.ProposeMunicipality)}.{nameof(request.ProposeMunicipality.FacilitiesLanguages)}[0]");
        }

        [Fact]
        public void When_merger_of_is_empty_then_validation_fails()
        {
            var request = new ProposeMergerRequest { NisCode = "12345", MergerOf = [] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.MergerOf));
        }

        [Fact]
        public void When_merger_of_contains_nis_code_then_validation_fails()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "12345", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergerRequest { NisCode = "12345", MergerOf = ["67890", "12345"] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == $"{nameof(request.MergerOf)}[1]");
        }

        [Fact]
        public void When_merger_of_contains_nis_code_that_does_not_exist_then_validation_fails()
        {
            var request = new ProposeMergerRequest { NisCode = "12345", MergerOf = ["67890"] };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == $"{nameof(request.MergerOf)}[0]");
        }
    }
}

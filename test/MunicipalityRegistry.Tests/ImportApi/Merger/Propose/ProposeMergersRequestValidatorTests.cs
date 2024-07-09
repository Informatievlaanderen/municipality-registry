namespace MunicipalityRegistry.Tests.ImportApi.Merger.Propose
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using MunicipalityRegistry.Api.Import;
    using MunicipalityRegistry.Api.Import.Merger.Propose;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using Xunit;

    public class ProposeMergersRequestValidatorTests
    {
        private readonly ProposeMergersRequestValidator _validator;
        private readonly FakeLegacyContext _fakeLegacyContext;
        private readonly FakeImportContext _fakeImportContext;

        public ProposeMergersRequestValidatorTests()
        {
            _fakeLegacyContext = new FakeLegacyContextFactory().CreateDbContext();
            _fakeImportContext = new FakeImportContextFactory().CreateDbContext();
            _validator = new ProposeMergersRequestValidator(_fakeLegacyContext, _fakeImportContext);
        }

        [Fact]
        public void When_merger_year_is_before_current_year_then_validation_fails()
        {
            var request = new ProposeMergersRequest { MergerYear = DateTime.Now.Year - 1 };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.MergerYear));
        }

        [Fact]
        public void When_merger_year_is_already_in_import_context_then_validation_fails()
        {
            var year = DateTime.Now.Year + 1;
            _fakeImportContext.MunicipalityMergers.Add(new MunicipalityMerger(year, Guid.NewGuid(), [], Guid.NewGuid()));
            _fakeImportContext.SaveChanges();

            var request = new ProposeMergersRequest { MergerYear = year };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.MergerYear));
        }

        [Fact]
        public void When_municipalities_is_empty_then_validation_fails()
        {
            var request = new ProposeMergersRequest();

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.Municipalities));
        }

        [Fact]
        public void When_municipalities_contains_duplicates_then_validation_fails()
        {
            var request = new ProposeMergersRequest
            {
                Municipalities = new List<ProposeMergerRequest>
                {
                    new ProposeMergerRequest { NisCode = "123", MergerOf = []},
                    new ProposeMergerRequest { NisCode = "123", MergerOf = []}
                }
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.Municipalities));
        }

        [Fact]
        public void When_municipalities_contains_duplicates_in_merger_of_then_validation_fails()
        {
            var request = new ProposeMergersRequest
            {
                Municipalities = new List<ProposeMergerRequest>
                {
                    new ProposeMergerRequest { NisCode = "123", MergerOf = new List<string> { "456" } },
                    new ProposeMergerRequest { NisCode = "456", MergerOf = new List<string> { "123" } }
                }
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.Municipalities));
        }

        [Fact]
        public void When_propose_municipality_is_null_then_validation_succeeds()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "12345", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "67890", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergersRequest
            {
                MergerYear = DateTime.Now.Year + 1,
                Municipalities = [new ProposeMergerRequest { NisCode = "12345", MergerOf = ["67890"] }]
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void When_propose_municipality_is_not_null_then_validation_succeeds()
        {
            _fakeLegacyContext.MunicipalityDetail.Add(new MunicipalityDetail { NisCode = "67890", MunicipalityId = Guid.NewGuid() });
            _fakeLegacyContext.SaveChanges();

            var request = new ProposeMergersRequest
            {
                MergerYear = DateTime.Now.Year + 1,
                Municipalities = [new ProposeMergerRequest
                {
                    NisCode = "12345",
                    ProposeMunicipality = new ProposeMergerMunicipalityRequest
                    {
                        OfficialLanguages = [Taal.NL],
                        FacilitiesLanguages = [Taal.FR],
                        Names = new Dictionary<Taal, string>
                        {
                            { Taal.NL, "naam" },
                            { Taal.FR, "nom" },
                            { Taal.DE, "name" },
                            { Taal.EN, "name" }
                        }
                    },
                    MergerOf = ["67890"]
                }]
            };

            var result = _validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }
    }
}

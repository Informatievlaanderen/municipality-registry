namespace MunicipalityRegistry.Api.Import.Merger.Propose
{
    using System;
    using System.Linq;
    using FluentValidation;
    using MunicipalityRegistry.Projections.Legacy;

    public sealed class ProposeMergerRequestValidator : AbstractValidator<ProposeMergerRequest>
    {
        public ProposeMergerRequestValidator(LegacyContext legacyContext)
        {
            RuleFor(request => request.NisCode)
                .NotEmpty()
                .Length(5)
                .DependentRules(() =>
                {
                    When(request => request.ProposeMunicipality is null, () =>
                    {
                        RuleFor(request => request.NisCode)
                            .Must(nisCode =>
                                legacyContext
                                    .MunicipalityDetail
                                    .Any(municipality => municipality.NisCode != nisCode));
                    });
                });

            When(request => request.ProposeMunicipality is not null, () =>
            {
                RuleFor(request => request.ProposeMunicipality!.OfficialLanguages)
                    .NotEmpty();

                RuleForEach(request => request.ProposeMunicipality!.OfficialLanguages)
                    .Must((request, value) => request.ProposeMunicipality!.Names.ContainsKey(value));

                RuleForEach(request => request.ProposeMunicipality!.FacilitiesLanguages)
                    .Must((request, value) => !request.ProposeMunicipality!.OfficialLanguages.Contains(value));
            });

            RuleFor(request => request.MergerOf)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleForEach(request => request.MergerOf)
                        .NotEmpty()
                        .Must((request, value) => !string.Equals(request.NisCode, value, StringComparison.InvariantCultureIgnoreCase));

                    RuleForEach(request => request.MergerOf)
                        .Must(nisCode => legacyContext
                            .MunicipalityDetail
                            .Any(municipality => municipality.NisCode == nisCode));
                });
        }
    }
}

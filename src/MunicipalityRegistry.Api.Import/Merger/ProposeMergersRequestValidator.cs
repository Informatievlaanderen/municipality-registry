namespace MunicipalityRegistry.Api.Import.Merger
{
    using System;
    using System.Linq;
    using FluentValidation;
    using Projections.Legacy;

    public sealed class ProposeMergersRequestValidator : AbstractValidator<ProposeMergersRequest>
    {
        public ProposeMergersRequestValidator(LegacyContext legacyContext, ImportContext importContext)
        {
            RuleFor(request => request.MergerYear)
                .GreaterThan(DateTime.Now.Year)
                .DependentRules(() =>
                {
                    RuleFor(request => request.MergerYear)
                        .Must(year => importContext.MunicipalityMergers.All(x => x.Year != year));
                });

            RuleFor(request => request.Municipalities)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleForEach(request => request.Municipalities)
                        .SetValidator(new ProposeMergerRequestValidator(legacyContext));

                    RuleFor(r => r.Municipalities)
                        .Must(request =>
                            request
                                .Select(x => x.NisCode.ToLowerInvariant())
                                .Concat(request.SelectMany(x => x.MergerOf.Select(y => y.ToLowerInvariant())))
                                .GroupBy(nisCode => nisCode)
                                .All(x => x.Count() == 1));
                });
        }
    }
}

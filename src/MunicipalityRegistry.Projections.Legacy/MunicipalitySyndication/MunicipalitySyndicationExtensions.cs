namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalitySyndicationExtensions
    {
        public static async Task CreateNewMunicipalitySyndicationItem<T>(
            this LegacyContext context,
            Guid municipalityId,
            Envelope<T> message,
            Action<MunicipalitySyndicationItem> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance
        {
            var municipalitySyndicationItem = await context.LatestPosition(municipalityId, ct);

            if (municipalitySyndicationItem == null)
                throw DatabaseItemNotFound(municipalityId);

            var provenance = message.Message.Provenance;

            var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                message.Position,
                message.EventName,
                provenance.Timestamp,
                applyEventInfoOn);

            newMunicipalitySyndicationItem.ApplyProvenance(provenance);
            newMunicipalitySyndicationItem.SetEventData(message.Message);

            await context
                .MunicipalitySyndication
                .AddAsync(newMunicipalitySyndicationItem, ct);
        }

        public static async Task<MunicipalitySyndicationItem> LatestPosition(
            this LegacyContext context,
            Guid municipalityId,
            CancellationToken ct)
            => context
                   .MunicipalitySyndication
                   .Local
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .MunicipalitySyndication
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);

        public static void ApplyProvenance(
            this MunicipalitySyndicationItem item,
            ProvenanceData provenance)
        {
            item.Application = provenance.Application;
            item.Modification = provenance.Modification;
            item.Operator = provenance.Operator;
            item.Organisation = provenance.Organisation;
            item.Reason = provenance.Reason;
        }

        public static void SetEventData<T>(this MunicipalitySyndicationItem syndicationItem, T message)
            => syndicationItem.EventDataAsXml = message.ToXml(message.GetType().GetCustomAttribute<EventNameAttribute>()!.Value).ToString(SaveOptions.DisableFormatting);

        private static ProjectionItemNotFoundException<MunicipalitySyndicationProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalitySyndicationProjections>(municipalityId.ToString("D"));
    }
}

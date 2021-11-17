namespace MunicipalityRegistry.Projections.StreamPublisher.Extensions
{
    using System;
    using System.Text.Json;
    using From = Municipality.Events;
    using Contracts = Be.Vlaanderen.Basisregisters.GrAr.Contracts;

    public static class EventMapperExtensions
    {
        private static string GetPayload<TMessage>(TMessage message)
            where TMessage : From.IMunicipalityMessage =>
            JsonSerializer.Serialize(message);

        public static Contracts.Envelope<Contracts.IQueueMessage> ToContract<TMessage>(this TMessage message)
            where TMessage : From.IMunicipalityMessage =>
            new Contracts.Envelope<Contracts.IQueueMessage>
            {
                EventName = typeof(TMessage).Name,
                Id = Guid.NewGuid().ToString("D"),
                Timestamp = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture),
                Payload = GetPayload(message)
            };
    }
}

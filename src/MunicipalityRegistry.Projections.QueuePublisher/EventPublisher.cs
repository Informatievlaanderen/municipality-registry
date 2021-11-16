namespace MunicipalityRegistry.Projections.QueuePublisher
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.RabbitMq;

    public class EventPublisher: TopicProducer<Envelope<IQueueMessage>>
    {
        public EventPublisher(MessageHandlerContext context) : base(context, "municipality-events")
        {
        }

        protected override void OnPublishMessagesExceptionHandler(Exception exception, Envelope<IQueueMessage>[] messages)
        {
            throw new NotImplementedException();
        }

    }
}

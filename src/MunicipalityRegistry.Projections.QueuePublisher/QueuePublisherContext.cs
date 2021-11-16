namespace MunicipalityRegistry.Projections.QueuePublisher
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class QueuePublisherContext : RunnerDbContext<QueuePublisherContext>
    {
        public override string ProjectionStateSchema => Schema.QueuePublisher;

        public DbSet<MessageDetail.MessageDetail> MessageDetail { get; set; }

        // This needs to be here to please EF
        public QueuePublisherContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public QueuePublisherContext(DbContextOptions<QueuePublisherContext> options)
            : base(options) { }
    }
}

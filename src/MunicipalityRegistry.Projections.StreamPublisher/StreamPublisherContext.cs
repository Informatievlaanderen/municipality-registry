namespace MunicipalityRegistry.Projections.StreamPublisher
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class StreamPublisherContext : RunnerDbContext<StreamPublisherContext>
    {
        public override string ProjectionStateSchema => Schema.StreamPublisher;

        public DbSet<MessageDetail.MessageDetail> MessageDetail { get; set; }

        // This needs to be here to please EF
        public StreamPublisherContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public StreamPublisherContext(DbContextOptions<StreamPublisherContext> options)
            : base(options) { }
    }
}

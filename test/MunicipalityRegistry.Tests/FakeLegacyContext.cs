namespace MunicipalityRegistry.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Projections.Legacy;

    public class FakeLegacyContext : LegacyContext
    {
        private readonly bool _dontDispose = false;

        // This needs to be here to please EF
        public FakeLegacyContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public FakeLegacyContext(DbContextOptions<LegacyContext> options, bool dontDispose = false)
            : base(options)
        {
            _dontDispose = dontDispose;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            base.OnConfiguring(optionsBuilder);
        }

        public override ValueTask DisposeAsync()
        {
            if (_dontDispose)
            {
                return new ValueTask(Task.CompletedTask);
            }

            return base.DisposeAsync();
        }
    }

    public class FakeLegacyContextFactory : IDesignTimeDbContextFactory<FakeLegacyContext>
    {
        private readonly bool _dontDispose;

        public FakeLegacyContextFactory(bool dontDispose = false)
        {
            _dontDispose = dontDispose;
        }

        public FakeLegacyContext CreateDbContext(params string[] args)
        {
            var builder = new DbContextOptionsBuilder<LegacyContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new FakeLegacyContext(builder.Options, _dontDispose);
        }
    }
}

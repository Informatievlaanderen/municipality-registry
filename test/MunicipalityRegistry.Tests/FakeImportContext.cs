namespace MunicipalityRegistry.Tests
{
    using System;
    using System.Threading.Tasks;
    using Api.Import;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class FakeImportContext : ImportContext
    {
        private readonly bool _dontDispose = false;

        // This needs to be here to please EF
        public FakeImportContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public FakeImportContext(DbContextOptions<ImportContext> options, bool dontDispose = false)
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

    public class FakeImportContextFactory : IDesignTimeDbContextFactory<FakeImportContext>
    {
        private readonly bool _dontDispose;

        public FakeImportContextFactory(bool dontDispose = false)
        {
            _dontDispose = dontDispose;
        }

        public FakeImportContext CreateDbContext(params string[] args)
        {
            var builder = new DbContextOptionsBuilder<ImportContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new FakeImportContext(builder.Options, _dontDispose);
        }
    }
}

namespace MunicipalityRegistry.Tests.ImportApi
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Http;
    using Xunit.Abstractions;

    public class ImportApiTest : MunicipalityRegistryTest
    {
        protected readonly FakeLegacyContext _legacyContext;
        protected readonly FakeImportContext _importContext;

        public ImportApiTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _legacyContext = new FakeLegacyContextFactory().CreateDbContext();
            _importContext = new FakeImportContextFactory().CreateDbContext();
        }

        protected T CreateMergerControllerWithUser<T>(bool useSqs = false) where T : ApiController
        {
            var controller = Activator.CreateInstance(typeof(T), _legacyContext, _importContext, Container.BeginLifetimeScope()) as T;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim("name", "Username"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            if (controller != null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

                return controller;
            }

            throw new Exception("Could not find controller type");
        }
    }
}

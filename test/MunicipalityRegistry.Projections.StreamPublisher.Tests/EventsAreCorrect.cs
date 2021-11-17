using System;
using MunicipalityRegistry.Projections.StreamPublisher.Extensions;
using Xunit;

namespace MunicipalityRegistry.Projections.StreamPublisher.Tests
{
    using Municipality.Events;

    public class EventsAreCorrect
	{
        [Fact]
        public void MunicipalityWasCorrectedToCurrentTest()
        {
            var m = new MunicipalityWasCorrectedToCurrent(new MunicipalityId(Guid.NewGuid()));
            var result = m.ToContract();

            Assert.Equal(nameof(MunicipalityWasCorrectedToCurrent), result.EventName);
            Assert.NotEmpty(result.Id);
            Assert.NotEmpty(result.Timestamp);
            Assert.NotEmpty(result.Payload);
        }

        [Fact]
        public void MunicipalityFacilityLanguageWasAdded()
        {
            var m = new MunicipalityFacilityLanguageWasAdded(new MunicipalityId(Guid.NewGuid()), Language.Dutch);
            var result = m.ToContract();

            Assert.Equal(nameof(MunicipalityFacilityLanguageWasAdded), result.EventName);
            Assert.NotEmpty(result.Id);
            Assert.NotEmpty(result.Timestamp);
            Assert.NotEmpty(result.Payload);
        }
	}
}

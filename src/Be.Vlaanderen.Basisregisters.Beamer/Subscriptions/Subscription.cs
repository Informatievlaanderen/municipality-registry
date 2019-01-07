namespace Be.Vlaanderen.Basisregisters.Beamer.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using SqlStreamStore;
    using SqlStreamStore.Streams;

    internal class Subscription : IDisposable
    {
        private readonly Dictionary<string, object[]> _handlers;
        private readonly IAllStreamSubscription _allStreamSubscription;
        private readonly IStreamStore _streamStore;

        public bool HasConnectedProjections => _handlers.Any();

        public Subscription(string connectionString, ILoggerFactory loggerFactory)
        {
            _handlers = new Dictionary<string, object[]>();

            // TODO: get Schema from connectedProjection?
            _streamStore = new MsSqlStreamStore(new MsSqlStreamStoreSettings(connectionString) { Schema = "MunicipalityRegistry"});
            var logger = loggerFactory.CreateLogger<Subscription>();

            // TODO: decide, per connectedProjection, whether they should subscribe or catchup
            // TODO: subscribe after catching up -> make sure no messages are lost!
            _allStreamSubscription = _streamStore.SubscribeToAll(
                // TODO: determine the best position to subscribe from. E.g. the lowest position of all the currently connected projections
                continueAfterPosition: Position.Start,
                streamMessageReceived: async (subscription, message, token) =>
                {
                    logger.LogInformation($"Received message {message.Type}, fanning out to {_handlers.Count} connected projections with {_handlers.Values.Sum(x => x.Length)} handlers.");

                    // TODO: resolve handlers based on message type, invoke said handlers (in parallel?)
                    // Parallel.ForEach(_handlerResolver(message).Handlers, handler => handler.Invoke);
                });
        }

        public void AddConnectedProjection(object connectedProjection)
        {
            var handlersProperty = connectedProjection
                .GetType()
                .GetProperty("Handlers", BindingFlags.Public | BindingFlags.Instance);

            if (!(handlersProperty.GetValue(connectedProjection) is object[] handlers))
                return;

            _handlers.Add(connectedProjection.GetType().Name, handlers);
        }

        public void RemoveConnectedProjection(string name) => _handlers.Remove(name);

        public void Dispose()
        {
            _allStreamSubscription?.Dispose();
            _streamStore?.Dispose();
        }
    }
}

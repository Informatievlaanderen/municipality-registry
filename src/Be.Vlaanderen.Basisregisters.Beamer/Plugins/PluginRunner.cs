namespace Be.Vlaanderen.Basisregisters.Beamer.Plugins
{
    using System;
    using Microsoft.Extensions.Logging;
    using Subscriptions;

    internal class PluginRunner
    {
        private readonly SubscriptionPool _subscriptionPool;

        public PluginRunner(ILoggerFactory loggerFactory) => _subscriptionPool = new SubscriptionPool(loggerFactory);

        public void SubscribePlugin(Plugin plugin)
        {
            var connectedProjection = Activator.CreateInstance(plugin.ConnectedProjectionType);
            var subscription = _subscriptionPool.GetOrAddSubscription(plugin.ConnectionString);

            subscription.AddConnectedProjection(connectedProjection);
        }

        public void UnsubscribePlugin(Plugin plugin)
        {
            var subscription = _subscriptionPool.GetSubscription(plugin.ConnectionString);

            if (subscription == null)
                return;

            subscription.RemoveConnectedProjection(plugin.ConnectedProjectionType.Name);

            if (!subscription.HasConnectedProjections)
                _subscriptionPool.CloseSubscription(plugin.ConnectionString);
        }
    }
}

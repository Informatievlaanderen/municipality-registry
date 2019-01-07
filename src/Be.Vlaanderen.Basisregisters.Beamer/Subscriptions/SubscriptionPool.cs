namespace Be.Vlaanderen.Basisregisters.Beamer.Subscriptions
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;

    internal class SubscriptionPool
    {
        private readonly ConcurrentDictionary<string, Subscription> _subscriptionPool;
        private readonly ILoggerFactory _loggerFactory;

        public SubscriptionPool(ILoggerFactory loggerFactory)
        {
            _subscriptionPool = new ConcurrentDictionary<string, Subscription>();
            _loggerFactory = loggerFactory;
        }

        public Subscription GetSubscription(string connectionString)
            => _subscriptionPool.TryGetValue(connectionString, out var value) ? value : null;

        public Subscription GetOrAddSubscription(string connectionString)
        {
            var subscription = GetSubscription(connectionString);

            if (subscription != null)
                return subscription;

            subscription = new Subscription(connectionString, _loggerFactory);
            _subscriptionPool.TryAdd(connectionString, subscription);

            return subscription;
        }

        public void CloseSubscription(string connectionString)
        {
            if (_subscriptionPool.TryRemove(connectionString, out var subscription))
                subscription.Dispose();
        }
    }
}

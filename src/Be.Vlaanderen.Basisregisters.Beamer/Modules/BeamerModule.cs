namespace Be.Vlaanderen.Basisregisters.Beamer.Modules
{
    using Autofac;
    using Plugins;
    using Subscriptions;

    public class BeamerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PluginManager>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<PluginRunner>()
                .AsSelf();

            builder
                .RegisterType<SubscriptionPool>()
                .AsSelf();
        }
    }
}

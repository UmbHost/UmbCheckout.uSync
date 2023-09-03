using UmbCheckout.Shared.Notifications.Configuration;
using UmbCheckout.uSync.Handlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbCheckout.uSync.Composers
{
    public class RegisterNotifications : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<OnConfigurationSavedNotification, ConfigurationHandler>();
        }
    }
}

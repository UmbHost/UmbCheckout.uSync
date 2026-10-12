using global::uSync.BackOffice;
using UmbCheckout.Shared.Notifications.Configuration;
using UmbCheckout.Shared.Persistence;
using UmbCheckout.uSync.Handlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbCheckout.uSync.Composers
{
    public class RegisterNotifications : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<OnConfigurationSavedNotification, ConfigurationHandler>();
            // A fresh install's first-boot import runs before the tables are migrated at startup
            builder.EnsureUmbCheckoutTablesOn<uSyncImportStartingNotification>();
        }
    }
}

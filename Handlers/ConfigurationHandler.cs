using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Models;
using UmbCheckout.Shared.Notifications.Configuration;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace UmbCheckout.uSync.Handlers
{
    [SyncHandler("umbCheckoutHander", Consts.Configuration.HandlerName, Consts.Configuration.SerializerFolder, 1,
        Icon = "icon-settings usync-addon-icon", EntityType = Consts.Configuration.EntityType)]
    public class ConfigurationHandler : SyncHandlerRoot<UmbCheckoutConfiguration, UmbCheckoutConfiguration>, ISyncHandler,
        INotificationHandler<OnConfigurationSavedNotification>
    {
        public override string Group => Consts.Group;

        private readonly IConfigurationService _configurationService;
        public ConfigurationHandler(ILogger<SyncHandlerRoot<UmbCheckoutConfiguration, UmbCheckoutConfiguration>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory, IConfigurationService configurationService) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        {
            _configurationService = configurationService;

            itemContainerType = UmbracoObjectTypes.Unknown;
        }

        public override IEnumerable<uSyncAction> ExportAll(UmbCheckoutConfiguration parent, string folder, HandlerSettings config,
            SyncUpdateCallback callback)
        {
            var item = _configurationService.GetConfiguration().Result;

            var actions = new List<uSyncAction>();
            if (item != null)
            {
                actions.AddRange(Export(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig));
            }

            return actions;
        }

        public void Handle(OnConfigurationSavedNotification notification)
        {
            if (!ShouldProcess()) return;

            try
            {
                var attempts = Export(notification.Configuration, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
                foreach (var attempt in attempts.Where(x => x.Success))
                {
                    CleanUp(notification.Configuration, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "uSync Save error");
            }
        }

        protected override IEnumerable<uSyncAction> DeleteMissingItems(UmbCheckoutConfiguration parent, IEnumerable<Guid> keysToKeep, bool reportOnly)
            => Enumerable.Empty<uSyncAction>();

        protected override IEnumerable<UmbCheckoutConfiguration> GetChildItems(UmbCheckoutConfiguration parent)
            => Enumerable.Empty<UmbCheckoutConfiguration>();

        protected override IEnumerable<UmbCheckoutConfiguration> GetFolders(UmbCheckoutConfiguration parent)
            => Enumerable.Empty<UmbCheckoutConfiguration>();

        protected override UmbCheckoutConfiguration GetFromService(UmbCheckoutConfiguration item)
            => _configurationService.GetConfiguration().Result ?? new UmbCheckoutConfiguration();

        protected override string GetItemName(UmbCheckoutConfiguration item)
            => item.Id.ToString();

        protected override string GetItemFileName(UmbCheckoutConfiguration item)
            => Consts.Configuration.FileName;

        private bool ShouldProcess()
        {
            if (_mutexService.IsPaused) return false;
            if (!DefaultConfig.Enabled) return false;
            return true;
        }
    }
}

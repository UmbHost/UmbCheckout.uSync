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
using uSync.BackOffice.SyncHandlers.Interfaces;
using uSync.BackOffice.SyncHandlers.Models;
using uSync.Core;

namespace UmbCheckout.uSync.Handlers
{
    [SyncHandler("umbCheckoutHander", Consts.Configuration.HandlerName, Consts.Configuration.SerializerFolder, 1,
        Icon = "icon-settings usync-addon-icon", EntityType = Consts.Configuration.EntityType)]
    public class ConfigurationHandler : SyncHandlerRoot<UmbCheckoutConfiguration, UmbCheckoutConfiguration>, ISyncHandler,
        INotificationAsyncHandler<OnConfigurationSavedNotification>
    {
        public override string Group => Consts.Group;

        private readonly IConfigurationService _configurationService;
        public ConfigurationHandler(ILogger<SyncHandlerRoot<UmbCheckoutConfiguration, UmbCheckoutConfiguration>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, ISyncFileService syncFileService, ISyncEventService mutexService, ISyncConfigService uSyncConfig, ISyncItemFactory itemFactory, IConfigurationService configurationService) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        {
            _configurationService = configurationService;

            ItemContainerType = UmbracoObjectTypes.Unknown;
        }

        public override async Task<IEnumerable<uSyncAction>> ExportAllAsync(UmbCheckoutConfiguration? parent, string[] folders, HandlerSettings config,
            SyncUpdateCallback? callback)
        {
            var item = await _configurationService.GetConfiguration();

            var actions = new List<uSyncAction>();
            if (item != null)
            {
                actions.AddRange(await ExportAsync(item, folders, config));
            }

            return actions;
        }

        public async Task HandleAsync(OnConfigurationSavedNotification notification, CancellationToken cancellationToken)
        {
            if (!ShouldProcessEvent()) return;

            try
            {
                if (notification.Configuration != null)
                {
                    var handlerFolders = GetDefaultHandlerFolders();
                    var attempts = await ExportAsync(notification.Configuration, handlerFolders, DefaultConfig);
                    foreach (var attempt in attempts.Where(x => x.Success))
                    {
                        if (attempt.FileName is null) continue;
                        await CleanUpAsync(notification.Configuration, attempt.FileName, handlerFolders.Last());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "uSync Save error");
            }
        }

        protected override Task<IEnumerable<uSyncAction>> DeleteMissingItemsAsync(UmbCheckoutConfiguration parent, IEnumerable<Guid> keysToKeep, bool reportOnly)
            => Task.FromResult(Enumerable.Empty<uSyncAction>());

        protected override Task<IEnumerable<UmbCheckoutConfiguration>> GetChildItemsAsync(UmbCheckoutConfiguration? parent)
            => Task.FromResult(Enumerable.Empty<UmbCheckoutConfiguration>());

        protected override Task<IEnumerable<UmbCheckoutConfiguration>> GetFoldersAsync(UmbCheckoutConfiguration? parent)
            => Task.FromResult(Enumerable.Empty<UmbCheckoutConfiguration>());

        protected override async Task<UmbCheckoutConfiguration?> GetFromServiceAsync(UmbCheckoutConfiguration? item)
            => await _configurationService.GetConfiguration() ?? new UmbCheckoutConfiguration();

        protected override string GetItemName(UmbCheckoutConfiguration item)
            => item.Id.ToString();

        protected override string GetItemFileName(UmbCheckoutConfiguration item)
            => Consts.Configuration.FileName;
    }
}

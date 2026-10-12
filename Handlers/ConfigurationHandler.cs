using UmbCheckout.Shared.Notifications;
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
using uSync.Core.Serialization;

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
            var item = await UmbCheckoutHooks.ReadStoredAsync(() => _configurationService.GetConfigurationAsync());

            var actions = new List<uSyncAction>();
            if (item != null)
            {
                actions.AddRange(await ExportItemAsync(item));
            }

            return actions;
        }

        private async Task<IEnumerable<uSyncAction>> ExportItemAsync(UmbCheckoutConfiguration item)
        {
            var folder = Path.Combine(RootFolders.First(), DefaultFolder);
            var attempt = await serializer.SerializeAsync(item, new SyncSerializerOptions());
            if (attempt.Success && attempt.Item != null)
            {
                var filename = Path.Combine(folder, $"{GetItemFileName(item)}.config");
                await syncFileService.SaveXElementAsync(attempt.Item, filename);
                return new[] { uSyncAction.SetAction(true, GetItemName(item), change: ChangeType.Export, filename: filename) };
            }
            return Enumerable.Empty<uSyncAction>();
        }

        public async Task HandleAsync(OnConfigurationSavedNotification notification, CancellationToken cancellationToken)
        {
            if (!ShouldProcess()) return;

            try
            {
                if (notification.Configuration != null)
                {
                    await ExportItemAsync(notification.Configuration);
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
            => await UmbCheckoutHooks.ReadStoredAsync(() => _configurationService.GetConfigurationAsync()) ?? new UmbCheckoutConfiguration();

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

using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Models;
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;

namespace UmbCheckout.uSync.Serializers
{
    [SyncSerializer("10A73824-7104-4A37-ADDB-118829041283", Consts.Configuration.SerializerName, Consts.Configuration.ItemType)]
    public class ConfigurationSerializer : SyncSerializerRoot<UmbCheckoutConfiguration>, ISyncSerializer<UmbCheckoutConfiguration>
    {
        private readonly IConfigurationService _configurationService;
        public ConfigurationSerializer(ILogger<SyncSerializerRoot<UmbCheckoutConfiguration>> logger, IConfigurationService configurationService) : base(logger)
        {
            _configurationService = configurationService;
        }

        protected override SyncAttempt<XElement> SerializeCore(UmbCheckoutConfiguration item, SyncSerializerOptions options)
        {
            var node = new XElement(ItemType,
                new XAttribute("Id", item.Id));

            node.Add(new XElement("BasketInCookieExpiry", item.BasketInCookieExpiry));
            node.Add(new XElement("BasketInDatabaseExpiry", item.BasketInDatabaseExpiry));
            var cancelPageUrl = new XElement("CancelPageUrl");
            var cancelPage = item.CancelPageUrl.FirstOrDefault();
            if (cancelPage != null)
            {
                cancelPageUrl.Add(new XElement("Icon", cancelPage.Icon));
                cancelPageUrl.Add(new XElement("Name", cancelPage.Name));
                cancelPageUrl.Add(new XElement("Published", cancelPage.Published));
                cancelPageUrl.Add(new XElement("Trashed", cancelPage.Trashed));
                cancelPageUrl.Add(new XElement("Udi", cancelPage.Udi));
                cancelPageUrl.Add(new XElement("Url", cancelPage.Url));
            }
            node.Add(cancelPageUrl);
            var successPageUrl = new XElement("SuccessPageUrl");
            var successPage = item.SuccessPageUrl.FirstOrDefault();
            if (successPage != null)
            {
                successPageUrl.Add(new XElement("Icon", successPage.Icon));
                successPageUrl.Add(new XElement("Name", successPage.Name));
                successPageUrl.Add(new XElement("Published", successPage.Published));
                successPageUrl.Add(new XElement("Trashed", successPage.Trashed));
                successPageUrl.Add(new XElement("Udi", successPage.Udi));
                successPageUrl.Add(new XElement("Url", successPage.Url));
            }
            node.Add(successPageUrl);
            node.Add(new XElement("EnableShipping", item.EnableShipping));
            node.Add(new XElement("StoreBasketInCookie", item.StoreBasketInCookie));
            node.Add(new XElement("StoreBasketInDatabase", item.StoreBasketInDatabase));

            return SyncAttempt<XElement>.Succeed(Consts.Configuration.ItemType, node, typeof(UmbCheckoutConfiguration), ChangeType.Export);
        }

        protected override SyncAttempt<UmbCheckoutConfiguration> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var configuration = node.Element(Consts.Configuration.ItemType);
            var item = new UmbCheckoutConfiguration
            {
                StoreBasketInDatabase = configuration!.Element("StoreBasketInDatabase").ValueOrDefault(false)
            };


            return SyncAttempt<UmbCheckoutConfiguration>.Succeed("Configuration", item, ChangeType.Import, Array.Empty<uSyncChange>());
        }

        public override UmbCheckoutConfiguration FindItem(int id) => _configurationService.GetConfiguration().Result ?? new UmbCheckoutConfiguration();

        public override UmbCheckoutConfiguration FindItem(Guid key) => _configurationService.GetConfiguration().Result ?? new UmbCheckoutConfiguration();

        public override UmbCheckoutConfiguration FindItem(string alias) => null!;

        public override void SaveItem(UmbCheckoutConfiguration item) => _configurationService.UpdateConfiguration(item);

        public override void DeleteItem(UmbCheckoutConfiguration item)
        {
        }

        public override string ItemAlias(UmbCheckoutConfiguration item) => string.Empty;

        public override Guid ItemKey(UmbCheckoutConfiguration item) => Guid.Empty;
    }
}

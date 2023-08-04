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
                new XAttribute("Id", item.Id),
                new XAttribute("Alias", ItemAlias(item)),
                new XAttribute("Key", ItemKey(item)));

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
            var successPageUrlElement = node.Element("SuccessPageUrl");
            var successPageUrl = Enumerable.Empty<MultiUrlPicker>();
            if (successPageUrlElement != null)
            {
                successPageUrl = new List<MultiUrlPicker>
                {
                    new()
                    {
                        Icon = successPageUrlElement.Element("Icon").ValueOrDefault(string.Empty),
                        Name = successPageUrlElement.Element("Name").ValueOrDefault(string.Empty),
                        Published = successPageUrlElement.Element("Published").ValueOrDefault(false),
                        Trashed = successPageUrlElement.Element("Trashed").ValueOrDefault(false),
                        Udi = successPageUrlElement.Element("Udi").ValueOrDefault(string.Empty),
                        Url = successPageUrlElement.Element("Url").ValueOrDefault(string.Empty)
                    }
                };
            }

            var cancelPageUrlElement = node.Element("CancelPageUrl");
            var cancelPageUrl = Enumerable.Empty<MultiUrlPicker>();
            if (cancelPageUrlElement != null)
            {
                cancelPageUrl = new List<MultiUrlPicker>
                {
                    new()
                    {
                        Icon = cancelPageUrlElement.Element("Icon").ValueOrDefault(string.Empty),
                        Name = cancelPageUrlElement.Element("Name").ValueOrDefault(string.Empty),
                        Published = cancelPageUrlElement.Element("Published").ValueOrDefault(false),
                        Trashed = cancelPageUrlElement.Element("Trashed").ValueOrDefault(false),
                        Udi = cancelPageUrlElement.Element("Udi").ValueOrDefault(string.Empty),
                        Url = cancelPageUrlElement.Element("Url").ValueOrDefault(string.Empty)
                    }
                };
            }

            var item = new UmbCheckoutConfiguration
            {
                Id = node.Attribute("Id").ValueOrDefault(0),
                StoreBasketInDatabase = node!.Element("StoreBasketInDatabase").ValueOrDefault(false),
                BasketInCookieExpiry = node!.Element("BasketInDatabaseExpiry").ValueOrDefault(30),
                BasketInDatabaseExpiry = node!.Element("BasketInDatabaseExpiry").ValueOrDefault(30),
                CancelPageUrl = cancelPageUrl,
                SuccessPageUrl = successPageUrl,
                EnableShipping = node!.Element("EnableShipping").ValueOrDefault(false),
                StoreBasketInCookie = node!.Element("StoreBasketInCookie").ValueOrDefault(false)
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

        public override string ItemAlias(UmbCheckoutConfiguration item) => "UmbCheckoutConfiguration";

        public override Guid ItemKey(UmbCheckoutConfiguration item) => item.Key;
    }
}

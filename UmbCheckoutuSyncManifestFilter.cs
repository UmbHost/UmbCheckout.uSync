using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

namespace UmbCheckout.uSync
{
    public class UmbCheckoutuSyncManifest : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ManifestFilters().Append<UmbCheckoutuSyncManifestFilter>();
        }
    }

    public class UmbCheckoutuSyncManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = $"{Shared.Consts.PackageName}.uSync",
                Version = UmbCheckoutVersion.Version.ToString(3),
                AllowPackageTelemetry = true,
                BundleOptions = BundleOptions.None
            });
        }
    }
}

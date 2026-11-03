using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace UmbCheckout.uSync
{
    public class UmbCheckoutuSyncManifest : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPackageManifestReader, UmbCheckoutuSyncReader>();
        }
    }

    internal sealed class UmbCheckoutuSyncReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            IEnumerable<PackageManifest> manifests =
            [
                new PackageManifest
                {
                    Id = $"{Shared.Consts.PackageName}.uSync",
                    Name = $"{Shared.Consts.PackageName}.uSync",
                    Version = UmbCheckoutVersion.PackageVersion,
                    AllowTelemetry = true,
                    Extensions = []
                }
            ];
            return Task.FromResult(manifests);
        }
    }
}

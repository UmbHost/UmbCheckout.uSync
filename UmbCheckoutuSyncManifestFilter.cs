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
            builder.Services.AddSingleton<IPackageManifestReader, UmbCheckoutuSyncManifestFilter>();
        }
    }

    internal sealed class UmbCheckoutuSyncManifestFilter : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            List<PackageManifest> manifest = [
                new()
                {
                    Id = $"{Shared.Consts.PackageName}.uSync",
                    Name = $"{Shared.Consts.PackageName}.uSync",
                    AllowTelemetry = true,
                    Version = UmbCheckoutVersion.Version.ToString(3),
                    Extensions = []
                }
            ];

            return Task.FromResult(manifest.AsEnumerable());
        }
    }
}

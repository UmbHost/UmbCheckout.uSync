using UmbCheckout.Shared;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace UmbCheckout.uSync.Manifests
{
    /// <summary>
    /// Registers the UmbCheckout uSync package manifest for telemetry purposes
    /// </summary>
    internal class UmbCheckoutuSyncPackageManifestReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            IEnumerable<PackageManifest> manifests =
            [
                new()
                {
                    Id = $"{Shared.Consts.PackageName}.uSync",
                    Name = $"{Shared.Consts.PackageName}.uSync",
                    Version = UmbCheckoutVersion.Version.ToString(3),
                    AllowTelemetry = true,
                    Extensions = []
                }
            ];

            return Task.FromResult(manifests);
        }
    }
}

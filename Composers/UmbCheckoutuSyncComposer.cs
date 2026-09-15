using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.uSync.Manifests;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

namespace UmbCheckout.uSync.Composers
{
    /// <summary>
    /// Registers the UmbCheckout uSync package manifest
    /// </summary>
    public class UmbCheckoutuSyncComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPackageManifestReader, UmbCheckoutuSyncPackageManifestReader>();
        }
    }
}

using System;
using DocumentManagement.Core.Component;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.DocumentStore.Blob
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add blob document store.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="cofigure">Configure option.</param>
        /// <returns>Service collection with document client.</returns>
        public static IServiceCollection AddBlobDocumentStore(this IServiceCollection services, Action<BlobStorageOptions> cofigure)
        {
            return services.AddOptions()
                .Configure(cofigure)
                .AddSingleton<IDocumentStore, BlobDocumentStore>();
        }
    }
}

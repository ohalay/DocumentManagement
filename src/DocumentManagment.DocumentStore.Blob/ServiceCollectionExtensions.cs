using System;
using System.Linq;
using DocumentManagement.Core.Component;
using DocumentManagement.DocumentStore.Blob.Decorators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
                .AddSingleton<IDocumentStore, BlobDocumentStore>()
                .Decorate<IDocumentStore, BlobDocumentStorRetrayDecorator>();
        }

        private static IServiceCollection Decorate<TInterface, TDecorator>(this IServiceCollection services)
            where TInterface : class
            where TDecorator : class, TInterface
        {
            var wrappedDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TInterface));

            if (wrappedDescriptor == null)
            {
                throw new InvalidOperationException($"{typeof(TInterface).Name} is not registered");
            }

            // Create the object factory for our decorator type,
            // specifying that we will supply TInterface explicitly.
            var objectFactory = ActivatorUtilities.CreateFactory(typeof(TDecorator), new[] { typeof(TInterface) });

            // Replace the existing registration with one
            // that passes an instance of the existing registration
            // to the object factory for the decorator.
            services.Replace(ServiceDescriptor.Describe(
                typeof(TInterface),
                s => (TInterface)objectFactory(s, new[] { s.CreateInstance(wrappedDescriptor) }),
                wrappedDescriptor.Lifetime));

            return services;
        }

        private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationFactory != null)
            {
                return descriptor.ImplementationFactory(services);
            }

            return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType);
        }
    }
}

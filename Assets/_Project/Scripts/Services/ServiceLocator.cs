using System;
using System.Collections.Generic;

namespace ConwayLife.Services
{
    /// <summary>
    /// Simple service locator for registering and retrieving service instances.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Registers a service instance by its type.
        /// </summary>
        /// <typeparam name="T">Service interface or type.</typeparam>
        /// <param name="service">Service instance to register.</param>
        public static void Register<T>(T service) where T : class
        {
            Type key = typeof(T);
            _services[key] = service;
        }

        /// <summary>
        /// Retrieves a registered service instance.
        /// </summary>
        /// <typeparam name="T">Service interface or type to retrieve.</typeparam>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
        public static T Resolve<T>() where T : class
        {
            Type key = typeof(T);

            if (!_services.ContainsKey(key))
            {
                throw new InvalidOperationException($"Service of type {key.Name} is not registered in the ServiceLocator.");
            }

            return _services[key] as T;
        }

        /// <summary>
        /// Clears all registered services.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}

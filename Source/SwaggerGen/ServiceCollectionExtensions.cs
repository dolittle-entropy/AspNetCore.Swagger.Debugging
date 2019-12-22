// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> for the Dolittle Swagger debugging tools.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Dolittle Swagger document generators.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> to add to.</param>
        /// <param name="setupAction">Optional callback for configuring <see cref="SwaggerGenOptions"/>.</param>
        /// <returns>Continued <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDolittleSwagger(
            this IServiceCollection services,
            Action<SwaggerGenOptions> setupAction = null)
        {
            services.AddSwaggerGen(setupAction);
            services.AddTransient<ISwaggerProvider, SwaggerGen.SwaggerGenerator>();
            services.AddTransient<ISchemaRegistryFactory, SwaggerGen.SchemaRegistryFactory>();
            return services;
        }
    }
}
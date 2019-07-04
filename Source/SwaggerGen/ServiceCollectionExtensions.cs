/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using Dolittle.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> for the Dolittle Swagger debugging tools
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Dolittle Swagger document generators
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddDolittleSwagger(
            this IServiceCollection services,
            Action<SwaggerGenOptions> setupAction = null
        )
        {
            services.AddSwaggerGen(setupAction);
            services.AddTransient<ISwaggerProvider, Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen.SwaggerGenerator>();
            services.AddTransient<ISchemaRegistryFactory, Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen.SchemaRegistryFactory>();
            return services;
        }
    }
}
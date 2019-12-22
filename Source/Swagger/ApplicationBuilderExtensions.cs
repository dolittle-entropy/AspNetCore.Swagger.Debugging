// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Extensions for ApplicationBuilder.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Dolittle Swagger Debugging tools for the given application.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to use Dolittle Swagger Debugging tools for.</param>
        /// <param name="swaggerUISetupAction">Callback for configuring <see cref="SwaggerUIOptions"/>.</param>
        /// <param name="swaggerSetupAction">Callback for configuring <see cref="SwaggerOptions"/>.</param>
        /// <returns>Continued <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDolittleSwagger(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> swaggerUISetupAction,
            Action<SwaggerOptions> swaggerSetupAction)
        {
            app.UseSwagger(swaggerSetupAction);
            app.UseSwaggerUI(_ =>
            {
                _.SwaggerEndpoint("Dolittle.Commands/swagger.json", "Commands");
                _.SwaggerEndpoint("Dolittle.Events/swagger.json", "Events");
                _.SwaggerEndpoint("Dolittle.Queries/swagger.json", "Queries");
                swaggerUISetupAction?.Invoke(_);
            });
            return app;
        }

        /// <summary>
        /// Use Dolittle Swagger Debugging tools for the given application.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to use Dolittle Swagger Debugging tools for.</param>
        /// <param name="swaggerUISetupAction">Callback for configuring <see cref="SwaggerUIOptions"/>.</param>
        /// <returns>Continued <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDolittleSwagger(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> swaggerUISetupAction)
        {
            return app.UseDolittleSwagger(swaggerUISetupAction, null);
        }

        /// <summary>
        /// Use Dolittle Swagger Debugging tools for the given application.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to use Dolittle Swagger Debugging tools for.</param>
        /// <returns>Continued <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDolittleSwagger(
            this IApplicationBuilder app)
        {
            return app.UseDolittleSwagger(null);
        }
    }
}
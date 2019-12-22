// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// Represents a provider that can generate <see cref="Schema"/> for <see cref="Type"/>.
    /// </summary>
    public interface ICanProvideSwaggerSchemas
    {
        /// <summary>
        /// Checks if the provider can provide a <see cref="Schema"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check for.</param>
        /// <returns>Whether the provider can generate a <see cref="Schema"/> or not.</returns>
        bool CanProvideFor(Type type);

        /// <summary>
        /// Generates a <see cref="Schema"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to generate for.</param>
        /// <param name="registry">The <see cref="ISchemaRegistry"/> to use for nested type schemas.</param>
        /// <param name="idManager">The <see cref="SchemaIdManager"/> to user for referenced types.</param>
        /// <returns>The <see cref="Schema"/>.</returns>
        Schema ProvideFor(Type type, ISchemaRegistry registry, SchemaIdManager idManager);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// An implementation of <see cref="ICanProvideSwaggerSchemas"/> for <see cref="ConceptAs{T}"/>.
    /// </summary>
    public class ConceptSchemaProvider : ICanProvideSwaggerSchemas
    {
        /// <inheritdoc/>
        public bool CanProvideFor(Type type)
        {
            return type.IsConcept();
        }

        /// <inheritdoc/>
        public Schema ProvideFor(Type type, ISchemaRegistry registry, SchemaIdManager idManager)
        {
            return registry.GetOrRegister(type.GetConceptValueType());
        }
    }
}
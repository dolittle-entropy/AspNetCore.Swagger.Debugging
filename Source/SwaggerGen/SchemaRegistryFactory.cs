// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using OriginalSchemaRegistryFactory = Swashbuckle.AspNetCore.SwaggerGen.SchemaRegistryFactory;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// Dolittle overload of <see cref="Swashbuckle.AspNetCore.SwaggerGen.SchemaRegistryFactory" />.
    /// </summary>
    public class SchemaRegistryFactory : ISchemaRegistryFactory
    {
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly SchemaRegistryOptions _schemaRegistryOptions;
        readonly OriginalSchemaRegistryFactory _originalFactory;
        readonly IInstancesOf<ICanProvideSwaggerSchemas> _schemaProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaRegistryFactory"/> class.
        /// </summary>
        /// <param name="mvcJsonOptionsAccessor"><see cref="IOptions{T}">Options</see> for <see cref="MvcJsonOptions"/>.</param>
        /// <param name="schemaRegistryOptionsAccessor"><see cref="IOptions{T}">Options</see> for <see cref="SchemaRegistryOptions"/>.</param>
        /// <param name="schemaProviders"><see cref="IInstancesOf{T}"/> of <see cref="ICanProvideSwaggerSchemas"/>.</param>
        public SchemaRegistryFactory(
            IOptions<MvcJsonOptions> mvcJsonOptionsAccessor,
            IOptions<SchemaRegistryOptions> schemaRegistryOptionsAccessor,
            IInstancesOf<ICanProvideSwaggerSchemas> schemaProviders)
        {
            _jsonSerializerSettings = mvcJsonOptionsAccessor.Value.SerializerSettings;
            _schemaRegistryOptions = schemaRegistryOptionsAccessor.Value;
            _schemaProviders = schemaProviders;

            _originalFactory = new OriginalSchemaRegistryFactory(_jsonSerializerSettings, _schemaRegistryOptions);
        }

        /// <inheritdoc/>
        public ISchemaRegistry Create()
        {
            return new SchemaRegistry(
                _originalFactory.Create(),
                _schemaProviders,
                new SchemaIdManager(_schemaRegistryOptions.SchemaIdSelector));
        }
    }
}
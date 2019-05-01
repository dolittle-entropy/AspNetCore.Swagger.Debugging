/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Dolittle.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using OriginalSchemaRegistryFactory = Swashbuckle.AspNetCore.SwaggerGen.SchemaRegistryFactory;

namespace Dolittle.AspNetCore.Swagger.Debugging.SwaggerGen
{
    /// <summary>
    /// Dolittle overload of <see cref="Swashbuckle.AspNetCore.SwaggerGen.SchemaRegistryFactory" />
    /// </summary>
    public class SchemaRegistryFactory : ISchemaRegistryFactory
    {
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly SchemaRegistryOptions _schemaRegistryOptions;
        readonly OriginalSchemaRegistryFactory _originalFactory;
        readonly IInstancesOf<ICanProvideSwaggerSchemas> _schemaProviders;

        /// <summary>
        /// Instanciates a <see cref="SchemaRegistryFactory"/>
        /// </summary>
        /// <param name="mvcJsonOptionsAccessor"></param>
        /// <param name="schemaRegistryOptionsAccessor"></param>
        /// <param name="schemaProviders"></param>
        public SchemaRegistryFactory(
            IOptions<MvcJsonOptions> mvcJsonOptionsAccessor,
            IOptions<SchemaRegistryOptions> schemaRegistryOptionsAccessor,
            IInstancesOf<ICanProvideSwaggerSchemas> schemaProviders
        )
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
                new SchemaIdManager(_schemaRegistryOptions.SchemaIdSelector)
            );
        }
    }
}
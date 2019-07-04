/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Tenancy;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// An implementation of <see cref="IDocumentGenerator{T}"/>
    /// </summary>
    /// <typeparam name="T">The artifact type</typeparam>
    public class DocumentGenerator<T> : IDocumentGenerator<T> where T : class
    {
        readonly IArtifactMapper<T> _artifactMapper;
        readonly ISchemaRegistry _schemaRegistry;

        Info _documentInfo;
        string _documentBasePath;
        Func<PropertyInfo,bool> _documentPropertyFilter;
        IList<IParameter> _documentGlobalParameters;

        /// <summary>
        /// Instanciates a new <see cref="DocumentGenerator{T}"/>
        /// </summary>
        /// <param name="artifactMapper"></param>
        /// <param name="schemaRegistryFactory"></param>
        public DocumentGenerator(
            IArtifactMapper<T> artifactMapper,
            ISchemaRegistryFactory schemaRegistryFactory
        )
        {
            _artifactMapper = artifactMapper;
            _schemaRegistry = schemaRegistryFactory.Create();
        }

        /// <inheritdoc/>
        public void Configure(Info info, string basePath, Func<PropertyInfo,bool> parameterFilter, params IParameter[] globalParameters)
        {
            _documentInfo = info;
            _documentBasePath = basePath;
            _documentPropertyFilter = parameterFilter;
            _documentGlobalParameters = new List<IParameter>(globalParameters);

            var tenantIdParameter = new NonBodyParameter
            {
                Name = "TenantId",
                In = "formData",
                Required = true,
                Default = TenantId.Development.Value,
            };
            AddSchemaFor(tenantIdParameter, _schemaRegistry.GetOrRegister(typeof(TenantId)));
            _documentGlobalParameters.Insert(0, tenantIdParameter);
        }

        /// <inheritdoc/>
        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            return new SwaggerDocument
            {
                Info = _documentInfo,
                BasePath = _documentBasePath,
                Paths = GeneratePaths(),
                Definitions = _schemaRegistry.Definitions,
                Consumes = new List<string> {{"multipart/form-data"}},
            };
        }

        IDictionary<string, PathItem> GeneratePaths()
        {
            var paths = new Dictionary<string, PathItem>();
            foreach (var path in _artifactMapper.ApiPaths)
            {
                var tags = new List<string> {{ path.Split('/')[1] }};

                paths.Add(path, new PathItem{
                    Post = GenerateOperation(_artifactMapper.GetTypeFor(path), tags),
                });
            }
            return paths;
        }

        Operation GenerateOperation(Type artifactType, IList<string> tags)
        {
            return new Operation
            {
                Tags = tags,
                Parameters = _documentGlobalParameters.Concat(artifactType.GetProperties().Where(_documentPropertyFilter).Select(_ => {
                    var parameter = new NonBodyParameter
                    {
                        Name = _.Name,
                        In = "formData",
                        Required = true,
                    };
                    AddSchemaFor(parameter, _schemaRegistry.GetOrRegister(_.PropertyType));
                    return (IParameter)parameter;
                })).ToList(),
            };
        }

        void AddSchemaFor(PartialSchema type, Schema schema)
        {
            type.Type = schema.Type;
            type.Format = schema.Format;
            if (schema.Items != null)
            {
                type.Items = new PartialSchema();
                AddSchemaFor(type.Items, schema.Items);
            }
        }
    }
}
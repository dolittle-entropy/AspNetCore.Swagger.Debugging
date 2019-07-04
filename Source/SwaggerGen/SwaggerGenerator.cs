/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Dolittle.Commands;
using Dolittle.Events;
using Dolittle.Queries;
using Dolittle.Runtime.Events;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using OriginalSwaggerGenerator = Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// Dolittle overload of <see cref="Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator"/>
    /// </summary>
    public class SwaggerGenerator : ISwaggerProvider
    {
        readonly IApiDescriptionGroupCollectionProvider _apiDescriptionsProvider;
        readonly ISchemaRegistryFactory _schemaRegistryFactory;
        readonly IDocumentGenerator<IEvent> _eventDocumentGenerator;
        readonly IDocumentGenerator<ICommand> _commandDocumentGenerator;
        readonly IDocumentGenerator<IQuery> _queryDocumentGenerator;
        readonly SwaggerGeneratorOptions _options;
        readonly OriginalSwaggerGenerator _originalGenerator;

        /// <summary>
        /// Instanciates a <see cref="SwaggerGenerator"/>
        /// </summary>
        /// <param name="apiDescriptionsProvider"></param>
        /// <param name="schemaRegistryFactory"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="eventDocumentGenerator"></param>
        /// <param name="commandDocumentGenerator"></param>
        /// <param name="queryDocumentGenerator"></param>
        public SwaggerGenerator(
            IApiDescriptionGroupCollectionProvider apiDescriptionsProvider,
            ISchemaRegistryFactory schemaRegistryFactory,
            IOptions<SwaggerGeneratorOptions> optionsAccessor,
            IDocumentGenerator<IEvent> eventDocumentGenerator,
            IDocumentGenerator<ICommand> commandDocumentGenerator,
            IDocumentGenerator<IQuery> queryDocumentGenerator
        )
        {
            _apiDescriptionsProvider = apiDescriptionsProvider;
            _schemaRegistryFactory = schemaRegistryFactory;
            _eventDocumentGenerator = eventDocumentGenerator;
            _commandDocumentGenerator = commandDocumentGenerator;
            _queryDocumentGenerator = queryDocumentGenerator;
            _options = optionsAccessor.Value;

            ConfigureGenerators();

            _originalGenerator = new OriginalSwaggerGenerator(_apiDescriptionsProvider, _schemaRegistryFactory, _options);
        }

        void ConfigureGenerators()
        {
            _commandDocumentGenerator.Configure(
                new Info
                {
                    Title = "Commands",
                },
                "/api/Dolittle/Debugging/Swagger/Commands",
                "POST",
                new Dictionary<string, Response> {
                    {"200", new Response {
                        Description = "Result of command handling"
                    }},
                },
                _ => true
            );
            _eventDocumentGenerator.Configure(
                new Info
                {
                    Title = "Events",
                },
                "/api/Dolittle/Debugging/Swagger/Events",
                "POST",
                new Dictionary<string, Response> {
                    {"200", new Response {
                        Description = "Event was successfully injected into the Event Store"
                    }},
                },
                _ => true,
                CreateFormParameterWithNameAndType("EventSourceId", "formData", typeof(EventSourceId))
            );
            _queryDocumentGenerator.Configure(
                new Info
                {
                    Title = "Queries",
                },
                "/api/Dolittle/Debugging/Swagger/Queries",
                "GET",
                new Dictionary<string, Response> {
                    {"200", new Response {
                        Description = "Result of query execution"
                    }},
                },
                _ => !_.Name.Equals("Query")
            );
        }

        IParameter CreateFormParameterWithNameAndType(string Name, string Location, Type type)
        {
            var parameter = new NonBodyParameter
            {
                Name = Name,
                In = Location,
                Required = true,
            };
            var schema = _schemaRegistryFactory.Create().GetOrRegister(type);
            parameter.Type = schema.Type;
            parameter.Format = schema.Format;
            return parameter;
        }

        /// <inheritdoc/>
        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            switch (documentName)
            {
                case "Dolittle.Commands":
                    return _commandDocumentGenerator.GetSwagger(documentName, host, basePath, schemes);
                case "Dolittle.Events":
                    return _eventDocumentGenerator.GetSwagger(documentName, host, basePath, schemes);
                case "Dolittle.Queries":
                    return _queryDocumentGenerator.GetSwagger(documentName, host, basePath, schemes);
                default:
                    System.Console.WriteLine($"!!!! Using original Generator !!!!");
                    return _originalGenerator.GetSwagger(documentName, host, basePath, schemes);
            }
        }
    }
}
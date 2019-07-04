/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Dolittle.Artifacts;
using Dolittle.AspNetCore.Debugging.Commands;
using Dolittle.AspNetCore.Debugging.Events;
using Dolittle.AspNetCore.Debugging.Queries;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Commands;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Queries;
using Dolittle.Runtime.Events;
using Dolittle.Serialization.Json;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactControllerBase{ICommand}"/> for handling Queries
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Queries")]
    public class QueriesController : ArtifactControllerBase<IQuery>
    {
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly IQueryCoordinator _queryCoordinator;
        readonly ISerializer _serializer;

        /// <summary>
        /// Instanciates a new <see cref="QueriesController"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use</param>
        /// <param name="artifactTypes"></param>
        /// <param name="objectFactory"></param>
        /// <param name="artifactTypeMap"></param>
        /// <param name="queryCoordinator"></param>
        /// <param name="serializer"></param>
        public QueriesController(
            ILogger logger,
            IArtifactMapper<IQuery> artifactTypes,
            IObjectFactory objectFactory,
            IArtifactTypeMap artifactTypeMap,
            IQueryCoordinator queryCoordinator,
            ISerializer serializer
        )
        : base(logger, artifactTypes, objectFactory)
        {
            _artifactTypeMap = artifactTypeMap;
            _queryCoordinator = queryCoordinator;
            _serializer = serializer;
        }

        /// <summary>
        /// The HTTP method handler
        /// </summary>
        /// <param name="path">The fully qualified type name of the query encoded as a path</param>
        [HttpGet("{*path}")]
        public IActionResult Handle([FromRoute] string path)
        {
            if (TryResolveTenantAndArtifact(path, HttpContext.Request.Query.ToDictionary(), out var tenantId, out var query))
            {
                var result = _queryCoordinator.Handle(tenantId, query);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = _serializer.ToJson(result),
                };
            }
            
            return new BadRequestResult();
        }
    }
}
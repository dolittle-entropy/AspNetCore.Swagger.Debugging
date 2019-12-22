// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.AspNetCore.Debugging.Queries;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Queries;
using Dolittle.Serialization.Json;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactControllerBase{ICommand}"/> for handling Queries.
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Queries")]
    public class QueriesController : ArtifactControllerBase<IQuery>
    {
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly IQueryCoordinator _queryCoordinator;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueriesController"/> class.
        /// </summary>
        /// <param name="artifactTypes"><see cref="IArtifactMapper{T}"/> for mapping queries.</param>
        /// <param name="objectFactory"><see cref="IObjectFactory"/> for creating instances of queries.</param>
        /// <param name="artifactTypeMap"><see cref="IArtifactTypeMap"/> for mapping artifacts to types.</param>
        /// <param name="queryCoordinator"><see cref="IQueryCoordinator"/> for coordinating execution of queries.</param>
        /// <param name="serializer">JSON <see cref="ISerializer"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public QueriesController(
            IArtifactMapper<IQuery> artifactTypes,
            IObjectFactory objectFactory,
            IArtifactTypeMap artifactTypeMap,
            IQueryCoordinator queryCoordinator,
            ISerializer serializer,
            ILogger logger)
            : base(artifactTypes, objectFactory, logger)
        {
            _artifactTypeMap = artifactTypeMap;
            _queryCoordinator = queryCoordinator;
            _serializer = serializer;
        }

        /// <summary>
        /// [GET] Action for performing a query / getting result from a query.
        /// </summary>
        /// <param name="path">The fully qualified type name of the query encoded as a path.</param>
        /// <returns><see cref="IActionResult"/> holding the query result.</returns>
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
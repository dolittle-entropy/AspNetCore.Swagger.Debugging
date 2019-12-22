// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.AspNetCore.Debugging.Commands;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Commands;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Serialization.Json;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactControllerBase{ICommand}"/> for handling Commands.
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Commands")]
    public class CommandsController : ArtifactControllerBase<ICommand>
    {
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly ICommandCoordinator _commandCoordinator;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsController"/> class.
        /// </summary>
        /// <param name="artifactTypes"><see cref="IArtifactMapper{T}"/> for mapping commands.</param>
        /// <param name="objectFactory"><see cref="IObjectFactory"/> for creating instances of commands.</param>
        /// <param name="artifactTypeMap"><see cref="IArtifactTypeMap"/> for mapping artifacts to types.</param>
        /// <param name="commandCoordinator"><see cref="ICommandCoordinator"/> for coordinating commands.</param>
        /// <param name="serializer">JSON <see cref="ISerializer"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public CommandsController(
            IArtifactMapper<ICommand> artifactTypes,
            IObjectFactory objectFactory,
            IArtifactTypeMap artifactTypeMap,
            ICommandCoordinator commandCoordinator,
            ISerializer serializer,
            ILogger logger)
            : base(artifactTypes, objectFactory, logger)
        {
            _artifactTypeMap = artifactTypeMap;
            _commandCoordinator = commandCoordinator;
            _serializer = serializer;
        }

        /// <summary>
        /// [POST] ACtion for handling a command.
        /// </summary>
        /// <param name="path">The fully qualified type name of the command encoded as a path.</param>
        /// <returns><see cref="IActionResult"/> holding the command result.</returns>
        [HttpPost("{*path}")]
        public IActionResult Handle([FromRoute] string path)
        {
            if (TryResolveTenantAndArtifact(path, HttpContext.Request.Form.ToDictionary(), out var tenantId, out var command))
            {
                var result = _commandCoordinator.Handle(tenantId, command);
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
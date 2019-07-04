/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Dolittle.Artifacts;
using Dolittle.AspNetCore.Debugging.Commands;
using Dolittle.AspNetCore.Debugging.Events;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Commands;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events;
using Dolittle.Serialization.Json;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactController{ICommand}"/> for handling Commands
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Commands")]
    public class CommandsController : ArtifactController<ICommand>
    {
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly ICommandCoordinator _commandCoordinator;
        readonly ISerializer _serializer;

        /// <summary>
        /// Instanciates a new <see cref="CommandsController"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use</param>
        /// <param name="artifactTypes"></param>
        /// <param name="objectFactory"></param>
        /// <param name="artifactTypeMap"></param>
        /// <param name="commandCoordinator"></param>
        /// <param name="serializer"></param>
        public CommandsController(
            ILogger logger,
            IArtifactMapper<ICommand> artifactTypes,
            IObjectFactory objectFactory,
            IArtifactTypeMap artifactTypeMap,
            ICommandCoordinator commandCoordinator,
            ISerializer serializer
        )
        : base(logger, artifactTypes, objectFactory)
        {
            _artifactTypeMap = artifactTypeMap;
            _commandCoordinator = commandCoordinator;
            _serializer = serializer;
        }

        /// <inheritdoc/>
        protected override IActionResult HandleReceivedArifact(TenantId tenantId, ICommand artifact)
        {
            var result = _commandCoordinator.Handle(tenantId, artifact);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = _serializer.ToJson(result),
            };
        }
    }
}
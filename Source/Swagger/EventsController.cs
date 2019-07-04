/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Dolittle.AspNetCore.Debugging.Events;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactControllerBase{IEvent}"/> for handling Events
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Events")]
    public class EventsController : ArtifactControllerBase<IEvent>
    {
        readonly IEventInjector _eventInjector;

        /// <summary>
        /// Instanciates a new <see cref="EventsController"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use</param>
        /// <param name="artifactTypes"></param>
        /// <param name="objectFactory"></param>
        /// <param name="eventInjector"></param>
        public EventsController(
            ILogger logger,
            IArtifactMapper<IEvent> artifactTypes,
            IObjectFactory objectFactory,
            IEventInjector eventInjector
        )
        : base(logger, artifactTypes, objectFactory)
        {
            _eventInjector = eventInjector;
        }

        /// <summary>
        /// The HTTP method handler
        /// </summary>
        /// <param name="path">The fully qualified type name of the event encoded as a path</param>
        [HttpPost("{*path}")]
        public IActionResult Handle([FromRoute] string path)
        {
            if (TryResolveTenantAndArtifact(path, HttpContext.Request.Form.ToDictionary(), out var tenantId, out var @event))
            {
                var eventSourceId = HttpContext.Request.Form["EventSourceId"].First().ParseTo(typeof(EventSourceId)) as EventSourceId;
                _eventInjector.InjectEvent(tenantId, eventSourceId, @event);
                return Ok();
            }
            
            return new BadRequestResult();
        }
    }
}
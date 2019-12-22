// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.AspNetCore.Debugging.Events;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// An implementation of an <see cref="ArtifactControllerBase{IEvent}"/> for handling Events.
    /// </summary>
    [Route("api/Dolittle/Debugging/Swagger/Events")]
    public class EventsController : ArtifactControllerBase<IEvent>
    {
        readonly IEventInjector _eventInjector;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController"/> class.
        /// </summary>
        /// <param name="artifactTypes"><see cref="IArtifactMapper{T}"/> for mapping events.</param>
        /// <param name="objectFactory"><see cref="IObjectFactory"/> for creating instances of events.</param>
        /// <param name="eventInjector"><see cref="IEventInjector"/> for injecting events.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public EventsController(
            IArtifactMapper<IEvent> artifactTypes,
            IObjectFactory objectFactory,
            IEventInjector eventInjector,
            ILogger logger)
            : base(artifactTypes, objectFactory, logger)
        {
            _eventInjector = eventInjector;
        }

        /// <summary>
        /// [POST] Action for injecting events.
        /// </summary>
        /// <param name="path">The fully qualified type name of the event encoded as a path.</param>
        /// <returns><see cref="IActionResult"/> holding the result from injecting the event.</returns>
        [HttpPost("{*path}")]
        public IActionResult Handle([FromRoute] string path)
        {
            if (TryResolveTenantAndArtifact(path, HttpContext.Request.Form.ToDictionary(), out var tenantId, out var @event))
            {
                var eventSourceId = HttpContext.Request.Form["EventSourceId"][0].ParseTo(typeof(EventSourceId)) as EventSourceId;
                _eventInjector.InjectEvent(tenantId, eventSourceId, @event);
                return Ok();
            }

            return new BadRequestResult();
        }
    }
}
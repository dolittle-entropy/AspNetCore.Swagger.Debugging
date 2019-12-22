// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Reflection;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Represents a controller that handles artifacts by fully qualified name encoded in the path.
    /// </summary>
    /// <typeparam name="T">Type of artifact to handle.</typeparam>
    public abstract class ArtifactControllerBase<T> : ControllerBase
        where T : class
    {
        readonly ILogger _logger;
        readonly IArtifactMapper<T> _artifactTypes;
        readonly IObjectFactory _objectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactControllerBase{T}"/> class.
        /// </summary>
        /// <param name="artifactTypes"><see cref="IArtifactMapper{T}"/> for mapping artifacts.</param>
        /// <param name="objectFactory"><see cref="IObjectFactory"/> for creating instances of artifacts.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        protected ArtifactControllerBase(
            IArtifactMapper<T> artifactTypes,
            IObjectFactory objectFactory,
            ILogger logger)
        {
            _artifactTypes = artifactTypes;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        /// <summary>
        /// Tries to resolve tenant and artifact based on the incoming request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="request">The request data.</param>
        /// <param name="tenant">The resolved tenant.</param>
        /// <param name="artifact">The created artifact.</param>
        /// <returns>Whether the resolution worked.</returns>
        protected bool TryResolveTenantAndArtifact(string path, IDictionary<string, StringValues> request, out TenantId tenant, out T artifact)
        {
            if (path[0] != '/') path = $"/{path}";

            var artifactType = _artifactTypes.GetTypeFor(path);
            var properties = new NullFreeDictionary<string, object>();

            tenant = request["TenantId"][0].ParseTo(typeof(TenantId)) as TenantId;

            foreach (var property in artifactType.GetProperties())
            {
                if (request.TryGetValue(property.Name, out var values))
                {
                    if (TryConvertFormValuesTo(property.PropertyType, values, out var result))
                    {
                        if (result.GetType().IsConcept())
                        {
                            result = result.GetConceptValue();
                        }

                        properties.Add(property.Name, result);
                    }
                }
            }

            artifact = _objectFactory.Build(artifactType, new PropertyBag(properties)) as T;
            return artifact != null;
        }

        bool TryConvertFormValuesTo(Type targetType, string[] values, out object result)
        {
            if (targetType.IsArray || targetType.IsEnumerable())
            {
                var innerType = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
                var list = new List<object>();

                if (values.Length == 1) values = values[0].Split(',');

                if (values.All(_ =>
                {
                    if (TryConvertFormValuesTo(innerType, new[] { _ }, out var innerResult))
                    {
                        list.Add(innerResult);
                        return true;
                    }

                    return false;
                }))
                {
                    result = list.ToArray();
                    return true;
                }

                list.Clear();

                if (values.All(_ =>
                {
                    var trimmed = _.Trim('"').Trim();
                    if (TryConvertFormValuesTo(innerType, new[] { trimmed }, out var innerResult))
                    {
                        list.Add(innerResult);
                        return true;
                    }

                    return false;
                }))
                {
                    result = list.ToArray();
                    return true;
                }
            }

            try
            {
                result = ParseStringTo(targetType, values[0]);
                if (!IsDefaultValue(targetType, result))
                {
                    return true;
                }
            }
            catch { }

            try
            {
                result = ParseStringTo(targetType, values.ToString());
                return !IsDefaultValue(targetType, result);
            }
            catch { }

            result = null;
            return false;
        }

        bool IsDefaultValue(Type type, object value)
        {
            if (type.IsPrimitive)
            {
                return value.Equals(Activator.CreateInstance(type));
            }
            else if (type == typeof(Guid))
            {
                return value.Equals(Guid.Empty);
            }
            else if (type.IsConcept())
            {
                return IsDefaultValue(type.GetConceptValueType(), value);
            }

            return value == null;
        }

        object ParseStringTo(Type type, string value)
        {
            if (type == typeof(DateTimeOffset))
            {
                // It seems like the PropertyBag expects this value somewhere else
                return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture).ToUnixTimeMilliseconds();
            }

            return value.ParseTo(type);
        }
    }
}
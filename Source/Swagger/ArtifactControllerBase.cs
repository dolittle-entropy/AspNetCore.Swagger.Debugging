/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.AspNetCore.Debugging.Swagger.Artifacts;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Reflection;
using Dolittle.Serialization.Json;
using Dolittle.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Represents a controller that handles artifacts by fully qualified name encoded in the path
    /// </summary>
    /// <typeparam name="T">Type of artifact to handle</typeparam>
    public abstract class ArtifactControllerBase<T> : ControllerBase where T : class
    {
        readonly ILogger _logger;
        readonly IArtifactMapper<T> _artifactTypes;
        readonly IObjectFactory _objectFactory;

        /// <summary>
        /// Instanciates a new <see cref="ArtifactControllerBase{T}"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use</param>
        /// <param name="artifactTypes"></param>
        /// <param name="objectFactory"></param>
        protected ArtifactControllerBase(
            ILogger logger,
            IArtifactMapper<T> artifactTypes,
            IObjectFactory objectFactory
        )
        {
            _logger = logger;
            _artifactTypes = artifactTypes;
            _objectFactory = objectFactory;
        }

        /// <summary>
        /// Tries to resolve tenant and artifact based on the incoming request
        /// </summary>
        /// <param name="path">The request path</param>
        /// <param name="request">The request data</param>
        /// <param name="tenant">The resolved tenant</param>
        /// <param name="artifact">The created artifact</param>
        /// <returns>Whether the resolution worked</returns>
        protected bool TryResolveTenantAndArtifact(string path, IDictionary<string, StringValues> request, out TenantId tenant, out T artifact)
        {
            if (path[0] != '/') path = $"/{path}";

            var artifactType = _artifactTypes.GetTypeFor(path);
            var properties = new NullFreeDictionary<string, object>();

            tenant = request["TenantId"].First().ParseTo(typeof(TenantId)) as TenantId;

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
            if (targetType.IsArray || targetType.IsEnumerable())
            {
                var innerType = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
                var list = new List<object>();

                if (values.Length == 1) values = values[0].Split(',');
                
                if (values.All(_ => {
                    if (TryConvertFormValuesTo(innerType, new [] {_}, out var innerResult))
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

                if (values.All(_ => {
                    var trimmed = _.Trim('"').Trim();
                    if (TryConvertFormValuesTo(innerType, new [] {trimmed}, out var innerResult))
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
            catch {}

            try
            {
                result = ParseStringTo(targetType, values.ToString());
                return !IsDefaultValue(targetType, result);
            }
            catch {}
            

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
                return DateTimeOffset.Parse(value).ToUnixTimeMilliseconds();
            }

            return value.ParseTo(type);
        }
    }
}
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Dolittle.AspNetCore.Debugging.Swagger
{
    /// <summary>
    /// Extensions for HttpContext.Request.Form/Query
    /// </summary>
    public static class RequestDataExtensions
    {
        /// <summary>
        /// Converts HttpContext.Request.Form/Query entries to a dictionary
        /// </summary>
        /// <param name="entries">The entries to add to a dictionary</param>
        /// <returns>A dictionary containing the original entries</returns>
        public static IDictionary<string, StringValues> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> entries)
        {
            var result = new Dictionary<string, StringValues>();
            foreach (var entry in entries)
            {
                result.Add(entry.Key, entry.Value);
            }
            return result;
        }
    }
}
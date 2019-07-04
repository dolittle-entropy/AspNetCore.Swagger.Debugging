/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;

namespace Dolittle.AspNetCore.Debugging.Swagger.SwaggerGen
{
    /// <summary>
    /// Represents a generator that generates a <see cref="SwaggerDocument"/> for a Dolittle artifact
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDocumentGenerator<T> : ISwaggerProvider where T : class
    {
        /// <summary>
        /// Configures metadata for the <see cref="SwaggerDocument"/>
        /// </summary>
        /// <param name="info"><see cref="Info"/> for the document</param>
        /// <param name="basePath">Base path for the operations</param>
        /// <param name="parameterFilter">A filter for selecting which parameters to show</param>
        /// <param name="globalParameters">A list of <see cref="IParameter"/> that will be prepended to the paramaters of all operations</param>
        void Configure(Info info, string basePath, Func<PropertyInfo,bool> parameterFilter, params IParameter[] globalParameters);
    }
}
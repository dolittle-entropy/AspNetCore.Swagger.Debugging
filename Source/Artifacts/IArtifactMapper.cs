/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.AspNetCore.Swagger.Debugging.Artifacts
{
    /// <summary>
    /// Represents a mapper that maps artifacts to paths and vice versa, for use with Swagger debugging tools
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArtifactMapper<T> where T : class
    {
        /// <summary>
        /// All the paths mapped from all the corresponding <typeparamref name="T">artifact type</typeparamref>
        /// </summary>
        /// <value></value>
        IEnumerable<string> ApiPaths { get; }

        /// <summary>
        /// Maps a path to an <see cref="Artifact"/>
        /// </summary>
        /// <param name="path">The path to look up</param>
        /// <returns>The <see cref="Artifact"/> corresponding to the provided path</returns>
        Artifact GetArtifactFor(string path);

        /// <summary>
        /// Maps a path to a <see cref="Type"/>
        /// </summary>
        /// <param name="path">The path to look up</param>
        /// <returns>The <see cref="Type"/> corresponding to the provided path</returns>
        Type GetTypeFor(string path);
    }
}
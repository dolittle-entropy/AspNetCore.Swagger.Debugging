// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.AspNetCore.Debugging.Swagger.Artifacts
{
    /// <summary>
    /// Represents a mapper that maps artifacts to paths and vice versa, for use with Swagger debugging tools.
    /// </summary>
    /// <typeparam name="T">Type of artifact.</typeparam>
    public interface IArtifactMapper<T>
        where T : class
    {
        /// <summary>
        /// Gets all the paths mapped from all the corresponding <typeparamref name="T">artifact type</typeparamref>.
        /// </summary>
        IEnumerable<string> ApiPaths { get; }

        /// <summary>
        /// Maps a path to an <see cref="Artifact"/>.
        /// </summary>
        /// <param name="path">The path to look up.</param>
        /// <returns>The <see cref="Artifact"/> corresponding to the provided path.</returns>
        Artifact GetArtifactFor(string path);

        /// <summary>
        /// Maps a path to a <see cref="Type"/>.
        /// </summary>
        /// <param name="path">The path to look up.</param>
        /// <returns>The <see cref="Type"/> corresponding to the provided path.</returns>
        Type GetTypeFor(string path);
    }
}
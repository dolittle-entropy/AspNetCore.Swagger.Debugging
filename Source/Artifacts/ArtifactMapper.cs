// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Applications.Configuration;
using Dolittle.Artifacts;
using Dolittle.Artifacts.Configuration;

namespace Dolittle.AspNetCore.Debugging.Swagger.Artifacts
{
    /// <summary>
    /// Implementation of an <see cref="IArtifactMapper{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of artifact.</typeparam>
    public class ArtifactMapper<T> : IArtifactMapper<T>
        where T : class
    {
        readonly Topology _topology;
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly ArtifactsConfiguration _artifacts;
        readonly IDictionary<string, Type> _artifactsByPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactMapper{T}"/> class.
        /// </summary>
        /// <param name="topology"><see cref="Topology"/> configuration.</param>
        /// <param name="artifacts">The <see cref="ArtifactsConfiguration"/>.</param>
        /// <param name="artifactTypeMap"><see cref="IArtifactTypeMap"/> for mapping artifacts and types.</param>
        public ArtifactMapper(
            Topology topology,
            ArtifactsConfiguration artifacts,
            IArtifactTypeMap artifactTypeMap)
        {
            _topology = topology;
            _artifacts = artifacts;
            _artifactTypeMap = artifactTypeMap;

            _artifactsByPath = new Dictionary<string, Type>();
            BuildMapOfAllCorrespondingArtifacts();
        }

        /// <inheritdoc/>
        public IEnumerable<string> ApiPaths => _artifactsByPath.Keys;

        /// <inheritdoc/>
        public Artifact GetArtifactFor(string path) => _artifactTypeMap.GetArtifactFor(GetTypeFor(path));

        /// <inheritdoc/>
        public Type GetTypeFor(string path) => _artifactsByPath[path];

        void BuildMapOfAllCorrespondingArtifacts()
        {
            if (_topology.Modules.Count > 0)
            {
                foreach (var module in _topology.Modules.OrderBy(_ => _.Value.Name))
                {
                    AddFeaturesRecursively(module.Value.Features, $"/{module.Value.Name}");
                }
            }
            else
            {
                AddFeaturesRecursively(_topology.Features, string.Empty);
            }
        }

        void AddFeaturesRecursively(IReadOnlyDictionary<Feature, FeatureDefinition> features, string prefix)
        {
            foreach (var feature in features)
            {
                if (_artifacts.TryGetValue(feature.Key, out var artifacts))
                {
                    AddArtifacts(
                        $"{prefix}/{feature.Value.Name}",
                        artifacts.Commands,
                        artifacts.EventSources,
                        artifacts.Events,
                        artifacts.Queries,
                        artifacts.ReadModels);
                }

                AddFeaturesRecursively(feature.Value.SubFeatures, $"{prefix}/{feature.Value.Name}");
            }
        }

        void AddArtifacts(string prefix, params IReadOnlyDictionary<ArtifactId, ArtifactDefinition>[] artifactsByTypes)
        {
            foreach (var artifactByType in artifactsByTypes)
            {
                foreach (var artifactDefinition in artifactByType)
                {
                    var artifactType = artifactDefinition.Value.Type.GetActualType();
                    if (typeof(T).IsAssignableFrom(artifactType))
                    {
                        _artifactsByPath.Add($"{prefix}/{artifactType.Name}", artifactType);
                    }
                }
            }
        }
    }
}
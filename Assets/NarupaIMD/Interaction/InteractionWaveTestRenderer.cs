﻿// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Narupa.Frontend.Manipulation;
using Narupa.Frontend.Utility;
using Narupa.Session;
using NarupaIMD;
using UnityEngine;

namespace NarupaXR.Interaction
{
    /// <summary>
    /// Manage instances of InteractionWaveRenderer so that all known 
    /// interactions are rendered using Mike's pretty sine wave method from 
    /// Narupa 1
    /// </summary>
    public class InteractionWaveTestRenderer : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private NarupaImdSimulation simulation;
        [SerializeField]
        private InteractionWaveRenderer waveTemplate;

        private IndexedPool<InteractionWaveRenderer> wavePool;
        
#pragma warning restore 0649

        private void Start()
        {
            wavePool = new IndexedPool<InteractionWaveRenderer>(CreateInstanceCallback, ActivateInstanceCallback, DeactivateInstanceCallback);
        }

        private void DeactivateInstanceCallback(InteractionWaveRenderer obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void ActivateInstanceCallback(InteractionWaveRenderer obj)
        {
            obj.gameObject.SetActive(true);
        }

        private InteractionWaveRenderer CreateInstanceCallback()
        {
            var renderer = Instantiate(waveTemplate, transform, true);
            renderer.gameObject.SetActive(true);
            return renderer;
        }

        private void Update()
        {
            var interactions = simulation.Imd.Interactions;
            var frame = simulation.FrameSynchronizer.CurrentFrame;
            
            wavePool.MapConfig(interactions.Values, MapConfigToInstance);
            
            void MapConfigToInstance(ImdSession.InteractionData interaction, 
                                     InteractionWaveRenderer renderer)
            {
                var particlePositionSim = computeParticleCentroid(interaction.ParticleIds);
                var particlePositionWorld = transform.TransformPoint(particlePositionSim);
                
                renderer.SetPositionAndForce(transform.TransformPoint(interaction.Position),
                                             particlePositionWorld, 
                                             0.5f);
            }

            Vector3 computeParticleCentroid(IReadOnlyList<int> particleIds)
            {
                var centroid = Vector3.zero;

                for (int i = 0; i < particleIds.Count; ++i)
                    centroid += frame.ParticlePositions[particleIds[i]];

                return centroid / particleIds.Count;
            }
        }
    }
}

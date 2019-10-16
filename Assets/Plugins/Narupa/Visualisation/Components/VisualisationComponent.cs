// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Narupa.Core;
using Narupa.Visualisation.Property;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Narupa.Visualisation.Components
{
    /// <summary>
    /// Wrapper <see cref="MonoBehaviour" /> around a node of the visualisation system,
    /// storing the node has a serialized parameter.
    /// </summary>
    [ExecuteAlways]
    public abstract class VisualisationComponent<TNode> : VisualisationComponent
        where TNode : new()
    {
        [SerializeField]
        protected TNode node = new TNode();

        protected override Type GetWrappedVisualisationNodeType()
        {
            return typeof(TNode);
        }

        public override object GetWrappedVisualisationNode()
        {
            return node;
        }
    }

    /// <summary>
    /// Wrapper <see cref="MonoBehaviour" /> around a node of the visualisation system,
    /// serializing links between various nodes. This allows the node system to be used
    /// as multiple nodes in a single <see cref="MonoBehaviour" /> or by using the
    /// subclasses of <see cref="VisualisationComponent" /> to setup a visualisation
    /// graph in the Editor.
    /// </summary>
    public abstract class VisualisationComponent : MonoBehaviour, ISerializationCallbackReceiver,
                                                   IPropertyProvider
    {
        protected virtual void OnEnable()
        {
            SetupLinks();
        }
        
        protected virtual void OnDisable()
        {
            
        }

        /// <summary>
        /// Describes a link from one visualisation node to another
        /// </summary>
        [Serializable]
        public class InputPropertyLink
        {
            /// <summary>
            /// The visualisation component that contains the node with the output field
            /// </summary>
            [SerializeField]
            public Object sourceComponent;

            /// <summary>
            /// The name of the output field
            /// </summary>
            [SerializeField]
            public string sourceFieldName;

            /// <summary>
            /// The name of the input field
            /// </summary>
            [SerializeField]
            public string destinationFieldName;
        }

        /// <summary>
        /// Serialized list of all the inputs
        /// </summary>
        [SerializeField]
        private List<InputPropertyLink> inputLinkCollection = new List<InputPropertyLink>();

        protected virtual void Awake()
        {
            SetupLinks();
        }

        /// <summary>
        /// Setup all the links that are serialized within
        /// <see cref="inputLinkCollection" />.
        /// </summary>
        private void SetupLinks()
        {
            foreach (var link in inputLinkCollection)
            {
                var sourceProvider = link.sourceComponent as IPropertyProvider;
                if (sourceProvider == null)
                    continue;

                FindAndSetupLink(sourceProvider, link.sourceFieldName, link.destinationFieldName);
            }
        }

        /// <summary>
        /// Setup a link between two property providers
        /// </summary>
        private static void LinkProperties<T>(IPropertyProvider source,
                                              string sourceName,
                                              IPropertyProvider destination,
                                              string destName)
        {
            if (!(source.GetProperty(sourceName) is IReadOnlyProperty<T> sourceOutput))
                throw new InvalidOperationException();
            if (!(destination.GetProperty(destName) is IProperty<T> destInput))
                throw new InvalidOperationException();

            destInput.LinkedProperty = sourceOutput;
        }

        /// <summary>
        /// Setup a link between two properties without knowing the type
        /// </summary>
        private void FindAndSetupLink(IPropertyProvider source,
                                      string sourceName,
                                      string destName)
        {
            var inputProperty = GetWrappedNodeField(destName);
            if (inputProperty == null)
                throw new InvalidOperationException(
                    $"{this} is missing a destination property with name {destName}");
            var type = inputProperty.FieldType;
            while (!type.IsGenericType ||
                   type.GetGenericTypeDefinition() != typeof(Property<>))
                type = type.BaseType;
            type = type.GetGenericArguments()[0];

            // Get the correct generic method
            var method = typeof(VisualisationComponent).GetMethod(nameof(LinkProperties),
                                                                  BindingFlags.Public
                                                                | BindingFlags.NonPublic
                                                                | BindingFlags.Static)
                                                       ?.MakeGenericMethod(type);

            if (method == null)
                throw new InvalidOperationException(
                    $"Failed to create {nameof(LinkProperties)} method for type {type}");

            method.Invoke(null, new object[]
            {
                source, sourceName, this, destName
            });
        }

        /// <summary>
        /// Get the visualisation node wrapped by this component.
        /// </summary>
        public abstract object GetWrappedVisualisationNode();

        /// <summary>
        /// Get the type of the node wrapped by this component.
        /// </summary>
        protected abstract Type GetWrappedVisualisationNodeType();

        public void OnBeforeSerialize()
        {
            ClearUpInvalidLinks();
        }

        /// <summary>
        /// Delete any links which don't have a valid destination. This is important so
        /// refactored field names result in invalid links being removed.
        /// </summary>
        private void ClearUpInvalidLinks()
        {
            inputLinkCollection.RemoveAll(
                link => string.IsNullOrEmpty(link.destinationFieldName) ||
                        GetWrappedNodeField(link.destinationFieldName) == null);
        }

        private FieldInfo GetWrappedNodeField(string fieldName)
        {
            var nodeType = GetWrappedVisualisationNodeType();

            return nodeType.GetFieldInSelfOrParents(fieldName,
                                                    BindingFlags.Instance |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic);
        }

        private object GetWrappedNodeValue(string fieldName)
        {
            var node = GetWrappedVisualisationNode();
            return GetWrappedNodeField(fieldName).GetValue(node);
        }

        public void OnAfterDeserialize()
        {
            ClearUpInvalidLinks();
        }

        /// <inheritdoc cref="IPropertyProvider.GetProperty" />
        public Property.Property GetProperty(string name)
        {
            return GetWrappedNodeValue(name) as Property.Property;
        }

        /// <inheritdoc cref="IPropertyProvider.GetProperties" />
        public IEnumerable<(string, Property.Property)> GetProperties()
        {
            var node = GetWrappedVisualisationNode();

            var allFields = GetWrappedVisualisationNodeType()
                .GetProperties(BindingFlags.Instance
                             | BindingFlags.Public);

            var validFields = allFields.Where(field => field.PropertyType.IsAssignableFrom(
                                                  typeof(
                                                      IProperty<>
                                                  )));

            return validFields.Select(field => (field.Name,
                                                field.GetValue(node) as
                                                    Property.Property));
        }
    }
}
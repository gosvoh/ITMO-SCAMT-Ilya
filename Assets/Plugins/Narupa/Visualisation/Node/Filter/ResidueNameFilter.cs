using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Node.Filter
{
    /// <summary>
    /// Filters particles by the name of their residues.
    /// </summary>
    [Serializable]
    public class ResidueNameFilter : VisualiserFilter
    {
        [SerializeField]
        private StringProperty pattern = new StringProperty();

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        [SerializeField]
        private StringArrayProperty residueNames = new StringArrayProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => pattern.HasNonNullValue()
                                             && particleResidues.HasNonEmptyValue()
                                             && residueNames.HasNonEmptyValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => pattern.IsDirty
                                             || particleResidues.IsDirty
                                             || residueNames.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            pattern.IsDirty = false;
            particleResidues.IsDirty = false;
            residueNames.IsDirty = false;
        }

        /// <inheritdoc cref="VisualiserFilter.MaximumFilterCount"/>
        protected override int MaximumFilterCount => particleResidues.Value.Length;

        /// <inheritdoc cref="VisualiserFilter.GetFilteredIndices"/>
        protected override IEnumerable<int> GetFilteredIndices()
        {
            var regex = new Regex(pattern.Value);
            for (var i = 0; i < particleResidues.Value.Length; i++)
            {
                if (regex.IsMatch(residueNames.Value[particleResidues.Value[i]]))
                    yield return i;
            }
        }
    }
}
// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;

namespace Narupa.Frame
{
    /// <summary>
    /// Represents a bond between two particles with indices A and B
    /// </summary>
    [Serializable]
    public struct BondPair
    {
        /// <summary>
        /// The index of the first particle
        /// </summary>
        public int A;

        /// <summary>
        /// The index of the second particle
        /// </summary>
        public int B;

        /// <summary>
        /// Construct a BondPair for a pair of two indices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public BondPair(int a, int b)
        {
            A = a;
            B = b;
        }
    }
}
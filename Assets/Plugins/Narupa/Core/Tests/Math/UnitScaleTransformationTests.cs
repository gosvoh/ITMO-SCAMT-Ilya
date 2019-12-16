using System.Collections.Generic;
using System.Linq;
using Narupa.Core.Math;
using Narupa.Testing;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Narupa.Core.Tests.Math
{
    public class UnitScaleTransformationTests
    {
        private static readonly int TestCount = 128;

        public static IEnumerable<UnitScaleTransformation> GetTransformations()
        {
            return SpatialTestData.GetRandomTransformationsUnitScale(TestCount, 912487);
        }

        public static IEnumerable<(UnitScaleTransformation, UnitScaleTransformation)>
            GetTransformationPairs()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUnitScale(TestCount, 32523),
                SpatialTestData.GetRandomTransformationsUnitScale(TestCount, 8643),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(UnitScaleTransformation, Vector3)>
            GetTransformationAndVectors()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUnitScale(TestCount, 62324),
                SpatialTestData.GetRandomPositions(TestCount, 14892),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(UnitScaleTransformation, Quaternion)>
            GetTransformationAndRotations()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUnitScale(TestCount, 12574),
                SpatialTestData.GetRandomRotations(TestCount, 845),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(Vector3, Quaternion)> GetTranslationAndRotations()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomPositions(TestCount, 3523),
                SpatialTestData.GetRandomRotations(TestCount, 75472),
                (a, b) => (a, b)
            );
        }

        #region Constructors

        [Test]
        public void Constructor(
            [ValueSource(nameof(GetTranslationAndRotations))]
            (Vector3, Quaternion) input)
        {
            var (position, rotation) = input;
            var transformation = new UnitScaleTransformation(position, rotation);
            MathAssert.AreEqual(position, transformation.position);
            MathAssert.AreEqual(rotation, transformation.rotation);
        }

        #endregion


        #region Constants

        [Test]
        public void Identity()
        {
            Assert.AreEqual(Matrix4x4.identity, UnitScaleTransformation.identity.matrix);
        }

        #endregion


        #region Inverse

        [Test]
        public void Inverse(
            [ValueSource(nameof(GetTransformations))] UnitScaleTransformation transformation)
        {
            var matrix = transformation.matrix;
            MathAssert.AreEqual(matrix.inverse, transformation.inverse.matrix);
        }

        #endregion


        #region Matrices

        [Test]
        public void PositionFromMatrix(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UnitScaleTransformation, Vector3) input)
        {
            var (transformation, position) = input;
            transformation.position = position;
            MathAssert.AreEqual(position, transformation.matrix.GetTranslation());
        }

        [Test]
        public void RotationFromMatrix(
            [ValueSource(nameof(GetTransformationAndRotations))]
            (UnitScaleTransformation, Quaternion) input)
        {
            var (transformation, rotation) = input;
            transformation.rotation = rotation;
            MathAssert.AreEqual(rotation, transformation.matrix.GetRotation());
        }

        [Test]
        public void InverseMatrix(
            [ValueSource(nameof(GetTransformations))] UnitScaleTransformation transformation)
        {
            var matrix = transformation.matrix;
            MathAssert.AreEqual(matrix.inverse, transformation.inverseMatrix);
        }

        #endregion


        #region Conversions

        [Test]
        public void AsMatrix(
            [ValueSource(nameof(GetTransformations))] UnitScaleTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, (Matrix4x4) transformation);
        }

        [Test]
        public void AsTransformation(
            [ValueSource(nameof(GetTransformations))] UnitScaleTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, ((Transformation) transformation).Matrix);
        }

        [Test]
        public void AsUniformScaleTransformation(
            [ValueSource(nameof(GetTransformations))] UnitScaleTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix,
                                ((UniformScaleTransformation) transformation).matrix);
        }

        #endregion


        #region Multiplication

        [Test]
        public void Multiplication(
            [ValueSource(nameof(GetTransformationPairs))]
            (UnitScaleTransformation, UnitScaleTransformation) input)
        {
            var (transformation1, transformation2) = input;
            var matrix1 = transformation1.matrix;
            var matrix2 = transformation2.matrix;
            MathAssert.AreEqual(matrix1 * matrix2, (transformation1 * transformation2).matrix);
        }

        #endregion


        #region Transformation of Points

        [Test]
        public void TransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UnitScaleTransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.MultiplyPoint3x4(vector),
                                transformation.TransformPoint(vector));
        }

        [Test]
        public void InverseTransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UnitScaleTransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.inverse.MultiplyPoint3x4(vector),
                                transformation.InverseTransformPoint(vector));
        }

        #endregion


        #region Transformation of Directions

        [Test]
        public void TransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UnitScaleTransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.MultiplyVector(vector),
                                transformation.TransformDirection(vector));
        }

        [Test]
        public void InverseTransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UnitScaleTransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.inverse.MultiplyVector(vector),
                                transformation.InverseTransformDirection(vector));
        }

        #endregion
    }
}
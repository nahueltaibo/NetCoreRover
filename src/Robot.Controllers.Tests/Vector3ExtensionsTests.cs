using System;
using System.Numerics;
using NUnit.Framework;
using Robot.Controllers.Extensions;

namespace Robot.Controllers.Tests
{
    [TestFixture]
    public class Vector3ExtensionsTests
    {
        const double Threshold = 1e-6;

        [TestCase(0, Math.PI / 2, Math.PI / 2)]
        [TestCase(0, 0, 0)]
        public void RotateVectorTest(double startAngle, double rotationAngle, double expectedResult)
        {
            var identity = Quaternion.Identity;
            var initialVector = Vector3.UnitX.Rotate(startAngle);
            Assert.AreEqual(1, initialVector.Length(), Threshold);
            var resultingVector = initialVector.Rotate(rotationAngle);
            Assert.AreEqual(1, initialVector.Length(), Threshold);
            Assert.AreEqual(expectedResult, Vector3.UnitX.AngleTo(resultingVector), Threshold);
        }

        [TestCase(Math.PI / 2, 0, -Math.PI / 2)]
        [TestCase(1 + Math.PI / 2, 1 - Math.PI / 2, -Math.PI)]
        [TestCase(0, Math.PI / 2, Math.PI / 2)]
        [TestCase(0, 0, 0)]
        public void CalculateAngleTest(double angle1, double angle2, double expectedResult)
        {
            var vector1 = Vector3.UnitX.Rotate(angle1);
            var vector2 = Vector3.UnitX.Rotate(angle2);
            var result = vector1.AngleTo(vector2);
            Assert.AreEqual(expectedResult, result, Threshold);
        }
    }
}
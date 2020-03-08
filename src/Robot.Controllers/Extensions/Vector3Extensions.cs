using System;
using System.Numerics;

namespace Robot.Controllers.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Rotates the vector to a specified angle
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="angle">The angle to rotate to, rad</param>
        /// <returns>Rotated vector</returns>
        public static Vector3 Rotate(this Vector3 vector, double angle)
        {
            if (vector == Vector3.Zero)
            {
                return Vector3.Zero;
            }

            var theta = Convert.ToSingle(angle);

            var matrix = Matrix4x4.CreateRotationZ(theta, Vector3.Zero);
            var resultingVector = Vector3.Transform(vector, matrix);
            return resultingVector;
        }

        /// <summary>
        /// Checks if two vectors are collinear with the the given threshold
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <param name="threshold">Threshold, rad</param>
        /// <returns></returns>
        public static bool IsCollinear(this Vector3 a, Vector3 b, float threshold)
        {
            var angle = AngleTo(a, b);
            while (Math.Abs(angle) > Math.PI)
            {
                angle += angle > 0 ? -Math.PI : Math.PI;
            }

            // returns if a difference between vectors is less than a threshold specified
            return Math.Abs(angle) < threshold;
        }

        /// <summary>
        /// Calculates angle from the first vector to second.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The angle from the first vector to second, values between -Math.PI to Math.PI</returns>
        /// <remarks>This method answers the question on which angle we should turn the first
        /// vector to make it collinear to the second one</remarks>
        public static double AngleTo(this Vector3 a, Vector3 b)
        {
            var aRotated = a.Rotate(Math.PI / 2);
            var aLength = a.Length();
            var bLength = b.Length();
            var cosinus = Vector3.Dot(a, b) / (aLength * bLength);
            var sinus = Vector3.Dot(aRotated, b) / (aLength * bLength);
            var angle = 0.0;
            if (Math.Abs(cosinus) > float.Epsilon)
            {
                var tg = sinus / cosinus;
                angle = Math.Atan(Convert.ToDouble(tg));
                if (cosinus < 0)
                {
                    angle += Math.PI;
                }

                if (angle > Math.PI)
                {
                    angle -= 2 * Math.PI;
                }
            }
            else
            {
                angle = sinus > 0 ? Math.PI / 2 : -Math.PI / 2;
            }

            return angle;
        }
    }
}
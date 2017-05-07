using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace CommonTools.Miscellaneous
{
    /// <summary>
    /// Contains frequently used orientation relevant functions.
    /// </summary>
    public static class OrientationUtil
    {
        /// <summary>
        /// Returns a unit vector represents the specified angle
        /// relatively to the x axis.
        /// </summary>
        /// <param name="angle">The angle. [rad]</param>
        /// <returns>The unit vector.</returns>
        public static Vector3 UnitVector(double angle)
        {
            return new Vector3(Math.Cos(angle), Math.Sin(angle));
        }

        /// <summary>
        /// Returns the angle of the specified two dimensional vector
        /// relatively to the x axis. [rad] [-PI,PI)
        /// </summary>
        public static double AngleOfVector(Vector3 v)
        {
            return Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// Returns whether <paramref name="b"/> is in clockwise direction from <paramref name="a"/>.
        /// Paralel vectors are treated as clockwise.
        /// </summary>
        /// <param name="a">Vector a.</param>
        /// <param name="b">Vector b.</param>
        /// <returns>Result of clockwise test.</returns>
        public static bool ClockWise(Vector3 a, Vector3 b)
        {
            Vector3 cp = Vector3.CrossProduct(a, b);
            return cp.Z >= 0;
        }

        /// <summary>
        /// Returns whether <paramref name="b"/> is in counterclockwise direction from <paramref name="a"/>.
        /// Paralel vectors are treated as clockwise, thus this function will return false for them.
        /// </summary>
        /// <param name="a">Vector a.</param>
        /// <param name="b">Vector b.</param>
        /// <returns>Result of counter-clockwise test.</returns>
        public static bool CounterClockWise(Vector3 a, Vector3 b)
        {
            Vector3 cp = Vector3.CrossProduct(a, b);
            return cp.Z < 0;
        }

        /// <summary>
        /// Returns the degree value in radian.
        /// </summary>
        /// <param name="deg">Angle in [deg].</param>
        /// <returns>Angle in [rad].</returns>
        public static double DegInRad(double deg)
        {
            return deg / 180 * Math.PI;
        }

        /// <summary>
        /// Returns the radian value in degree.
        /// </summary>
        /// <param name="rad">Angle in [rad].</param>
        /// <returns>Angle in [deg].</returns>
        public static double RadInDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /// <summary>
        /// Converts the given angle to relative angle. Returns an angle between [-PI;PI).
        /// Source: http://www.koders.com/csharp/fid64CC213FA473C179BCBB000AC153D0F6E55CEBEA.aspx
        /// </summary>
        /// <param name="angle">Angle to normalize in [rad].</param>
        /// <returns>Normalized angle in [rad].</returns>
        public static double NormalizeAngle(double angle)
        {
            double ret = angle;
            if (angle > 0)
            {
                double div = (angle + Math.PI) / (2 * Math.PI);
                div = Math.Truncate(div);
                return angle - (div * (2 * Math.PI));
            }
            else
            {
                angle = -angle;

                double div = (angle + Math.PI) / (2 * Math.PI);
                div = Math.Truncate(div);
                return -(angle - (div * (2 * Math.PI)));
            }
        }

        /// <summary>
        /// Converts the given angle to absolute angle. Returns an angle between [0;2PI).
        /// Source: http://www.koders.com/csharp/fid64CC213FA473C179BCBB000AC153D0F6E55CEBEA.aspx
        /// </summary>
        /// <param name="angle">Angle to normalize in [rad].</param>
        /// <returns>Normalized angle in [rad].</returns>
        public static double NormalizeAnglePositive(double angle)
        {
            if (angle > 0)
            {
                double div = angle / (2 * Math.PI);
                div = Math.Truncate(div);
                return angle - (div * (2 * Math.PI));
            }
            else
            {
                angle = -angle;

                double div = angle / (2 * Math.PI);
                div = Math.Truncate(div);
                return (2 * Math.PI) - (angle - (div * (2 * Math.PI)));
            }
        }

        /// <summary>
        /// Normalizes any number to an arbitrary range 
        /// by assuming the range wraps around when going below min or above max.
        /// Source: http://stackoverflow.com/questions/1628386/normalise-orientation-between-0-and-360
        /// </summary>
        /// <param name="value">Value to be normalized.</param>
        /// <param name="intervalStart">Beginning of the goal interval.</param>
        /// <param name="intervalEnd">End of the goal interval.</param>
        /// <returns>Normalized value of <paramref name="value"/>.</returns>
        public static double GenericNormalizer(double value, double intervalStart, double intervalEnd)
        {
            double width = intervalEnd - intervalStart;   // 
            double offsetValue = value - intervalStart;   // value relative to 0

            return (offsetValue - (Math.Floor(offsetValue / width) * width)) + intervalStart;
            // + start to reset back to start of original range
        }

        #region Obsolete

        /// <summary>
        /// Converts the given angle to an angle
        /// that represent the same rotation, but is always in the
        /// [-PI;PI) range (would be -180 - 180 in degrees).
        /// Source: http://www.java2s.com/Code/Java/Development-Class/Normalizesanangletoarelativeangle.htm
        /// </summary>
        /// <param name="angle">The input angle, in radians.</param>
        /// <returns>The normalized angle.</returns>
        [Obsolete]
        public static double NormalizeAngleDepr(double angle)
        {
            return (angle %= 2 * Math.PI) >= 0 ? (angle < Math.PI) ? angle : angle - 2 * Math.PI : (angle >= -Math.PI) ? angle : angle + 2 * Math.PI;
        }

        /// <summary>
        /// Converts the given angle to an angle
        /// that represent the same rotation, but is always in the
        /// [0;2*PI) range (would be 0 - 360 in degrees).
        /// Source: http://www.java2s.com/Code/Java/Development-Class/Normalizesanangletoarelativeangle.htm
        /// </summary>
        /// <param name="angle">The input angle, in radians.</param>
        /// <returns>The normalized angle.</returns>
        [Obsolete]
        public static double NormalizeAnglePositiveDepr(double angle)
        {
            return (angle %= 2 * Math.PI) >= 0 ? angle : (angle + 2 * Math.PI);
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonTools.Miscellaneous
{
    /// <summary>
    /// Contains frequently used mathematical functions.
    /// </summary>
    public static class MathUtil
    {
        private static Random rand = new Random();

        /// <summary>
        /// Returns the value of a two dimensional Gaussian distirbution in the specified point.
        /// </summary>
        /// <param name="xmean">The x coordinate of the mean.</param>
        /// <param name="ymean">The y coordinate of the mean.</param>
        /// <param name="sigmax">The standard deviation on x axis.</param>
        /// <param name="sigmay">The standard deviation on y axis.</param>
        /// <param name="x">The x coordinate of the specified point.</param>
        /// <param name="y">The y coordinate of the specified point.</param>
        /// <returns>The value of the Gaussian distribution in the specified point.</returns>
        public static double GaussValue(double xmean, double ymean, double sigmax, double sigmay, double x, double y)
        {
            return Math.Exp(-(Math.Pow(xmean - x, 2) / Math.Pow(sigmax, 2) + Math.Pow(ymean - y, 2) / Math.Pow(sigmay, 2)) / 2.0);
        }

        /// <summary>
        /// Returns a random value around the specified mean that follows a Gaussian distribution.
        /// Source: http://stackoverflow.com/questions/218060/random-gaussian-variables
        /// </summary>
        /// <param name="mean">Mean of the distribution.</param>
        /// <param name="sigma">The standard deviation of the distribution.</param>
        /// <returns>Random value around the mean.</returns>
        public static double RandomGauss(double mean, double sigma)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + sigma * randStdNormal; //random normal(mean,sigma^2)
            return randNormal;
        }

        
    }
}

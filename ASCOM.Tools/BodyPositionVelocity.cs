using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Tools
{
    /// <summary>
    /// Position and velocity vector for a solar system object
    /// </summary>
    public class BodyPositionVelocity
    {
        /// <summary>
        /// Set the object using individual parameter values
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="vx">X Velocity</param>
        /// <param name="vy">Y Velocity</param>
        /// <param name="vz">Z Velocity</param>
        public BodyPositionVelocity(double x, double y, double z, double vx, double vy, double vz)
        {
            X = x;
            Y = y;
            Z = z;

            VelocityX = vx;
            VelocityY = vy;
            VelocityZ = vz;
        }

        /// <summary>
        /// Set the object using position and velocity vector arrays
        /// </summary>
        /// <param name="position">X, Y and Z coordinates as a three element 1D array</param>
        /// <param name="velocity">X, Y and Z velocities as a three element 1D array</param>
        public BodyPositionVelocity(double[] position, double[] velocity)
        {
            X = position[0];
            Y = position[1];
            Z = position[2];

            VelocityX = velocity[0];
            VelocityY = velocity[1];
            VelocityZ = velocity[2];
        }

        /// <summary>
        /// X Coordinate
        /// </summary>
        public double X { get; } = 0.0;

        /// <summary>
        /// Y coordinate
        /// </summary>
        public double Y { get; } = 0.0;

        /// <summary>
        /// Z Coordinate
        /// </summary>
        public double Z { get; } = 0.0;

        /// <summary>
        /// Distance, calculated from the X, Y and Z coordinates
        /// </summary>
        public double Distance
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        /// <summary>
        /// Velocity X component
        /// </summary>
        public double VelocityX { get; } = 0.0;

        /// <summary>
        /// Velocity Y component
        /// </summary>
        public double VelocityY { get; } = 0.0;

        /// <summary>
        /// Velocity Z component
        /// </summary>
        public double VelocityZ { get; } = 0.0;
    }
}

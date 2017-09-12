﻿using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    // acknowledgement: 
    // part of this implementation is based on Richard Potter's Vector3 library
    // which contains adapted codebase from Lucas Viñas Livschitz's CSOpenGL library
    public class Vector : IVector
    {
        
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }
        
        public double DistanceFrom(IVector other)
        {
            return
                Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        public IVector Plus(IVector other)
        {
            return new Vector(X + other.X, Y + other.Y);
        }

        public IVector Minus(IVector other)
        {
            return new Vector(X - other.X, Y - other.Y);
        }

        public IVector MultiplyByScalar(double scalar)
        {
            return new Vector(scalar * X, scalar * Y);
        }

        public double X { get; private set; }
        
        public double Y { get; private set; }

        public bool IsInCounterClockWiseDirectionFrom(IVector other)
        {
             return ZComponentOfCrossProductWith(other) < 0;
        }

        public bool IsInClockWiseDirectionFrom(IVector other)
        {
            return ZComponentOfCrossProductWith(other) > 0;
        }

        private double ZComponentOfCrossProductWith(IVector other)
        {
            return X * other.Y - Y * other.X;
        }

        public override bool Equals(object obj)
        {
            Vector other = obj as Vector;
            if (other != null)
            {
                return X.Equals(other.X) && Y.Equals(other.Y); // TODO: add precision handling
            }
            else
            {
                return false;
            }
        }
 
        public override int GetHashCode()
        {
            return (int) ((X + Y) % Int32.MaxValue);
        }
        
    }
}
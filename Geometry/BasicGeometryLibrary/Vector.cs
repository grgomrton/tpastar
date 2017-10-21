using System;

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

        public double X
        {
            get; 
            private set;
        }

        public double Y
        {
            get; 
            private set;
        }
        
        public IVector Plus(IVector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return new Vector(X + other.X, Y + other.Y);
        }

        public IVector Minus(IVector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return new Vector(X - other.X, Y - other.Y);
        }

        public IVector Times(double scalar)
        {
            return new Vector(scalar * X, scalar * Y);
        }
        
        public double DistanceFrom(IVector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return other.Minus(this).Length();
        }

        public bool IsInCounterClockWiseDirectionFrom(IVector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return ZComponentOfCrossProductWith(other) <= 0.0;
        }

        public bool IsInClockWiseDirectionFrom(IVector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return ZComponentOfCrossProductWith(other) >= 0.0;
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            Vector otherVector = other as Vector;
            if (otherVector != null)
            {
                return otherVector.DistanceFrom(this) < VectorEqualityCheck.Tolerance;
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

        public override string ToString()
        {
            return String.Format("({0:0.00}, {1:0.00})", X, Y);
        }
        
        private double ZComponentOfCrossProductWith(IVector other)
        {
            return X * other.Y - Y * other.X;
        }
        
    }
    
    public static class VectorExtensions
    {
        
        public static double DotProduct(this IVector a, IVector b)
        {
            return a.X * b.X +
                   a.Y * b.Y;
        }

        public static double Length(this IVector a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }
        
    }
}
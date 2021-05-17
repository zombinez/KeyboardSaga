﻿using System;

namespace KeyboardSagaGame.Classes
{
    public class Vector
	{
		public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
		public double Angle { get { return Math.Atan2(Y, X); } }
		public readonly double X;
		public readonly double Y;

		public Vector(double x, double y)
		{
			X = x;
			Y = y;
		}

		protected bool Equals(Vector other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Vector)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.X - b.X, a.Y - b.Y);
		}

		public static Vector operator *(Vector a, double k)
		{
			return new Vector(a.X * k, a.Y * k);
		}

		public static Vector operator /(Vector a, double k)
		{
			return new Vector(a.X / k, a.Y / k);
		}

		public static Vector operator *(double k, Vector a)
		{
			return a * k;
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.X + b.X, a.Y + b.Y);
		}

		public Vector Normalize()
		{
			return Length > 0 ? this * (1 / Length) : this;
		}

		public Vector Rotate(double angle)
		{
			return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
		}
	}
}
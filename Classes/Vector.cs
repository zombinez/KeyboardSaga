using System;

namespace KeyboardSagaGame.Classes
{
    public class Vector
	{
		public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
		public readonly double X;
		public readonly double Y;

		public Vector(double x, double y)
		{
			X = x;
			Y = y;
		}

		protected bool Equals(Vector other) => X.Equals(other.X) && Y.Equals(other.Y);

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

		public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);

		public static Vector operator *(Vector a, double k) => new Vector(a.X * k, a.Y * k);

		public static Vector operator /(Vector a, double k) => new Vector(a.X / k, a.Y / k);

		public static Vector operator *(double k, Vector a) => a * k;

		public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);

		public Vector Normalize() => Length > 0 ? this * (1 / Length) : this;
	}
}
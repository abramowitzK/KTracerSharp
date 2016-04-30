using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public class BoundingSphere{
		public Vector3 Pos { get; set; }
		private float m_radius;
		public float Radius {
			get {
				return m_radius;
			}
			set {
				m_radius = value;
				RadiusSquared = Radius * Radius;
			}
		}
		private float RadiusSquared {
			get; set;
		}

		public BoundingSphere(Vector3 center, float radius) {
			Radius = radius;
			Pos = center;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Intersect(Ray ray) {
			var xo = ray.Start.X;
			var yo = ray.Start.Y;
			var zo = ray.Start.Z;
			var xd = ray.Dir.X;
			var yd = ray.Dir.Y;
			var zd = ray.Dir.Z;
			//always equals 1 since this is a normalized vector
			//double a = 1.0f;// xd*xd + yd*yd + zd*zd;
			var b = 2.0f * (xd * (xo - Pos.X) + yd * (yo - Pos.Y) + zd * (zo - Pos.Z));
			var c = (float)(Math.Pow(xo - Pos.X, 2.0) + Math.Pow(yo - Pos.Y, 2.0) + Math.Pow(zo - Pos.Z, 2.0) - RadiusSquared);
			var descrim = b * b - 4 * c;
			const float epsilon = 0.00000000001f;
			if (descrim < epsilon) {
				return false;
			}
			var t0 = (-b - (float)Math.Sqrt(b * b - 4 * c)) / 2.0f;
			var t1 = (-b + (float)Math.Sqrt(b * b - 4 * c)) / 2.0f;
			if (t0 < epsilon) {
				if (t1 < epsilon) {
					return false;
				}
			}
			return true;
		}
	}
}

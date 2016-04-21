using System;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Audio;

namespace KTracerSharp {
	public class Sphere : RenderObject {
		public Sphere(Vector3 pos, Quaternion rotation, float scale, Vector4 color, float radius)
			: base(pos, rotation, scale, color) {
			Radius = radius;
			RadiusSquared = radius*radius;
		}

		private float m_radius;
		public float Radius {
			get { return m_radius; }
			set {
				m_radius = value;
				RadiusSquared = Radius*Radius;
			}
		}
		private float RadiusSquared { get; set;}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Intersect(Ray ray, ref float tMin, ref Vector3 intPoint, ref Vector3 normal) {
			var xo = ray.Start.X;
			var yo = ray.Start.Y;
			var zo = ray.Start.Z;
			var xd = ray.Dir.X;
			var yd = ray.Dir.Y;
			var zd = ray.Dir.Z;
			//always equals 1 since this is a normalized vector
			//double a = 1.0f;// xd*xd + yd*yd + zd*zd;
			var b = 2.0f*(xd*(xo - Pos.X) + yd*(yo - Pos.Y) + zd*(zo - Pos.Z));
			var c = (float) (Math.Pow(xo - Pos.X, 2.0) + Math.Pow(yo - Pos.Y, 2.0) + Math.Pow(zo - Pos.Z, 2.0) - RadiusSquared);
			var descrim = b*b - 4*c;
			const float epsilon = 0.00000000001f;
			if (descrim < epsilon) {
				return false;
			}
			var t0 = (-b - (float) Math.Sqrt(b*b - 4*c))/2.0f;
			var t1 = (-b + (float) Math.Sqrt(b*b - 4*c))/2.0f;
			if (t0 < epsilon) {
				if (t1 < epsilon) {
					return false;
				}
				tMin = t1;
				intPoint = ray.Start + tMin*ray.Dir;
				normal = intPoint - Pos;
				return true;
			}
			tMin = t1 > epsilon ? Math.Min(t1, t0) : t0;
			intPoint = ray.Start + tMin*ray.Dir;
			normal = intPoint - Pos;

			return true;
		}

		public override void Rotate(float x, float y, float z) {
			throw new NotImplementedException();
		}

		public override void Translate(float x, float y, float z) {
			Pos += new Vector3(x, y, z);
		}

		public override void UniformScale(float s) {
			Radius = Radius*s;
		}

		public override ObjectType GetObjectType() {
			return ObjectType.Sphere;
		}
	}
}

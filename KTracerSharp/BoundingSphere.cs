using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public class BoundingSphere{
		public BoundingSphere Parent { get; set; }
		public BoundingSphere LeftChild { get; set; }
		public BoundingSphere RightChild { get; set; }
		public List<RenderObject> Objects;
		public List<Triangle> Triangles; 
		public int NumObjects { get; set; }
		public int NumTriangles { get; set; }
		public Vector3 Pos { get; set; }
		private float m_radius;

		public static BoundingSphere ConstructBoundingSphereFromList(List<RenderObject> list) {
			var maxx = float.MinValue;
			var minx = float.MaxValue;
			var maxy = float.MinValue;
			var miny = float.MaxValue;
			var maxz = float.MinValue;
			var minz = float.MaxValue;
			//Calculate the points on the bounding box;
			foreach (var obj in list) {
				float temp;
				if ((temp = obj.BoundingBox.Pos.X + obj.BoundingBox.Radius) > maxx)
					maxx = temp;
				if ((temp = obj.BoundingBox.Pos.X - obj.BoundingBox.Radius) < minx)
					minx = temp;
				if ((temp = obj.BoundingBox.Pos.Y + obj.BoundingBox.Radius) > maxy)
					maxy = temp;
				if ((temp = obj.BoundingBox.Pos.Y - obj.BoundingBox.Radius) < miny)
					miny = temp;
				if ((temp = obj.BoundingBox.Pos.Z + obj.BoundingBox.Radius) > maxz)
					maxz = temp;
				if ((temp = obj.BoundingBox.Pos.Z - obj.BoundingBox.Radius) < minz)
					minz = temp;
			}
			var center = new Vector3((maxx + minx) / 2.0f, (maxy + miny) / 2.0f, (maxz + minz) / 2.0f);
			var radius = (new Vector3((maxx - minx)/2.0f, (maxy-miny)/2.0f,(maxz - minz)/2.0f) - center).LengthSquared;
			radius = (float)Math.Sqrt(radius);
			var b = new BoundingSphere(center, radius);
			b.Objects = new List<RenderObject>(list);
			return b;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BoundingSphere ConstructFromTriangles(List<Triangle> list) {

			var maxx = float.MinValue;
			var minx = float.MaxValue;
			var maxy = float.MinValue;
			var miny = float.MaxValue;
			var maxz = float.MinValue;
			var minz = float.MaxValue;
			//Calculate the points on the bounding box;
			 var verts = new List<Vertex>(new Vertex[list.Count*3]);
			for (int i = 0; i < list.Count*3; i += 3) {
				var f = i/3;
				verts[i] = list[f].V1;
				verts[i + 1] = list[f].V2;
				verts[i + 2] = list[f].V3;
			}
			foreach (var p in verts) {
				if (p.Point.X > maxx)
					maxx = p.Point.X;
				else if (p.Point.X < minx)
					minx = p.Point.X;
				if (p.Point.Y > maxy)
					maxy = p.Point.Y;
				else if (p.Point.Y < miny)
					miny = p.Point.Y;
				if (p.Point.Z > maxz)
					maxz = p.Point.Z;
				else if (p.Point.Z < minz)
					minz = p.Point.Z;
			}
			var center = new Vector3((maxx + minx) / 2.0f, (maxy + miny) / 2.0f, (maxz + minz) / 2.0f);
			var radius = float.MinValue;
			foreach (var p in verts) {
				var temp = (p.Point - center).LengthSquared;
				if (temp > radius)
					radius = temp;
			}
			radius = (float)Math.Sqrt(radius);
			var b = new BoundingSphere(center, radius);
			b.Triangles = new List<Triangle>(list);
			return b;
		}

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

using System;
using System.Collections.Generic;
using OpenTK;

namespace KTracerSharp {
	public class TriangleMesh : RenderObject {
		private readonly List<int> m_indices;

		private readonly List<Vector3> m_vertices;

		public TriangleMesh(Vector3 pos, Quaternion rotation, float scale, Vector4 color, List<Vector3> vertices,
			List<int> indices) : base(pos, rotation, scale, color) {
			m_vertices = vertices;
			m_indices = indices;
		}

		public TriangleMesh(List<Vector3> vertices, List<int> indices)
			: base(Vector3.Zero, Quaternion.Identity, 1.0f, new Vector4(0, 0, 1.0f, 1.0f)) {
			m_vertices = vertices;
			m_indices = indices;
		}

		public override bool Intersect(Ray ray, ref float tMin, out Vector3 intPoint, out Vector3 normal) {
			var tmin = 1000000f;
			Vector3 intPoint2 = Vector3.Zero;
			Vector3 normal2 = Vector3.Zero;
			for (var i = 0; i < m_indices.Count; i += 3) {
				var v1 = m_vertices[m_indices[i]];
				var v2 = m_vertices[m_indices[i + 1]];
				var v3 = m_vertices[m_indices[i + 2]];
				float t;

				if (RayIntersectsTriangle(ray, v1, v2, v3, out t, out intPoint2, out normal2)) {
					if (t < tmin) {
						tmin = t;
					}
				}
			}
			if (tmin < 100000)
				tMin = tmin;
			intPoint = intPoint2;
			normal = normal2;
			return tmin < 1000000 && tmin > 0;
		}

		public override void Rotate(float x, float y, float z) {
			var m = new Quaternion(x,y,z);
			for (var i = 0; i < m_vertices.Count; i++) {
				m_vertices[i] = Vector3.Transform(m_vertices[i], m);
			}
		}

		public override void Translate(float x, float y, float z) {
			var m = Matrix4.CreateTranslation(x, y, z);
			for (var i = 0; i < m_vertices.Count; i++) {
				m_vertices[i] = Vector3.TransformPosition(m_vertices[i], m);
			}
		}

		public override ObjectType GetObjectType() {
			return ObjectType.TriangleMesh;
		}

		private bool RayIntersectsTriangle(Ray ray, Vector3 v1, Vector3 v2, Vector3 v3, out float t, out Vector3 intPoint,
			out Vector3 normal) {
			//Moller-Trumbore algorithm
			//https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
			var p = ray.Start;
			var d = ray.Dir;
			var e1 = v2 - v1;
			var e2 = v3 - v1;
			const float epsilon = 0.00000001f;
			var h = Vector3.Cross(d, e2);
			var a = Vector3.Dot(e1, h);
			if (a > -epsilon && a < epsilon) {
				t = float.MaxValue;
				intPoint = Vector3.Zero;
				normal = Vector3.Zero;
				return false;
			}
			var f = 1.0f/a;
			var s = p - v1;
			var u = Vector3.Dot(s, h)*f;
			if (u < 0.0 || u > 1.0) {
				t = float.MaxValue;
				intPoint = Vector3.Zero;
				normal = Vector3.Zero;
				return false;
			}
			var q = Vector3.Cross(s, e1);
			var v = Vector3.Dot(d, q)*f;
			if (v < 0.0 || u + v > 1.0) {
				t = float.MaxValue;
				intPoint = Vector3.Zero;
				normal = Vector3.Zero;
				return false;
			}
			t = Vector3.Dot(e2, q)*f;
			intPoint = Vector3.Zero;
			normal = Vector3.Zero;
			return !(t < 0);
		}
	}
}
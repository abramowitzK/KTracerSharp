using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OpenTK;

namespace KTracerSharp {
	public class Vertex {
		public Vector3 Point;
		public Vector3 Normal;
		public Vector4 Color;
		public Vector2 TexCoords;

		public Vertex(Vector3 p, Vector3 n) {
			Point = p;
			Normal = n;
		}
	}

	public class TriangleMesh : RenderObject {
		private readonly List<int> m_indices;
		private readonly List<Vector3> m_points;
		private readonly List<Vector3> m_normals;
		private readonly List<Vertex> m_vertices;
		public TriangleMesh(Vector3 pos, Quaternion rotation, float scale, Vector4 color, List<Vector3> points,
			List<int> indices) : base(pos, rotation, scale, color) {
			m_points = points;
			m_indices = indices;

		}

		public TriangleMesh(List<Vector3> points, List<int> indices, List<Vector3> normals )
			: base(Vector3.Zero, Quaternion.Identity, 1.0f, new Vector4(0, 0, 1.0f, 1.0f)) {
			m_points = points;
			m_indices = indices;
			m_normals = normals;
			m_vertices = new List<Vertex>(points.Count);
			for(var i = 0; i < m_points.Count; i++) {
				m_vertices.Add(new Vertex(m_points[i], m_normals[i]));
			}
		}

		public TriangleMesh(List<Vertex> verts, List<int> indices) : base(Vector3.Zero, Quaternion.Identity, 1.0f, new Vector4(0, 0, 1.0f, 1.0f)) {
			m_vertices = verts;
			m_indices = indices;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Intersect(Ray ray, ref float tMin, ref Vector3 intPoint, ref Vector3 normal) {
			var start = ray.Start;
			var dir = ray.Dir;
			for (var i = 0; i < m_indices.Count; i+=3) {
				var v1 = m_vertices[i];
				var v2 = m_vertices[i+1];
				var v3 = m_vertices[i + 2];
				float t = 0;
				if (RayIntersectsTriangle(ref start, ref dir, v1,  v2,  v3, ref t, ref intPoint, ref normal)) {
					if (t < tMin) {
						tMin = t;
					}
				}
			}
			return tMin < 1000000.0f;
		}

		public override void Rotate(float x, float y, float z) {
			var m = new Quaternion(x,y,z);
			for (var i = 0; i < m_points.Count; i++) {
				m_points[i] = Vector3.Transform(m_points[i], m);
			}
		}

		public override void Translate(float x, float y, float z) {
			var m = Matrix4.CreateTranslation(x, y, z);
			for (var i = 0; i < m_points.Count; i++) {
				m_points[i] = Vector3.TransformPosition(m_points[i], m);
			}
		}

		public override ObjectType GetObjectType() {
			return ObjectType.TriangleMesh;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RayIntersectsTriangle(ref Vector3 p, ref Vector3 d, Vertex v1,  Vertex v2,  Vertex v3, ref float t, ref Vector3 intPoint,
			ref Vector3 normal) {
			//Moller-Trumbore algorithm
			//https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
			var e1 = v2.Point - v1.Point;
			var e2 = v3.Point - v1.Point;
			const float epsilon = 0.00000001f;
			var h = Vector3.Cross(d, e2);
			var a = Vector3.Dot(e1, h);
			if (a < epsilon && a > -epsilon) {
				return false;
			}
			a = 1.0f/a;
			var s = p - v1.Point;
			var u = Vector3.Dot(s, h)*a;
			if (u < 0.0 || u > 1.0) {
				return false;
			}
			h = Vector3.Cross(s, e1);
			var v = Vector3.Dot(d, h)*a;
			if (v < 0.0 || u + v > 1.0) {
				return false;
			}
			t = Vector3.Dot(e2, h)*a;
			if (t > 0) {
				intPoint = p + d * t;
				//normal = (1 - u - v) * v1.Normal + u*v2.Normal + v*v3.Normal;
				normal = v1.Normal;
				normal.Normalize();
				return true;
			}
			return false;
		}
	}
}
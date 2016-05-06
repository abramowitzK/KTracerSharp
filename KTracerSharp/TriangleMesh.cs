using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using OpenTK;

namespace KTracerSharp {
	public class Triangle {
		public Vertex V1;
		public Vertex V2;
		public Vertex V3;
		public TriangleMesh Parent;
		public Triangle(Vertex v1, Vertex v2, Vertex v3, TriangleMesh parent) {
			V1 = v1;
			V2 = v2;
			V3 = v3;
			Parent = parent;
		}
	}

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
		private List<Triangle> m_triangles;

		public List<int> GetIndices() {
			return m_indices;
		}

		public List<Vertex> GetVertices() {
			return m_vertices;
		}

		public List<Triangle> GetTriangles() {
			return m_triangles;
		}

		public TriangleMesh(TriangleMesh mesh) : base(Vector3.Zero, Quaternion.Identity, 1.0f, new Vector4(0, 0, 1.0f, 1.0f)) {
			m_vertices = new List<Vertex>(mesh.GetVertices());
			m_indices = new List<int>(mesh.GetIndices());
			m_triangles = new List<Triangle>(new Triangle[m_indices.Count / 3]);
		}

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
			m_triangles = new List<Triangle>( new Triangle[m_indices.Count/3]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Intersect(Ray ray, ref float tMin, ref Vector3 intPoint, ref Vector3 normal) {
			var start = ray.Start;
			var dir = ray.Dir;
			bool hit = false;
			for (var i = 0; i < m_indices.Count; i += 3) {
				var v1 = m_vertices[m_indices[i]];
				var v2 = m_vertices[m_indices[i + 1]];
				var v3 = m_vertices[m_indices[i + 2]];
				if (RayIntersectsTriangle(ref start, ref dir, v1, v2, v3, ref tMin, ref intPoint, ref normal)) {
					hit = true;
				}
			}
			return hit;
		}

		public override void Rotate(float x, float y, float z) {
			var m = new Quaternion(x,y,z);
			for (var i = 0; i < m_vertices.Count; i++) {
				m_vertices[i].Point = Vector3.Transform(m_vertices[i].Point, m);
			}
		}

		public override void Translate(float x, float y, float z) {
			var m = Matrix4.CreateTranslation(x, y, z);
			for (var i = 0; i < m_vertices.Count; i++) {
				m_vertices[i].Point = Vector3.TransformPosition(m_vertices[i].Point, m);
			}
		}

		public override void UniformScale(float s) {
			var m = Matrix4.CreateScale(s);
			for (var i = 0; i < m_vertices.Count; i++) {
				m_vertices[i].Point = Vector3.Transform(m_vertices[i].Point, m);
			}
		}
		//This is not the most optimal sphere but calculating the optimal one is more difficult than I anticipated...
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void CalculateBoundingSphere() {
			var maxx = float.MinValue;
			var minx = float.MaxValue;
			var maxy = float.MinValue;
			var miny = float.MaxValue;
			var maxz = float.MinValue;
			var minz = float.MaxValue;
			//Calculate the points on the bounding box;
			foreach (var p in m_vertices) {
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
			var center = new Vector3((maxx + minx)/2.0f, (maxy+miny)/2.0f, (maxz + minz)/2.0f);
			var radius = float.MinValue;
			foreach (var p in m_vertices) {
				var temp = (p.Point - center).LengthSquared;
				if (temp > radius)
					radius = temp;
			}
			radius = (float)Math.Sqrt(radius);
			BoundingBox = new BoundingSphere(center, radius);
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
			var temp = Vector3.Dot(e2, h)*a;
			if (temp > 0 && temp < t) {
				t = temp;
				intPoint = p + (d * t);
				normal = (1 - u - v) * v1.Normal + u*v2.Normal + v*v3.Normal;
				return true;
			}
			return false;
		}

		public void GenerateTriangles() {
			for (int i = 0; i < m_indices.Count; i+=3) {
				int f = i/3;
				m_triangles[f] = new Triangle(m_vertices[m_indices[i]], m_vertices[m_indices[i+1]], m_vertices[m_indices[i+2]], this);
			}
		}
	}
}
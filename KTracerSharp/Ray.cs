using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KTracerSharp {
	internal class HitInfo {
		public readonly Vector3 Normal;
		public readonly Vector3 Intersect;
		public readonly float Tvalue;
		public HitInfo(Vector3 n, Vector3 i, float t) {
			Normal = n;
			Intersect = i;
			Tvalue = t;
		}
	}

	public class Ray {
		public Ray(Vector3 dir, Vector3 start) {
			Dir = dir;
			Start = start;
		}

		public Vector3 Dir { get; set; }

		public Vector3 Start { get; set; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IntersectWithListOfTriangle(Ray ray, List<Triangle> tris, ref float tMin, ref Vector3 intPoint, ref Vector3 normal) {
			var start = ray.Start;
			var dir = ray.Dir;
			foreach (var tri in tris) {
				float t = float.MaxValue;
				Vector3 intp = Vector3.Zero;
				Vector3 norm = Vector3.Zero;
				if (RayIntersectsTriangle(ref start, ref dir, tri.V1, tri.V2, tri.V3, ref t, ref intp, ref norm)) {
					if (t < tMin) {
						tMin = t;
						intPoint = intp;
						normal = norm;
					}
				}
			}
			return tMin < 1000000.0f && tMin > 0;
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IntersectBVH(BoundingSphere parent, ref float tmin, ref Vector3 inter, ref Vector3 norm, ref RenderObject closest) {
			if (parent.Intersect(this)) {
				//There should only ever be 0 or 2 children
				if (parent.LeftChild != null && parent.RightChild != null) {
					bool leftHit = false;
					bool rightHit = false;
					RenderObject temp = null;
					float t = float.MaxValue;
					Vector3 tn = Vector3.One;
					Vector3 tp = Vector3.One;
					leftHit = IntersectBVH(parent.LeftChild, ref tmin, ref inter, ref norm, ref closest);
					rightHit = IntersectBVH(parent.RightChild, ref t, ref tp, ref tn, ref temp);
					if (t < tmin) {
						tmin = t;
						closest = temp;
						inter = tp;
						norm = tn;
					}
					return leftHit || rightHit;
				}
				else {
					// this means both were null so this is a single node
					if (parent.Object != null) {
						if (parent.Object.Intersect(this, ref tmin, ref inter, ref norm)) {
							closest = parent.Object;
							return true;
						}
					} else {
						if (IntersectWithListOfTriangle(this, parent.Triangles, ref tmin, ref inter, ref norm)) {
							closest = parent.Triangles[0].Parent;
							return true;
						}
					}
				}
			}//We didn't hit anything
			return false;

		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4 Trace(Scene s, int d) {
			try {
				var tmin = float.MaxValue;
				var inter = Vector3.Zero;
				var norm = Vector3.Zero;
				RenderObject closestObj = null;
				IntersectBVH(s.Root, ref tmin, ref inter, ref norm, ref closestObj);
				if (closestObj != null) {
					var col = Vector4.Zero;
					var pHit = inter;
					var nHit = norm;
					nHit.Normalize();
					foreach (var l in s.Lights) {
						var dir = l.Pos - (pHit + (nHit*0.002f));
						dir.Normalize();
						var lightRay = new Ray(dir, Vector3.Add(pHit, nHit*0.002f));
						var blocked = false;
						float t = float.MaxValue;
						RenderObject closest = null;
						blocked = lightRay.IntersectBVH(s.Root, ref t, ref inter, ref norm, ref closest);
						if (blocked) {
							//Need to also handle this case : (object1) (Light)  (object2). Should not block light one object 2 unless obj1 is in between light and object2;
							var light = l.Pos - lightRay.Start;
							var intRay = inter - pHit;
							if (light.Length > intRay.Length) {

							}
							else
								blocked = false;
						}
						if (!blocked) {
							var lightDir = l.Pos - lightRay.Start;
							var attenuation = 1.0f/lightDir.LengthSquared;
							var v = Start - pHit;
							var h = (lightDir + v).Normalized();
							var lD = l.Intensity*attenuation*Math.Max(0, Vector3.Dot(nHit, lightDir));
							var spec =
								(float) (l.Intensity*attenuation*(Math.Pow(Math.Max(0, Vector3.Dot(nHit, h)), closestObj.Mat.Shinyness)));
							col += lD*closestObj.Mat.DiffuseColor*closestObj.Mat.KD + spec * closestObj.Mat.SpecularColor * closestObj.Mat.KS + closestObj.Mat.AmbientColor * closestObj.Mat.KA;
							if (closestObj.Mat.MType == MaterialType.Reflective && d > 0) {
								var c1 = -Vector3.Dot(norm, this.Dir);
								var reflect = this.Dir + (2*norm*c1);
								col += closestObj.Mat.KR*new Ray(reflect, pHit+(norm*0.002f)).Trace(s, d-1);
							}
						}
						else {
							col = Vector4.Add(col, (new Vector4(0.0f, 0.0f, 0.0f, 1.0f)));
						}
					}
					return col + closestObj.Mat.AmbientColor*closestObj.Mat.KA;
				}
				
			}
			catch (NullReferenceException e) {
				Console.WriteLine(e.ToString());
			}
			return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
		}
	}
}

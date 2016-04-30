using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenTK;

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

		public Vector4 Trace(Scene s, int d) {
			try {
				var tmin = float.MaxValue;
				var closestTmin = 1e6f;
				var inter = Vector3.Zero; //not using these yet.
				var norm = Vector3.Zero;
				RenderObject closestObj = null;
				HitInfo closestHitInfo = null;

				for (var i = 0; i < s.Objects.Count; i++) {
					if (s.Objects[i].BoundingBox.Intersect(this)) {
						if (s.Objects[i].Intersect(this, ref tmin, ref inter, ref norm)) {
							if (tmin < closestTmin) {
								closestObj = s.Objects[i];
								closestTmin = tmin;
								closestHitInfo = new HitInfo(new Vector3(norm), new Vector3(inter), tmin);
							}
						}
					}
				}
				if (closestObj != null) {
					var col = Vector4.Zero;
					var pHit = closestHitInfo.Intersect;
					var nHit = closestHitInfo.Normal;
					nHit.Normalize();
					foreach (var l in s.Lights) {
						var dir = l.Pos - (pHit + (nHit*0.02f));
						dir.Normalize();
						var lightRay = new Ray(dir, Vector3.Add(pHit, nHit*0.02f));
						var blocked = false;
						float t = float.MaxValue;
						foreach (var o in s.Objects) {
							/*blocked = o.Intersect(lightRay, ref t, ref inter, ref norm);
						if (blocked) {
							//Need to also handle this case : (object1) (Light)  (object2). Should not block light one object 2 unless obj1 is in between light and object2;
							var light = l.Pos - lightRay.Start;
							var intRay = inter - pHit;
							if (light.Length > intRay.Length) {
								//Object is between light and hit point
								break;
							}
							blocked = false;
						}*/
						}
						if (!blocked) {
							var lightDir = l.Pos - lightRay.Start;
							var attenuation = 1.0f/lightDir.LengthSquared;
							var v = Start - pHit;
							var h = (lightDir + v).Normalized();
							var lD = l.Intensity*attenuation*Math.Max(0, Vector3.Dot(nHit, lightDir));
							var spec =
								(float) (l.Intensity*attenuation*(Math.Pow(Math.Max(0, Vector3.Dot(nHit, h)), closestObj.Mat.Shinyness)));
							col += lD*closestObj.Mat.DiffuseColor*closestObj.Mat.KD;
							col += spec*closestObj.Mat.SpecularColor*closestObj.Mat.KS;
							col += closestObj.Mat.AmbientColor*closestObj.Mat.KA;
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

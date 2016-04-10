using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
			if (d == 0)
				return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			var tmin = float.MaxValue;
			var closestTmin = float.MaxValue;
			var inter = Vector3.Zero; //not using these yet.
			var norm = Vector3.Zero;
			RenderObject closestObj = null;
			HitInfo closestHitInfo = null;

			for (var i = 0; i < s.Objects.Count; i++) {
				if (s.Objects[i].Intersect(this, ref tmin, ref inter, ref norm)) {
					if (tmin < closestTmin) {
						closestObj = s.Objects[i];
						closestTmin = tmin;
						closestHitInfo = new HitInfo(new Vector3(norm), new Vector3(inter), tmin);
					}
				}
			}
			if (closestObj != null) {
				var col = Vector4.Zero;
				var pHit = closestHitInfo.Intersect;
				var nHit = closestHitInfo.Normal;
				foreach (var l in s.Lights) {
					var dir = l.Pos - (pHit+nHit*0.1f);
					dir.Normalize();
					var lightRay = new Ray(dir, Vector3.Add(pHit, nHit*0.1f));
					var blocked = false;
					float t = float.MaxValue;
					foreach (var o in s.Objects) {
						blocked = o.Intersect(lightRay, ref t, ref inter, ref norm);
						if (blocked)
							break;
					}
					if (!blocked) {
						var lightDir = l.Pos - lightRay.Start;
						var h = (lightDir + (this.Start - pHit)).Normalized();
						var lD = l.Intensity*Math.Max(0, Vector3.Dot(nHit, lightDir));
						var spec = (float) (l.Intensity*(Math.Pow(Math.Max(0, Vector3.Dot(nHit, h)), 5.0)));
						col  += new Vector4(closestObj.Color*lD);
						col += spec*new Vector4(1f, 1f, 1f, 1f);
					}
					else {
						col = Vector4.Add(col, (new Vector4(0.0f, 0.0f, 0.0f, 1.0f)));
					}
				}
				return col + s.AmbientColor;
			} 
			return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
		}
	}
}
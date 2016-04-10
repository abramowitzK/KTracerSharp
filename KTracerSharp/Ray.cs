using System;
using System.Linq;
using OpenTK;

namespace KTracerSharp {
	public class Ray {
		public Ray(Vector3 dir, Vector3 start) {
			Dir = dir;
			Start = start;
		}

		public Vector3 Dir { get; set; }

		public Vector3 Start { get; set; }

		public Vector4 Trace(Scene s, int d) {
			if(d == 0)
				return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			var tmin = float.MaxValue;
			var closestTmin = float.MaxValue;
			var inter = Vector3.Zero; //not using these yet.
			var norm = Vector3.Zero;
			RenderObject closestObj = null;
			for (var i = 0; i < s.Objects.Count; i++) {
				if (s.Objects[i].Intersect(this, ref tmin, ref inter, ref norm)) {
					if (tmin < closestTmin) {
						closestObj = s.Objects[i];
						closestTmin = tmin;
					}
				}
			}
			if (closestObj != null) {
				var col = Vector4.Zero;
				//these only work for spheres
				var pHit = Start + Dir*closestTmin;
				var nHit = pHit - closestObj.Pos;
				foreach (var l in s.Lights) {

					var dir = l.Pos - pHit ;
					dir.Normalize();
					var lightRay = new Ray(dir, Vector3.Add(pHit, nHit*0.1f));
					bool blocked = false;
					foreach(var o in s.Objects) {
						blocked = o.Intersect(lightRay, ref tmin, ref inter, ref norm);
					}
					if (!blocked) {
						var lightDir = l.Pos - lightRay.Start;
						var h = (lightDir + (this.Start-pHit)).Normalized();
						float lD = l.Intensity*Math.Max(0, Vector3.Dot(nHit, lightDir));
						float spec = (float)(Math.Pow(Math.Max(0, Vector3.Dot(nHit, h)), 50.0));
						col = Vector4.Add(col, new Vector4(closestObj.Color*lD));
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
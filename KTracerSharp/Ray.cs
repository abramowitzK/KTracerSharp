using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
namespace KTracerSharp {
	public class Ray{

		public Vector3 Dir {
			get; set;
		}

		public Vector3 Start{
			get; set;
		}

		public Ray(Vector3 dir, Vector3 start) {
			Dir = dir;
			Start = start;
		}
	
		public Vector4 Trace(Scene s, int d) {
			float tmin = float.MaxValue;
			float closestTmin = float.MaxValue;
			Vector3 inter = Vector3.Zero;//not using these yet.
			Vector3 norm = Vector3.Zero;
			RenderObject closestObj = null;
			for(var i = 0; i < s.Objects.Count; i++) {
				if (s.Objects[i].Intersect(this, ref tmin, ref inter, ref norm)) {
					if (tmin < closestTmin) {
						closestObj = s.Objects[i];
						closestTmin = tmin;
					}
				}
			}
			if (closestObj != null) {
				return new Vector4(closestObj.Color);
			}
			return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
		}

	}
}

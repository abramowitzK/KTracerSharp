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
			float tmin = 1e6f;
			float closestTmin = float.MaxValue;
			Vector3 inter;//not using these yet.
			Vector3 norm;
			RenderObject closestObj = null;
			foreach (var obj in s.Objects) {
				if (obj.Intersect(this, ref tmin, out inter, out norm)) {
					if (tmin < closestTmin) {
						closestObj = obj;
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

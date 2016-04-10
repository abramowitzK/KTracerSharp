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
				return new Vector4(closestObj.Color);
			}
			return new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
		}
	}
}
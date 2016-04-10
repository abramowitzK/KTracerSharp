using System;
using OpenTK;

namespace KTracerSharp {
	public class Camera {
		public Camera(Vector3 pos, Vector3 up, Vector3 forward, float viewAngle, float distanceToPlane) {
			ViewAngle = ConvertToRadians(viewAngle);
			DistanceToPlane = distanceToPlane;
			Position = pos;
			Up = up.Normalized();
			Forward = forward.Normalized();
			Right = Vector3.Cross(Up, Forward).Normalized();
		}

		public float ConvertToRadians(float angle) {
			return (float)(Math.PI/180.0)*angle;
		}

		public Ray[,] GenerateRays(int hRes, int vRes) {
			var rays = new Ray[hRes,vRes];
			var y = Up;
			var sJ = DistanceToPlane * 2.0f * (float)Math.Tan(  ViewAngle / 2.0f);
			var sK = sJ * (vRes / (float)hRes);
			var posOfPixel = Position + DistanceToPlane * Forward - (float)(sJ / 2.0) * Right + (float)(sK / 2.0) * y;
			for (var i = 0; i < hRes; i++) {
				for (var j = 0; j < vRes; j++) {
					var dir = (posOfPixel + sJ * (i / (float)(hRes - 1)) * Right - sK * (j / (float)(vRes - 1)) * y) - Position;
					dir = dir.Normalized();
					rays[i,j] = new Ray(new Vector3(dir), Position);
				}
			}
			return rays;
		}

		private float ViewAngle {
			get; set; 
			
		}
		private float DistanceToPlane {
			get; set;
		}

		private Vector3 Position {
			get; set; 
			
		}
		private Vector3 Up {
			get; set;
		}
		private Vector3 Right {
			get; set;
		}
		private Vector3 Forward {
			get; set;
		}
	}
}

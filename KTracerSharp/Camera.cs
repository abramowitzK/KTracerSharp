using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	class Camera {
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
			Ray[,] rays = new Ray[hRes,vRes];
			Vector3 y = Up;
			var s_j = DistanceToPlane * 2.0f * (float)Math.Tan(  ViewAngle / 2.0f);
			float s_k = s_j * (vRes / (float)hRes);
			Vector3 posOfPixel = Position + (DistanceToPlane * Forward) - (((float)(s_j / 2.0)) * Right) + (((float)(s_k / 2.0)) * y);
			for (int i = 0; i < hRes; i++) {
				for (int j = 0; j < vRes; j++) {
					Vector3 dir = (posOfPixel + s_j * (i / (float)(hRes - 1)) * Right - s_k * (j / (float)(vRes - 1)) * y) - Position;
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

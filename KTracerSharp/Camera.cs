using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace KTracerSharp {
	public class Camera {
		public static float Tolerance = 0.05f;
		public Vector4 BaseColor;
		public Camera(Vector3 pos, Vector3 up, Vector3 forward, float viewAngle, float distanceToPlane) {
			ViewAngle = ConvertToRadians(viewAngle);
			DistanceToPlane = distanceToPlane;
			Position = pos;
			Up = up.Normalized();
			Forward = forward.Normalized();
			Right = Vector3.Cross(Up, Forward).Normalized();
			BaseColor = new Vector4(0.05f, 0.05f, 0.05f, 1.0f);
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GenerateRays(int hRes, int vRes, ref Image image, Scene s, ref Image intensityImage) {
			var rays = new bool[hRes+1, vRes+1];
			var tempColors = new Vector4[hRes+1, vRes+1];
			var y = Up;
			var sJ = DistanceToPlane * 2.0f * (float)Math.Tan(ViewAngle / 2.0f);
			var sK = sJ * (vRes / (float)hRes);
			var posOfPixel = Position + DistanceToPlane * Forward - (float)(sJ / 2.0) * Right + (float)(sK / 2.0) * y;
			Vector3 dir, dir2, dir3, dir4;
			for (var i = 0; i < hRes; i++) {
				for (var j = 0; j < vRes; j++) {
					int numrays = 0;
					dir = (posOfPixel + sJ * (i / (float)(hRes - 1)) * Right - sK * (j / (float)(vRes - 1)) * y) - Position;
					dir = dir.Normalized();
					if (!rays[i, j]) {
						tempColors[i, j] = new Ray(new Vector3(dir), Position).Trace(s, 2);
						rays[i, j] = true;
					}
					dir2 = (posOfPixel + sJ * ((i + 1) / (float)(hRes - 1)) * Right - sK * (j / (float)(vRes - 1)) * y) - Position;
					dir2 = dir2.Normalized();
					if (!rays[i + 1, j]) {
						tempColors[i+1, j] = new Ray(new Vector3(dir2), Position).Trace(s, 2);
						rays[i + 1, j] = true;
					}
					dir3 = (posOfPixel + sJ * (i / (float)(hRes - 1)) * Right - sK * ((j + 1) / (float)(vRes - 1)) * y) - Position;
					dir3 = dir3.Normalized();
					if (!rays[i, j + 1]) {
						tempColors[i, j+1] = new Ray(new Vector3(dir3), Position).Trace(s, 2);
						rays[i, j + 1] = true;
					}
					dir4 = (posOfPixel + sJ * ((i + 1) / (float)(hRes - 1)) * Right - sK * ((j + 1) / (float)(vRes - 1)) * y) - Position;
					dir4 = dir4.Normalized();
					if (!rays[i + 1, j + 1]) {
						tempColors[i+1, j+1] = new Ray(new Vector3(dir4), Position).Trace(s, 2);
						rays[i + 1, j + 1] = true;
					}
					if (!IsPixelOkay(ref tempColors[i, j], ref tempColors[i + 1, j], ref tempColors[i + 1, j + 1], ref tempColors[i, j + 1])) {
						image.Set(i,j, SubdividePixel(ref numrays, s, 2, ref tempColors[i, j], ref tempColors[i + 1, j], ref tempColors[i + 1, j + 1], ref tempColors[i, j + 1], ref dir, ref dir2, ref dir3, ref dir4));
						intensityImage.Set(i, j, intensityImage.Get(i, j) + BaseColor*numrays);
					}
					else { //No tolerance issues. just avg the pixels
						image.Set(i,j, AverageVector4(ref tempColors[i,j], ref tempColors[i+1, j], ref tempColors[i+1, j+1],ref tempColors[i, j+1]));
						intensityImage.Set(i,j, intensityImage.Get(i,j) + BaseColor*4f);
					}
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsPixelOkay(ref Vector4 v1, ref Vector4 v2, ref Vector4 v3, ref Vector4 v4) {
			Vector4 temp = Vector4.One;
			if ((temp = v1 - v2) != null && Math.Abs(temp.X) > Tolerance || (Math.Abs(temp.Y) > Tolerance) ||
			    (Math.Abs(temp.Z) > Tolerance) || (Math.Abs(temp.W) > Tolerance)) {
				return false;
			} 
			if ((temp = v1 - v4) != null && Math.Abs(temp.X) > Tolerance || (Math.Abs(temp.Y) > Tolerance) ||
			           (Math.Abs(temp.Z) > Tolerance) || (Math.Abs(temp.W) > Tolerance)) {
				return false;
			} 
			if ((temp = v3 - v2) != null && Math.Abs(temp.X) > Tolerance || (Math.Abs(temp.Y) > Tolerance) ||
			         (Math.Abs(temp.Z) > Tolerance) || (Math.Abs(temp.W) > Tolerance)) {
				return false;
			} 
			if ((temp = v3 - v4) != null && Math.Abs(temp.X) > Tolerance || (Math.Abs(temp.Y) > Tolerance) ||
			           (Math.Abs(temp.Z) > Tolerance) || (Math.Abs(temp.W) > Tolerance)) {
				return false;
			}
			return true;

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector4 AverageVector4(ref Vector4 v1, ref Vector4 v2, ref Vector4 v3, ref Vector4 v4) {
			return ((v1 + v2 + v3 + v4)/4.0f);
		}

		private Vector4 SubdividePixel(ref int numRays, Scene s , int recursions, ref Vector4 v1, ref Vector4 v2, ref Vector4 v3, ref Vector4 v4, ref Vector3 dir1, ref Vector3 dir2, ref Vector3 dir3, ref Vector3 dir4) {
			if (recursions == 0) {
				return AverageVector4(ref v1, ref v2, ref v3, ref v4);
			}
			numRays += 4;
			var tmd = (dir1 + dir2)/2f;
			var md = (dir1 + dir2 + dir3 + dir4)/4f;
			var rmd = (dir2 + dir3)/2f;
			var bmd = (dir3 + dir4)/2f;
			var lmd = (dir4 + dir1)/2f;
			var TopMid = new Ray(tmd, Position).Trace(s, 2);
			var Mid = new Ray(md, Position).Trace(s, 2);
			var RightMid = new Ray(rmd, Position).Trace(s, 2);
			var BottomMid = new Ray(bmd, Position).Trace(s, 2);
			var LeftMid = new Ray(lmd, Position).Trace(s, 2);
			//TopLeft
			var pixel = new Vector4[4];
			if (IsPixelOkay(ref v1, ref TopMid, ref Mid, ref LeftMid)) {
				pixel[0] = AverageVector4(ref v1, ref TopMid, ref Mid, ref LeftMid);
			} else {
				pixel[0] = SubdividePixel(ref numRays, s, recursions - 1, ref v1, ref TopMid, ref Mid, ref LeftMid, ref dir1, ref tmd, ref md, ref lmd);
			}
			//Top Right
			if (IsPixelOkay(ref TopMid, ref v2, ref RightMid, ref Mid)) {
				pixel[1] = AverageVector4(ref TopMid, ref v2, ref RightMid, ref Mid);
			} else {
				pixel[1] = SubdividePixel(ref numRays,s, recursions - 1, ref TopMid, ref v2, ref RightMid, ref Mid, ref tmd, ref dir2, ref rmd, ref md);
			}
			//Bottom right
			if (IsPixelOkay(ref Mid, ref RightMid, ref v3, ref BottomMid)) {
				pixel[2] = AverageVector4(ref Mid, ref RightMid, ref v3, ref BottomMid);
			} else {
				pixel[2] = SubdividePixel(ref numRays,s, recursions - 1, ref Mid, ref RightMid, ref v3, ref BottomMid, ref md, ref rmd, ref dir3, ref bmd);
			}
			//Bottom left
			if (IsPixelOkay(ref LeftMid, ref Mid, ref BottomMid, ref v4)) {
				pixel[3] = AverageVector4(ref LeftMid, ref Mid, ref BottomMid, ref v4);
			} else {
				pixel[3] = SubdividePixel(ref numRays, s, recursions - 1, ref LeftMid, ref Mid, ref BottomMid, ref v4, ref lmd, ref md, ref bmd, ref dir4);
			}
			return AverageVector4(ref pixel[0], ref pixel[1], ref pixel[2], ref pixel[3]);
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

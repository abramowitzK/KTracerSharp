using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	class ProceduralMaterial : Material{
		public Vector4 GetColor(float x, float y, float z) {
			float radius = (float)(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2)));
			float angle;

			if (x > -0.0001 && x < 0.0001) {
				angle = 3.14159f / 2.0f;
			} else {
				angle = (float)Math.Atan(y / x);
			}
			radius = radius + (3.0f * (float)Math.Sin(55.0 * angle + y / 10000.0));
			while (radius > 2.0) {
				radius /= 1.3f;
			}
			Vector4 color;
			if (radius > 1.2) {
				color = new Vector4(0.7f, 0.3f, 0.0f, 1.0f);

			} else if (radius < 1.2 && radius > 0.3) {
				color = new Vector4(0.55f, 0.4f, 0.2f, 1.0f);
			} else
				color = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
			return color;
		}

		public ProceduralMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks) : base(shinyness, dColor, sColor, ambient, ka, kd, ks) {
			MType = MaterialType.Procedural;
		}

		public ProceduralMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, float kr) : base(shinyness, dColor, sColor, ambient, ka, kd, ks, kr) {
		}

		public ProceduralMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, bool refractive, float indexOfRefraction) : base(shinyness, dColor, sColor, ambient, ka, kd, ks, refractive, indexOfRefraction) {
		}
	}
}

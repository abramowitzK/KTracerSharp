using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	class TexturedMaterial : Material {
		private const int texSize = 1024;
		private float[,] noiseArray;
		private Vector4 [,] texture;
 		private void GenerateTexture() {
			//Seed the random number generator
			Random r = new Random(DateTime.UtcNow.Second);
			for (int x = 0; x < texSize; x++) {
				for (int y = 0; y < texSize; y++) {
					noiseArray[x, y] = (float) r.NextDouble();
				}
			}
			marble();
		}
		public float smooth(float x, float y) {
			//get fractional part of x and y
			float fractX = x - (int)x;
			float fractY = y - (int)y;
			//wrap around
			int x1 = ((int)x + texSize) % texSize;
			int y1 = ((int)y + texSize) % texSize;
			//neighbor values
			int x2 = (x1 + texSize - 1) % texSize;
			int y2 = (y1 + texSize - 1) % texSize;
			//smooth the noise with bilinear interpolation
			float value = 0.0f;
			value += fractX * fractY * noiseArray[y1,x1];
			value += (1 - fractX) * fractY * noiseArray[y1,x2];
			value += fractX * (1 - fractY) * noiseArray[y2,x1];
			value += (1 - fractX) * (1 - fractY) * noiseArray[y2,x2];

			return value;
		}
		float generateSwirls(float x, float y, float size) {
			float ret = 0.0f;
			float init = size;
			while (size >= 1.0) {
				float newX = x / size;
				float newY = y / size;
				ret += smooth(newX, newY) * size;
				size = (float)(size / 3.5);
			}
			ret = (float)(256.0 * ret / init);
			return (256.0f * ret / init);
		}
		void marble() {
			Vector4 color;
			for (int x = 0; x < texSize; x++) {
				for (int y = 0; y < texSize; y++) {
					float value = (float)(5.0 * generateSwirls(y, x, 256.0f) / 256.0f);
					float sineValue = (float)(256.0 * Math.Abs(Math.Sin((float)(value * Math.PI))));
					color.X = ((20.0f + sineValue) / 256.0f);
					color.Y = ((100.0f + sineValue) / 256.0f);
					color.Z = ((50.0f + sineValue) / 256.0f);
					color.W = 1.0f;
					texture[x,y] = color;
				}
			}
		}

		public Vector4 GetDiffuse(float x, float y) {
			int xt = (int)(texSize*x);
			int yt = (int) (texSize*y);
			return texture[xt, yt];
		}

		public TexturedMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks) : base(shinyness, dColor, sColor, ambient, ka, kd, ks) {

			noiseArray = new float[texSize, texSize];
			texture = new Vector4[texSize, texSize];
			GenerateTexture();
			MType = MaterialType.Textured;
		}

		public TexturedMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, float kr) : base(shinyness, dColor, sColor, ambient, ka, kd, ks, kr) {
		}

		public TexturedMaterial(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, bool refractive, float indexOfRefraction) : base(shinyness, dColor, sColor, ambient, ka, kd, ks, refractive, indexOfRefraction) {
		}
	}
}

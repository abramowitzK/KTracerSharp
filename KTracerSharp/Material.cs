using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KTracerSharp {
	public enum MaterialType {
		None,
		Phong,
		Reflective,
		Refractive,
	}

	public class Material {
		public float Shinyness { get; set; }
		public float KD { get; set; }
		public float KA { get; set; }
		public float KS { get; set; }
		public float KR { get; set; }
		public float N { get; set; }
		public Vector4 AmbientColor { get; set; }
		public Vector4 DiffuseColor { get; set; }
		public Vector4 SpecularColor { get; set; }
		public MaterialType MType { get; set; }

		public Material(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks) {
			Shinyness = shinyness;
			AmbientColor = ambient;
			DiffuseColor = dColor;
			SpecularColor = sColor;
			MType = MaterialType.Phong;
			KA = ka;
			KD = kd;
			KS = ks;
			KR = 0;
			N = 1f;

		}
		public Material(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, float kr) {
			Shinyness = shinyness;
			AmbientColor = ambient;
			DiffuseColor = dColor;
			SpecularColor = sColor;
			MType = MaterialType.Reflective;
			KA = ka;
			KD = kd;
			KS = ks;
			KR = kr;
			N = 1f;
		}
		public Material(float shinyness, Vector4 dColor, Vector4 sColor, Vector4 ambient, float ka, float kd, float ks, bool refractive, float indexOfRefraction) {
			Shinyness = shinyness;
			AmbientColor = ambient;
			DiffuseColor = dColor;
			SpecularColor = sColor;
			MType = MaterialType.Refractive;
			KA = ka;
			KD = kd;
			KS = ks;
			KR = 0;
			N = indexOfRefraction;
		}
	}
}

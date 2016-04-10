using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KTracerSharp {
	public class Material {
		public float Shinyness { get; set; }
		public Vector4 DiffuseColor { get; set; }
		public Vector4 SpecularColor { get; set; }

		public Material(float shinyness, Vector4 dColor, Vector4 sColor) {
			Shinyness = shinyness;
			DiffuseColor = dColor;
			SpecularColor = sColor;
		}
	}
}

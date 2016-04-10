using OpenTK;

namespace KTracerSharp {
	public class Light {
		public Vector3 Pos { get; set; }
		public Vector4 Color { get; set; }
		public float Intensity { get; set; }

		public Light(Vector3 pos, Vector4 color, float intensity) {
			Pos = pos;
			Color = color;
			Intensity = intensity;
		}
	}
}

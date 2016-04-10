using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public enum ObjectType {
		None,
		Sphere,
		TriangleMesh,
	}

	public abstract class RenderObject {
		public Vector4 Color { get; set; }
		public Vector3 Pos { get; set; }
		public Quaternion Rot { get; set; }
		public float Scale { get; set; }

		protected RenderObject(Vector3 pos, Quaternion rotation, float scale, Vector4 color) {
			Pos = pos;
			Rot = rotation;
			Scale = scale;
			Color = color;
		}

		public virtual ObjectType GetObjectType() {
			return ObjectType.None;
		}

		public abstract bool Intersect(Ray ray, ref float tMin, ref Vector3 intPoint, ref Vector3 normal);
		public abstract void Rotate(float x, float y, float z);
		public abstract void Translate(float x, float y, float z);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace KTracerSharp {
	public class Scene {
		private Camera Cam { get; set; }

		public Scene() {
			Cam = new Camera(new Vector3(0.0f, 0.0f, -10.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), 45.0f, 10.0f);
			Objects = new List<RenderObject>();
		}
		public Image Render() {
			int height = 1024;
			int width = 1024;
			Image im = new Image(width, height);
			var rays = Cam.GenerateRays(width, height);
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					im.Set(i, j, rays[i,j].Trace(this, 1));
				}
			}
			return im;
		}

		public void AddObject(RenderObject obj) {
			Objects.Add(obj);
		}

		public IList<RenderObject> Objects { get; private set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
namespace KTracerSharp {
	public class Scene {
		private Camera Cam { get; set; }
		private object m_lock = new object();
		public Scene() {
			Cam = new Camera(new Vector3(0.0f, 0.0f, -10.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), 45.0f, 10.0f);
			Objects = new List<RenderObject>();
		}
		public Image Render() {
			int height = 1024;
			int width = 1024;
			Image im = new Image(width, height);
			var rays = Cam.GenerateRays(width, height);
			/*for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					im.Set(i, j, rays[i,j].Trace(this, 1));
				}
			}*/
			var threads = new Task[8];
			int index = 1024/8;
			for (var i = 0; i < 8; i++) {
				int temp = i;
				threads[i] = Task.Factory.StartNew(() => RenderTask(ref im, rays, temp*index, (temp + 1)*(index), height));
			}
			Task.WaitAll(threads);
			return im;
		}

		public void RenderTask(ref Image im, Ray[,] rays, int start, int end, int height) {
			for (int i = start; i < end; i++) {
				for (int j = 0; j < height; j++) {
					im.Set(i, j, rays[i, j].Trace(this, 1));
				}
				Task.Delay(0);
			}
		}

		public void AddObject(RenderObject obj) {
			Objects.Add(obj);
		}

		public IList<RenderObject> Objects { get; private set; }
	}
}

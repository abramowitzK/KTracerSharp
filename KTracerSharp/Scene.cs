using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public class Scene {
		public Scene() {
			Cam = new Camera(new Vector3(-10.0f, 10.0f, -10.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, -1f, 1.0f), 45.0f,
				10.0f);
			Objects = new List<RenderObject>();
			Lights = new List<Light>();
		}

		private Camera Cam { get; }
		public Vector4 AmbientColor { get; set; }
		public IList<RenderObject> Objects { get;}
		public IList<Light> Lights { get; }

		public Image Render() {
			const int height = 1024;
			const int width = 1024;
			var im = new Image(width, height);
			var rays = Cam.GenerateRays(width, height);
			var threads = new Task[Environment.ProcessorCount];
			var index = height/Environment.ProcessorCount;
			for (var i = 0; i < Environment.ProcessorCount; i++) {
				var temp = i;
				threads[i] = Task.Factory.StartNew(() => RenderTask(ref im, rays, temp*index, (temp + 1)*index, height));
			}
			Task.WaitAll(threads);
			return im;
		}

		public void RenderTask(ref Image im, Ray[,] rays, int start, int end, int height) {
			for (var i = start; i < end; i++) {
				for (var j = 0; j < height; j++) {
					im.Set(i, j, rays[i, j].Trace(this, 2));
				}
#if !__MonoCS__
				Task.Delay(0);
#endif
			}
		}

		public void AddObject(RenderObject obj) {
			Objects.Add(obj);
		}

		public void AddLight(Light l) {
			Lights.Add(l);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public class Scene {
		public const int MaxTris = 1000;
		public Scene() {
			Cam = new Camera(new Vector3(20.0f, 0.0f, 20.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(-1f, 0f, -1.0f), 45.0f,
				10.0f);
			Objects = new List<RenderObject>();
			Lights = new List<Light>();
		}

		public BoundingSphere Root { get; set; }
		private Camera Cam { get; set; }
		public Vector4 AmbientColor { get; set; }
		public IList<RenderObject> Objects { get; set;}
		public IList<Light> Lights { get; set;}

		public Image Render() {
			foreach (var obj in Objects) {
				obj.CalculateBoundingSphere();
				var t = obj as TriangleMesh;
				t?.GenerateTriangles();
			}

			Root = ConstructBVH(Objects.ToList(), false);
			const int height = 2048;
			const int width = 2048;
			//Must divide evenly into resolution currently
			var numThreads = 64;
			var im = new Image(width, height);
			var rays = Cam.GenerateRays(width, height);
			var threads = new Task[numThreads];
			var index = height/numThreads;
			for (var i = 0; i < numThreads; i++) {
				var temp = i;
				threads[i] = Task.Factory.StartNew(() => RenderTask(ref im, rays, temp*index, (temp + 1)*index, height));
			}
			Task.WaitAll(threads);
			return im;
		}

		private BoundingSphere ConstructBVH(List<RenderObject> obj, bool sortByX) {
			if (obj.Count == 1) {
				if (obj[0] is Sphere)
					return obj[0].BoundingBox;
				else {
					return ConstructTriangleLevelBVH((obj[0] as TriangleMesh)?.GetTriangles(), !sortByX);
				}
			}
			else {
				BoundingSphere x = BoundingSphere.ConstructBoundingSphereFromList(obj);
				var sorted = obj.OrderBy(o => sortByX ? o.Pos.X : o.Pos.Y).ToList();
				var count = sorted.Count/2;
				x.LeftChild = ConstructBVH(sorted.Take(count).ToList(), !sortByX);
				x.RightChild = ConstructBVH(sorted.Skip(count).Take(sorted.Count - count).ToList(), !sortByX);
				return x;
			}
		}

		private BoundingSphere ConstructTriangleLevelBVH(List<Triangle> tris, bool sortByX) {
			BoundingSphere x = BoundingSphere.ConstructFromTriangles(tris);
			x.NumTriangles = tris.Count;
			if (tris.Count > MaxTris) {
				var sorted = tris.OrderBy(o => sortByX ? o.V1.Point.X : o.V1.Point.Y).ToList();
				var count = sorted.Count / 2;
				x.LeftChild = ConstructTriangleLevelBVH(sorted.Take(count).ToList(), !sortByX);
				x.RightChild = ConstructTriangleLevelBVH(sorted.Skip(count).Take(sorted.Count - count).ToList(), !sortByX);
			}
			return x;
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

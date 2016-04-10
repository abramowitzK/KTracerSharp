using System;
using System.Diagnostics;
using OpenTK;
namespace KTracerSharp {
	public class Program {
		public static void Main(string[] args) {
			var loader = new SMFMeshLoader();
			loader.LoadFile("bound-bunny_1k.smf");
			loader.LoadFile("teapot.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			var mesh2 = loader.GetMesh("teapot.smf");
			var scene = new Scene();
			var s = new Sphere(new Vector3(1.0f, 0.0f, 0.0f), Quaternion.Identity, 1.0f, new Vector4(0.9f, 0.1f, 0.1f, 1.0f), 0.25f);
			var s2 = new Sphere(new Vector3(3.0f, 0.0f, 0.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.5f, 0.1f, 1.0f), 1f);
			scene.AddLight(new Light(new Vector3(-10f, 0f, 0f), new Vector4(1.0f,1.0f,1.0f,1.0f), 0.1f));
			scene.AddLight(new Light(new Vector3(10, 0f, 0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 0.1f));
			scene.AddObject(s);
			scene.AddObject(s2);
			scene.AmbientColor = new Vector4(0.07f,0.07f,0.07f,1.0f);
			mesh.Translate(0, -1.5f, 0);
			mesh2.Translate(0, 0.75f, 0);
			//scene.AddObject(mesh);
			//scene.AddObject(mesh2);
			var watch = new Stopwatch();
			watch.Start();
			var i = scene.Render();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds/1000.0);
#if __MonoCS__
			i.WriteToPPM("out.ppm");
#else
			i.WriteToPNG("out.png");
#endif
		}
	}
}

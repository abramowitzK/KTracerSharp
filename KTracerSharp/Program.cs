using System;
using System.Diagnostics;
using OpenTK;
//uncomment this for antialiasing
//#define ANTIALIAS
namespace KTracerSharp {
	public class Program {
		public static void Main(string[] args) {
			var loader = new SMFMeshLoader();
			loader.LoadFile("bound-bunny_1k.smf");
			loader.LoadFile("bound-bunny.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			mesh.Mat = new Material(100f, new Vector4(0.7f, 0.0f, 0.7f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.7f, 0f, 0.7f, 1f), 0.05f, 0.7f, 0.9f);
			mesh.UniformScale(5);
            var mesh2 = loader.GetMesh("bound-bunny.smf");
			var scene = new Scene();
			var s = new Sphere(new Vector3(-5.0f, 5.0f, 0.0f), Quaternion.Identity, 1.0f, new Vector4(0.9f, 0.1f, 0.1f, 1.0f), 2f);
			s.Mat = new Material(150f, new Vector4(0.7f, 0.7f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.7f, 0.7f, 0.0f, 1f), 0.05f, 0.5f, 0.7f);
			var s2 = new Sphere(new Vector3(5.0f, -5.0f, 0.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.4f, 0.1f, 1.0f), 3f);
			s2.Mat = new Material(5f, new Vector4(0.0f, 0.7f, 0.7f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.0f, 0.7f, 0.7f, 1f), 0.05f, 0.5f, 0.7f);
			scene.AddLight(new Light(new Vector3(0f, 0f, 3f), new Vector4(1.0f,1.0f,1.0f,1.0f), 2.0f));
			scene.AddLight(new Light(new Vector3(0, 1f, 1f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 1.0f));
			scene.AddObject(s);
			scene.AddObject(s2);
			scene.AmbientColor = new Vector4(0.07f,0.07f,0.07f,1.0f);
			//mesh.Translate(0, 0f, 0);
			mesh2.Mat = new Material(20f, new Vector4(0.8f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.8f, 0f, 0.0f, 1f), 0.05f, 0.7f, 0.9f);
			mesh2.UniformScale(3f);
			mesh2.Translate(-5.0f, -5.0f, 0f);
			scene.AddObject(mesh);
			scene.AddObject(mesh2);
			var watch = new Stopwatch();
			watch.Start();
			var i = scene.Render();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds/1000.0);
#if ANTIALIAS
			i.AntialiasAndWriteToFile("out.png");
#else
#if __MonoCS__
			i.WriteToPPM("out.ppm");
#else
			i.WriteToPNG("out.png");
#endif
#endif
		}
	}
}

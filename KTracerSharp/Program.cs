//uncomment this for antialiasing
//#define ANTIALIAS
using System;
using System.Diagnostics;
using OpenTK;


namespace KTracerSharp {
	public class Program {
		public static void Main(string[] args) {
			if(args.Length > 0)
				int.TryParse(args[0], out Scene.MaxTris);
			var watch = new Stopwatch();
			watch.Start();
			var loader = new SMFMeshLoader();
			loader.LoadFile("box.smf");
			loader.LoadFile("bound-bunny_1k.smf");
			loader.LoadFile("lwall.smf");
			loader.LoadFile("icos.smf");
			loader.LoadFile("bound-cow.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			mesh.Mat = new Material(1000f, new Vector4(0.0f, 0.2f, 0.8f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.0f, 0.2f, 0.8f, 1f), 0.03f, 0.4f, 0.6f);
			mesh.UniformScale(2);
			var mesh3 = loader.GetMesh("box.smf");
			var mesh2 = loader.GetMesh("lwall.smf");
			mesh2.Mat = new Material(150f, new Vector4(0.1f, 1f, 0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.1f, 1f, 0.0f, 1f), 0.15f, 0.4f, 0.6f);
			var mesh4 = loader.GetMesh("icos.smf");
			mesh4.Mat = new Material(200f, new Vector4(0.0f, 0.7f, 0.7f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
					new Vector4(0.0f, 0.7f, 0.7f, 1f), 0.01f, 0.0f, 1.0f, true, 1.2f);
			var mesh5 = loader.GetMesh("bound-cow.smf");
			mesh5.Mat = new Material(100f, new Vector4(0.9f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.9f, 0.0f, 0.0f, 1f), 0.1f, 0.3f, 0.6f);
			var scene = new Scene();
			var s2 = new Sphere(new Vector3(0.0f, 0.0f, 5.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.4f, 0.1f, 1.0f), 3f) {
				Mat = new Material(250f, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
					new Vector4(1.0f, 1.0f, 1.0f, 1f), 0.0005f, 0.0005f, 0.5f, true, 1.33f)
			};
			var s3 = new Sphere(new Vector3(-3.0f, 0.0f, 10f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.4f, 0.1f, 1.0f), 0.25f) {
				Mat = new Material(250f, new Vector4(1.0f, 0.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
		new Vector4(1.0f, 0.0f, 1.0f, 1f), 0.05f, 0.5f, 0.5f)
			};
			//scene.AddLight(new Light(new Vector3(0,10,0), Vector4.One,150f ));
			mesh5.UniformScale(3f);
			mesh5.Translate(0,2,0);
			scene.AddLight(new Light(new Vector3(0, 10, 20), Vector4.One, 400f));
			scene.AddObject(s2);
			mesh2.Scale(100, 100, 1);
			mesh4.UniformScale(3f);
			mesh4.Translate(6,0,5);
			mesh2.Translate(0,0,-10);
			scene.AddObject(mesh2);
			scene.AddObject(mesh5);
			scene.AddObject(mesh4);
			mesh.UniformScale(3f);
			mesh.Translate(-3,0, -5f);
			scene.AddObject(mesh);
			scene.AddObject(s3);
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

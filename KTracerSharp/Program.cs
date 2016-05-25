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
			loader.LoadFile("bound-cow.smf");
			loader.LoadFile("rwall.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			mesh.Mat = new Material(150f, new Vector4(0.0f, 0.2f, 0.8f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.0f, 0.2f, 0.8f, 1f), 0.15f, 0.5f, 0.5f);
			mesh.UniformScale(2);
			var mesh3 = loader.GetMesh("box.smf");
			var mesh2 = loader.GetMesh("lwall.smf");
			mesh2.Mat = new Material(150f, new Vector4(0.1f, 0.1f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.1f, 0.1f, 1.0f, 1f), 0.15f, 0.5f, 0.5f);
			var mesh4 = loader.GetMesh("bound-cow.smf");
			var mesh5 = loader.GetMesh("rwall.smf");
			var scene = new Scene();
			var s = new Sphere(new Vector3(-5.0f, 5.0f, 0.0f), Quaternion.Identity, 1.0f, new Vector4(0.9f, 0.1f, 0.1f, 1.0f), 2f) {
				Mat = new Material(150f, new Vector4(0.7f, 0.7f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
					new Vector4(0.7f, 0.7f, 0.0f, 1f), 0.05f, 0.5f, 0.7f)
			};
			var s2 = new Sphere(new Vector3(5.0f, -5.0f, -2.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.4f, 0.1f, 1.0f), 3f) {
				Mat = new Material(200f, new Vector4(0.0f, 0.7f, 0.7f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
					new Vector4(0.0f, 0.7f, 0.7f, 1f), 0.0f, 0.0f, 1.0f, 1.0f)
			};
			var s3 = new Sphere(new Vector3(9.0f, 4.0f, 6.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.4f, 0.1f, 1.0f), 5.4f) {
				Mat = new Material(5f, new Vector4(0.9f, 0.1f, 0.1f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.9f, 0.1f, 0.1f, 1f), 0.3f, 0.6f, 0.4f)
			};
			var s4 = new Sphere(new Vector3(-10.0f, 20.0f, 20.0f), Quaternion.Identity, 1f, new Vector4(0.1f, 0.9f, 0.1f, 1.0f), 5f) {
				Mat = new Material(200f, new Vector4(0.7f, 0.7f, 0.7f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.7f, 0.7f, 0.7f, 1f), 0.05f, 0.5f, 0.7f)
			};
			//scene.AddLight(new Light(new Vector3(10f, 4f, 0f), new Vector4(1.0f,1.0f,1.0f,1.0f), 10.0f));
			scene.AddLight(new Light(new Vector3(0, 10f, 0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), 30.0f));
			scene.AddLight(new Light(new Vector3(0, 15f, 5f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 20.0f));
			//scene.AddLight(new Light(new Vector3(-3, 0f, 0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 3.0f));
			//scene.AddObject(s);
			scene.AddObject(s2);
			//scene.AddObject(s3);
			scene.AddObject(s4);
			scene.AmbientColor = new Vector4(0.07f,0.07f,0.07f,1.0f);
			//mesh.Translate(0, 0f, 0);
			mesh3.Mat = new Material(20f, new Vector4(0.8f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				new Vector4(0.8f, 0f, 0.0f, 1f), 0.05f, 0.7f, 0.9f);
			mesh4.Mat = new Material(150f, new Vector4(0.5f, 0.7f, 0.3f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
	new Vector4(0.5f, 0.7f, 0.3f, 1f), 0.1f, 0.6f, 0.4f);
			mesh2.UniformScale(3f);
			mesh2.Translate(-6.0f, -6.0f, 0f);
			mesh3.Rotate(90f, 0f, 0f);

			scene.AddObject(mesh);
			//scene.AddObject(mesh2);
			mesh4.UniformScale(10f);
			mesh4.Translate(-8,0,0);
			mesh.Rotate(0,-90f, 0f);
			mesh.UniformScale(3.0f);
			mesh.Translate(0,7, 5);
			mesh3.Scale(10f, 1f, 100f);
			mesh3.Translate(0,-10, 0);
			mesh2.Rotate(0,-90f, 0f);
			mesh2.Scale(10f, 100, 100f);
			mesh2.Translate(0, -10, 0);
			scene.AddObject(mesh3);
			scene.AddObject(mesh2);
			scene.AddObject(mesh4);
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

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
			var scene = new Scene();
			SMFMeshLoader loader = new SMFMeshLoader();
			loader.LoadFile("bound-bunny_1k.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			mesh.UniformScale(4f);
			mesh.Mat = new ProceduralMaterial(200, Vector4.One, Vector4.One, Vector4.One, .2f, 0.4f, 0.6f);
			var sphere = new Sphere(new Vector3(2,3,2), Quaternion.Identity, 1, Vector4.One, 2);
			sphere.Mat = new TexturedMaterial(1000, Vector4.One, Vector4.One, Vector4.One, 0.1f, .4f, .6f );
			scene.AddObject(sphere);
			scene.AddObject(mesh);
			scene.AddLight(new Light(new Vector3(0,0,5), Vector4.One, 10));
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

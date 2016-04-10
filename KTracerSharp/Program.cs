using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace KTracerSharp {
	class Program {
		static void Main(string[] args) {
			var loader = new SMFMeshLoader();
			loader.LoadFile("bound-bunny_1k.smf");
			loader.LoadFile("teapot.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			var mesh2 = loader.GetMesh("teapot.smf");
			var scene = new Scene();
			var s = new Sphere(new Vector3(-3.0f, 0.0f, 0.0f), Quaternion.Identity, 1.0f, Vector4.One, 0.25f);
			var s2 = new Sphere(new Vector3(3.0f, 0.0f, 0.0f), Quaternion.Identity, 1f, Vector4.One, 1f);
			scene.AddObject(s);
			scene.AddObject(s2);
			mesh.Translate(0, -1.5f, 0);
			mesh2.Translate(0, 0.75f, 0);
			scene.AddObject(mesh);
			scene.AddObject(mesh2);
			Stopwatch watch = new Stopwatch();
			watch.Start();
			var i = scene.Render();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds/1000.0);
			i.WriteToPPM("out.ppm");
	//		i.WriteToPNG("out.png");
		}
	}
}

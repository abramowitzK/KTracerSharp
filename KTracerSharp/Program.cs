using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace KTracerSharp {
	class Program {
		static void Main(string[] args) {
			var loader = new SMFMeshLoader();
			loader.LoadFile("bound-bunny_1k.smf");
			var mesh = loader.GetMesh("bound-bunny_1k.smf");
			var scene = new Scene();
			Sphere s = new Sphere(new Vector3(-3.0f, 0.0f, 0.0f), Quaternion.Identity, 1.0f, Vector4.One, 0.25f);
			//scene.AddObject(s);
			scene.AddObject(mesh);
			//scene.AddTriangleMesh(&mesh2);
			//auto start = chrono::high_resolution_clock::now();
			Image i = scene.Render();
			//auto end = chrono::high_resolution_clock::now();
			//auto total = end - start;
			//float total2 = total.count() / 1.0e9;
			//Logger::Log("Rendering finished in: " + std::to_string(total2) + " seconds.");
			i.WriteToPNG("image.png");
		}
	}
}

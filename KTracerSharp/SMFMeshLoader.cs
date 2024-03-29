﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;

namespace KTracerSharp {
	public class SMFMeshLoader {
		private readonly Dictionary<string, TriangleMesh> m_resourceMap;

		public SMFMeshLoader() {
			m_resourceMap = new Dictionary<string, TriangleMesh>();
		}

		public void LoadFile(string filename) {
			//Create an input filestream
			var stream = new StreamReader(filename);
			var vertices = new List<Vector3>();
			var indices = new List<int>();
			var faceNormals = new List<Vector3>();
			string line;
			while (stream.Peek() >= 0) {
				line = stream.ReadLine();
				if (line == null)
					continue;
				switch (line[0]) {
					case 'f':
						ParseFace(indices, line);
						break;
					case 'v':
						ParseVertex(vertices, line);
						break;
					default:
						break;
				}
			}
			var normals = new List<Vector3>(new Vector3[vertices.Count]);
			for (var i = 0; i < indices.Count; i += 3) {
				var p1 = vertices[indices[i]];
				var p2 = vertices[indices[i + 1]];
				var p3 = vertices[indices[i + 2]];
				var u = p2 - p1;
				var v = p3 - p1;
				var n = Vector3.Cross(u, v);
				faceNormals.Add(new Vector3(n));
			}
			for (var i = 0; i < indices.Count; i++) {
				var f = i/3;
				var v = indices[i];
				normals[v] += faceNormals[f];
			}
			for (var i = 0; i < normals.Count; i++) {
				normals[i] = normals[i].Normalized();
			}
			List<Vertex> verts = new List<Vertex>(new Vertex[vertices.Count]);

			for (var i = 0; i < verts.Count; i++) {
				verts[i] = new Vertex(new Vector3( vertices[i]), new Vector3( normals[i]));
			}
			//CalcVertexNormals(indices, faceNormals, normals);
			m_resourceMap[filename] = new TriangleMesh(verts, indices);
		}

		public TriangleMesh GetMesh(string filename) {
			if(m_resourceMap.ContainsKey(filename))
				return new TriangleMesh(m_resourceMap[filename]);
			else {
				throw new KeyNotFoundException("File does not exist in map");
			}
		}

		private void CalcFaceNormals(int triangle, List<int> indices, List<Vector3> vertices,
			ref List<Vector3> faceNormals) {
		}

		private void CalcVertexNormals(List<int> indices, List<Vector3> faceNormals, List<Vector3> normals) {
			for (var i = 0; i < indices.Count; i++) {
				var f = i / 3;
				var v = indices[i];
				normals[v] += faceNormals[f];
				normals[v].Normalize();
			}
		}

		private void ParseVertex(List<Vector3> vertices, string line) {
			//Split line on space delimiter
			var split = line.Split(' ');
			var ret = new Vector3(1.0f);
			for (var i = 1; i < 4; i++) {
				ret[i - 1] = float.Parse(split[i]);
			}
			vertices.Add(new Vector3(ret));
		}

		private void ParseFace(List<int> indices, string line) {
			var split = line.Split(' ');
			for (var i = 1; i < 4; i++) {
				indices.Add(int.Parse(split[i]) - 1);
			}
		}
	}
}
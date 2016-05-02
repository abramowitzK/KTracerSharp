using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTracerSharp {
	public class OctreeNode {
		public OctreeNode() {
			Children = new OctreeNode[8];
		}

		public OctreeNode[] Children { get; }
	}

	public class Octree {
		public OctreeNode Root { get; set; }

		public Octree() {

		}
	}
}

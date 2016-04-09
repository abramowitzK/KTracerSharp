using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KTracerSharp {
	public class Image {
		public Image(int hRes, int vRes) {
			Height = hRes;
			Width = vRes;
			m_data = new Vector4[hRes,vRes];
		}

		public void WriteToPNG(string FileName) {
			using (var b = new Bitmap(Width, Height)) {
				for(var i = 0; i < Height; i++) {
					for (var j = 0; j < Width; j++) {
						Color c = Color.FromArgb(
							255,
							(int) (m_data[i, j].X*255),
							(int) (m_data[i, j].Y*255),
							(int) (m_data[i, j].Z*255));
						b.SetPixel(i, j, c);
					}
				}
				b.Save(FileName, ImageFormat.Png);
			}
		}
		public void Set(int i, int j, Vector4 data) {
			m_data[i,j] = data;
		}

		private Vector4[,] m_data;
		public int Height { get; set; }
		public int Width { get; set; }
	}
}

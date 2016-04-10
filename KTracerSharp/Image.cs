
using System;
using System.IO;
#if !__MonoCS__
using System.Drawing;
using System.Drawing.Imaging;
#endif
using OpenTK;

namespace KTracerSharp {
	public class Image {
		public Image(int hRes, int vRes) {
			Height = hRes;
			Width = vRes;
			m_data = new Vector4[hRes,vRes];
		}
		/**
		Need this for linux since I can't find a png library that works on linux...GOing to use imagemagick to convert
		on running program
		*/
		public void WriteToPPM(string FileName) {
			
			StreamWriter text = new StreamWriter(FileName);
			text.Write("P6" + " ");
			text.Write(Width);
			text.Write(" ");
			text.Write(Height);
			text.Write(" "+"255" + " ");
			text.Close();
			var ofs = new BinaryWriter(new FileStream(FileName, FileMode.Append));
			for (int i = 0; i < Height; i++) {
				for (int j = 0; j < Width; j++) {
					ofs.Write((byte)(Math.Min(1.0f, m_data[j,i].X)*255));
					ofs.Write((byte)(Math.Min(1.0f, m_data[j,i].Y)*255));
					ofs.Write((byte)(Math.Min(1.0f, m_data[j,i].Z)*255));
				}
			}
			ofs.Close();
	}
#if !__MonoCS__
		public void WriteToPNG(string FileName) {
			using (var b = new Bitmap(Width, Height)) {
				for(var i = 0; i < Height; i++) {
					for (var j = 0; j < Width; j++) {
						Color c = Color.FromArgb(
							255,
							(int) (Math.Min(1.0f, m_data[i, j].X)*255),
							(int) (Math.Min(1.0f, m_data[i, j].Y)*255),
							(int) (Math.Min(1.0f, m_data[i, j].Z)*255));
						b.SetPixel(i, j, c);
					}
				}
				b.Save(FileName, ImageFormat.Png);
			}
		}
#endif
		public void Set(int i, int j, Vector4 data) {
				m_data[i,j] = data;
		}

		private Vector4[,] m_data;
		public int Height { get; set; }
		public int Width { get; set; }
	}
}

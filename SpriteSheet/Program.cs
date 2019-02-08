using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteSheet {
	class Program {
		static void Usage() {
			Console.WriteLine("Usage: SpriteSheet [options] <directory | inputFileSpec> <outputfile>");
			return;
		}

		static void Main(string[] args) {
			if (args.Length < 2) {
				Usage();
				return;
			}

			bool dir = Directory.Exists(args[0]);
			bool success;
			if (dir) {
				success = CreateSpriteSheet(Directory.EnumerateFiles(args[0]).OrderBy(d => d).ToArray(), args[1]);
			}
			else {
				int index = args[0].LastIndexOf('\\');
				var directory = index < 0 ? Directory.GetCurrentDirectory() : args[0].Substring(0, index);
				var pattern = index < 0 ? args[0] : args[0].Substring(index + 1);
				success = CreateSpriteSheet(Directory.EnumerateFiles(directory, pattern).OrderBy(d => d).ToArray(), args[1]);
			}
			if(success)
				Console.WriteLine("Created sprite sheet successfully!");
			else
				Console.WriteLine("Failed to create sprite sheet.");
		}

		private static bool CreateSpriteSheet(string[] files, string output) {
			if (files.Length == 0) {
				Console.WriteLine("No files selected.");
				return false;
			}
			try {
				var bmps = new Image[files.Length];
				int width = 0, height = 0;
				Bitmap outputBmp = null;
				Graphics outputGfx = null;
				for (int i = 0; i < bmps.Length; i++) {
					bmps[i] = Image.FromFile(files[i]);
					if (width == 0) {
						width = bmps[i].Width;
						height = bmps[i].Height;
						outputBmp = new Bitmap(width, height * files.Length);
						outputGfx = Graphics.FromImage(outputBmp);
					}
					else if (width != bmps[i].Width || height != bmps[i].Height) {
						Console.WriteLine("Bitmaps not the same width/height");
						return false;
					}
					outputGfx.DrawImage(bmps[i], new Rectangle(0, height * i, width, height));
					
				}
				outputGfx.Dispose();
				outputBmp.Save(output, ImageFormat.Png);
				return true;
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				return false;
			}
		}
	}
}

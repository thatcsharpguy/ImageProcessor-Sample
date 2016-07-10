using System;
using System.Drawing;
using System.IO;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;

namespace ImageProcessorSample
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var balonBytes = File.ReadAllBytes("photo/balon.jpg");

			ISupportedImageFormat format = new PngFormat { Quality = 10 };
			Size size = new Size(300, 0);

			using (var inStream = new MemoryStream(balonBytes))
			using (var imageFactory = new ImageFactory(false))
			{

				// Resize
				imageFactory.Load(inStream)
							.Resize(size)
							.Save("photo/bolaResized.jpg");

				// Lower quality
				imageFactory.Load(inStream)
				            .Quality(5) // Only works with jpg
							.Save("photo/bolaLow.jpg");
				
				// Change format
				imageFactory.Load(inStream)
				            .Format(format)
							.Save("photo/balon.png");

			}


			var michaBytes = File.ReadAllBytes("photo/micha.jpg");

			using (var inStream = new MemoryStream(michaBytes))
			using (var imageFactory = new ImageFactory(false))
			{

				// Make a Micha cool portrait
				imageFactory.Load(inStream)
				            .Filter(MatrixFilters.HiSatch)
							.Save("photo/michaArt.jpg");

				// Invert-a Micha
				imageFactory.Load(inStream)
				            .Filter(MatrixFilters.Invert)
							.Save("photo/michaInverse.jpg");
				
				// Michstagram
				imageFactory.Load(inStream)
				            .Filter(MatrixFilters.Sepia)
				            .Tint(Color.LightSalmon)
							.Saturation(50)
							.Save("photo/michaInstagram.jpg");
				
			}


			var motherBoardBytes = File.ReadAllBytes("photo/motherboard.jpg");

			using (var inStream = new MemoryStream(motherBoardBytes))
			using (var imageFactory = new ImageFactory(false))
			{

				// Cropping
				var motherboard = imageFactory.Load(inStream)
											  .Crop(new Rectangle(100, 100, 250, 250))
											  .Save("photo/motherboardCropped.jpg");

				motherboard = motherboard.Rotate(10f)
				                         .Save("photo/motherboardRotated.jpg");

				motherboard = motherboard.Flip(true, true)
										 .Save("photo/motherboardFlipped.jpg");
				
			}



			// Cognitive services:
			if (!String.IsNullOrEmpty(Keys.CognitiveServices))
			{
				
			}
		}
	}
}

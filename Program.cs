using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using Microsoft.ProjectOxford.Face;

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
				Console.WriteLine("Working on balon.jpg");

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
				Console.WriteLine("Working on micha.jpg");

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
				Console.WriteLine("Working on motherboard.jpg");

				// Cropping
				var motherboard = imageFactory.Load(inStream)
											  .Crop(new Rectangle(100, 100, 250, 250))
											  .Save("photo/motherboardCropped.jpg");

				motherboard.Rotate(10f)
				           .Save("photo/motherboardRotated.jpg");

				motherboard.Flip(true, true)
				           .Save("photo/motherboardFlipped.jpg");
				
			}



			// Cognitive services:
			if (!String.IsNullOrEmpty(Keys.CognitiveServices))
			{
				FaceServiceClient faceServiceClient = new FaceServiceClient(Keys.CognitiveServices);


				// Cropping faces:
				var robbieBytes = File.ReadAllBytes("photo/robbie3.jpg");

				using (var inStream = new MemoryStream(robbieBytes))
				using (var imageFactory = new ImageFactory(false))
				{
					Console.WriteLine("Working on robbie3.jpg");

					var detectionTask = faceServiceClient.DetectAsync(inStream);
					detectionTask.Wait();

					var face = detectionTask.Result.FirstOrDefault();

					if (face != null)
					{
						var faceContainer = new Rectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);

						inStream.Position = 0;

						imageFactory.Load(inStream)
									.Crop(faceContainer)
									.Save("photo/robbieFace.jpg");
					}
				}


				// Pixelate faces:
				var friendsBytes = File.ReadAllBytes("photo/friends2.jpg");

				using (var inStream = new MemoryStream(friendsBytes))
				using (var imageFactory = new ImageFactory(false))
				{
					Console.WriteLine("Working on friends2.jpg");

					var detectionTask = faceServiceClient.DetectAsync(inStream);
					detectionTask.Wait();

					var faces = detectionTask.Result;

					inStream.Position = 0;
					var friendsImage = imageFactory.Load(inStream);
					foreach (var face in faces)
					{
						var faceContainer = new Rectangle(face.FaceRectangle.Left,face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);
						friendsImage.Pixelate(20, faceContainer);
					}

					friendsImage.Save("photo/friendsAnonymous.jpg");
				}
			}
		}
	}
}

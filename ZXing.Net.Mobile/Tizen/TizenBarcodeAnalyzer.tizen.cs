using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmSharp;
using Tizen.Multimedia;
using Tizen.Multimedia.Vision;

namespace ZXing.UI
{
	class TizenBarcodeAnalyzer
	{
		public static Task<Result[]> AnalyzeBarcodeAsync(StillImage image, bool multiple)
		{
			var source = new MediaVisionSource(image.Data, (uint)image.Resolution.Width, (uint)image.Resolution.Height, ColorSpace.NV12);
			return AnalyzeBarcodeAsync(source, multiple);
		}
		public static async Task<Result[]> AnalyzeBarcodeAsync(MediaVisionSource source, bool multiple)
		{
			var point = new Tizen.Multimedia.Point(0, 0);
			var size = new Tizen.Multimedia.Size((int)source.Width, (int)source.Height);
			var roi = new Tizen.Multimedia.Rectangle(point, size);
			try
			{
				var barcodeLists = await BarcodeDetector.DetectAsync(source, roi);
				if (barcodeLists.Count() == 0)
					return null;

				var resultList = new List<Result>();

				foreach (var resultBarcode in barcodeLists)
				{
					var text = resultBarcode.Message;
					var rawbytes = new byte[source.Buffer.Length];
					source.Buffer.CopyTo(rawbytes, 0, source.Buffer.Length);

					ResultPoint[] resultPoint =
					{
						new ResultPoint(resultBarcode.Region.Points.ElementAt(0).X, resultBarcode.Region.Points.ElementAt(0).Y),
						new ResultPoint(resultBarcode.Region.Points.ElementAt(1).X, resultBarcode.Region.Points.ElementAt(1).Y),
						new ResultPoint(resultBarcode.Region.Points.ElementAt(2).X, resultBarcode.Region.Points.ElementAt(2).Y),
						new ResultPoint(resultBarcode.Region.Points.ElementAt(3).X, resultBarcode.Region.Points.ElementAt(3).Y)
					};

					BarcodeFormat format;
					switch (resultBarcode.Type)
					{
						case BarcodeType.Code128:
							format = BarcodeFormat.CODE_128;
							break;
						case BarcodeType.Code39:
							format = BarcodeFormat.CODE_39;
							break;
						case BarcodeType.Ean13:
							format = BarcodeFormat.EAN_13;
							break;
						case BarcodeType.Ean8:
							format = BarcodeFormat.EAN_8;
							break;
						case BarcodeType.I25:
							format = BarcodeFormat.ITF;
							break;
						case BarcodeType.QR:
							format = BarcodeFormat.QR_CODE;
							break;
						case BarcodeType.UpcA:
							format = BarcodeFormat.UPC_A;
							break;
						case BarcodeType.UpcE:
							format = BarcodeFormat.UPC_E;
							break;
						default:
							format = BarcodeFormat.All_1D;
							break;
					}

					resultList.Add(new Result(text, rawbytes, resultPoint, format));
					if (!multiple)
						break;
				}

				return resultList.ToArray();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}

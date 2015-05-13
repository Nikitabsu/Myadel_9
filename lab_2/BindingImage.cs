using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace ImageInfoViewer
{
	class BindingImage
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public string Format { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public float HorisontalResolution { get; set; }
		public float VerticalResolution { get; set; }
		public string CompressionType { get; set; }
		public int ColorDepth { get; set; }
		public string ChrominanceTable { get; set; }
		public string LuminanceTable { get; set; }

		private string GetCompressionType(Image image)
		{
			int compressionTagIndex = Array.IndexOf(image.PropertyIdList, 0x103);
			int Type = 0;
			if (compressionTagIndex > -1)
			{
				PropertyItem compressionTag = image.PropertyItems[compressionTagIndex];
				Type = BitConverter.ToInt16(compressionTag.Value, 0);
			}
			string Result = "No compression";
			switch (Type)
			{
				case 2:
					Result = "CCITT modified Huffman RLE";
					break;
				case 3:
					Result = "CCITT Group 3 fax encoding";
					break;
				case 4:
					Result = "CCITT Group 4 fax encoding";
					break;
				case 5:
					Result = "LZW";
					break;
				case 6:
					Result = "'old-style' JPEG";
					break;
				case 7:
					Result = "'new-style' JPEG";
					break;
				case 32773:
					Result = "Macintosh RLE";
					break;
			}
			return Result;
		}

		private string GetPcxCompressionType(byte flag)
		{
			if (flag == 0)
				return "No compression";
			else
				return "RLE";
		}
		private string GetChrominanceTable(Image image)
		{
			string tmp = "";
			int compressionTagIndex = Array.IndexOf(image.PropertyIdList, 0x5091);
			if (compressionTagIndex > -1)
			{
				PropertyItem compressionTag = image.PropertyItems[compressionTagIndex];
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
						tmp += String.Format("{0,6:X} ", compressionTag.Value[i * 16 + j]);
					tmp += "\n";
				}
			}
			else
				tmp = "не jpeg";
			return tmp;
		}
		private string GetLuminanceTable(Image image)
		{
			string tmp = "";
			int compressionTagIndex = Array.IndexOf(image.PropertyIdList, 0x5090);
			if (compressionTagIndex > -1)
			{
				PropertyItem compressionTag = image.PropertyItems[compressionTagIndex];
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
						tmp += String.Format("{0,6:X} ", compressionTag.Value[i * 16 + j]);
					tmp += "\n";
				}
			}
			else
				tmp = "не jpeg";
			return tmp;
		}
		public BindingImage(int num, string name, Image image)
		{
			ColorDepth = Image.GetPixelFormatSize(image.PixelFormat);
			Number = num;
			Name = name;
			Format = new ImageFormatConverter().ConvertToString(image.RawFormat);
			Height = image.Height;
			Width = image.Width;
			HorisontalResolution = image.HorizontalResolution;
			VerticalResolution = image.VerticalResolution;
			CompressionType = GetCompressionType(image);
			ChrominanceTable = GetChrominanceTable(image);
			LuminanceTable = GetLuminanceTable(image);
		}
	}
}

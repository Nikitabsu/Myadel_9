using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Windows.Controls;

namespace TrompeLeCode.HistogramSample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private variables

		private Microsoft.Win32.OpenFileDialog ofd;
		private System.Drawing.Bitmap bmp;

		private BitmapImage localImagePath = null;
		private PointCollection redColorHistogramPoints = null;
		private PointCollection greenColorHistogramPoints = null;
		private PointCollection blueColorHistogramPoints = null;
		private Double redColorHistogramMean;
		private Double greenColorHistogramMean;
		private Double blueColorHistogramMean;

		#endregion

		#region Public Properties

		public String ImageURL { get; set; }

		public Double BlueColorHistogramMean
		{
			get
			{
				return this.blueColorHistogramMean;
			}
			set
			{
				if (this.blueColorHistogramMean != value)
				{
					this.blueColorHistogramMean = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramMean"));
					}
				}
			}
		}

		public Double GreenColorHistogramMean
		{
			get
			{
				return this.greenColorHistogramMean;
			}
			set
			{
				if (this.greenColorHistogramMean != value)
				{
					this.greenColorHistogramMean = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramMean"));
					}
				}
			}
		}

		public Double RedColorHistogramMean
		{
			get
			{
				return this.redColorHistogramMean;
			}
			set
			{
				if (this.redColorHistogramMean != value)
				{
					this.redColorHistogramMean = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramMean"));
					}
				}
			}
		}

		public BitmapImage LocalImagePath
		{
			get
			{
				return this.localImagePath;
			}
			set
			{
				if (this.localImagePath != value)
				{
					this.localImagePath = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("LocalImagePath"));
					}
				}
			}
		}

		public PointCollection RedColorHistogramPoints
		{
			get
			{
				return this.redColorHistogramPoints;
			}
			set
			{
				if (this.redColorHistogramPoints != value)
				{
					this.redColorHistogramPoints = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramPoints"));
					}
				}
			}
		}

		public PointCollection GreenColorHistogramPoints
		{
			get
			{
				return this.greenColorHistogramPoints;
			}
			set
			{
				if (this.greenColorHistogramPoints != value)
				{
					this.greenColorHistogramPoints = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramPoints"));
					}
				}
			}
		}

		public PointCollection BlueColorHistogramPoints
		{
			get
			{
				return this.blueColorHistogramPoints;
			}
			set
			{
				if (this.blueColorHistogramPoints != value)
				{
					this.blueColorHistogramPoints = value;
					if (this.PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramPoints"));
					}
				}
			}
		}

		#endregion

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;

			ofd = new Microsoft.Win32.OpenFileDialog();
		}

		#endregion

		#region Event Handlers

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			ofd.Filter = "All Supported Image Files|*.jpg;*.jpeg;*.jfif;*.tif;*.tiff;*.dib;*.rle;*.bmp;*.png;*.ico;*.gif;*.exif";
			ofd.ShowReadOnly = true;
			ofd.Title = "Выберите изображения";
			if (ofd.ShowDialog() == true)
			{
				this.ImageURL = ofd.FileName;
				this.Cursor = Cursors.Wait;
				try
				{
					if (String.IsNullOrWhiteSpace(this.ImageURL))
					{
						MessageBox.Show("Image URL is mandatory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					String localFilePath = null;
					try
					{
						localFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
						using (WebClient client = new WebClient())
						{
							client.DownloadFile(this.ImageURL, localFilePath);
						}
					}
					catch (Exception)
					{
						MessageBox.Show("Invalid image URL. Image could not be retrieved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					using (bmp = new System.Drawing.Bitmap(localFilePath))
					{

						MemoryStream ms = new MemoryStream();
						bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						ms.Position = 0;
						BitmapImage bi = new BitmapImage();
						bi.BeginInit();
						bi.StreamSource = ms;
						bi.EndInit();

						this.LocalImagePath = bi;

						// RGB
						ImageStatistics rgbStatistics = new ImageStatistics(bmp);
						this.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
						this.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
						this.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);

						this.RedColorHistogramMean = rgbStatistics.Red.Mean;
						this.GreenColorHistogramMean = rgbStatistics.Green.Mean;
						this.BlueColorHistogramMean = rgbStatistics.Blue.Mean;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error generating histogram: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				finally
				{
					this.Cursor = Cursors.Arrow;
				}
			}
		}

		#endregion

		#region Private Methods

		private PointCollection ConvertToPointCollection(int[] values)
		{
			int max = values.Max();

			PointCollection points = new PointCollection();
			// first point (lower-left corner)
			points.Add(new Point(0, max));
			// middle points
			for (int i = 0; i < values.Length; i++)
			{
				points.Add(new Point(i, max - values[i]));
			}
			// last point (lower-right corner)
			points.Add(new Point(values.Length - 1, max));

			return points;
		}
		#endregion
		
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			double value = slider.Value;
			BrightnessCorrection filter = new BrightnessCorrection((int)Math.Round(value));
			this.Cursor = Cursors.Wait;
			try
			{
				if (String.IsNullOrWhiteSpace(this.ImageURL))
				{
					MessageBox.Show("Image URL is mandatory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				String localFilePath = null;
				try
				{
					localFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
					using (WebClient client = new WebClient())
					{
						client.DownloadFile(this.ImageURL, localFilePath);
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Invalid image URL. Image could not be retrieved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				using (bmp = new System.Drawing.Bitmap(localFilePath))
				{
					filter.ApplyInPlace(bmp);
					MemoryStream ms = new MemoryStream();
					bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					ms.Position = 0;
					BitmapImage bi = new BitmapImage();
					bi.BeginInit();
					bi.StreamSource = ms;
					bi.EndInit();
					this.LocalImagePath = bi;

					// RGB
					ImageStatistics rgbStatistics = new ImageStatistics(bmp);
					this.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
					this.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
					this.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);

					this.RedColorHistogramMean = rgbStatistics.Red.Mean;
					this.GreenColorHistogramMean = rgbStatistics.Green.Mean;
					this.BlueColorHistogramMean = rgbStatistics.Blue.Mean;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error generating histogram: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
			}
		}

		private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			double value = slider.Value;
			ContrastCorrection filter = new ContrastCorrection((int)Math.Round(value));
			this.Cursor = Cursors.Wait;
			try
			{
				if (String.IsNullOrWhiteSpace(this.ImageURL))
				{
					MessageBox.Show("Image URL is mandatory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				String localFilePath = null;
				try
				{
					localFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
					using (WebClient client = new WebClient())
					{
						client.DownloadFile(this.ImageURL, localFilePath);
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Invalid image URL. Image could not be retrieved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				using (bmp = new System.Drawing.Bitmap(localFilePath))
				{
					filter.ApplyInPlace(bmp);
					MemoryStream ms = new MemoryStream();
					bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					ms.Position = 0;
					BitmapImage bi = new BitmapImage();
					bi.BeginInit();
					bi.StreamSource = ms;
					bi.EndInit();
					this.LocalImagePath = bi;

					// RGB
					ImageStatistics rgbStatistics = new ImageStatistics(bmp);
					this.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
					this.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
					this.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);

					this.RedColorHistogramMean = rgbStatistics.Red.Mean;
					this.GreenColorHistogramMean = rgbStatistics.Green.Mean;
					this.BlueColorHistogramMean = rgbStatistics.Blue.Mean;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error generating histogram: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
			}
		}
	}
}

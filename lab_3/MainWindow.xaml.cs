using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace RasterOtr
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Point start;
		private Point end;
		private bool perm = false;
		private int mashtab = 20;
		private List<List<Rectangle>> arrayR;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!perm)
			{
				start = e.GetPosition(this);
				start.X = Convert.ToInt32((int)start.X / mashtab);
				start.Y = Convert.ToInt32((int)start.Y / mashtab);
				perm = true;
				drawPoints(start);
			}
			else
			{
				end = e.GetPosition(this);
				end.X = Convert.ToInt32((int)end.X / mashtab);
				end.Y = Convert.ToInt32((int)end.Y / mashtab);
				switch (getAlgoritm.SelectedIndex)
				{
					case 0:
						stepAlgorithm();
						break;
					case 1:
						CDA();
						break;
					case 2:
						brezenxem();
						break;
					default:
						break;
				}
				perm = false;
			}
		}

		private bool drawPoints(Point point)
		{
			Rectangle rectangle = arrayR.ElementAt((int)point.X).ElementAt((int)point.Y);
			rectangle.Fill = System.Windows.Media.Brushes.Blue;
			return true;
		}

		public void stepAlgorithm()
		{
			Stopwatch stime = new Stopwatch();
			stime.Start();

			int x1 = Convert.ToInt32(start.X), x2 = Convert.ToInt32(end.X), y1 = Convert.ToInt32(start.Y), y2 = Convert.ToInt32(end.Y);
			double dx = x2 - x1;
			double dy = y2 - y1;

			if (Math.Abs(y2 - y1) >= Math.Abs(x2 - x1))
			{

				if ((x1 == x2) && (y1 == y2))
					drawPoints(new Point(x1, y1));
				else
				{
					if (y2 < y1)
					{
						int tmp = x2;
						x2 = x1;
						x1 = tmp;

						tmp = y2;
						y2 = y1;
						y1 = tmp;
					}

					double k = dx / (double)dy;
					double q = x1 - k * y1;

					for (int y = y1; y < y2; y++)
					{
						int x = (int)(k * y + q);
						drawPoints(new Point(x, y));
					}
				}
			}
			else
			{
				if (x2 < x1)
				{
					int tmp = x2;
					x2 = x1;
					x1 = tmp;

					tmp = y2;
					y2 = y1;
					y1 = tmp;
				}

				double k = dy / (double)dx;
				double q = y1 - k * x1;

				for (int x = x1; x < x2; x++)
				{
					int y = (int)(k * x + q);
					drawPoints(new Point(x, y));
				}
			}
			drawPoints(end);

			stime.Stop();
			TimeSpan ts = stime.Elapsed;

			lTime.Content = String.Format("{0}", ts.TotalMilliseconds);
		}

		public void CDA()
		{
			Stopwatch stime = new Stopwatch();
			stime.Start();

			int x1 = Convert.ToInt32(start.X), x2 = Convert.ToInt32(end.X), y1 = Convert.ToInt32(start.Y), y2 = Convert.ToInt32(end.Y);

			double dx = x2 - x1;
			double dy = y2 - y1;

			if (Math.Abs(y2 - y1) <= Math.Abs(x2 - x1))
			{

				if ((x1 == x2) && (y1 == y2))
					drawPoints(new Point(x1, y1));
				else
				{
					if (x2 < x1)
					{
						int tmp = x2;
						x2 = x1;
						x1 = tmp;

						tmp = y2;
						y2 = y1;
						y1 = tmp;
					}

					double k = (double)dy / dx;
					int cele_y;
					double y = (double)y1;

					for (int x = x1; x <= x2; x++)
					{
						cele_y = (int)Math.Round(y);
						drawPoints(new Point(x, cele_y));
						y += k;
					}
				}
			}
			else
			{

				if (y2 < y1)
				{
					int tmp = x2;
					x2 = x1;
					x1 = tmp;

					tmp = y2;
					y2 = y1;
					y1 = tmp;
				}

				double k = (double)dx / dy;
				int cele_x;
				double x = (double)x1;
				for (int y = y1; y <= y2; y++)
				{
					cele_x = (int)Math.Round(x);
					drawPoints(new Point(cele_x, y));
					x += k;
				}
			}

			stime.Stop();
			TimeSpan ts = stime.Elapsed;

			lTime.Content = String.Format("{0}", ts.TotalMilliseconds);
		}
		public void brezenxem()
		{
			Stopwatch stime = new Stopwatch();
			stime.Start();

			int x1 = Convert.ToInt32(start.X), x2 = Convert.ToInt32(end.X), y1 = Convert.ToInt32(start.Y), y2 = Convert.ToInt32(end.Y);
			if ((x1 == x2) && (y1 == y2))
				drawPoints(new Point(x1, y1));
			else
			{
				int dx = Math.Abs(x2 - x1);
				int dy = Math.Abs(y2 - y1);
				int rozdil = dx - dy;

				int posun_x, posun_y;

				if (x1 < x2) posun_x = 1; else posun_x = -1;
				if (y1 < y2) posun_y = 1; else posun_y = -1;

				while ((x1 != x2) || (y1 != y2))
				{

					int p = 2 * rozdil;

					if (p > -dy)
					{
						rozdil = rozdil - dy;
						x1 = x1 + posun_x;
					}
					if (p < dx)
					{
						rozdil = rozdil + dx;
						y1 = y1 + posun_y;
					}
					drawPoints(new Point(x1, y1));
				}
			} 

			stime.Stop();
			TimeSpan ts = stime.Elapsed;

			lTime.Content = String.Format("{0}", ts.TotalMilliseconds);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			canvas.Children.Clear();
			perm = false;
			Line line = new Line();
			Size size = canvas.RenderSize;
			line.X1 = (size.Width / 2) - (size.Width / 2 % mashtab);
			line.Y1 = 0;
			line.X2 = (size.Width / 2) - (size.Width / 2 % mashtab);
			line.Y2 = size.Height;
			line.Stroke = System.Windows.Media.Brushes.Black;
			line.StrokeThickness = 3;
			canvas.Children.Add(line);
			line = new Line();
			line.X1 = 0;
			line.Y1 = (size.Height / 2) - (size.Height / 2 % mashtab);
			line.X2 = size.Width;
			line.Y2 = (size.Height / 2) - (size.Height / 2 % mashtab);
			line.Stroke = System.Windows.Media.Brushes.Black;
			line.StrokeThickness = 3;
			canvas.Children.Add(line);
			int x = 0, y = 0;
			arrayR = new List<List<Rectangle>>();
			while (x < size.Width - mashtab)
			{
				List<Rectangle> tmpList = new List<Rectangle>();
				while (y < size.Height - mashtab)
				{
					Rectangle rectangle = new Rectangle();
					rectangle.StrokeThickness = 0.5;
					rectangle.Stroke = System.Windows.Media.Brushes.Blue;
					rectangle.Width = mashtab;
					rectangle.Height = mashtab;
					Canvas.SetTop(rectangle, y);
					Canvas.SetLeft(rectangle, x);
					canvas.Children.Add(rectangle);
					y += mashtab;

					tmpList.Add(rectangle);
				}
				arrayR.Add(tmpList);
				x += mashtab;
				y = 0;
			}
		}
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			mashtab = (int)e.NewValue;
			Window_SizeChanged(null, null);
		}
	}
}
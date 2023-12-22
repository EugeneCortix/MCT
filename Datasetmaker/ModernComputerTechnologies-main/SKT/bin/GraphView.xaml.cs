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
using System.Windows.Shapes;

namespace SKT
{
    /// <summary>
    /// Логика взаимодействия для GraphView.xaml
    /// </summary>
    public partial class GraphView : Window
    {
        double a = 0;
        List<double> Bx;
        List<double> Bz;
        int N;
        double X;
        DrawingGroup drawingGroup = new DrawingGroup();
        public GraphView(List<double> Bx, List<double> Bz, int N,double X)
        {
            InitializeComponent();
            buildaxes();
            this.Bx = Bx;
            this.Bz = Bz;
            this.N = N;
            this.X = X;
            DrawPoints();

        }
        private void buildaxes()
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(Brushes.Black, 2);
            lines.Children.Add(new LineGeometry(new Point(a, GraphImage.Height), new Point(a, 0))); // z
            lines.Children.Add(new LineGeometry(new Point(a, GraphImage.Height / 2), new Point(GraphImage.Width, GraphImage.Height / 2))); // x

            myGeometryDrawing.Geometry = lines;
            drawingGroup.Children.Add(myGeometryDrawing);
            GraphImage.Source = new DrawingImage(drawingGroup);

        }
        private void DrawPoints()
        {
            GeometryDrawing BxGeometryDrawing = new GeometryDrawing();
           //GeometryDrawing BzGeometryDrawing = new GeometryDrawing();

            GeometryGroup points = new GeometryGroup();

            BxGeometryDrawing.Pen = new Pen(Brushes.Red, 3);
           //BzGeometryDrawing.Pen = new Pen(Brushes.Green, 3);

            for (int i=0; i<N; i++)
            {
                EllipseGeometry myElipse1 = new EllipseGeometry();
                //EllipseGeometry myElipse2 = new EllipseGeometry();
                myElipse1.Center = new Point(GetX(i * X / (N - 1)), GetY(Bx[i],Bx));
                //myElipse2.Center = new Point(GetX(i * X / (N - 1)), GetY(Bz[i], Bz));
                myElipse1.RadiusX = 1;
                myElipse1.RadiusY = 1;
                points.Children.Add(myElipse1);
               // myElipse2.RadiusX = 1;
               // myElipse2.RadiusY = 1;
               // points.Children.Add(myElipse2);
            }
           // points.Children.Add(new LineGeometry(new Point(a, 0 ), new Point(result, 0)));
            //points.Children.Add(new LineGeometry(new Point(0, GraphImage.Height ), new Point(result, GraphImage.Height )));

            BxGeometryDrawing.Geometry = points;
            drawingGroup.Children.Add(BxGeometryDrawing);

           //BzGeometryDrawing.Geometry = points;
           //drawingGroup.Children.Add(BzGeometryDrawing);

            GraphImage.Source = new DrawingImage(drawingGroup);

        }
        private double GetX(double X)
        {
            double x_min = 0,
                   x_max = this.X,
                   graphX_min = 0,
                   graphX_max = GraphImage.Width,
                   result;

            result = graphX_min - (graphX_min - graphX_max) * (X - x_min) / (x_max - x_min);
            return result;
        }
        private double GetY(double Y, List<double>B)
        {
            double y_min = B.Min(),
                   y_max = B.Max(),
                   graphY_min = GraphImage.Height,
                   graphY_max = 0,
                   result;

            result = graphY_min - (graphY_min - graphY_max) * (Y - y_min) / (y_max - y_min);
            return result;
        }

    }
}

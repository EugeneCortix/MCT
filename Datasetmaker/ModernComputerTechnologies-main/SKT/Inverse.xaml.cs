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
using System.Xml.Linq;

namespace SKT
{
    /// <summary>
    /// Логика взаимодействия для Inverse.xaml
    /// </summary>

    public partial class Inverse : Window
    {
        double a = 0; // Only for drawning

        DrawingGroup drawingGroup = new DrawingGroup();
        List<Element> elementsInv;
        List<DataInGrid> datasInGridInv;
        Dictionary<int, RectangleGeometry> dictionaryOfRectangle;
        List<GraphView> GraphList = new List<GraphView>();

        InverseProblem CalcInverse;
        double kx; // Scaling x
        double ky; // Scaling y
        double x;
        double z;

        public DirectProblem CalicDirectProblem;



        public Inverse()
        {
            InitializeComponent();
            buildaxes();
        }
        private void buildaxes()
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(Brushes.Black, 3);
            lines.Children.Add(new LineGeometry(new Point(a, graphImage.Height), new Point(a, 0))); // z
            lines.Children.Add(new LineGeometry(new Point(a, graphImage.Height / 2), new Point(graphImage.Width, graphImage.Height / 2))); // x

            myGeometryDrawing.Geometry = lines;
            drawingGroup.Children.Add(myGeometryDrawing);
            graphImage.Source = new DrawingImage(drawingGroup);

        }
        private void fieldButtonInv_Click(object sender, RoutedEventArgs e)
        {
            drawingGroup.Children.Clear();

            buildaxes();
            elementsInv = new List<Element>();
            datasInGridInv = new List<DataInGrid>();
            string yval = yValInv.Text;
            string xval = xValInv.Text;
            if (!(double.TryParse(yval, out double number1)) || !(double.TryParse(xval, out double number2)))
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
            }
            else
            {
                x = double.Parse(xval);
                z = double.Parse(yval);

                kx = (graphImage.Width - a) / x;
                ky = (graphImage.Height / 2) / z;

                DrawField(x / int.Parse(xCrushInv.Text), z / int.Parse(yCrushInv.Text));

                MakeElements();

                GridOfWInv.ItemsSource = datasInGridInv;

            }
        }
        private void DrawField(double dx, double dz)
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(Brushes.Red, 1);

            lines.Children.Add(new LineGeometry(new Point(a, graphImage.Height / 2 + z * ky), new Point(x * kx - a, graphImage.Height / 2 + z * ky)));
            lines.Children.Add(new LineGeometry(new Point(x * kx - a, graphImage.Height / 2), new Point(x * kx - a, graphImage.Height / 2 + z * ky)));

            for (int i = 1; i < double.Parse(xCrushInv.Text); i++)
            {
                lines.Children.Add(new LineGeometry(new Point(i * dx * kx, graphImage.Height / 2), new Point(i * dx * kx, graphImage.Height / 2 + z * ky)));
            }
            for (int j = 1; j < double.Parse(yCrushInv.Text); j++)
            {
                lines.Children.Add(new LineGeometry(new Point(a, j * dz * ky + graphImage.Height / 2), new Point(x * kx - a, j * dz * ky + graphImage.Height / 2)));
            }

            myGeometryDrawing.Geometry = lines;
            drawingGroup.Children.Add(myGeometryDrawing);
            graphImage.Source = new DrawingImage(drawingGroup);
        }
        private void MakeElements()
        {
            string zcrush = yCrushInv.Text;
            string xcrush = xCrushInv.Text;
            if (!(int.TryParse(zcrush, out int number1)) || !(int.TryParse(xcrush, out int number2)))
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
            }
            else if (int.Parse(xcrush) <= 0 || int.Parse(zcrush) <= 0)
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
            }
            else
            {
                double dx = x / int.Parse(xcrush);
                double dz = z / int.Parse(zcrush);

                for (int j = 0; j < int.Parse(zcrush); j++)
                    for (int i = 0; i < int.Parse(xcrush); i++)
                    {
                        Element el = new Element();
                        el.point = new Point() { X = i * dx, Y = -(z - j * dz) };
                        el.hx = dx;
                        el.hz = dz;
                        elementsInv.Add(el);
                        datasInGridInv.Add(new DataInGrid(i + j * int.Parse(xcrush), el.px, el.pz));
                    }
            }
        }
        private byte GetColor(double P, List<double> List_P)
        {
            double p_min = List_P.Min(),
                   p_max = List_P.Max(),
                   color_min = 255,
                   color_max = 100,
                   result;

            result = color_min - (color_min - color_max) * (P - p_min) / (p_max - p_min);

            return (byte)Math.Round(result);
        }
        private void DrawRect(double xe, double ze, byte color)
        {
            GeometryDrawing toRect = new GeometryDrawing();

            GeometryGroup Rectangles = new GeometryGroup();

            RectangleGeometry myRectangleGeometry;

            for (int i = 0; i < elementsInv.Count; i++)
            {
                if (
                    xe > elementsInv[i].point.X &&
                    xe < elementsInv[i].point.X + elementsInv[i].hx &&
                    ze > elementsInv[i].point.Y &&
                    ze < elementsInv[i].point.Y + elementsInv[i].hz)
                {
                    myRectangleGeometry = new RectangleGeometry();
                    myRectangleGeometry.Rect = new Rect(
                        elementsInv[i].point.X * graphImage.Width / x + 1,
                        -elementsInv[i].point.Y * (graphImage.Height / 2) / z + (graphImage.Height / 2) - elementsInv[i].hz * (graphImage.Height / 2) / z + 1,
                        elementsInv[i].hx * graphImage.Width / x - 2,
                        elementsInv[i].hz * (graphImage.Height / 2) / z - 2);

                    Rectangles.Children.Add(myRectangleGeometry);

                    //dictionaryOfRectangle.Add(i, myRectangleGeometry);

                    break;
                }
            }
            //Rectangles.Children.Clear();
            //toRect.Geometry = null;
            //if (drawingGroup.Children.Count > 2)
            //    drawingGroup.Children.RemoveAt(drawingGroup.Children.Count - 1);

            //foreach (var Rect in dictionaryOfRectangle)
            //{
            //    Rectangles.Children.Add(Rect.Value);
            //}

            toRect.Geometry = Rectangles;
            toRect.Brush = new SolidColorBrush(Color.FromRgb(color, color, color));

            drawingGroup.Children.Add(toRect);

            graphImage.Source = new DrawingImage(drawingGroup);
        }

        private void CALC_P_Click(object sender, RoutedEventArgs e)
        {
            double alfa_reg, celX, celZ;
            if (!int.TryParse(yCrushInv.Text, out int number1) || !int.TryParse(xCrushInv.Text, out int number2) || !double.TryParse(alfa_regular_tBox.Text, out alfa_reg))
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
                return;
            }
            else if (elementsInv == null)
            {
                MessageBox.Show("Задай сетку, дуралей!");
                return;

            }
            else if (CalicDirectProblem.N < elementsInv.Count)
            {
                MessageBox.Show("Сетка слишком подробная, дуралей!");
                return;
            }
            else
            {
                CalcInverse = new InverseProblem(CalicDirectProblem.S, elementsInv, CalicDirectProblem.N, CalicDirectProblem.X, alfa_reg);

                double tmp = 0;
                for (int i = 0; i< CalicDirectProblem.N; i++)
                {
                    tmp += (CalicDirectProblem.Bx[i] - CalcInverse.Bx[i]) * (CalicDirectProblem.Bx[i] - CalcInverse.Bx[i]) + (CalicDirectProblem.Bz[i] - CalcInverse.Bz[i]) * (CalicDirectProblem.Bz[i] - CalcInverse.Bz[i]); 
                }
               
                Func.Text = tmp.ToString();

                for (int i = 0; i < datasInGridInv.Count; i++)
                {
                    datasInGridInv[i].px = CalcInverse.Px[i];
                    datasInGridInv[i].pz = CalcInverse.Pz[i];
                }

                //drawingGroup = new DrawingGroup();
                dictionaryOfRectangle = new Dictionary<int, RectangleGeometry>();

                for (int i = 0; i < elementsInv.Count; i++)
                {
                    celX = elementsInv[i].point.X + elementsInv[i].hx / 2;
                    celZ = elementsInv[i].point.Y + elementsInv[i].hz / 2;

                    DrawRect(celX, celZ, GetColor(CalcInverse.Px[i], CalcInverse.Px));
                }
                //celX = elementsInv[0].point.X + elementsInv[0].hx / 2;
                //celZ = elementsInv[0].point.Y + elementsInv[0].hz / 2;

                //DrawRect(celX, celZ, GetColor(CalcInverse.Px[0], CalcInverse.Px));


                GridOfWInv.ItemsSource = datasInGridInv;
                GridOfWInv.Items.Refresh();
            }
        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            if (GraphList.Count == 0)
            {
                GraphList.Add(new GraphView(CalicDirectProblem.Bx, CalicDirectProblem.Bz, CalicDirectProblem.N, CalicDirectProblem.X));
                // GraphList[0].CalicDirectProblem = new DirectProblem(elements, int.Parse(NB.Text), elements[elements.Count - 1].point.X + elements[elements.Count - 1].hx);
                GraphList[0].Show();

            }
            else
            {
                GraphList[0].Close();
                GraphList.Clear();
                GraphList.Add(new GraphView(CalicDirectProblem.Bx, CalicDirectProblem.Bz, CalicDirectProblem.N, CalicDirectProblem.X));
                // GraphList[0].CalicDirectProblem = new DirectProblem(elements, int.Parse(NB.Text), elements[elements.Count - 1].point.X + elements[elements.Count - 1].hx);
                GraphList[0].Show();
            }
        }
        private void Graph1_Click(object sender, RoutedEventArgs e)
        {
            if (GraphList.Count == 0)
            {
                GraphList.Add(new GraphView(CalcInverse.Bx, CalcInverse.Bz, CalcInverse.N, CalcInverse.X));
                // GraphList[0].CalicDirectProblem = new DirectProblem(elements, int.Parse(NB.Text), elements[elements.Count - 1].point.X + elements[elements.Count - 1].hx);
                GraphList[0].Show();

            }
            else
            {
                GraphList[0].Close();
                GraphList.Clear();
                GraphList.Add(new GraphView(CalcInverse.Bx, CalcInverse.Bz, CalcInverse.N, CalcInverse.X));
                // GraphList[0].CalicDirectProblem = new DirectProblem(elements, int.Parse(NB.Text), elements[elements.Count - 1].point.X + elements[elements.Count - 1].hx);
                GraphList[0].Show();
            }
        }

    }
}

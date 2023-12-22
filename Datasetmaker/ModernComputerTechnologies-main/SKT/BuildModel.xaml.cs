// Modern computer technologies
using Microsoft.Win32;
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

namespace SKT
{
    /// <summary>
    /// Логика взаимодействия для BuildModel.xaml
    /// </summary>
    public partial class BuildModel : Window
    {
        bool flag; // Way of crushing
        double a = 0; // Only for drawning
        double kx; // Scaling x
        double ky; // Scaling y
        double x;
        double z;
        // For drawning
        double ex1;
        double ez1;
        int det;
        int xc; int zc;
        int NoN = 0;

        double xl;
        double xr;
        double zu;
        double zd;

        List<Element> elements;
        List<DataInGrid> datasInGrid;
        DrawingGroup drawingGroup = new DrawingGroup();
        Dictionary<int, RectangleGeometry> dictionaryOfRectangle;
        DirectProblem CalicDirectProblem;
        List<Inverse> InverseList = new List<Inverse>();

        public BuildModel()
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

            // Nodes
            NoN = int.Parse(NB.Text);
            for (int i = 0; i < NoN; i++)
            {
                LineGeometry node = new LineGeometry(new Point(graphImage.Width / NoN * i, graphImage.Height / 2 + 3),
                    new Point(graphImage.Width / NoN * i, graphImage.Height / 2 - 3));
                lines.Children.Add(node);
            }
            myGeometryDrawing.Geometry = lines;
            drawingGroup.Children.Add(myGeometryDrawing);
            graphImage.Source = new DrawingImage(drawingGroup);
        }
        private void updateVars()
        {
            drawingGroup.Children.Clear();

            dictionaryOfRectangle = new Dictionary<int, RectangleGeometry>();
            buildaxes();
            elements = new List<Element>();
            datasInGrid = new List<DataInGrid>();
            string yval = yVal.Text;
            string xval = xVal.Text;
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

                MakeElements();
                DrawField(x / int.Parse(xCrush.Text), z / int.Parse(yCrush.Text));

                xc = int.Parse(xCrush.Text);
                zc = int.Parse(yCrush.Text);

                GridOfW.ItemsSource = datasInGrid;
            }
        }
        private void FieldButton_Click(object sender, RoutedEventArgs e)
        {
            flag = true;
            updateVars();

            //drawingGroup.Children.Clear();

            //dictionaryOfRectangle = new Dictionary<int, RectangleGeometry>();
            //buildaxes();
            //elements = new List<Element>();
            //datasInGrid = new List<DataInGrid>();
            //string yval = yVal.Text;
            //string xval = xVal.Text;
            //if(!(double.TryParse(yval, out double number1)) || !(double.TryParse(xval, out double number2)))
            //{
            //    MessageBox.Show("Ты что вводишь, дуралей?");
            //}
            //else
            //{
            //    x = double.Parse(xval);
            //    z = double.Parse(yval);

            //    kx = (graphImage.Width - a) / x;
            //    ky = (graphImage.Height/2) / z;

            //    DrawField(x / int.Parse(xCrush.Text), z / int.Parse(yCrush.Text));

            //    MakeElements();

            //    GridOfW.ItemsSource = datasInGrid;
            //}
        }
        private void DrawField(double dx, double dz)
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(Brushes.Red, 1);

            lines.Children.Add(new LineGeometry(new Point(a, graphImage.Height / 2 + z * ky), new Point(x * kx - a, graphImage.Height / 2 + z * ky)));
            lines.Children.Add(new LineGeometry(new Point(x * kx - a, graphImage.Height / 2), new Point(x * kx - a, graphImage.Height / 2 + z * ky)));

            if (flag)
            {
                for (int i = 1; i < double.Parse(xCrush.Text); i++)
            {
                lines.Children.Add(new LineGeometry(new Point(i * dx * kx, graphImage.Height / 2), new Point(i * dx * kx, graphImage.Height / 2 + z * ky)));
            }
            for (int j = 1; j < double.Parse(yCrush.Text); j++)
            {
                lines.Children.Add(new LineGeometry(new Point(a, j * dz * ky + graphImage.Height / 2), new Point(x * kx - a, j * dz * ky + graphImage.Height / 2)));
            }
            }
            else
            {
                foreach (var element in elements)
                {
                    Rect rect = new Rect();
                    rect.X = element.point.X * kx;
                    rect.Y = graphImage.Height / 2 + (-element.point.Y - element.hz) * ky;
                    rect.Width = element.hx * kx;
                    rect.Height = element.hz * ky;
                    RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                    myRectangleGeometry.Rect = rect;
                    lines.Children.Add(myRectangleGeometry);
                }
            }

            myGeometryDrawing.Geometry = lines;
            drawingGroup.Children.Add(myGeometryDrawing);
            graphImage.Source = new DrawingImage(drawingGroup);
        }
        private void MakeElements()
        {
            if (flag)
            {
                string zcrush = yCrush.Text;
            string xcrush = xCrush.Text;
            if (!(int.TryParse(zcrush, out int number1)) || !(int.TryParse(xcrush, out int number2)))
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
            }
            else if (int.Parse(xcrush) <=0 || int.Parse(zcrush) <= 0)
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
            }
            else
            {
                double dx = x / int.Parse(xcrush);
                double dz = z / int.Parse(zcrush);

                for(int j = 0; j < int.Parse(zcrush); j++)
                    for(int i = 0; i < int.Parse(xcrush); i++)
                    {
                        Element el = new Element();
                        el.point = new Point() {X = i * dx, Y = - (z - j * dz) };
                        el.hx = dx;
                        el.hz = dz;
                        elements.Add(el);
                        datasInGrid.Add(new DataInGrid(i + j * int.Parse(xcrush), el.px, el.pz));
                    }
            }
            }

            else
            {

                if (!(double.TryParse(RectXl.Text, out double number1)) || !(double.TryParse(RectXr.Text, out double number2))
                    || !(double.TryParse(RectZd.Text, out double number3)) || !(double.TryParse(RectZu.Text, out double number4))
                    || !(int.TryParse(DetailRect.Text, out int number5)))
                {
                    MessageBox.Show("Ты что вводишь, дуралей?");
                }
                else if (int.Parse(DetailRect.Text) < 1 || double.Parse(RectZd.Text) > 0 || double.Parse(RectZu.Text) > 0
                     || double.Parse(RectXl.Text) < 0 || double.Parse(RectXr.Text) < 0)
                {
                    MessageBox.Show("Ты что вводишь, дуралей?");
                }
                else
                {
                    det = int.Parse(DetailRect.Text);
                    xl = double.Parse(RectXl.Text);
                    xr = double.Parse(RectXr.Text);
                    zu = double.Parse(RectZd.Text);
                    zd = double.Parse(RectZu.Text);

                    if (xl > xr)
                    {
                        double t = xl;
                        xl = xr;
                        xr = t;
                    }
                    if (zd > zu)
                    {
                        double t = zd;
                        zd = zu;
                        zu = t;
                    }

                    List<Element> eli = new List<Element>();

                    Element el1 = new Element();
                    el1.point = new Point() { X = 0, Y = zu };
                    el1.hx = xl;
                    el1.hz = -zu;
                    eli.Add(el1);

                    Element el2 = new Element();
                    el2.point = new Point() { X = xr - (xr - xl), Y = zu };
                    el2.hx = (xr - xl);
                    el2.hz = -zu;
                    eli.Add(el2);

                    Element el3 = new Element();
                    el3.point = new Point() { X = x - (x - xr), Y = zu };
                    el3.hx = (x - xr);
                    el3.hz = -zu;
                    eli.Add(el3);

                    Element el4 = new Element();
                    el4.point = new Point() { X = 0, Y = zu + (zd - zu) };
                    el4.hx = xl;
                    el4.hz = -(zd - zu);
                    eli.Add(el4);

                    Element el5 = new Element();
                    el5.point = new Point() { X = xr - (xr - xl), Y = zu + (zd - zu) };
                    el5.hx = (xr - xl);
                    el5.hz = -(zd - zu);
                    eli.Add(el5);

                    Element el6 = new Element();
                    el6.point = new Point() { X = x - (x - xr), Y = zu + (zd - zu) };
                    el6.hx = (x - xr);
                    el6.hz = -(zd - zu);
                    eli.Add(el6);

                    Element el7 = new Element();
                    el7.point = new Point() { X = 0, Y = zd - (zd + z) }; // parameter z > 0
                    el7.hx = xl;
                    el7.hz = z + zd;
                    eli.Add(el7);

                    Element el8 = new Element();
                    el8.point = new Point() { X = xr - (xr - xl), Y = zd - (zd + z) };
                    el8.hx = (xr - xl);
                    el8.hz = z + zd;
                    eli.Add(el8);

                    Element el9 = new Element();
                    el9.point = new Point() { X = x - (x - xr), Y = zd - (zd + z) };
                    el9.hx = (x - xr);
                    el9.hz = z + zd;
                    eli.Add(el9);

                    //ind for datasInGrid
                    int ind = 0;

                    foreach (Element el in eli)
                    {
                        for (int i = 0; i < det; i++)
                            for (int j = 0; j < det; j++)
                            {
                                Element elem = new Element();
                                elem.point = new Point() { X = el.point.X + el.hx / det * i, Y = el.point.Y + el.hz / det * j };
                                elem.hz = el.hz / det;
                                elem.hx = el.hx / det;
                                elements.Add(elem);
                                datasInGrid.Add(new DataInGrid(ind, el.px, el.pz));
                                ind++;
                            }
                    }


                }
            }

            /*
             Map of the area crushing 
             
            -------------------------
            |   1   |   2   |   3   |
            -------------------------
            |   4  |    5   |   6   |
            ------------------------- 
            |   7   |   8   |   9   |
            -------------------------

             */
        }
        private void DrawRect(double xe, double ze)
        {
            GeometryDrawing toRect = new GeometryDrawing();

            GeometryGroup Rectangles = new GeometryGroup();

            for (int i = 0; i < elements.Count; i++)
            {
                if (
                    xe / graphImage.Width * x > elements[i].point.X &&
                    xe / graphImage.Width * x < elements[i].point.X + elements[i].hx &&
                    -(ze - (graphImage.Height / 2)) / (graphImage.Height / 2) * z > elements[i].point.Y &&
                    -(ze - (graphImage.Height / 2)) / (graphImage.Height / 2) * z < elements[i].point.Y + elements[i].hz)
                {
                    if (elements[i].px == 0)
                    {
                        elements[i].px = datasInGrid[i].px = 1;
                        elements[i].pz = datasInGrid[i].pz = 1;

                        RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                        myRectangleGeometry.Rect = new Rect(
                            elements[i].point.X * graphImage.Width / x + 1,
                            -elements[i].point.Y * (graphImage.Height / 2) / z + (graphImage.Height / 2) - elements[i].hz * (graphImage.Height / 2) / z + 1,
                            elements[i].hx * graphImage.Width / x - 2,
                            elements[i].hz * (graphImage.Height / 2) / z - 2);

                        dictionaryOfRectangle.Add(i, myRectangleGeometry);
                    }
                    else
                    {
                        dictionaryOfRectangle.Remove(i);
                        elements[i].px = datasInGrid[i].px = 0;
                        elements[i].pz = datasInGrid[i].pz = 0;
                    }
                    break;
                }
            }
            Rectangles.Children.Clear();
            toRect.Geometry = null;
            if (drawingGroup.Children.Count > 2)
            drawingGroup.Children.RemoveAt(drawingGroup.Children.Count - 1);

            foreach(var Rect in dictionaryOfRectangle)
            {
                Rectangles.Children.Add(Rect.Value);
            }

            toRect.Geometry = Rectangles;
            toRect.Brush = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            drawingGroup.Children.Add(toRect);

            graphImage.Source = new DrawingImage(drawingGroup);
        }
        private void graphImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (elements !=null)
            {
                ex1 = e.GetPosition(graphImage).X;
                ez1 = e.GetPosition(graphImage).Y;
            }
                
                //DrawRect(e.GetPosition(graphImage).X, e.GetPosition(graphImage).Y);

          //  GridOfW.Items.Refresh();
        }
        private void GridOfW_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataInGrid data = GridOfW.SelectedItem as DataInGrid;
            elements[data.i].px = data.px;
            elements[data.i].pz = data.pz;

            DrawRect(elements[data.i].point.X * graphImage.Width / x + 2, elements[data.i].point.Y * (graphImage.Height / 2) / z + (graphImage.Height / 2) - elements[data.i].hz * (graphImage.Height / 2) / z + 2);
        }
        private void GraphImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double ex2;
            double ez2;

            if (elements != null)
            {
                ex2 = e.GetPosition(graphImage).X;
                ez2 = e.GetPosition(graphImage).Y;
            }
            else return;
            //Switching
            if(ex1 > ex2)
            {
                double stack = ex1;
                ex1 = ex2;
                ex2 = stack;
            }
            if (ez1 > ez2)
            {
                double stack = ez1;
                ez1 = ez2;
                ez2 = stack;
            }
            double elsizex = graphImage.Width / double.Parse(xCrush.Text);
            double elsizez = graphImage.Height / double.Parse(yCrush.Text)/2;
            while(ez2 >= ez1)
            {
                double exclone = ex1;
                while(exclone <= ex2)
                {
                    DrawRect(exclone, ez1);
                    exclone += elsizex;
                }
                ez1 += elsizez;
            }
            GridOfW.Items.Refresh();
        }
        private void CALC_B_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(NB.Text, out int number1) || int.Parse(NB.Text) <= 0)
            {
                MessageBox.Show("Ты что вводишь, дуралей?");
                return;
            }
            else if (elements == null)
            {
                MessageBox.Show("Задай сетку, дуралей!");
                return;
            }


            CalicDirectProblem = new DirectProblem(elements, int.Parse(NB.Text), elements[elements.Count - 1].point.X + elements[elements.Count - 1].hx);

            if (InverseList.Count == 0)
            {
                InverseList.Add(new Inverse());
                InverseList[0].CalicDirectProblem = CalicDirectProblem;
                InverseList[0].Show();
                
            }
            else
            {
                InverseList[0].Close();
                InverseList.Clear();
                InverseList.Add(new Inverse());
                InverseList[0].CalicDirectProblem = CalicDirectProblem;
                InverseList[0].Show();
            }

        }
        private void RectButton_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            updateVars();
        }
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files | *.txt";
            saveFileDialog.Title = "Save Configurations as...";
            if (saveFileDialog.ShowDialog() == false)
                return;

            string s = "";
            s += NoN.ToString() + '\n';
            if (flag) s += '1';
            else s += 0;
            s += '\n';
            s += x.ToString() + ' ' + z.ToString() + '\n'; //FieldSize
            if (flag)
            {
                s += xc.ToString() + ' ' + zc.ToString() + '\n';
            }
            else
            {
                // Crushing-2
                s += xl.ToString() + ' ' + xr.ToString() + ' ' + zu.ToString() + ' ' + zd.ToString() + '\n';
            }
            // Table brushung
            s += datasInGrid.Count.ToString() + '\n';
            foreach (var item in datasInGrid)
            {
                s += item.i.ToString() + ' ' + item.px.ToString() + ' ' + item.pz.ToString() + '\n';
            }
            System.IO.File.WriteAllText(saveFileDialog.FileName, s);
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open File";
            if (openFileDialog.ShowDialog() == false)
                return;
            string[] fileText = System.IO.File.ReadAllLines(openFileDialog.FileName);
            //
            NB.Text = fileText[0];
            // Type of Crushing
            if (fileText[1] == "0") flag = false;
            else flag = true;
            //Field
            double dx = 0; double dz = 0; // For drawning
            string[] sizes = fileText[2].Split(' ');
            xVal.Text = sizes[0];
            yVal.Text = sizes[1];
            dx = double.Parse(sizes[0]);
            dz = double.Parse(sizes[1]);
            // Spliting
            string[] splits = fileText[3].Split(' ');
            if (flag)
            {
                xCrush.Text = splits[0];
                yCrush.Text = splits[1];
                dx /= double.Parse(splits[0]);
                dz /= double.Parse(splits[1]);
            }
            else
            {
                RectXl.Text = splits[0];
                RectXr.Text = splits[1];
                RectZu.Text = splits[2];
                RectZd.Text = splits[3];
            }
            // DatasInGrid
            updateVars();
            int datval = int.Parse(fileText[4]);
            for (int i = 5; i < datval + 5; i++) //Till the end
            {
                string[] gridatas = fileText[i].Split(' ');
                datasInGrid.Add(new DataInGrid(int.Parse(gridatas[0]), double.Parse(gridatas[1]),
                    double.Parse(gridatas[2])));
            }
            // Building
            MakeElements();
            DrawField(dx, dz);
        }

    }
    public class DataInGrid
    {
        public DataInGrid(int i, double px, double pz)
        {
            this.i = i;
            this.px = px;
            this.pz = pz;
        }
        public int i { get; set; }
        public double px { get; set; }
        public double pz { get; set; }
    }


}

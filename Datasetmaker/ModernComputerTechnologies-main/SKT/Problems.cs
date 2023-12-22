using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace SKT

{
    // The struct of an element
    /*
     *-------------*
     |             |
     |hz           |
     |             |
     |      hx     |
     p-------------*
         
         */
    public class Element
    {
        public Point point { get; set; }
        public double hx { get; set; }
        public double hz { get; set; }
        public double px { get; set; }
        public double pz { get; set; }
        public Element()
        {
            px = 0;
            pz = 0;
        }
    }

    public class DirectProblem
    {
        public List<double> Bx { get; set; }
        public List<double> Bz { get; set; }
        public List<Element> elements;
        public int N;
        public double X;
        public double[] S;

        public DirectProblem(List<Element> elements, int N, double X)
        {
            this.elements = elements;
            this.N = N;
            this.X = X;
            Calc();
        }
        public void Calc()
        {
            Bx = new List<double>();
            Bz = new List<double>();
            S = new double[2 * N];

            if (Bx.Count != 0 || Bz.Count != 0)
            {
                Bx.Clear();
                Bz.Clear();
            }
            for (int i = 0; i < N; i++) 
            {
                Bx.Add(0);
                Bz.Add(0);

                foreach (var element in elements)
                {
                    double x_loc = i * X / (N - 1) - (element.point.X + element.hx / 2), 
                           z_loc =  20.0 - (element.point.Y + element.hz / 2), 
                           r = Math.Sqrt(x_loc * x_loc + z_loc * z_loc), 
                           mes = element.hx * element.hz;

                    Bx[i] += mes / (4.0 * Math.PI * r * r * r) * (
                        element.px * (3.0 * x_loc * x_loc / (r * r) - 1.0) +
                        element.pz * (3 * x_loc * z_loc) / (r * r));

                    Bz[i] += mes / (4.0 * Math.PI * r * r * r) * (
                        element.px * (3.0 * x_loc * z_loc) / (r * r) +
                        element.pz * (3.0 * z_loc * z_loc / (r * r) - 1.0));
                }
            }

            for (int i = 0; i < 2 * N; i+=2)
            {
                S[i] = Bx[i / 2];
                S[i + 1] = Bz[i / 2];
            }

        }
    }
    public class InverseProblem
    {
        private List<List<double>> L;
        private double[,] A;
        private double[] b, S;
        private double alfa_regular;
        public List<double> Px { get; set; }
        public List<double> Pz { get; set; }
        public List<Element> elements;
        public int N;
        public double X;
        public List<double> Bx { get; set; }
        public List<double> Bz { get; set; }



        public InverseProblem(double[] S, List<Element> elements, int N, double X, double alfa_regular)
        {
            this.S = S;
            this.elements = elements;
            this.N = N;
            this.X = X;
            this.alfa_regular = alfa_regular;
            Calc();
        }
        private double Calc_Koeff(Element el) // mes(Omega)/4*pi
        {
            return (el.hx * el.hz) / (4 * Math.PI);
        }
        private void CalcL()
        {
            L = new List<List<double>>();
            double x_loc, z_loc, r;

            for (int i = 0; i < 2 * N; i+=2)
            {
                L.Add(new List<double>());
                L.Add(new List<double>());
                L[i] = new List<double>();
                L[i + 1] = new List<double>();

                for (int j = 0; j < 2 * elements.Count; j+=2)
                {
                    L[i].Add(new double());
                    L[i].Add(new double());
                    L[i + 1].Add(new double());
                    L[i + 1].Add(new double());
                    x_loc = (i / 2) * X / (N - 1) - (elements[j / 2].point.X + elements[j / 2].hx / 2);
                    z_loc = 20 - (elements[j / 2].point.Y + elements[j / 2].hz / 2);

                    r = Math.Sqrt(x_loc * x_loc + z_loc * z_loc);

                    L[i][j] = Calc_Koeff(elements[j / 2]) / (r * r * r) *
                        ((3 * x_loc * x_loc) / (r * r) - 1);
                    
                    L[i][j + 1] = Calc_Koeff(elements[j / 2]) / (r * r * r) *
                        (3 * x_loc * z_loc) / (r * r);

                    L[i + 1][j] = Calc_Koeff(elements[j / 2]) / (r * r * r) *
                        (3 * x_loc * z_loc) / (r * r);

                    L[i + 1][j + 1] = Calc_Koeff(elements[j / 2]) / (r * r * r) *
                        ((3 * z_loc * z_loc) / (r * r) - 1);
                }
            }
        }
        private void CalcA()
        {
            int A_size = L[0].Count;
            A = new double[A_size, A_size];

            for (int i = 0; i < A_size; i++)
                for (int j = 0; j < A_size; j++)
                {
                    A[i, j] = 0;
                    for (int k = 0; k < L.Count; k++)
                        A[i, j] += L[k][i] * L[k][j];
                    
                    if (i == j)
                        A[i, j] += alfa_regular;

                }
        }
        private void CalcB()
        {
            int b_size = L[0].Count;
            b = new double[b_size];

            for (int i = 0; i < b_size; i++)
                for (int j = 0; j < L.Count; j++)
                    b[i] += L[j][i] * S[j];
        }
        private void CalcBxz()
        {
            Bx = new List<double>();
            Bz = new List<double>();
            S = new double[2 * N];

            if (Bx.Count != 0 || Bz.Count != 0)
            {
                Bx.Clear();
                Bz.Clear();
            }
            for (int i = 0; i < N; i++)
            {
                Bx.Add(0);
                Bz.Add(0);

                foreach (var element in elements)
                {
                    double x_loc = i * X / (N - 1) - (element.point.X + element.hx / 2),
                           z_loc = 20.0 - (element.point.Y + element.hz / 2),
                           r = Math.Sqrt(x_loc * x_loc + z_loc * z_loc),
                           mes = element.hx * element.hz;

                    Bx[i] += mes / (4.0 * Math.PI * r * r * r) * (
                        element.px * (3.0 * x_loc * x_loc / (r * r) - 1.0) +
                        element.pz * (3 * x_loc * z_loc) / (r * r));

                    Bz[i] += mes / (4.0 * Math.PI * r * r * r) * (
                        element.px * (3.0 * x_loc * z_loc) / (r * r) +
                        element.pz * (3.0 * z_loc * z_loc / (r * r) - 1.0));
                }
            }
        }

        public void Calc()
        {

            //calc L
            CalcL();

            //calc A = LT * L
            CalcA();

            //calc b = LT * B
            CalcB();

            //calc gauss Ax=b
            SLAE_Full solve = new SLAE_Full(b.Length);

            for (int i = 0; i < b.Length; i++)
            {
                solve[i] = b[i];
                for (int j = 0; j < b.Length; j++)
                    solve[i, j] = A[i, j];

            }

            double[] result = new double[b.Length];

            result = solve.solveSLAE();

            Px = new List<double>();
            Pz = new List<double>();
            for (int i = 0; i < result.Length; i+=2)
            {
                Px.Add(result[i]);
                Pz.Add(result[i + 1]);
                elements[i / 2].px = Px[i / 2];
                elements[i / 2].pz = Pz[i / 2];
            }

            CalcBxz();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT
{
    public class SLAE_Full
    {
        private double[,] A;
        private double[] b;

        public SLAE_Full(double[,] A, double[] b)
        {
            this.A = A;
            this.b = b;
        }

        public double this[int iInd, int jInd]
        {
            get => A[iInd, jInd];
            set => A[iInd, jInd] = value;
        }

        public double this[int iInd]
        {
            get => b[iInd];
            set => b[iInd] = value;
        }


        public SLAE_Full(int nSLAE)
        {
            A = new double[nSLAE, nSLAE];
            b = new double[nSLAE];
        }
        public void resizeSLAE(int nSLAE)
        {
            if (nSLAE == b.Length) return;
            A = new double[nSLAE, nSLAE];
            b = new double[nSLAE];
        }
        public void solveSLAE(double[] ans)
        {
            int nSLAE = b.Length;
            if (ans.Length != nSLAE)
                throw new Exception("Size of the input array is not compatable with size of SLAE");




            for (int i = 0; i < nSLAE; i++)
            {
                double del = A[i, i];
                double absDel = Math.Abs(del);
                int iSwap = i;


                for (int j = i + 1; j < nSLAE; j++) // ищем максимальный элемент по столбцу
                {
                    if (absDel < Math.Abs(A[j, i]))
                    {
                        del = A[j, i];
                        absDel = Math.Abs(del);
                        iSwap = j;
                    }
                }

                if (iSwap != i)
                {
                    double buf;
                    for (int j = i; j < nSLAE; j++)
                    {
                        buf = A[i, j];
                        A[i, j] = A[iSwap, j];
                        A[iSwap, j] = buf;
                    }
                    buf = b[i];
                    b[i] = b[iSwap];
                    b[iSwap] = buf;
                }

                for (int j = i; j < nSLAE; j++)
                    A[i, j] /= del;

                b[i] /= del;

                for (int j = i + 1; j < nSLAE; j++)
                {
                    if (A[j, i] == 0) continue;

                    double el = A[j, i];
                    for (int k = i; k < nSLAE; k++)
                    {
                        A[j, k] -= A[i, k] * el;
                    }

                    b[j] -= b[i] * el;
                }
            }

            for (int i = nSLAE - 1; i > -1; i--)
            {
                for (int j = i + 1; j < nSLAE; j++)
                    b[i] -= ans[j] * A[i, j];
                ans[i] = b[i];
            }
        }


        public double[] solveSLAE()
        {
            double[] ans = new double[b.Length];
            solveSLAE(ans);
            return ans;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    public class Second_neiron : First_neiron<double>
    {
        public double[] delta;
        public double deltas;

        public new dynamic inputs;
        public Second_neiron(List<First_neiron<double>> inputs) : base(null)
        {
            this.inputs = new List<First_neiron<double>>();
            foreach (First_neiron<double> input in inputs)
            {
                this.inputs.Add(input);
                this.weights.Add(this.random.NextDouble() - 0.5);
            }
            this.delta = new double[inputs.Count];
        }
        public Second_neiron(List<Second_neiron> inputs) : base(null)
        {
            this.inputs = new List<Second_neiron>();
            foreach (Second_neiron input in inputs)
            {
                this.inputs.Add(input);
                this.weights.Add(this.random.NextDouble() - 0.5);
            }
            this.delta = new double[inputs.Count];
        }

        public new void setsumfunc()
        {
            try
            {
                this.summ = 0;
                for (int i = 0; i < this.inputs.Count; i++)
                {
                    this.summ += this.inputs[i].getactivfunc() * this.weights[i];
                }
            }
            catch
            {
                this.summ = this.inputs[0].getactivfunc();
            }
        }

        public new Double getsumfunc()
        {
            setsumfunc();
            return summ;
        }

        public new Double getactivfunc()
        {
            return this.activationfunc.Invoke(getsumfunc());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    public abstract class Base_neiron<T>
    {
        public List<T> inputs = new List<T>();
        public List<Double> weights = new List<Double>();
        public Func<T, Double> activationfunc;
        public Random random = new Random();

        public T summ;
        public Base_neiron(List<T> inputs)
        {
            foreach (T input in inputs)
            {
                this.inputs.Add(input);
                this.weights.Add(random.NextDouble());
            }
        }

        public T getsumfunc()
        {
            setsumfunc();
            return summ;
        }

        public void setsumfunc() { }

        public void setactivfunc(Func<T, Double> activationfunc)
        {
            this.activationfunc = activationfunc;
        }

        public Double getactivfunc()
        {
            return this.activationfunc.Invoke(this.summ);
        }
    }
}

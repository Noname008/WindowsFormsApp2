using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    public class First_neiron<T> : Base_neiron<T>
    {
        public First_neiron(List<T> inputs) : base(new List<T>()) { }
        public First_neiron(T input) : base(new List<T>())
        {
            this.inputs.Add(input);
            this.weights.Add(1);
        }

        public new Double getactivfunc()
        {
            return this.activationfunc.Invoke(this.inputs[0]);
        }
    }
}

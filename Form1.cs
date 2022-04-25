using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        const double Alpha = 0.2;
        const double Eps = 0.2;
        String fol = "..\\..\\selections\\";



        List<First_neiron<Color>> first = new List<First_neiron<Color>>();
        List<First_neiron<double>> first_ = new List<First_neiron<double>>();
        List<Second_neiron> second = new List<Second_neiron>();
        List<Second_neiron> final = new List<Second_neiron>();

        Image image;
        Bitmap[][] sample;
        Bitmap sourse;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.sample = new Bitmap[10][];
            for (int i = 0; i < 10; i++) {
                this.sample[i] = new Bitmap[8];
                for (int j = 0; j < 8; j++)
                {
                    this.sample[i][j] = new Bitmap(Image.FromFile(fol + i + "\\" + j + ".jpg", true), 10, 10);
                }
            }
        }

        private void load_save(Bitmap sourse)
        {
            if (first.Count == 0)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        first.Add(new First_neiron<Color>(sourse.GetPixel(i, j)));
                        first.Last().setactivfunc(activ_first);
                        first_.Add(new First_neiron<double>(first.Last().getactivfunc()));
                        first_.Last().setactivfunc(activ_first);
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    TextBox a = new TextBox();
                    a.Width = 150;
                    a.Text = 0 + "";
                    a.ReadOnly = true;
                    a.Margin = new Padding();
                    tableLayoutPanel1.Controls.Add(a);
                    second.Add(new Second_neiron(first_));
                    second[i].setactivfunc(activ_second);
                }

                for (int i = 0; i < 10; i++)
                {
                    TextBox a = new TextBox();
                    a.Width = 150;
                    a.Text = 0 + "";
                    a.ReadOnly = true;
                    flowLayoutPanel4.Controls.Add(a);
                    final.Add(new Second_neiron(second));
                    final[i].setactivfunc(activ_final);
                }
            }
            else
            {
                set_sourse(sourse);
            }
            update();
        }

        private void set_sourse(Bitmap sourse)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    first[j * 10 + i].inputs[0] = sourse.GetPixel(i, j);
                }
            }
        }

        private void load(object sender, EventArgs e)
        {
            this.image = load();
            if (this.image != null)
            {
                pictureBox1.Image = this.image;
                this.sourse = new Bitmap(this.image,10,10);
                pictureBox2.Image = new Bitmap(this.sourse,200,200);
                load_save(this.sourse);
            }
        }

        private Image load()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "jpg files (*.jpg)|*.jpg";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return Image.FromFile((String)openFileDialog.FileName, true);
                }
            }
            Console.Error.WriteLine("Error load image");
            return null;
        }

        private void update()
        {
            update_first();
            try
            {
                for (int i = 0; i < second.Count; i++)
                {
                    tableLayoutPanel1.Controls[i].Text = second[i].getactivfunc() + "";
                }
                for (int i = 0; i < final.Count; i++)
                {
                    flowLayoutPanel4.Controls[i].Text = i + ": " + final[i].getactivfunc();
                }
            }
            catch
            {
                Console.Error.WriteLine("Text update error");
            }
        }

        private void update_first()
        {
            for (int i = 0; i < first.Count; i++)
            {
                first_[i].inputs[0] = first[i].getactivfunc();
            }
        }





        // активация с помощью сигмоидальной функции
        //L[k].z[i] = 1 / (1 + Math.Exp(-y));
        //L[k].df[i] = L[k].z[i] * (1 - L[k].z[i]);

        // активация с помощью гиперболического тангенса
        //L[k].z[i] = Math.Tanh(y);
        //L[k].df[i] = 1 - L[k].z[i] * L[k].z[i];

        // активация с помощью ReLU
        //L[k].z[i] = y > 0 ? y : 0;
        //L[k].df[i] = y > 0 ? 1 : 0;

        Thread thread;

        private void Network(object sender, EventArgs e)
        {
            load_save(sample[0][0]);
            this.thread = new Thread(toError);
            this.thread.Start();
        }

        private void Thread_abort(object sender, EventArgs e)
        {
            this.thread.Abort();
        }

        private void toError()
        {
            int Epoch = 0;
            double[] a = new double[10];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 0;
            }
            double error = 100;
            while (error > Eps)
            {
                run_epoch(a, out error);
                Console.WriteLine(++Epoch + ":" + error);
            }
        }

        private void run_epoch(double[] a, out double errorsum)
        {
            double error = 0;
            errorsum = 0;
            for (int i = 0; i < 10; i++)
            {
                a[i] = 1;
                for (int j = 0; j < 8; j++)
                {
                    //load_save(sample[i][j]);
                    set_sourse(sample[i][j]);
                    update_first();
                    Backwarderror(a, out error);
                    errorsum += error * error / 2;
                }
                a[i] = 0;
            }
        }

        void Backwarderror(double[] truue,out double error)
        {
            error = 0;
            for (int i = 0; i < final.Count; i++)
            {
                double e = final[i].getactivfunc() - truue[i];
                final[i].deltas = e * df(final[i].getactivfunc());
                error += e * e / 2;
            }
            update_delt();
            update_weights(second);
            update_weights(final);

            /*List<Thread> threads = new List<Thread>();
            threads.Add(new Thread(() => update_delt()));
            threads.Add(new Thread(() => update_weights(second)));
            threads.Add(new Thread(() => update_weights(final)));
            foreach (Thread t in threads)
            {
                t.Start();
                t.IsBackground = true;
            }*/
        }

        public void update_delt()
        {
            for (int i = 0; i < second.Count; i++)
            {
                update_delta(second[i], i, final);
            }
        }

        public void update_delta(Second_neiron p, int i, List<Second_neiron> next)
        {
            p.deltas = 0;

            for (int j = 0; j < next.Count; j++)
            {
                p.deltas += next[j].weights[i] * next[j].deltas;
            }
            p.deltas *= df(p.getactivfunc());
        }

        private void update_weights(List<Second_neiron> a)
        {
            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < a[i].weights.Count; j++)
                {
                    a[i].weights[j] -= Alpha * a[i].deltas * a[i].inputs[j].getactivfunc();
                }
            }
        }






        // входной слой
        private Double activ_first(Color a)
        {
            if (a.GetBrightness() < 0.99)
            {
                return 1;
            }
            return 0;
        }

        private Double activ_first(Double a)
        {
            return a;
        }



        // скрытый слои
        private Double activ_second(Double a)
        {
            return 1 / (1 + Math.Exp(-a));
        }



        // выходной слой
        private Double activ_final(Double a)
        {
            return 1 / (1 + Math.Exp(-a));
        }

        public Double df(Double a)
        {
            return a * (1 - a);
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }
    }
}

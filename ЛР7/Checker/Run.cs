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
using System.Collections;

namespace Checkers
{
    class Run
    {
        public Checker checker;
        public Cell to;
        public ArrayList taken;

        public Run(Checker check, Cell to)
        {
            this.checker = check;
            this.to = to;
            taken = new ArrayList();
        }
    }

    class Runs
    {
        ArrayList runs;
        Random rnd;

        public int count { get { return runs.Count; } }
        public Run this[int a] { get { return (Run)runs[a]; } set { runs[a] = value; } }

        public Runs()
        {
            runs = new ArrayList();
            rnd = new Random();
        }

        public Run GetRun()
        {
            if (count < 1)
                return null;
            else return this[rnd.Next(count)];
        }

        public void Add(Run run)
        {
            runs.Add(run);
        }
    }
}

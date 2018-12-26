using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    class RandomClass
    {
        int[] randoms;
        int count;
        Random rnd;

        public RandomClass(int max)
        {
            rnd = new Random();
            randoms = new int[max];
            count = 0;
        }

        public void Clear()
        {
            count = 0;
        }

        public int Get
        {
            get
            {
                if (count < 1)
                    return -1;

                return randoms[rnd.Next(count)];
            }
        }

        public void Add(int number)
        {
            if (count == randoms.Count())
                return;

            randoms[count] = number;
            count++;        
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public int this[int index]
        {
            get
            {
                return randoms[index];
            }
        }
    }
}

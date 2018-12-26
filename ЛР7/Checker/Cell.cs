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
    enum Letter { A, B, C, D, E, F, G, H }

    class Cell
    {
        public int x, y;
        public Letter letter;
        public int n;

        public Cell(int i, int j)
        {
            x = i;
            y = j;
            n = i + 1;
            letter = (Letter)j;
        }

        public Cell(Letter l, int n)
        {
            letter = l;
            this.n = n;

            x = (int)l;
            y = 8 - n;
        }

        public void Set(Cell position)
        {
            x = position.x;
            y = position.y;
            letter = position.letter;
            n = position.n;
        }

        public bool SamePosition(Cell position)
        {
            if (letter == position.letter && n == position.n)
                return true;
            else return false;
        }

        public Cell[] NearsQueen(int direction, Board b)
        {
            ArrayList array = new ArrayList(8);

            Letter letter1 = letter;
            int m = n;

            array.Add(new Cell(letter1, n));
            Checker check;

            switch(direction)
            {
                case 1:
                    while (letter1 != Letter.A && m != 8)
                    {
                        letter1--;
                        m++;

                        b.Who(new Cell(letter1, m), out check);

                        if (check is null)
                            array.Add(new Cell(letter1, m));
                        else break;
                    }
                    break;
                case 2:
                    while (letter1 != Letter.H && m != 8)
                    {
                        letter1++;
                        m++;

                        b.Who(new Cell(letter1, m), out check);

                        if (check is null)
                            array.Add(new Cell(letter1, m));
                        else break;
                    }
                    break;
                case 3:
                    while (letter1 != Letter.A && m != 1)
                    {
                        letter1--;
                        m--;

                        b.Who(new Cell(letter1, m), out check);

                        if (check is null)
                            array.Add(new Cell(letter1, m));
                        else break;
                    }
                    break;
                case 4:
                    while (letter1 != Letter.H && m != 1)
                    {
                        letter1++;
                        m--;

                        b.Who(new Cell(letter1, m), out check);

                        if (check is null)
                            array.Add(new Cell(letter1, m));
                        else break;
                    }
                    break;
                default: break;
            }

            Cell[] result = null;
            if (array.Count > 0)
            {
                result = new Cell[array.Count];

                for (int i = 0; i < array.Count; i++)
                {
                    result[i] = (Cell)array[i];
                }
            }

            return result;
        }

        public Cell NearQueen(int dir, Board b)
        {
            Letter letter1 = letter;
            int m = n;
            Checker check;

            switch(dir)
            {
                case 1:
                    while(letter1 != Letter.A && m != 8)
                    {
                        letter1--;
                        m++;
                        b.Who(new Cell(letter1, m), out check);
                        if (check != null)
                            return new Cell(letter1, m);
                    }
                    break;
                case 2:
                    while (letter1 != Letter.H && m != 8)
                    {
                        letter1++;
                        m++;
                        b.Who(new Cell(letter1, m), out check);
                        if (check != null)
                            return new Cell(letter1, m);
                    }
                    break;
                case 3:
                    while (letter1 != Letter.A && m != 1)
                    {
                        letter1--;
                        m--;
                        b.Who(new Cell(letter1, m), out check);
                        if (check != null)
                            return new Cell(letter1, m);
                    }
                    break;
                case 4:
                    while (letter1 != Letter.H && m != 1)
                    {
                        letter1++;
                        m--;
                        b.Who(new Cell(letter1, m), out check);
                        if (check != null)
                            return new Cell(letter1, m);
                    }
                    break;
                default: break;
            }

            return null;
        }

        public Cell Near(int dir)
        {
            switch(dir)
            {
                case 1:
                    if (letter == Letter.A || n == 8)
                        return null;
                    return new Cell((Letter)letter - 1, n + 1);
                case 2:
                    if (letter == Letter.H || n == 8)
                        return null;
                    return new Cell((Letter)letter + 1, n + 1);
                case 3:
                    if (letter == Letter.A || n == 1)
                        return null;
                    return new Cell((Letter)letter - 1, n - 1);
                case 4:
                    if (letter == Letter.H || n == 1)
                        return null;
                    return new Cell((Letter)letter + 1, n - 1);
                default: break;
            }
            return null;
        }

        public override string ToString()
        {
            return letter.ToString() + n.ToString();
        }

    }
}

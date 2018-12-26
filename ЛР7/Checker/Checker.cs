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
    enum Color { White, Black, Green }

    class Checker
    {
        public Color color;
        public bool queen;
        public Cell position;
        public Ellipse[] ellipses;
        
        Board board;
        Canvas canva;
        Tree tree;

        public Checker(Canvas canvas, Color c, Cell pos)
        {
            canva = canvas;
            color = c;
            position = pos;
            ellipses = new Ellipse[8];
            ellipses[0] = null;
            ellipses[1] = null;
        }

        public void SetPosition(Cell pos)
        {
            double size = canva.Width;
            double cellSize = size / 8.0;
            double tab = 0.8;

            this.position.Set(pos);

            if ((position.n == 1 && color == Color.Black) || (position.n == 8 && color == Color.White))
            {
                queen = true;

                ellipses[1] = new Ellipse();
                ellipses[1].Stroke = Brushes.Red;
                ellipses[1].StrokeThickness = 3;
                ellipses[1].Width = tab * cellSize / 2;
                ellipses[1].Height = tab * cellSize / 2;
                canva.Children.Add(ellipses[1]);
            }

            ellipses[0].Margin = new Thickness(pos.x * cellSize + cellSize * (1 - tab) / 2, pos.y * cellSize + cellSize * (1 - tab) / 2, 0, 0);

            if (ellipses[1] != null)
            {
                ellipses[1].Margin = new Thickness(pos.x * cellSize + (cellSize - ellipses[1].Width) / 2, pos.y * cellSize + (cellSize - ellipses[1].Height) / 2, 0, 0);
            }
        }

        public void Delete()
        {
            canva.Children.Remove(ellipses[0]);
            canva.Children.Remove(ellipses[1]);
        }

        void AddRun(Runs runs, int dir)
        {
            Checker checkerWho;
            Checker checker;
            Run run;

            Cell near = position.Near(dir);

            if(board.Who(near, out checkerWho))
            {
                if (checkerWho is null)
                    runs.Add(new Run(this, near));
                else
                {
                    if(checkerWho.color != color)
                    {
                        near = checkerWho.position.Near(dir);
                        if(board.Who(near, out checker))
                        {
                            if(checker is null)
                            {
                                run = new Run(this, near);
                                run.taken.Add(checkerWho);
                                runs.Add(run);
                            }
                        }
                    }
                }
            }
        }

        void AddRunQueen(Runs runs, int dir)
        {
            Cell[] nears = position.NearsQueen(dir, board);

            for (int i = 1; i < nears.Count(); i++)
                runs.Add(new Run(this, nears[i]));
        }

        void AddRunTakeQueen(Cell start, int dir, Node previos)
        {
            ArrayList array = new ArrayList();
            Cell near;

            if (start is null)
                near = position.NearQueen(dir, board);
            else near = start.NearQueen(dir, board);

            Checker who;
            board.Who(near, out who);

            if (who is null)
                return;

            if (who.color == color)
                return;

            near = near.Near(dir);
            Checker w;

            if(board.Who(near, out w))
            {
                if (w is null)
                {
                    Node current = new Node(previos, near, who, dir);

                    if (tree is null)
                        tree = new Tree(current, board);
                    tree.count++;

                    int count = tree.count;
                    Cell[] nears;

                    for(int i = 1; i <= 4; i++)
                    {
                        if ((dir == 1 && i == 4) || (dir == 2 && i == 3) || (dir == 3 && i == 2) || (dir == 4 && i == 1))
                            continue;
                        nears = near.NearsQueen(dir, board);

                        for (int j = 0; j < nears.Count(); j++)
                            AddRunTakeQueen(nears[j], i, current);
                    }

                    if (count == tree.count)
                        tree.lists.Add(current);
                }
            }
        }

        void AddRunTake(Cell start, int dir, Node previous)
        {
            Cell near;
            Checker who;
            Checker w;

            if (start is null)
                near = position.Near(dir);
            else near = start.Near(dir);

            if(board.Who(near, out who))
            {
                if(who != null)
                {
                    if(who.color != color)
                    {
                        near = who.position.Near(dir);
                        if(board.Who(near, out w))
                        {
                            if(w is null)
                            {
                                Node current = new Node(previous, near, who, dir);
                                if (tree is null)
                                    tree = new Tree(current, board);
                                tree.count++;

                                int count = tree.count;

                                for (int i = 1; i <= 4; i++)
                                {
                                    if ((dir == 1 && i == 4) || (dir == 2 && i == 3) || (dir == 3 && i == 2) || (dir == 4 && i == 1))
                                        continue;

                                    AddRunTake(near, i, current);
                                }

                                if (count == tree.count)
                                    tree.lists.Add(current);
                            }
                        }
                    }
                }
            }
        }

        public Runs GetRuns(Board b)
        {
            board = b;
            Runs result = new Runs();

            if(queen)
            {
                for(int i = 1; i <= 4; i++)
                    AddRunQueen(result, i);
            }
            else
            {
                if(color == Color.White)
                {
                    AddRun(result, 1);
                    AddRun(result, 2);
                }
                else
                {
                    AddRun(result, 3);
                    AddRun(result, 4);
                }
            }

            return result;
        }

        public Runs GetRunsTake(Board b)
        {
            board = b;
            Runs result = new Runs();

            if(queen)
            {
                tree = null;
                for(int i = 1; i <= 4; i++)
                {
                    AddRunTakeQueen(null, i, null);
                    if (tree != null)
                    {
                        tree.AddRuns(this, ref result);
                        tree = null;
                    }
                }
            }
            else
            {
                tree = null;
                for (int i = 1; i <= 4; i++)
                {
                    AddRunTake(null, i, null);
                    if (tree != null)
                    {
                        tree.AddRuns(this, ref result);
                        tree = null;
                    }
                }
            }

            return result;
        }

        public bool Run(Board b, Run r)
        {
            Runs runsTake = GetRunsTake(b);
            if(runsTake.count > 0)
            {
                Run run;
                if (r is null)
                    run = runsTake.GetRun();
                else run = r;

                SetPosition(run.to);

                for(int i = 0; i < run.taken.Count; i++)
                {
                    Checker taken = (Checker)run.taken[i];
                    canva.Children.Remove(taken.ellipses[0]);
                    canva.Children.Remove(taken.ellipses[1]);
                    if (taken.color == Color.White)
                        b.checkersWhite.Remove(taken);
                    else b.checkersBlack.Remove(taken);
                }
                return true;
            }

            Runs runs = GetRuns(b);
            if(runs.count > 0)
            {
                Run run;
                if (r is null)
                    run = runs.GetRun();
                else run = r;

                SetPosition(run.to);
                return true;
            }

            return false;
        }
        
    }

    class Checkers
    {
        ArrayList arr;

        public Checkers(Color c, Canvas g)
        {
            arr = new ArrayList(12);

            if (c == Color.White)
            {
                arr.Add(new Checker(g, c, new Cell(Letter.A, 1)));
                arr.Add(new Checker(g, c, new Cell(Letter.A, 3)));
                arr.Add(new Checker(g, c, new Cell(Letter.B, 2)));
                arr.Add(new Checker(g, c, new Cell(Letter.C, 1)));
                arr.Add(new Checker(g, c, new Cell(Letter.C, 3)));
                arr.Add(new Checker(g, c, new Cell(Letter.D, 2)));
                arr.Add(new Checker(g, c, new Cell(Letter.E, 1)));
                arr.Add(new Checker(g, c, new Cell(Letter.E, 3)));
                arr.Add(new Checker(g, c, new Cell(Letter.F, 2)));
                arr.Add(new Checker(g, c, new Cell(Letter.G, 1)));
                arr.Add(new Checker(g, c, new Cell(Letter.G, 3)));
                arr.Add(new Checker(g, c, new Cell(Letter.H, 2)));
            }
            else
            {
                arr.Add(new Checker(g, c, new Cell(Letter.A, 7)));
                arr.Add(new Checker(g, c, new Cell(Letter.B, 6)));
                arr.Add(new Checker(g, c, new Cell(Letter.B, 8)));
                arr.Add(new Checker(g, c, new Cell(Letter.C, 7)));
                arr.Add(new Checker(g, c, new Cell(Letter.D, 6)));
                arr.Add(new Checker(g, c, new Cell(Letter.D, 8)));
                arr.Add(new Checker(g, c, new Cell(Letter.E, 7)));
                arr.Add(new Checker(g, c, new Cell(Letter.F, 6)));
                arr.Add(new Checker(g, c, new Cell(Letter.F, 8)));
                arr.Add(new Checker(g, c, new Cell(Letter.G, 7)));
                arr.Add(new Checker(g, c, new Cell(Letter.H, 6)));
                arr.Add(new Checker(g, c, new Cell(Letter.H, 8)));
            }

        }

        public Checker this[int n]
        {
            get
            {
                return (Checker)arr[n];
            }
        }

        public int Count
        {
            get
            {
                return arr.Count;
            }
        }

        public void RemoveAt(int n)
        {
            arr.RemoveAt(n);
        }

        public void Remove(Checker Check)
        {
            Check.Delete();

            arr.Remove(Check);
        }

        public void Move(int n, Cell Pos)
        {
            Checker Check = this[n];
            Check.SetPosition(Pos);
        }

    }
}

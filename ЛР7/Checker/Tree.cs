using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Checkers
{
    class Node
    {
        public Node previous;
        public Cell position;
        public Checker taken;
        public int dir;

        public Node(Node prev, Cell pos, Checker t, int d)
        {
            previous = prev;
            position = pos;
            taken = t;
            dir = d;
        }

    }

    class Tree
    {
        Node root;
        Board board;
        public ArrayList lists;
        public int count;

        public Tree(Node root, Board board)
        {
            this.root = root;
            this.board = board;

            lists = new ArrayList(12);
            count = 0;
        }

        public ArrayList Up(Node list)
        {
            ArrayList result = new ArrayList(6);
            Node current = list;
            result.Add(current);

            while(current.previous != null)
            {
                current = current.previous;
                result.Add(current);
            }

            return result;
        }

        public void AddRuns(Checker checker, ref Runs runs)
        {
            for(int i = 0; i < lists.Count; i++)
            {
                Node node = (Node)lists[i];
                Run run = new Run(checker, node.position);
                ArrayList array = Up(node);

                for(int j = 0; j < array.Count; j++)
                {
                    run.taken.Add(((Node)array[j]).taken);
                }

                runs.Add(run);
                Cell[] addition = node.position.NearsQueen(node.dir, board);

                for(int j = 1; j < addition.Count(); j++)
                {
                    Run newRun = new Run(checker, addition[j]);

                    for(int k = 0; k < run.taken.Count; k++)
                    {
                        newRun.taken.Add(run.taken[k]);
                    }

                    runs.Add(newRun);
                }
            }
        }
    }
}

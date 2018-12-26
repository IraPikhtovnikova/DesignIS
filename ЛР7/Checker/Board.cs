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
    class Board
    {
        Canvas canva;
        public Checkers checkersWhite, checkersBlack;

        public Board(Canvas canvas)
        {
            canva = canvas;            

            checkersWhite = new Checkers(Color.White, canva);
            checkersBlack = new Checkers(Color.Black, canva);
            DrawCell();
            DrawCheckers();
        }

        public void DrawCell()
        {
            double size = canva.Width;
            double cellWidth = size / 8.0;

            bool light = false;

            for (int i = 0; i < 8; i++)
            {
                light = !light;

                for(int j = 0; j < 8; j++)
                {
                    Rectangle cell = new Rectangle();

                    if(light)
                    {
                        cell.Fill = Brushes.LightGoldenrodYellow;                        
                    }
                    else
                    {
                        cell.Fill = Brushes.SaddleBrown;
                    }
                    light = !light;
                    cell.Width = cellWidth;
                    cell.Height = cellWidth;
                    cell.Margin = new Thickness(i * cellWidth, j * cellWidth, 0, 0);
                    canva.Children.Add(cell);
                }

                Rectangle border = new Rectangle();
                border.Width = size;
                border.Height = size;
                border.Stroke = Brushes.Black;
                border.Margin = new Thickness(0);
                canva.Children.Add(border);
            }
        }

        public bool Who(Cell position, out Checker who)
        {
            who = null;

            if (position is null)
                return false;
            
            for(int i = 0; i < checkersWhite.Count; i++)
            {
                if(checkersWhite[i].position.SamePosition(position))
                {
                    who = checkersWhite[i];
                    return true;
                }
            }

            for (int i = 0; i < checkersBlack.Count; i++)
            {
                if (checkersBlack[i].position.SamePosition(position))
                {
                    who = checkersBlack[i];
                    return true;
                }
            }

            return true;
        }

        Ellipse DrawChecker(Cell pos, Color c)
        {
            double size = canva.Width;
            double cellSize = size / 8.0;
            double coef = 0.8;

            Ellipse ellipse = new Ellipse();

            if (c == Color.White)
                ellipse.Fill = Brushes.White;
            else if (c == Color.Black) ellipse.Fill = Brushes.Black;
            else ellipse.Fill = Brushes.Green;
            ellipse.Width = coef * cellSize;
            ellipse.Height = coef * cellSize;
            ellipse.Margin = new Thickness(pos.x * cellSize + cellSize * (1 - coef) / 2, pos.y * cellSize + cellSize * (1 - coef) / 2, 0, 0);

            canva.Children.Add(ellipse);
            return ellipse;
        }

        public void Over(Color c)
        {
            string result = "";
            if (c == Color.White)
                result = "Black";
            else result = "White";

            MessageBox.Show(result + " wins");

        }

        void DrawCheckers()
        {
            for(int i = 0; i < checkersWhite.Count; i++)
            {
                checkersWhite[i].ellipses[0] = DrawChecker(checkersWhite[i].position, Color.White);
            }

            for (int i = 0; i < checkersBlack.Count; i++)
            {
                checkersBlack[i].ellipses[0] = DrawChecker(checkersBlack[i].position, Color.Black);
            }
        }

        public Cell GetCell(Point point)
        {
            double size = canva.Width;
            double cellSize = size / 8.0;

            double x = point.X;
            double y = point.Y;

            if(x < 0 || x > size || y < 0 || y > size)
            {
                MessageBox.Show("Choose checker");
                return null;
            }

            int number = (int)Math.Ceiling(x / cellSize);
            Letter letter = (Letter)(number - 1);
            number = (int)Math.Ceiling(y / cellSize);
            int n = 9 - number;

            return new Cell(letter, n);
        }

        public void LightChecker(Checker ch, bool action)
        {
            if (action)
                ch.ellipses[0].Fill = Brushes.Green;
            else
            {
                ch.ellipses[0].Fill = Brushes.White;
                DrawCell();
            }
        }
    }
}

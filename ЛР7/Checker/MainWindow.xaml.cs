using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Windows.Threading;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Checkers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board board;
        Checker checkerFor = null;
        RandomClass rnd;
        bool isWhite = true;
        bool isGame = true;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            board = new Board(cBoard);
            rnd = new RandomClass(64);            
        }

        private void wClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void wRun(object sender, RoutedEventArgs e)
        {
            if (isWhite)
                RunCheckers(board.checkersWhite);
            else RunCheckers(board.checkersBlack);
            isWhite = !isWhite;
        }

        void RunCheckers(Checkers checkers)
        {
            Runs runs;
            Runs runsTaken;
            rnd.Clear();

            for (int i = 0; i < checkers.Count; i++)
            {
                runsTaken = checkers[i].GetRunsTake(board);

                if (runsTaken.count > 0)
                    rnd.Add(i);
            }

            if (rnd.Count > 0)
            {
                checkers[rnd.Get].Run(board, null);
                return;
            }

            rnd.Clear();

            for (int i = 0; i < checkers.Count; i++)
            {
                runs = checkers[i].GetRuns(board);

                if (runs.count > 0)
                    rnd.Add(i);
            }

            if (rnd.Count > 0)
                checkers[rnd.Get].Run(board, null);
            else
            {
                isGame = false;
                Color color;

                if (isWhite)
                {
                    color = Color.White;
                    ChangePlayer(true);
                }
                else
                {
                    color = Color.Black;
                    ChangePlayer(false);
                }
                board.Over(color);
                board = new Board(cBoard);
            }
        }

        private void wAutoRun(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(OnTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            isGame = true;
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (isGame)
                wRun(null, null);
            else timer.Stop();
        }

        private void wClick(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(cBoard);
            Cell cell = board.GetCell(point);

            if (checkerFor is null)
            {
                board.Who(cell, out checkerFor);

                if (checkerFor is null)
                    return;

                if (checkerFor.color != Color.White)
                {
                    checkerFor = null;
                    return;
                }
                board.LightChecker(checkerFor, true);
            }
            else
            {
                ArrayList array = new ArrayList();
                Run run = new Run(checkerFor, cell);
                Runs runs;
                Runs runsTaken;

                for (int i = 0; i < board.checkersWhite.Count; i++)
                {
                    runsTaken = board.checkersWhite[i].GetRunsTake(board);
                    for (int j = 0; j < runsTaken.count; j++)
                        array.Add(runsTaken[j]);
                }

                if (array.Count > 0)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        Run r = (Run)array[i];

                        if (run.checker.position.SamePosition(r.checker.position) && run.to.SamePosition(r.to))
                        {
                            r.checker.Run(board, r);
                            isWhite = !isWhite;
                            wRun(null, null);
                            checkerFor = null;
                            return;
                        }
                    }
                    checkerFor = null;
                    return;
                }
                else
                {
                    array.Clear();
                    for(int i = 0; i < board.checkersWhite.Count; i++)
                    {
                        runs = board.checkersWhite[i].GetRuns(board);
                        for(int j = 0; j < runs.count; j++)
                        {
                            array.Add(runs[j]);
                        }
                    }

                    if(array.Count > 0)
                    {
                        for(int i = 0; i < array.Count; i++)
                        {
                            Run r = (Run)array[i];
                            if(run.checker.position.SamePosition(r.checker.position) && run.to.SamePosition(r.to))
                            {
                                r.checker.Run(board, r);
                                isWhite = !isWhite;
                                wRun(null, null);
                                checkerFor = null;
                                return;
                            }
                        }
                        board.LightChecker(checkerFor, false);
                        checkerFor = null;
                        return;
                    }
                    else
                    {
                        isGame = false;
                        Color color;
                        if (isWhite)
                        {
                            color = Color.White;
                            //ChangePlayer(true);
                        }
                        else
                        {
                            color = Color.Black;
                            //ChangePlayer(false);
                        }
                        board.Over(color);
                        board = new Board(cBoard);
                    }
                }
            }
        }

        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\irapi\Documents\checkers.mdf;Integrated Security=True;Connect Timeout=30";

        void ChangePlayer(bool win)
        {
            string sqlExpression = "ChangePlayer";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = txbName.Text
                };
                command.Parameters.Add(nameParam);

                int wins = 0, lose = 0;
                if(win)
                {
                    wins = GetWin();
                    wins++;
                }
                else
                {
                    lose = GetLose();
                    lose++;
                }

                SqlParameter winParam = new SqlParameter
                {
                    ParameterName = "@win",
                    Value = wins
                };
                command.Parameters.Add(winParam);

                SqlParameter loseParam = new SqlParameter
                {
                    ParameterName = "@lose",
                    Value = lose
                };
                command.Parameters.Add(loseParam);

                var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();
                MessageBox.Show(result.ToString());
            }
        }

        int GetWin()
        {
            string sqlExpression = "GetWins";
            var result = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = txbName.Text
                };
                command.Parameters.Add(nameParam);

                result = (int)command.ExecuteScalar();
                if ((int)result == -1)
                {
                    AddPlayer();
                    result = 0;
                }
            }

            return (int)result;
        }

        void AddPlayer()
        {
            string sqlExpression = "AddPlayer";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = txbName.Text
                };
                command.Parameters.Add(nameParam);


                SqlParameter winParam = new SqlParameter
                {
                    ParameterName = "@win",
                    Value = 0
                };
                command.Parameters.Add(winParam);

                SqlParameter loseParam = new SqlParameter
                {
                    ParameterName = "@lose",
                    Value = 0
                };
                command.Parameters.Add(loseParam);


                //var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                var result = command.ExecuteNonQuery();
            }
        }

        int GetLose()
        {
            string sqlExpression = "GetLose";
            var result = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = txbName.Text
                };
                command.Parameters.Add(nameParam);

                result = (int)command.ExecuteScalar();
                if ((int)result == -1)
                {
                    AddPlayer();
                    result = 0;
                }
            }

            return (int)result;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private MarkType[] mGameBoard;

        private int[] winCombo;

        public MainWindow()
        {
            InitializeComponent();
            
            StartNewGame();
        }

        private void StartNewGame()
        {
            mGameBoard = new MarkType[9];
            winCombo = new int[3];

            for (int i = 0; i < mGameBoard.Length; i++)
                mGameBoard[i] = MarkType.Empty;

            Container.Children.Cast<Button>().ToList().ForEach(Button =>
            {
                Button.Content = String.Empty;
                Button.Background = Brushes.White;
            });
        }

        private void PlayerMove(object sender, RoutedEventArgs e)
        {
            if (CheckForWin(mGameBoard) != GameStates.GoingOn)
            {
                StartNewGame();
                return;
            }

            var button = (Button)sender;
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            var index = column + (row * 3);

            if (mGameBoard[index] != MarkType.Empty)
                return;

            mGameBoard[index] = MarkType.Cross;

            button.Content = "X";
            button.Foreground = Brushes.Blue;
            
            if (CheckForWin(mGameBoard) == GameStates.GoingOn)
                BestMove();
            else
                DeclareWinner();
        }

        private void PlaceCircleOnBoard(int pos)
        {
            mGameBoard[pos] = MarkType.Circle;
            int column = pos % 3;
            int row = pos / 3;

            List<Button> buttons = Container.Children.Cast<Button>().ToList();
            foreach (Button button in buttons)
            {
                if (Grid.GetRow(button) == row && Grid.GetColumn(button) == column)
                {
                    button.Content = "O";
                    button.Foreground = Brushes.Orange;
                }
            }
        }

        private void BestMove()
        {
            int bestScore = int.MinValue;
            int move = 0;

            for (int i = 0; i < mGameBoard.Length; i++)
            {
                if (mGameBoard[i] == MarkType.Empty)
                {
                    mGameBoard[i] = MarkType.Circle;
                    int currentScore = Minimax(false, 0, mGameBoard);
                    mGameBoard[i] = MarkType.Empty;

                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        move = i;
                    }
                }
            }

            PlaceCircleOnBoard(move);

            if (CheckForWin(mGameBoard) != GameStates.GoingOn)
                DeclareWinner();
        }

        private int Minimax(bool isMaximizing, int depth, MarkType[] gameBoard)
        {
            GameStates status = CheckForWin(gameBoard);

            if (status == GameStates.PlayerWon)
                return -10 + depth;
            else if (status == GameStates.AiWon)
                return 10 - depth;
            else if (status == GameStates.Tie)
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < gameBoard.Length; i++)
                {
                    if (gameBoard[i] == MarkType.Empty)
                    {
                        gameBoard[i] = MarkType.Circle;
                        bestScore = Math.Max(bestScore, Minimax(false, depth + 1, gameBoard));
                        gameBoard[i] = MarkType.Empty;
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < gameBoard.Length; i++)
                {
                    if (gameBoard[i] == MarkType.Empty)
                    {
                        gameBoard[i] = MarkType.Cross;
                        bestScore = Math.Min(bestScore, Minimax(true, depth + 1, gameBoard));
                        gameBoard[i] = MarkType.Empty;
                    }
                }

                return bestScore;
            }
        }

        private GameStates CheckForTie(MarkType[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == MarkType.Empty)
                    return GameStates.GoingOn;
            }
            return GameStates.Tie;
        }

        private GameStates CheckForWin(MarkType[] board)
        {
            int[][] winCombinations =
            { 
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 3, 6 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 0, 4, 8 },
                new int[] { 2, 4, 6 }
            };

            foreach (int[] winCombination in winCombinations)
            {
                if (board[winCombination[0]] == MarkType.Cross &&
                    board[winCombination[1]] == MarkType.Cross &&
                    board[winCombination[2]] == MarkType.Cross)
                {
                    winCombo = winCombination;
                    return GameStates.PlayerWon;
                }

                if (board[winCombination[0]] == MarkType.Circle &&
                    board[winCombination[1]] == MarkType.Circle &&
                    board[winCombination[2]] == MarkType.Circle)
                {
                    winCombo = winCombination;
                    return GameStates.AiWon;
                }
            }

            return CheckForTie(board);
        }

        private void DeclareWinner()
        {
            GameStates whoWon = CheckForWin(mGameBoard);

            List<Button> buttons = Container.Children.Cast<Button>().ToList();

            if (whoWon == GameStates.Tie)
            {
                foreach (Button button in buttons)
                    button.Background = Brushes.Yellow;

                return;
            }

            for (int i = 0; i < winCombo.Length; i++)
            {
                int column = winCombo[i] % 3;
                int row = winCombo[i] / 3;

                foreach (Button button in buttons)
                {
                    if (Grid.GetRow(button) == row && Grid.GetColumn(button) == column)
                    {
                        button.Background = whoWon == GameStates.PlayerWon ? Brushes.Green : Brushes.Red;
                    }
                }
            }
        }
    }
}

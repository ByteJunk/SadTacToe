using System;

namespace SadTacToe
{
    public class GameBoard
    {
        private int[] _board;
        public int[] Board { get => _board; private set => _board = value; }
        private bool _gameOver;
        public bool GameOver { get => _gameOver; private set => _gameOver = value; }

        /// <summary>
        /// Esta variável contém todos os padrões que correspondem a vitória.
        /// </summary>
        private static int[][] linhas = new int[][] {
                new int[] {0, 1, 2},
                new int[] {3, 4, 5},
                new int[] {6, 7, 8},
                new int[] {0, 3, 6},
                new int[] {1, 4, 7},
                new int[] {2, 5, 8},
                new int[] {0, 4, 8},
                new int[] {2, 4, 6}
        };

        public GameBoard()
        {
            _board = new int[9];
            _gameOver = false;
        }

        public GameBoard(int[] board)
        {
            _board = board;
            _gameOver = false;
        }

        public Jogador AvaliarVitoria()
        {
            Jogador j = Jogador.Vazio;
            foreach (var linha in linhas)
            {
                int Total = 0;
                foreach (var casa in linha)
                {
                    Total += Board[casa];
                }
                if (Math.Abs(Total) == 3)
                {
                    j = (Jogador)(int)(Total / 3);
                    break;
                }
            }
            return j;
        }

        public Boolean Jogar(Jogador j, int pos)
        {
            if (pos + 1 <= Board.Length)
                if ((Jogador)Board[pos] == Jogador.Vazio)
                {
                    Board[pos] = (int)j;
                    GameOver = true;
                    foreach (var i in Board)
                    {
                        if (i == (int)Jogador.Vazio) GameOver = false;
                    }
                    return true;
                }
            return false;
        }
    }
}

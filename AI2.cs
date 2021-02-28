using System;
using System.Collections;
using System.Collections.Generic;

namespace SadTacToe
{
    public static class AI2
    {
        public static int ProxJogada(GameBoard gameBoard)
        {
            List<int> possiveisVitorias = new List<int>();
            List<int> possiveisEmpates = new List<int>();
            List<int> possiveisDerrotas = new List<int>();

            for (int posicao = 0; posicao < gameBoard.Board.Length; posicao++)
            {
                if (gameBoard.Board[posicao] == 0)
                {
                    var novaBoard = new GameBoard((int[])gameBoard.Board.Clone());
                    novaBoard.Jogar(Jogador.Computador, posicao);

                    var novoVencedor = AvaliarMinMax(novaBoard, 0, Jogador.Computador);

                    if (novoVencedor == Jogador.Computador)
                        possiveisVitorias.Add(posicao);
                    if (novoVencedor == Jogador.Vazio)
                        possiveisEmpates.Add(posicao);
                    if (novoVencedor == Jogador.Humano)
                        possiveisDerrotas.Add(posicao);
                }
            }

            var rnd = new Random();

            if (possiveisVitorias.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"A escolher cenário de vitórias ({possiveisVitorias.Count} possibilidades)");
                return possiveisVitorias[rnd.Next(possiveisVitorias.Count)];
            }
            if (possiveisEmpates.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"A escolher cenário de empates ({possiveisEmpates.Count} possibilidades)");
                return possiveisEmpates[rnd.Next(possiveisEmpates.Count)];
            }

            System.Diagnostics.Debug.WriteLine($"A escolher cenário de derrotas ({possiveisDerrotas.Count} possibilidades)");
            return possiveisDerrotas[rnd.Next(possiveisDerrotas.Count)];

        }

        /// <summary>
        /// Calcula quem ganha com base numa jogada
        /// </summary>
        /// <param name="board">O tabuleiro de jogo atual</param>
        /// <param name="depth">Nivel de simulação em que estamos</param>
        /// <param name="quemJoga">De quem é a vez de jogar</param>
        /// <returns></returns>
        private static Jogador AvaliarMinMax(GameBoard board, int depth, Jogador quemJoga)
        {
            var vencedor = board.AvaliarVitoria();

            if (vencedor == Jogador.Computador || vencedor == Jogador.Humano || board.GameOver)
                return vencedor;

            // Ainda não há vencedor e não acabou, vamos ter que simular mais uma ronda
            var outroJogador = (Jogador)((int)quemJoga * -1);
            List<int> possiveisVitorias = new List<int>();
            List<int> possiveisEmpates = new List<int>();
            List<int> possiveisDerrotas = new List<int>();
            for (int posicao = 0; posicao < board.Board.Length; posicao++)
            {
                if (board.Board[posicao] == 0)
                {
                    var novaBoard = new GameBoard((int[])board.Board.Clone());

                    novaBoard.Jogar(outroJogador, posicao);

                    var novoVencedor = AvaliarMinMax(novaBoard, depth+1, outroJogador);

                    // O jogador vai fazer sempre a melhor jogada, e ganhar.
                    if (novoVencedor == quemJoga)
                        possiveisVitorias.Add(posicao);
                    if (novoVencedor == Jogador.Vazio)
                        possiveisEmpates.Add(posicao);
                    if (novoVencedor == outroJogador)
                        possiveisDerrotas.Add(posicao);
                }
            }
            // Se o jogador não ganhou, é porque não era possível ganhar

            if (possiveisDerrotas.Count > 0)
                return outroJogador;
            if (possiveisEmpates.Count > 0)
                return Jogador.Vazio;

            return quemJoga;
        }
    }
}
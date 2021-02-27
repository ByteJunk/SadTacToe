using System;
using SadConsole;
using Microsoft.Xna.Framework;
using Console = SadConsole.Console;
using System.Linq;

namespace SadTacToe
{
    public class Program
    {
        #region Região onde declaramos variáveis globais

        /// <summary>
        /// Serve para controlar o estado do jogo: true = jogo acabou (alguém ganhou ou é empate).
        /// </summary>
        public static Boolean GameComplete;

        /// <summary>
        /// O tabuleiro de jogo. Aqui são guardadas todas as jogadas.
        /// </summary>
        private static GameBoard board;

        /// <summary>
        /// O Cursor que permite mover entre os quadrados de jogo.
        /// </summary>
        private static SadConsole.Entities.Entity cursor;

        /// <summary>
        /// O ecrã onde vamos desenhar o tabuleiro de jogo.
        /// </summary>
        private static Console gameConsole;

        /// <summary>
        /// O ecrã onde vamos mostrar os resultados.
        /// </summary>
        private static Console scoreBoard;

        /// <summary>
        /// Aqui guardamos quem é a vez de jogar agora
        /// </summary>
        private static Jogador quemJoga;

        /// <summary>
        /// Aqui guardamos quem foi o primeiro a jogar (para depois trocar)
        /// </summary>
        private static Jogador quemComecou = Jogador.Computador;

        /// <summary>
        /// Quantos jogos o jogador 1 já ganhou.
        /// </summary>
        private static int pontosJogador1 = 0;

        /// <summary>
        /// Quantos jogos o jogador 2 já ganhou.
        /// </summary>
        private static int pontosJogador2 = 0;

        /// <summary>
        /// Largura da janela principal, em colunas.
        /// </summary>
        public const int Width = 80;

        /// <summary>
        /// Altura da janela principal, em linhas.
        /// Queremos 7 linhas, cada uma com o quádruplo do tamanho, 7x4=28
        /// Com duas linhas de tamanho normal para o fundo, =30.
        /// </summary>
        public const int Height = 28;

        #endregion

        #region Região onde temos os nossos Métodos

        /// <summary>
        /// O método chamado "Main()" é sempre o primeiro a ser chamado.
        /// </summary>
        static void Main()
        {
            // Inicializa o motor de jogo com o tamanho pretendido.
            SadConsole.Game.Create(Width, Height);

            // Associa um Método nosso (Init) que é chamado quando ocorre o evento "OnInitialize".
            SadConsole.Game.OnInitialize = Init;

            // Associa um Método nosso (Update) que é chamado a cada frame, para atualizarmos o estado.
            SadConsole.Game.OnUpdate = Update;

            // Começa o jogo.
            SadConsole.Game.Instance.Run();

            // Limpa tudo depois de terminarmos o jogo.
            SadConsole.Game.Instance.Dispose();
        }

        /// <summary>
        /// Este método é chamado depois do Main(), é aqui que preparamos o que vamos precisar.
        /// </summary>
        static void Init()
        {
            // No início de cada partida, invertemos a ordem dos jogadores
            quemComecou = (Jogador)((int)quemComecou * -1);

            // E aqui definimos, com base no anterior, se é o jogador1 a jogar agora
            quemJoga = quemComecou;

            // Estamos a começar um jogo
            GameComplete = false;

            // Daqui para baixo vamos tratar de gráficos
            // Preparar umm tipo de letra mais giro (quadrangular) e GRANDE!
            var fontMaster = Global.LoadFont("font/simplemood.font");
            var fontVezes1 = fontMaster.GetFont(Font.FontSizes.One);
            var fontVezes4 = fontMaster.GetFont(Font.FontSizes.Four);

            // Criar uma consola "mãe", que vai ser o nosso ecrã
            var console = new Console(Width, Height);
            console.Font = fontVezes1;
            Global.CurrentScreen = console;

            // Preparar a consola que contém o nosso tabuleiro
            gameConsole = new Console(7, 7);
            gameConsole.Font = fontVezes4;
            console.Children.Add(gameConsole);

            // Preparar a consola onde vamos mostrar os resultados
            scoreBoard = new Console(12, Height);
            scoreBoard.Position = new Point(28, 0);
            scoreBoard.Font = fontVezes1;
            console.Children.Add(scoreBoard);

            // Vamos inicializar um novo tabuleiro
            board = new GameBoard();

            // Vamos chamar um Método que desenha a nosso tabuleiro.
            PrintGameConsole();

            // Agora que temos um tabuleiro, vamos criar um Cursor para sabermos onde estamos.
            cursor = new SadConsole.Entities.Entity(1, 1);
            cursor.Position = new Point(1, 1);
            cursor.Font = fontVezes4;
            cursor.Animation.CurrentFrame[0].Glyph = '_';
            console.Children.Add(cursor);
        }

        /// <summary>
        /// Este Método "imprime" para a consola de jogo as jogadas que temos no $GameBoard,
        /// e depois cria as bordas e o tabuleiro de jogo com caractéres ASCII
        /// </summary>
        static void PrintGameConsole()
        {
            // Mostrar os X e O que já tenham sido jogados
            gameConsole.Print(1, 1, $"{Glifo(board.Board[0])} {Glifo(board.Board[1])} {Glifo(board.Board[2])}");
            gameConsole.Print(1, 3, $"{Glifo(board.Board[3])} {Glifo(board.Board[4])} {Glifo(board.Board[5])}");
            gameConsole.Print(1, 5, $"{Glifo(board.Board[6])} {Glifo(board.Board[7])} {Glifo(board.Board[8])}");
            // Linha 1
            gameConsole.SetGlyph(1, 2, 196); // -
            gameConsole.SetGlyph(2, 2, 197); // +
            gameConsole.SetGlyph(3, 2, 196); // -
            gameConsole.SetGlyph(4, 2, 197); // +
            gameConsole.SetGlyph(5, 2, 196); // -
            // Linha 2
            gameConsole.SetGlyph(1, 4, 196); // -
            gameConsole.SetGlyph(2, 4, 197); // +
            gameConsole.SetGlyph(3, 4, 196); // -
            gameConsole.SetGlyph(4, 4, 197); // +
            gameConsole.SetGlyph(5, 4, 196); // -
            // Coluna 1
            gameConsole.SetGlyph(2, 1, 179); // |
            gameConsole.SetGlyph(2, 3, 179); // |
            gameConsole.SetGlyph(2, 5, 179); // |
            // Coluna 2
            gameConsole.SetGlyph(4, 1, 179); // |
            gameConsole.SetGlyph(4, 3, 179); // |
            gameConsole.SetGlyph(4, 5, 179); // |
            // Borda exterior
            gameConsole.SetGlyph(0, 0, 201);
            for (int i = 1; i < 6; i++) gameConsole.SetGlyph(i, 0, 205);
            gameConsole.SetGlyph(6, 0, 187);
            gameConsole.SetGlyph(0, 6, 200);
            for (int i = 1; i < 6; i++) gameConsole.SetGlyph(i, 6, 205);
            gameConsole.SetGlyph(6, 6, 188);
            for (int i = 1; i < 6; i++) gameConsole.SetGlyph(0, i, 186);
            for (int i = 1; i < 6; i++) gameConsole.SetGlyph(6, i, 186);
            // Atualizar o ecrã com o marcador
            scoreBoard.Print(1, 1, "SadTacToe");
            for (int i = 0; i < 11; i++) scoreBoard.SetGlyph(i, 2, 210);
            scoreBoard.Print(1, 4, "Jogador 1");
            scoreBoard.Print(4, 6, "VS.");
            scoreBoard.Print(1, 8, "Jogador 2");
            scoreBoard.Print(4 - pontosJogador1.ToString().Length, 10, pontosJogador1.ToString());
            scoreBoard.Print(5, 10, "-");
            scoreBoard.Print(7, 10, pontosJogador2.ToString());
            for (int i = 0; i < 11; i++) scoreBoard.SetGlyph(i, 12, 210);
            scoreBoard.Print(0, 14, "Agora joga:");
            scoreBoard.Print(1, 15, quemJoga == Jogador.Humano ? "Jogador 1" : "Jogador 2");

            scoreBoard.Print(0, 25, "Tragedia by");
            scoreBoard.Print(0, 26, "Joao Ornelas");
            scoreBoard.SetGlyph(5, 27, 2);
        }

        /// <summary>
        /// Método chamado a cada frame. Vamos tratar de input no teclado.
        /// </summary>
        /// <param name="time">Tempo decorrido desde o último frame. No nosso cazso não importa.</param>
        static void Update(GameTime time)
        {
            // Se o jogo já acabou, só olhamos para duas teclas: Y e N
            if (GameComplete)
            {
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Y))
                {
                    System.Diagnostics.Debug.WriteLine("Inicializar");
                    Init();
                }
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.N))
                {
                    System.Diagnostics.Debug.WriteLine("Sair");
                    Quit();
                }
                // Mesmo que não haja Y/N, vamos sair deste Método, porque não queremos tratar outras teclas.
                return;
            }

            // Se for a vez do computador, vamos jogar
            if (quemJoga == Jogador.Computador)
            {
                //O PC é quase instantâneo, não queremos isso. Vamos nanar
                System.Threading.Thread.Sleep(500);

                int jogada = AI.ProxJogada(board);

                if (board.Jogar(quemJoga, jogada))
                {
                    quemJoga = (Jogador)((int)(quemJoga) * -1);
                    System.Diagnostics.Debug.WriteLine("Jogada registada, passada a vez para o outro jogador");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("A ignorar: não é possível jogar nesta posição.");
                }

                // O tabuleiro pode ter sido alterado, vamos redesenhá-lo
                PrintGameConsole();

                // Se houve um novo movimento, alguém pode ter ganho, ou ter acabado o jogo.
                Jogador j = board.AvaliarVitoria();

                if (board.GameOver || j != Jogador.Vazio) DeclararVitoria(j);

            }
            else
            {
                // Vamos mover o cursor por ter sido premida a tecla: Up arrow
                // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    cursor.Position += new Point(0, -2);
                    if (cursor.Position.Y < 0) cursor.Position += new Point(0, 2);
                }
                // Vamos mover o cursor por ter sido premida a tecla: Down arrow
                // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    cursor.Position += new Point(0, 2);
                    if (cursor.Position.Y > 5) cursor.Position += new Point(0, -2);
                }
                // Vamos mover o cursor por ter sido premida a tecla: Left arrow
                // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    cursor.Position += new Point(-2, 0);
                    if (cursor.Position.X < 0) cursor.Position += new Point(2, 0);
                }
                // Vamos mover o cursor por ter sido premida a tecla: Right arrow
                // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    cursor.Position += new Point(2, 0);
                    if (cursor.Position.X > 5) cursor.Position += new Point(-2, 0);
                }
                // A Barra de Espaços insere um caracter, dependendo de quem está a jogar.
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    // Vamos calcular qual o índice da nossa GameBoard a que corresponde a posição 
                    // atual do cursor de jogo
                    var gameBoardPos = (cursor.Position.X - 1) / 2 + (cursor.Position.Y - 1) / 2 * 3;
                    System.Diagnostics.Debug.WriteLine($"Space @ {cursor.Position} (posição: {gameBoardPos})");

                    if (board.Jogar(quemJoga, gameBoardPos))
                    {
                        quemJoga = (Jogador)((int)(quemJoga) * -1);
                        System.Diagnostics.Debug.WriteLine("Jogada registada, passada a vez para o outro jogador");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("A ignorar: não é possível jogar nesta posição.");
                    }

                    // O tabuleiro pode ter sido alterado, vamos redesenhá-lo
                    PrintGameConsole();

                    // Se houve um novo movimento, alguém pode ter ganho, ou ter acabado o jogo.
                    Jogador j = board.AvaliarVitoria();

                    if (board.GameOver || j != Jogador.Vazio) DeclararVitoria(j);
                }
            }
        }

        private static void DeclararVitoria(Jogador j)
        {
            if (j == Jogador.Humano)
            {
                System.Diagnostics.Debug.WriteLine("O Humano venceu!");
                pontosJogador1 += 1;
                // Vamos mostrar a mensagem de vitória
                MensagemFim("O jogador 1 venceu!");
            }
            else if(j == Jogador.Computador)
            {
                System.Diagnostics.Debug.WriteLine("O Computador venceu!");
                pontosJogador2 += 1;
                // Vamos mostrar a mensagem de vitória
                MensagemFim("O jogador 2 venceu!");
            }
            // Se já não houver espaços em branco para jogar no tabuleiro
            // e ninguém venceu então é um empate.
            else
            {
                System.Diagnostics.Debug.WriteLine("Empate.");
                MensagemFim("O jogo acabou empatado.");
            }
        }

        /// <summary>
        /// Vamos mostrar uma mensagem no centro de uma nova consola vazia
        /// </summary>
        /// <param name="msg">A mensagem que queremos mostrar</param>
        private static void MensagemFim(string msg)
        {
            // Vamos marcar o jogo como tendo sido concluído
            GameComplete = true;

            // Preparar e mostrar a mensagem de fim
            var fontMaster = Global.LoadFont("font/SomethingBoxy.font");
            var fontVezes2 = fontMaster.GetFont(Font.FontSizes.Two);
            var fontVezes1 = fontMaster.GetFont(Font.FontSizes.One);

            var MsgFimConsole = new Console(16, 5);
            MsgFimConsole.Position = new Point(2, 2);
            
            for(int i = 0; i < MsgFimConsole.Width; i++) MsgFimConsole.Print(i, 0, "-");
            for(int i = 0; i < MsgFimConsole.Width; i++) MsgFimConsole.Print(i, 3, "-");

            MsgFimConsole.Font = fontVezes2;
            MsgFimConsole.Cursor.Move(new Point(0, 1)).Print(msg);
            
            // Vamos questionar os jogadores se querem jogar outra vez
            var MsgFimConsole2 = new Console(32, 2);
            MsgFimConsole2.Font = fontVezes1;
            MsgFimConsole2.Position = new Point(4, 10);
            
            MsgFimConsole2.Print(0, 1, "Jogar novamente? Y/N");
            MsgFimConsole.Children.Add(MsgFimConsole2);

            // Definir este ecrã que acabamos de criar como ativo.
            Global.CurrentScreen = MsgFimConsole;
        }

        private static string Glifo(int i)
        {
            if(i > 0) return "O";
            if(i < 0) return "X";
            return " ";
        }
        /// <summary>
        /// Sair do jogo.
        /// </summary>

        private static int Minimax(GameBoard t, Jogador j)
        {
            if (t.AvaliarVitoria() != Jogador.Vazio | t.GameOver)
                return (int)t.AvaliarVitoria();

            int jogada = -1;
            int score = -2;

            for(int i = 0; i < 9; i++)
            {
                if( (Jogador)t.Board[i] == Jogador.Vazio) 
                {
                    var novoT = t;
                    novoT.Jogar(j, i);
                    var pontosJogada = -Minimax(novoT, (Jogador)((int)j*-1));
                    if(pontosJogada > score) {
                        score = pontosJogada;
                        jogada = i;
                    }
                }
            }
            if(jogada == -1)
                return 0;
            
            return score;
        }
        private static void Quit()
        {
            SadConsole.Game.Instance.Exit();
        }
        #endregion
    }
}

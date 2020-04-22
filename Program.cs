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
        public static string[] GameBoard;

        /// <summary>
        /// O Cursor que permite mover entre os quadrados de jogo.
        /// </summary>
        private static SadConsole.Entities.Entity cursor;

        /// <summary>
        /// O ecrã onde vamos desenhar o tabuleiro de jogo.
        /// </summary>
        private static Console gameConsole;

        /// <summary>
        /// Variável de controlo, se True, é a vez do jogador1, que é o X
        /// </summary>
        private static Boolean jogador1;

        /// <summary>
        /// Variável de controlo, se True, é o jogador 1 que começa.
        /// </summary>
        private static Boolean jogadorInicial = false;

        /// <summary>
        /// Quantos jogos o jogador 1 já ganhou.
        /// </summary>
        private static int pontosJogador1 = 0;

        /// <summary>
        /// Quantos jogos o jogador 2 já ganhou.
        /// </summary>
        private static int pontosJogador2 = 1;

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
            // Criar uma consola "mãe", que vai ser o nosso ecrã
            var console = new Console(Width, Height);
            SadConsole.Global.CurrentScreen = console;

            for(int i = 0; i < Width; i+=10)
            {
                console.Print(i, Height - 1, "0123456789");
            }
            // Preparar umm tipo de letra mais giro (quadrangular) e GRANDE!
            var fontMaster = SadConsole.Global.LoadFont("font/SomethingBoxy.font");
            var fontVezes4 = fontMaster.GetFont(SadConsole.Font.FontSizes.Four);

            // Preparar a consola que contém o nosso tabuleiro
            gameConsole = new Console(7, 7);
            gameConsole.Font = fontVezes4;
            console.Children.Add(gameConsole);

            // Limpar todas as jogadas. O primeiro elemento é o canto superior esquerdo
            // do quadrado de jogo, e o último o canto inferior direito.
            GameBoard = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " " };

            // Vamos chamar um Método que desenha a nosso tabuleiro.
            PrintGameConsole();

            // Agora que temos um tabuleiro, vamos criar um Cursor para sabermos onde estamos.
            cursor = new SadConsole.Entities.Entity(1, 1);
            cursor.Position = new Point(1, 1);
            cursor.Font = fontVezes4;
            cursor.Animation.CurrentFrame[0].Glyph = '_';
            console.Children.Add(cursor);

            // No início de cada partida, invertemos a ordem dos jogadores
            jogadorInicial = !jogadorInicial;

            // E aqui definimos, com base no anterior, se é o jogador1 a jogar agora
            jogador1 = jogadorInicial;

            // Estamos a começar um jogo
            GameComplete = false;
        }

        /// <summary>
        /// Este Método "imprime" para a consola de jogo as jogadas que temos no $GameBoard,
        /// e depois cria as bordas e o tabuleiro de jogo com caractéres ASCII
        /// </summary>
        static void PrintGameConsole()
        {
            // Mostrar os X e O que já tenham sido jogados
            gameConsole.Print(1, 1, $"{GameBoard[0]} {GameBoard[1]} {GameBoard[2]}");
            gameConsole.Print(1, 3, $"{GameBoard[3]} {GameBoard[4]} {GameBoard[5]}");
            gameConsole.Print(1, 5, $"{GameBoard[6]} {GameBoard[7]} {GameBoard[8]}");
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
        }

        /// <summary>
        /// Método chamado a cada frame. Vamos tratar de input no teclado.
        /// </summary>
        /// <param name="time">Tempo decorrido desde o último frame. No nosso caso não importa.</param>
        static void Update(GameTime time)
        {
            // Se o jogo já acabou, só olhamos para duas teclas: Y e N
            if (GameComplete)
            {
                if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Y))
                {
                    System.Console.WriteLine("Inicializar");
                    Init();
                }
                if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.N))
                {
                    System.Console.WriteLine("Sair");
                    Quit();
                }
                // Mesmo que não haja Y/N, vamos sair deste Método, porque não queremos tratar outras teclas.
                return;
            }
            // Vamos mover o cursor por ter sido premida a tecla: Up arrow
            // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                cursor.Position += new Point(0, -2);
                if (cursor.Position.Y < 0) cursor.Position += new Point(0, 2);
            }
            // Vamos mover o cursor por ter sido premida a tecla: Down arrow
            // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                cursor.Position += new Point(0, 2);
                if (cursor.Position.Y > 5) cursor.Position += new Point(0, -2);
            }
            // Vamos mover o cursor por ter sido premida a tecla: Left arrow
            // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                cursor.Position += new Point(-2, 0);
                if (cursor.Position.X < 0) cursor.Position += new Point(2, 0);
            }
            // Vamos mover o cursor por ter sido premida a tecla: Right arrow
            // Alteramos a posição do cursor em 2 caracteres, porque há uma linha pelo meio
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                cursor.Position += new Point(2, 0);
                if (cursor.Position.X > 5) cursor.Position += new Point(-2, 0);
            }
            // A Barra de Espaços insere um caracter, dependendo de quem está a jogar.
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                // Vamos calcular qual o índice da nossa GameBoard a que corresponde a posição 
                // atual do cursor de jogo
                var gameBoardPos = (cursor.Position.X - 1) / 2 + (cursor.Position.Y - 1) / 2 * 3;
                System.Console.WriteLine($"Space @ {cursor.Position} (posição: {gameBoardPos})");

                // Só guardamos o input se esta posição ainda não tiver uma jogada (está em branco)
                if (GameBoard[gameBoardPos] == " ")
                {
                    // Se for a vez do jogador1, colocamos um X, senão um O
                    GameBoard[gameBoardPos] = jogador1 ? "X" : "O";
                    // ...e passamos a vez ao outro jogador
                    jogador1 = !jogador1;
                }
                else
                {
                    System.Console.WriteLine("A ignorar: já ocupado.");
                }

                // O tabuleiro pode ter sido alterado, vamos redesenhá-lo
                PrintGameConsole();

                // Se houve um novo movimento, alguém pode ter ganho, ou ter acabado o jogo.
                // Vamos confirmar, e AvaliarVitoria()
                AvaliarVitoria();
            }
        }

        private static void AvaliarVitoria()
        {
            // Vamos ver, para cada linha possível, se existem 3 caracteres iguais
            foreach (int[] linha in linhas)
            {
                if (GameBoard[linha[0]] == GameBoard[linha[1]]
                    && GameBoard[linha[0]] == GameBoard[linha[2]]
                    && GameBoard[linha[0]] != " ")
                {
                    // Há 3 caracteres iguais numa linha, vamos ver quem ganhou
                    if (GameBoard[linha[0]] == "X")
                    {
                        System.Console.WriteLine("O jogador 1 venceu!");
                        pontosJogador1 += 1;
                        // Vamos mostrar a mensagem de vitória
                        MensagemFim("O jogador 1 venceu!");
                    }
                    else
                    {
                        System.Console.WriteLine("O jogador 2 venceu!");
                        pontosJogador2 += 1;
                        // Vamos mostrar a mensagem de vitória
                        MensagemFim("O jogador 2 venceu!");
                    }
                }
            }
            // Se já não houver espaços em branco para jogar no tabuleiro
            // e ninguém venceu então é um empate.
            if (!GameBoard.Contains(" "))
            {
                System.Console.WriteLine("Empate.");
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

            // Preparar e mostrar à mensagem de fim
            var MsgFimConsole = new Console(12, 12);
            MsgFimConsole.Position = new Point(2, 2);
            var fontMaster = SadConsole.Global.LoadFont("font/SomethingBoxy.font");
            MsgFimConsole.Font = fontMaster.GetFont(SadConsole.Font.FontSizes.Two);
            MsgFimConsole.Print(1, 1, msg);

            // Vamos questionar os jogadores se querem jogar outra vez
            var MsgFimConsole2 = new Console(24, 6);
            MsgFimConsole2.Position = new Point(2, 6);
            MsgFimConsole2.Font = fontMaster.GetFont(SadConsole.Font.FontSizes.One);
            MsgFimConsole2.Print(0, 1, "Jogar novamente? Y/N");
            MsgFimConsole.Children.Add(MsgFimConsole2);

            // Definir este ecrã que acabamos de criar como ativo.
            SadConsole.Global.CurrentScreen = MsgFimConsole;
        }

        /// <summary>
        /// Sair do jogo.
        /// </summary>
        private static void Quit()
        {
            SadConsole.Game.Instance.Exit();
        }
        #endregion
    }
}

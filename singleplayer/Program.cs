using System;
using System.Collections.Generic;
using System.Threading;

namespace singleplayer
{
    class Program
    {
        private static readonly string m_WallSymbol = "#";
        private static readonly string m_PlayerHeadSymbol = "O";
        private static readonly string m_PlayerBodySymbol = "o";
        private static readonly string m_PickableSymbol = "f";

        private static int m_Fps = 5;
        private static Vector2Int m_GridSize = new Vector2Int(10, 10);

        private static GameWorld m_GameWorld = null;
        
        private static IKeyboard m_Keyboard = new Keyboard();

        private static int m_FrameCount = 0;

        static void Main(string[] args)
        {
            m_GameWorld = new GameWorld(m_GridSize);

            m_GameWorld.Player.Head = new Vector2Int(2, 2);
            m_GameWorld.Player.Direction = new Vector2Int(1, 0);

            m_GameWorld.Player.OnDeath += (snake) =>
            {
                // temp bandaid: repaint all window or text content with different length will remain after lose
                Console.Clear();
            };

            Thread drawThread = new Thread(DrawHandler);
            Thread inputThread = new Thread(InputHandler);

            drawThread.Start();
            inputThread.Start();
        }

        private static void InputHandler()
        {
            while (true)
            {
                InputFrame();
                Thread.Sleep(50);
            }
        }

        private static void DrawHandler()
        {
            int delay = (int)(1f / m_Fps * 1000);

            while (true)
            {
                m_GameWorld.Tick();

                m_FrameCount += 1;

                ClearFrame();
                DrawFrame();

                Thread.Sleep(delay);
            }
        }

        private static void InputFrame()
        {
            // this affects even when console window is not active/foreground

            //if (m_Keyboard.IsKeyPressed(KeyboardKey.ArrowUp))
            //{
            //    m_GameWorld.Player.SetDirection(0, -1);
            //}
            //else if (m_Keyboard.IsKeyPressed(KeyboardKey.ArrowDown))
            //{
            //    m_GameWorld.Player.SetDirection(0, 1);
            //}
            //else if (m_Keyboard.IsKeyPressed(KeyboardKey.ArrowLeft))
            //{
            //    m_GameWorld.Player.SetDirection(-1, 0);
            //}
            //else if (m_Keyboard.IsKeyPressed(KeyboardKey.ArrowRight))
            //{
            //    m_GameWorld.Player.SetDirection(1, 0);
            //}
            //else if (m_Keyboard.IsKeyPressed(KeyboardKey.Space))
            //{
            //    m_GameWorld.DebugShouldPlayerGrow = true;
            //}

            ConsoleKeyInfo info = Console.ReadKey();
            ConsoleKey key = info.Key;
            
            if (key == ConsoleKey.UpArrow)
            {
                m_GameWorld.Player.SetDirection(0, -1);
            }
            else if (key == ConsoleKey.DownArrow)
            {
                m_GameWorld.Player.SetDirection(0, 1);
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                m_GameWorld.Player.SetDirection(-1, 0);
            }
            else if (key == ConsoleKey.RightArrow)
            {
                m_GameWorld.Player.SetDirection(1, 0);
            }
            else if (key == ConsoleKey.Spacebar)
            {
                m_GameWorld.DebugShouldPlayerGrow = true;
            }
        }

        private static void ClearFrame()
        {
            // todo: find proper way to clear

            // https://stackoverflow.com/questions/5435460/console-application-how-to-update-the-display-without-flicker#answer-47446144

            Console.SetCursorPosition(0, 0);
        }

        private static void DrawFrame()
        {
            DrawGrid();

            Console.WriteLine($"Frame: {m_FrameCount}");
            Console.WriteLine($"Score: {m_GameWorld.Player.Score}");
            Console.WriteLine($"Body: {m_GameWorld.Player.Body.Count}");
        }

        private static void PrintWallRow()
        {
            for (int x = 0; x < m_GameWorld.GridSize.X + 2; x++)
            {
                Console.Write(m_WallSymbol);
            }

            Console.Write("\n");
        }

        private static void DrawGrid()
        {
            PrintWallRow();

            for (int y = 0; y < m_GameWorld.GridSize.Y; y++)
            {
                Console.Write(m_WallSymbol);

                ConsoleColor color = Console.ForegroundColor;

                for (int x = 0; x < m_GameWorld.GridSize.X; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (pos == m_GameWorld.Player.Head)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(m_PlayerHeadSymbol);
                        Console.ForegroundColor = color;
                        continue;
                    }

                    if (pos == m_GameWorld.Pickable)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(m_PickableSymbol);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    
                    if (m_GameWorld.Player.Body.Contains(pos))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(m_PlayerBodySymbol);
                        Console.ForegroundColor = color;
                        continue;
                    }

                    Console.Write(" ");
                }

                Console.Write(m_WallSymbol);

                Console.Write("\n");
            }

            PrintWallRow();
        }
    }
}

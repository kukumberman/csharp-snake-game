using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace singleplayer
{
    class Program
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static readonly int VK_SPACE = 0x20;
        private static readonly int VK_LEFT = 0x25;
        private static readonly int VK_UP = 0x26;
        private static readonly int VK_RIGHT = 0x27;
        private static readonly int VK_DOWN = 0x28;

        private static readonly Random m_Random = new Random();

        private static readonly string m_WallSymbol = "#";

        private static int m_Score = 0;

        private static bool m_DebugShouldGrow = false;

        private static Vector2Int m_GridSize = new Vector2Int(10, 10);
        private static Vector2Int m_Pickable = new Vector2Int(5, 5);

        private static Vector2Int m_Direction = new Vector2Int(1, 0);
        private static Vector2Int m_Head = new Vector2Int(2, 2);
        private static List<Vector2Int> m_Body = new List<Vector2Int>();

        static void Main(string[] args)
        {
            Thread drawThread = new Thread(DrawHandler);
            Thread inputThread = new Thread(InputHandler);

            drawThread.Start();
            inputThread.Start();
        }

        private static bool IsKeyPressed(int key)
        {
            return (GetAsyncKeyState(key) & 1) != 0;
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
            while (true)
            {
                // todo: find proper way to clear
                Console.SetCursorPosition(0, 0); 

                TickFrame();

                DrawFrame();

                Console.WriteLine($"Score: {m_Score}");
                Console.WriteLine($"Body: {m_Body.Count}");

                Thread.Sleep(250);
            }
        }

        private static void InputFrame()
        {
            if (IsKeyPressed(VK_UP))
            {
                SetDirection(0, -1);
            }
            else if (IsKeyPressed(VK_DOWN))
            {
                SetDirection(0, 1);
            }
            else if (IsKeyPressed(VK_LEFT))
            {
                SetDirection(-1, 0);
            }
            else if (IsKeyPressed(VK_RIGHT))
            {
                SetDirection(1, 0);
            }
            else if (IsKeyPressed(VK_SPACE))
            {
                m_DebugShouldGrow = true;
            }
        }

        private static void TickFrame()
        {
            m_Head.X += m_Direction.X;
            m_Head.Y += m_Direction.Y;

            if (m_Head.X == m_GridSize.X)
            {
                m_Head.X = 0;
            }
            else if (m_Head.X < 0)
            {
                m_Head.X = m_GridSize.X - 1;
            }

            if (m_Head.Y == m_GridSize.Y)
            {
                m_Head.Y = 0;
            }
            else if (m_Head.Y < 0)
            {
                m_Head.Y = m_GridSize.Y - 1;
            }

            bool shouldEat = m_Head == m_Pickable;

            bool shouldGrow = shouldEat || m_DebugShouldGrow;

            if (shouldEat)
            {
                SpawnFood();
            }

            if (shouldGrow)
            {
                m_Score += 1;

                m_Body.Insert(0, m_Head);
            }
            else if (m_Body.Count == 0)
            {
                m_Body.Add(m_Head);
            }
            else if (m_Body.Count == 1)
            {
                m_Body[0] = m_Head;
            }
            else if (m_Body.Count > 1)
            {
                m_Body.Insert(0, m_Head);
                m_Body.RemoveAt(m_Body.Count - 1);
            }

            for (int i = 1; i < m_Body.Count; i++)
            {
                if (m_Body[i] == m_Head)
                {
                    DeathHandler();
                    break;
                }
            }

            if (m_DebugShouldGrow)
            {
                m_DebugShouldGrow = false;
            }
        }

        private static void DrawFrame()
        {
            for (int x = 0; x < m_GridSize.X + 2; x++)
            {
                Console.Write(m_WallSymbol);
            }

            Console.Write("\n");

            for (int y = 0; y < m_GridSize.Y; y++)
            {
                Console.Write(m_WallSymbol);

                ConsoleColor color = Console.ForegroundColor;

                for (int x = 0; x < m_GridSize.X; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (pos == m_Head)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("O");
                        Console.ForegroundColor = color;
                        continue;
                    }

                    if (pos == m_Pickable)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("f");
                        Console.ForegroundColor = color;
                        continue;
                    }
                    
                    if (m_Body.Contains(pos))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("o");
                        Console.ForegroundColor = color;
                        continue;
                    }

                    Console.Write(" ");
                }

                Console.Write(m_WallSymbol);

                Console.Write("\n");
            }

            for (int x = 0; x < m_GridSize.X + 2; x++)
            {
                Console.Write(m_WallSymbol);
            }

            Console.Write("\n");
        }

        private static void SetDirection(int x, int y)
        {
            if (Math.Abs(m_Direction.X) != Math.Abs(x))
            {
                m_Direction.X = x;
            }
            if (Math.Abs(m_Direction.Y) != Math.Abs(y))
            {
                m_Direction.Y = y;
            }
        }

        private static void SpawnFood()
        {
            // 1st way

            //m_Pickable.X = m_Random.Next(m_GridSize.X);
            //m_Pickable.Y = m_Random.Next(m_GridSize.Y);

            // 2nd way - generate unique

            List<Vector2Int> emptyCells = new List<Vector2Int>();

            List<Vector2Int> solidCells = new List<Vector2Int>();
            solidCells.Add(m_Head);
            solidCells.Add(m_Pickable);
            solidCells.AddRange(m_Body);

            for (int y = 0; y < m_GridSize.Y; y++)
            {
                for (int x = 0; x < m_GridSize.X; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!solidCells.Contains(pos))
                    {
                        emptyCells.Add(pos);
                    }
                }
            }

            int randomIndex = m_Random.Next(emptyCells.Count);
            m_Pickable = emptyCells[randomIndex];
        }

        private static void DeathHandler()
        {
            m_Score = 0;
            m_Head = new Vector2Int(0, 0);
            m_Direction = new Vector2Int(1, 0);
            m_Body.Clear();
        }
    }
}

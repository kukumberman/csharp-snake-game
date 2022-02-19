using System;
using System.Collections.Generic;

namespace singleplayer
{
    class GameWorld
    {
        public Vector2Int GridSize;
        public Vector2Int Pickable;

        public Snake Player = new Snake();

        public bool DebugShouldPlayerGrow = false;

        private readonly Random m_Random = new Random();

        public GameWorld(Vector2Int gridSize)
        {
            GridSize = gridSize;

            Pickable = new Vector2Int(5, 5);
        }

        public void Tick()
        {
            Player.Move(GridSize);

            bool shouldEat = Player.Head == Pickable;

            if (shouldEat)
            {
                SpawnFood();
            }

            bool shouldGrow = shouldEat || DebugShouldPlayerGrow;

            if (shouldGrow)
            {
                Player.Grow();
            }
            else
            {
                Player.MoveBody();
            }

            if (Player.CheckSelf())
            {
                DeathHandler();
            }

            if (DebugShouldPlayerGrow)
            {
                DebugShouldPlayerGrow = false;
            }
        }

        private void DeathHandler()
        {
            Player.Score = 0;
            Player.Head = new Vector2Int(0, 0);
            Player.Direction = new Vector2Int(1, 0);
            Player.Body.Clear();
        }

        private List<Vector2Int> GetEmptyCells()
        {
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            List<Vector2Int> solidCells = new List<Vector2Int>();
            solidCells.Add(Player.Head);
            solidCells.Add(Pickable);
            solidCells.AddRange(Player.Body);

            for (int y = 0; y < GridSize.Y; y++)
            {
                for (int x = 0; x < GridSize.X; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!solidCells.Contains(pos))
                    {
                        emptyCells.Add(pos);
                    }
                }
            }

            return emptyCells;
        }

        private void SpawnFood()
        {
            // 1st way

            //m_Pickable.X = m_Random.Next(m_GridSize.X);
            //m_Pickable.Y = m_Random.Next(m_GridSize.Y);

            // 2nd way - generate unique

            var emptyCells = GetEmptyCells();
            int randomIndex = m_Random.Next(emptyCells.Count);
            Pickable = emptyCells[randomIndex];
        }
    }
}

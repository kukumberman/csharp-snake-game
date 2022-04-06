using System;
using System.Collections.Generic;

namespace singleplayer
{
    class Snake
    {
        public Vector2Int Head;
        public Vector2Int Direction;
        public List<Vector2Int> Body = new List<Vector2Int>();
        public int Score;

        public event Action<Snake> OnDeath = null;

        public void Move(Vector2Int gridSize)
        {
            Head.X += Direction.X;
            Head.Y += Direction.Y;

            if (Head.X == gridSize.X)
            {
                Head.X = 0;
            }
            else if (Head.X < 0)
            {
                Head.X = gridSize.X - 1;
            }

            if (Head.Y == gridSize.Y)
            {
                Head.Y = 0;
            }
            else if (Head.Y < 0)
            {
                Head.Y = gridSize.Y - 1;
            }
        }

        public void Grow()
        {
            Score += 1;
            Body.Insert(0, Head);
        }

        public void MoveBody()
        {
            if (Body.Count == 0)
            {
                Body.Add(Head);
            }
            else if (Body.Count == 1)
            {
                Body[0] = Head;
            }
            else if (Body.Count > 1)
            {
                Body.Insert(0, Head);
                Body.RemoveAt(Body.Count - 1);
            }
        }

        public bool CheckSelf()
        {
            for (int i = 1; i < Body.Count; i++)
            {
                if (Body[i] == Head)
                {
                    OnDeath?.Invoke(this);
                    return true;
                }
            }

            return false;
        }

        public void SetDirection(int x, int y)
        {
            if (Math.Abs(Direction.X) != Math.Abs(x))
            {
                Direction.X = x;
            }
            if (Math.Abs(Direction.Y) != Math.Abs(y))
            {
                Direction.Y = y;
            }
        }
    }
}

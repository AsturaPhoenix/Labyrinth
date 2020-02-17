using System;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth2D
{
    public class Controller : MonoBehaviour
    {
        public float Speed;
        public Maze Maze;

        private Vector2 velocity;
        private Queue<int> directionQueue = new Queue<int>();

        public Vector2 Position
        {
            get => new Vector2((transform.localPosition.x - 1.5f) / 2, (-transform.localPosition.y - .5f) / 2);
            set
            {
                transform.localPosition = new Vector3(1.5f + 2 * value.x, -.5f - 2 * value.y, 0);
            }
        }

        public int Direction
        {
            get
            {
                if (velocity.x < 0)
                    return 0;
                if (velocity.y < 0)
                    return 1;
                if (velocity.x > 0)
                    return 2;
                if (velocity.y > 0)
                    return 3;
                return -1;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        velocity = new Vector2(-Speed, 0);
                        break;
                    case 1:
                        velocity = new Vector2(0, -Speed);
                        break;
                    case 2:
                        velocity = new Vector2(Speed, 0);
                        break;
                    case 3:
                        velocity = new Vector2(0, Speed);
                        break;
                    default:
                        velocity = Vector2.zero;
                        break;
                }
            }
        }

        public void EnqueueDirection(int direction)
        {
            if (velocity == Vector2.zero && directionQueue.Count == 0)
                PrimeMovement(direction);

            directionQueue.Enqueue(direction);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                EnqueueDirection(0);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                EnqueueDirection(2);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                EnqueueDirection(1);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                EnqueueDirection(3);
        }

        private void PrimeMovement(int nextDirection)
        {
            Debug.Assert(velocity == Vector2.zero);

            var position = Position;
            var quantized = new Vector2Int((int)Math.Round(position.x), (int)Math.Round(position.y));
            Vector2 residual = quantized - position;

            Maze.Walls walls = Maze[quantized.x, quantized.y];

            if (!walls[nextDirection] &&
                ((nextDirection == 0 || nextDirection == 2) && residual.y != 0 ||
                (nextDirection == 1 || nextDirection == 3) && residual.x != 0))
            {
                if (residual.x < 0)
                {
                    Direction = 0;
                }
                else if (residual.x > 0)
                {
                    Direction = 2;
                }
                else if (residual.y < 0)
                {
                    Direction = 1;
                }
                else if (residual.y > 0)
                {
                    Direction = 3;
                }
            }
        }

        private const int PlanAheadDistance = 5;

        private bool CanExecuteNextSoon()
        {
            int direction = Direction;
            Debug.Assert(direction != -1);
            Debug.Assert(directionQueue.Count > 0);

            var position = Position;
            var quantized = new Vector2Int((int)Math.Round(position.x), (int)Math.Round(position.y));
            var step = new Vector2Int(Math.Sign(velocity.x), Math.Sign(velocity.y));

            int nextDirection = directionQueue.Peek();

            Vector2 residual = quantized - position;
            bool skipCurrent = residual.x != 0 && Math.Sign(residual.x) != step.x ||
                residual.y != 0 && Math.Sign(residual.y) != step.y;

            for (int distance = 0; distance < PlanAheadDistance; ++distance, quantized += step)
            {
                if (skipCurrent)
                {
                    --distance;
                    skipCurrent = false;
                }
                else if (!Maze[quantized.x, quantized.y][nextDirection])
                {
                    return true;
                }

                if (Maze[quantized.x, quantized.y][direction])
                    return false;
            }
            return false;
        }

        public void FixedUpdate()
        {
            var position = Position;
            var quantized = new Vector2Int((int)Math.Round(position.x), (int)Math.Round(position.y));
            float frameTime = Time.fixedDeltaTime;

            Maze.Walls walls = Maze[quantized.x, quantized.y];
            var nextDirection = directionQueue.Count > 0 ? directionQueue.Peek() : -1;
            bool canExecuteNext = nextDirection != -1 && !walls[nextDirection];

            Vector2 residual;

            if (velocity != Vector2.zero)
            {
                var direction = Direction;

                if (nextDirection != -1 && (Math.Abs(nextDirection - direction) == 2 || !CanExecuteNextSoon()))
                {
                    velocity = Vector2.zero;
                    directionQueue.Clear();
                    return;
                }

                residual = quantized - position;

                float timeToCenter = residual.x != 0 ? residual.x / velocity.x :
                             residual.y != 0 ? residual.y / velocity.y :
                             0;

                if (timeToCenter > frameTime || !(walls[direction] || canExecuteNext && timeToCenter >= 0))
                {
                    Position += velocity * frameTime;
                    return;
                }
                else if (timeToCenter > 0)
                {
                    Position = quantized;
                    frameTime -= timeToCenter;
                    velocity = Vector2.zero;
                }
            }

            if (directionQueue.Count > 0)
            {
                residual = quantized - Position;

                if (canExecuteNext && residual.x == 0 && residual.y == 0 ||
                    residual.x != 0 && (nextDirection == 0 || nextDirection == 2) ||
                    residual.y != 0 && (nextDirection == 1 || nextDirection == 3))
                {
                    Direction = directionQueue.Dequeue();
                    Position += velocity * frameTime;
                }
                else
                {
                    directionQueue.Clear();
                }
            }
        }
    }
}

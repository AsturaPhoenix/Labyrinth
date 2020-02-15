using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            directionQueue.Enqueue(0);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            directionQueue.Enqueue(2);
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            directionQueue.Enqueue(1);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            directionQueue.Enqueue(3);

        int direction = Direction;
        if (direction != -1 && directionQueue.Count > 0 && (Math.Abs(directionQueue.Peek() - direction) == 2 || !CanExecuteNextSoon()))
        {
            velocity = Vector2.zero;
            directionQueue.Clear();
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

    void FixedUpdate()
    {
        var position = Position;
        var quantized = new Vector2Int((int)Math.Round(position.x), (int)Math.Round(position.y));
        float frameTime = Time.deltaTime;

        Maze.Walls walls = Maze[quantized.x, quantized.y];
        bool canExecuteNext = directionQueue.Count > 0 && !walls[directionQueue.Peek()];

        Vector2 residual;

        if (velocity != Vector2.zero)
        {
            residual = quantized - position;

            float timeToCenter = residual.x != 0 ? residual.x / velocity.x :
                         residual.y != 0 ? residual.y / velocity.y :
                         0;

            if (timeToCenter > frameTime || !(walls[Direction] || canExecuteNext && timeToCenter >= 0))
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
            var nextDirection = directionQueue.Peek();

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

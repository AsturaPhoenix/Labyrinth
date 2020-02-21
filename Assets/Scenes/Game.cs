using UnityEngine;

public interface Game
{
    int[] Dimensions { get; }
    void NewMaze();
    GameObject Settings();
    void Export();
    bool Import();
}

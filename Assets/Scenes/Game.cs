using UnityEngine;

public interface Game
{
    int[] Dimensions { get; }
    void NewMaze();
    GameObject Settings();
    string Export();
    bool Import(string serialized);
}

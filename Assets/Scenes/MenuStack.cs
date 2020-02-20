using UnityEngine;

public interface MenuStack
{
    void Shift(GameObject submenu);
    void Pop();
    void Destroy();
}

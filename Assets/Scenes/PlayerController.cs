using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    private Rigidbody2D physics;
    
    void Start()
    {
        physics = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        physics.AddForce(Speed * new Vector2
        {
            x = (Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0),
            y = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0)
        });
    }
}

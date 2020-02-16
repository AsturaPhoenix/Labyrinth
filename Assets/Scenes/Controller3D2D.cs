using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller3D2D : MonoBehaviour
{
    public float Force, Torque;

    private Rigidbody physics;

    void Start()
    {
        physics = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void FixedUpdate()
    {
        Vector3 force = new Vector3();

        if (Input.GetKey(KeyCode.LeftArrow))
            physics.AddRelativeTorque(0, -Torque, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            physics.AddRelativeTorque(0, Torque, 0);
        
        physics.AddRelativeTorque(0, Torque * Input.GetAxis("Mouse X"), 0);

        if (Input.GetKey(KeyCode.A))
            --force.x;
        if (Input.GetKey(KeyCode.D))
            ++force.x;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetMouseButton(0))
            ++force.z;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetMouseButton(2))
            --force.z;

        physics.AddRelativeForce(Force * force.normalized);
    }
}

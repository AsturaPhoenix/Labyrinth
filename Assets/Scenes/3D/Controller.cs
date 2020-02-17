using UnityEngine;

namespace Labyrinth3D
{
    public class Controller : MonoBehaviour
    {
        public float Acceleration, Speed, Torque;

        private Rigidbody physics;

        private void Start()
        {
            physics = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                physics.AddRelativeTorque(0, -Torque, 0);
            if (Input.GetKey(KeyCode.RightArrow))
                physics.AddRelativeTorque(0, Torque, 0);
            if (Input.GetKey(KeyCode.Q))
                physics.AddRelativeTorque(0, 0, Torque);
            if (Input.GetKey(KeyCode.E))
                physics.AddRelativeTorque(0, 0, -Torque);

            physics.AddRelativeTorque(-Torque * Input.GetAxis("Mouse Y"), Torque * Input.GetAxis("Mouse X"), 0);
            
            Vector3 direction = new Vector3();

            if (Input.GetKey(KeyCode.A))
                --direction.x;
            if (Input.GetKey(KeyCode.D))
                ++direction.x;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetMouseButton(0))
                ++direction.z;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetMouseButton(1))
                --direction.z;
            if (Input.GetKey(KeyCode.LeftShift))
                ++direction.y;
            if (Input.GetKey(KeyCode.LeftControl))
                --direction.y;

            Vector3 acceleration = Acceleration * (physics.rotation * direction.normalized);
            if ((physics.velocity + acceleration * Time.fixedDeltaTime).sqrMagnitude > Speed * Speed)
                acceleration = Vector3.ClampMagnitude(acceleration, Speed / physics.velocity.magnitude);

            physics.AddForce(acceleration, ForceMode.Acceleration);
        }
    }
}

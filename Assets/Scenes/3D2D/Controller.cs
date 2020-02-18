using UnityEngine;

namespace Labyrinth3D2D
{
    public class Controller : MonoBehaviour
    {
        public float Acceleration, Speed, Torque, JumpSpeed;

        private Rigidbody physics;
        private float y0;
        private bool jump = false;

        private bool Grounded => physics.position.y <= y0;

        private void Start()
        {
            physics = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            y0 = physics.position.y;
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2)) && Grounded)
                jump = true;
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                physics.AddRelativeTorque(0, -Torque, 0);
            if (Input.GetKey(KeyCode.RightArrow))
                physics.AddRelativeTorque(0, Torque, 0);

            physics.AddRelativeTorque(0, Torque * Input2.GetAxis("Mouse X"), 0);

            if (Grounded)
            {
                Vector3 direction = new Vector3();

                if (Input.GetKey(KeyCode.A))
                    --direction.x;
                if (Input.GetKey(KeyCode.D))
                    ++direction.x;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetMouseButton(0))
                    ++direction.z;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetMouseButton(1))
                    --direction.z;

                Vector3 acceleration = Acceleration * (physics.rotation * direction.normalized);
                if ((physics.velocity + acceleration * Time.fixedDeltaTime).sqrMagnitude > Speed * Speed)
                    acceleration = Vector3.ClampMagnitude(acceleration, Speed / physics.velocity.magnitude);

                physics.AddForce(acceleration, ForceMode.Acceleration);

                if (jump)
                {
                    physics.AddForce(0, JumpSpeed, 0, ForceMode.VelocityChange);
                }
            }

            jump = false;
        }
    }
}

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

        private void OnTriggerEnter(Collider other)
        {
            GetComponentInParent<GameMenuLauncher>().Show();
        }

        private void Update()
        {
            if (Input.GetAxis("Jump") > 0 && Grounded)
                jump = true;

            physics.AddRelativeTorque(0, Torque * Input2.GetAxis("Frame Yaw"), 0);
        }

        private void FixedUpdate()
        {
            physics.AddRelativeTorque(0, Torque * Input.GetAxis("Yaw"), 0);

            if (Grounded)
            {
                Vector3 direction = new Vector3(Input.GetAxis("X"), 0, Input.GetAxis("Z") + Input.GetAxis("Z (Pitch-Locked)"));

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

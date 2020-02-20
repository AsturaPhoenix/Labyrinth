using UnityEngine;

namespace Labyrinth3D
{
    public class Controller : MonoBehaviour
    {
        public float Acceleration, Speed, AngularAcceleration, LevelingConstant, LevelingAcceleration, ScrollMultiplier = 1;

        private Rigidbody physics;
        private Vector2 scroll = new Vector2();

        private void Start()
        {
            physics = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnTriggerEnter(Collider other)
        {
            GetComponentInParent<GameMenuLauncher>().Show();
        }

        private void Update()
        {
            scroll += ScrollMultiplier * Input.mouseScrollDelta;

            physics.AddTorque(0, AngularAcceleration * Input2.GetAxis("Mouse X"), 0, ForceMode.Acceleration);
            physics.AddRelativeTorque(-AngularAcceleration * Input2.GetAxis("Mouse Y"), 0, 0, ForceMode.Acceleration);
        }

        private void FixedUpdate()
        {
            Vector2 rotation = new Vector2();

            if (Input.GetKey(KeyCode.LeftArrow))
                --rotation.y;
            if (Input.GetKey(KeyCode.RightArrow))
                ++rotation.y;
            if (Input.GetKey(KeyCode.DownArrow))
                --rotation.x;
            if (Input.GetKey(KeyCode.UpArrow))
                ++rotation.x;
            
            physics.AddTorque(0, AngularAcceleration * rotation.y, 0, ForceMode.Acceleration);

            float dTheta = (180 - (180 + physics.rotation.eulerAngles.z) % 360) * Mathf.Deg2Rad;
            physics.AddRelativeTorque(AngularAcceleration * rotation.x, 0,
                Mathf.Clamp(LevelingConstant * dTheta, -LevelingAcceleration, LevelingAcceleration), ForceMode.Acceleration);

            Vector3 direction = new Vector3();

            if (Input.GetKey(KeyCode.A))
                --direction.x;
            if (Input.GetKey(KeyCode.D))
                ++direction.x;
            if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
                ++direction.z;
            if (Input.GetKey(KeyCode.S) || Input.GetMouseButton(1))
                --direction.z;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.R))
                ++direction.y;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.F)) 
                --direction.y;

            if (scroll.x >= 1)
            {
                ++direction.x;
                --scroll.x;
            }
            else if (scroll.x <= -1)
            {
                --direction.x;
                ++scroll.x;
            }
            if (scroll.y >= 1)
            {
                ++direction.y;
                --scroll.y;
            }
            else if (scroll.y <= -1)
            {
                --direction.y;
                ++scroll.y;
            }

            Vector3 acceleration = Acceleration * (physics.rotation * direction.normalized);
            if ((physics.velocity + acceleration * Time.fixedDeltaTime).sqrMagnitude > Speed * Speed)
                acceleration = Vector3.ClampMagnitude(acceleration, Speed / physics.velocity.magnitude);

            physics.AddForce(acceleration, ForceMode.Acceleration);
        }
    }
}

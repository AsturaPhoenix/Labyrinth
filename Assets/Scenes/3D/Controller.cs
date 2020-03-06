using UnityEngine;

namespace Labyrinth3D
{
    public class Controller : MonoBehaviour
    {
        public float Acceleration, Speed, AngularAcceleration, AngularEquilibriumConstant, LevelingAcceleration, ScrollMultiplier = 1, ScrollAheadTime;

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
            float scrollAheadFrames = ScrollAheadTime / Time.fixedDeltaTime;
            scroll.x = Mathf.Clamp(scroll.x, -scrollAheadFrames, scrollAheadFrames);
            scroll.y = Mathf.Clamp(scroll.y, -scrollAheadFrames, scrollAheadFrames);

            physics.AddTorque(0, AngularAcceleration * Input2.GetAxis("Frame Yaw"), 0, ForceMode.Acceleration);
            physics.AddRelativeTorque(AngularAcceleration * Input2.GetAxis("Frame Pitch"), 0, 0, ForceMode.Acceleration);
        }

        private static float NormalizeDegreesAboutZero(float degrees) => (degrees % 360 + 360 + 180) % 360 - 180;
        private float RotateTowards(float degreesRemaining, float angularAcceleration) =>
            Mathf.Clamp(AngularEquilibriumConstant * NormalizeDegreesAboutZero(degreesRemaining) * Mathf.Deg2Rad, -angularAcceleration, angularAcceleration);

        private void FixedUpdate()
        {
            physics.AddTorque(0, AngularAcceleration * Input.GetAxis("Yaw"), 0, ForceMode.Acceleration);

            Vector3 euler = physics.rotation.eulerAngles;
            float nex = Mathf.Clamp(NormalizeDegreesAboutZero(euler.x) / 90, -1, 1);
            physics.AddRelativeTorque(
                AngularAcceleration * (Mathf.Clamp(Input.GetAxis("Pitch"), -1 - nex, 1 - nex) - Input.GetAxis("Jump") * nex),
                0,
                RotateTowards(-euler.z, LevelingAcceleration), ForceMode.Acceleration);

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

using UnityEngine;
using UnityEngine.InputSystem;

namespace Labyrinth3D
{
    public class Controller : MonoBehaviour
    {
        public InputActionAsset InputActions;
        public float Acceleration, Speed, AngularAcceleration, AngularEquilibriumConstant, LevelingAcceleration, ScrollAheadTime;

        private Rigidbody physics;
        private TouchManipulation touch;
        private Vector2 scroll;
        private Vector2 touchLook;
        private float pinchThrottle;

        private InputAction look, clampedPitch, resetPitch, strafe, scrollStrafe, thrust;

        private void Start()
        {
            physics = GetComponent<Rigidbody>();
            touch = GetComponent<TouchManipulation>();

            look = InputActions["Look"];
            clampedPitch = InputActions["Clamped Pitch"];
            resetPitch = InputActions["Reset Pitch"];
            strafe = InputActions["Strafe"];
            scrollStrafe = InputActions["Scroll Strafe"];
            thrust = InputActions["Thrust"];
            InputActions.Enable();
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnTriggerEnter(Collider other)
        {
            GetComponentInParent<GameMenuLauncher>().Show();
        }

        private static float NormalizeDegreesAboutZero(float degrees) => (degrees % 360 + 360 + 180) % 360 - 180;
        private float RotateTowards(float degreesRemaining, float angularAcceleration) =>
            Mathf.Clamp(AngularEquilibriumConstant * NormalizeDegreesAboutZero(degreesRemaining) * Mathf.Deg2Rad, -angularAcceleration, angularAcceleration);

        private void FixedUpdate()
        {
            touchLook = touch.Delta == null ? Vector2.zero : touchLook + touch.Delta.Value;

            var lookVector = look.ReadValue<Vector2>() + touchLook;

            Vector3 euler = physics.rotation.eulerAngles;
            float nex = Mathf.Clamp(NormalizeDegreesAboutZero(euler.x) / 90, -1, 1);
            
            lookVector.y += Mathf.Clamp(clampedPitch.ReadValue<float>(), -1 + nex, 1 + nex) + resetPitch.ReadValue<float>() * nex;

            physics.AddTorque(0, AngularAcceleration * lookVector.x, 0, ForceMode.Acceleration);
            physics.AddRelativeTorque(-AngularAcceleration * lookVector.y, 0,
                RotateTowards(-euler.z, LevelingAcceleration), ForceMode.Acceleration);
            
            scroll += scrollStrafe.ReadValue<Vector2>();
            float scrollAheadFrames = ScrollAheadTime / Time.fixedDeltaTime;
            scroll.x = Mathf.Clamp(scroll.x, -scrollAheadFrames, scrollAheadFrames);
            scroll.y = Mathf.Clamp(scroll.y, -scrollAheadFrames, scrollAheadFrames);
            
            var strafeVector = strafe.ReadValue<Vector2>();
            pinchThrottle = touch.ScaleDelta == null? 0 : Mathf.Clamp((pinchThrottle + 1) * touch.ScaleDelta.Value - 1, -.9f, 1);

            Vector3 direction = new Vector3(strafeVector.x, strafeVector.y,
                thrust.ReadValue<float>() + pinchThrottle);

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

            float speedLimit = pinchThrottle == 0 ? Speed : Mathf.Abs(pinchThrottle) * Speed;

            Vector3 acceleration = Acceleration * (physics.rotation * direction.normalized);
            if ((physics.velocity + acceleration * Time.fixedDeltaTime).sqrMagnitude > speedLimit * speedLimit)
                acceleration = Vector3.ClampMagnitude(acceleration, speedLimit / physics.velocity.magnitude);

            physics.AddForce(acceleration, ForceMode.Acceleration);
        }
    }
}

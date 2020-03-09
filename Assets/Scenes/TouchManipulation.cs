using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using V1 = UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using V2 = UnityEngine.InputSystem.EnhancedTouch;

public class TouchManipulation : MonoBehaviour
{
    public Vector2? Delta { get; private set; }
    public float? ScaleDelta { get; private set; }

    public float ScaleFactor = 1;

    private float? deviation;

    private void OnEnable()
    {
        Input.simulateMouseWithTouches = false;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        Input.simulateMouseWithTouches = true;
    }

    private void FixedUpdate()
    {
        var touches = V2.Touch.activeTouches;

        if (touches.Count == 0)
        {
            Delta = null;
            ScaleDelta = null;
            deviation = null;
        }
        else
        {
            var moves = from t in touches where t.phase != V1.TouchPhase.Began select t;
            
            var movePositions = from m in moves select m.screenPosition;
            if (deviation != null && deviation > 0)
            {
                Debug.Assert(movePositions.Any());

                var moveCentroid = movePositions.Average();
                ScaleDelta = movePositions.Average(v => (v - moveCentroid).magnitude) / deviation.Value;
            }
            else
            {
                ScaleDelta = null;
            }

            var nextPositions = from t in touches where
                                t.phase == V1.TouchPhase.Began ||
                                t.phase == V1.TouchPhase.Moved ||
                                t.phase == V1.TouchPhase.Stationary select t.screenPosition;
            if (nextPositions.Any())
            {
                var centroid = nextPositions.Average();
                deviation = (from t in touches select t.screenPosition).Average(v => (v - centroid).magnitude);
            }
            else
            {
                deviation = null;
            }

            Delta = ScaleFactor * moves.Select(t => t.delta).Average() / Mathf.Min(Screen.width, Screen.height);
        }
    }
}

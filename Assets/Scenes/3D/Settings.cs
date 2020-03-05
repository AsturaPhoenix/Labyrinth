using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Labyrinth3D
{
    public class Settings : MonoBehaviour
    {
        public GameObject WallTypeToggles, WallTypePrototypes;
        public Render Render;

        private void Start()
        {
            var toggles = from toggle in WallTypeToggles.transform.Cast<Transform>() select toggle.GetComponent<Toggle>();
            var prototypes = from toggle in WallTypePrototypes.transform.Cast<Transform>() select toggle.GetComponentInChildren<WallPreview>();
            foreach (var wallType in toggles.Zip(prototypes.Where(prototype => prototype != null), (toggle, prototype) => new { toggle, prototype }))
            {
                var toggle = wallType.toggle;
                var prototype = wallType.prototype;

                toggle.onValueChanged.AddListener(_ =>
                {
                    Render.Wall = prototype.Wall;
                    Render.Edge = prototype.Edge;
                    Render.Corner = prototype.IncludeCorner ? prototype.Corner : null;

                    Render.UpdateModels();
                });

                toggle.isOn = Render.Wall == prototype.Wall &&
                    Render.Edge == prototype.Edge &&
                    Render.Corner == prototype.IncludeCorner ? prototype.Corner : null;
            }
        }
    }
}
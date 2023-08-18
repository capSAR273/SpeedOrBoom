using UnityEngine;

namespace SpeedOrBoom.Modules
{
    [DisallowMultipleComponent]
    internal class DelayModule : MonoBehaviour
    {
        private readonly SpeedOrBoom speedClass = new SpeedOrBoom();
        public void modeChange()
        {
            speedClass.gamemodeActive = !speedClass.gamemodeActive;
        }
    }
}

using UnityEngine;
using TLDLoader;
using DelayModule = SpeedOrBoom.Modules.DelayModule;

namespace SpeedOrBoom
{
    public class SpeedOrBoom : Mod
    {
        private readonly DelayModule delayMod = new DelayModule();

        public bool fastEnough;
        public bool gamemodeActive;
        public bool bombWatchingSpeed = false;
        public float minSpeed = 80.0f;
        public float radius = 150.0F;

        private GUIStyle redStyle = null;
        private GUIStyle greenStyle = null;
        private GUIStyle blueStyle = null;

        public float carBodyPower = 100.0F;
        public float partPower = 30.0F;

        public override string ID => "8723";
        public override string Name => "Speed or Boom";
        public override string Author => "Fat Rat";
        public override string Version => "1.0";

        public override void OnLoad()
        {
            if (mainscript.M.load)
                return;
        }
        public override void OnGUI()
        {
            InitStyles();
#if DEBUG
            if ( mainscript.M.player.lastCar != null)
            {

                GUI.Label(new Rect(
                100, 100, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=35>Gamemode Active: {0}</size></color>", // format string
                gamemodeActive)); // format arguments

                GUI.Label(new Rect(
                100, 150, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Fast Enough: {0}</size></color>", // format string
                fastEnough)); // format arguments
            }
                GUI.Label(new Rect(
                100, 200, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Bomb Watching Speed: {0}</size></color>", // format string
                bombWatchingSpeed)); // format arguments
#endif
            if (mainscript.M.player.lastCar != null || mainscript.M.player.Car != null)
            {
                if (!bombWatchingSpeed && !gamemodeActive)
                {
                    GUI.Box(new Rect(25, 150, 35, 25), string.Format("<color=black><size=15>{0}</size></color>", (int)mainscript.M.player.lastCar.speed), redStyle);
                }
                else if (bombWatchingSpeed && gamemodeActive)
                {
                    GUI.Box(new Rect(25, 150, 35, 25), string.Format("<color=yellow><size=15>{0}</size></color>", (int)mainscript.M.player.lastCar.speed), blueStyle);
                }
                else if (gamemodeActive)
                {
                    GUI.Box(new Rect(25, 150, 35, 25), string.Format("<color=black><size=15>{0}</size></color>", (int)mainscript.M.player.lastCar.speed), greenStyle);
                }
            }
        }
        private void InitStyles()
        {
            if (redStyle == null)
            {
                redStyle = new GUIStyle(GUI.skin.box);
                redStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 1f));
            }
            if (greenStyle == null)
            {
                greenStyle = new GUIStyle(GUI.skin.box);
                greenStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 1f));
            }
            if (blueStyle == null)
            {
                blueStyle = new GUIStyle(GUI.skin.box);
                blueStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 1f, 1f));
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public override void Update()
        {
            try
            {
                if (mainscript.M.player.Car != null)
                {
                    //Pissing resets the bomb activation if you are somewhat standstill in the car
                    if (mainscript.M.player.pissing && mainscript.M.player.Car.speed > 0 && mainscript.M.player.Car.speed < 10)
                    {
                        delayMod.Invoke("modeChange", 1.0f);
                    }
                    //Player has met the min speed requirement for the bomb to be enabled and to be going fast enough
                    if (gamemodeActive && mainscript.M.player.Car.speed > minSpeed)
                    {
                        bombWatchingSpeed = true;
                    }
                    //Car dipped under the minimum speed, might blow up
                    else if (mainscript.M.player.Car.speed < minSpeed - 1)
                    {
                        if (gamemodeActive && mainscript.M.player.lastCar.speed < minSpeed - 1 && bombWatchingSpeed)
                        {
                            //Go Boom
                            explode();
                        }
                    }
                }
            }
            catch { }
        }

        

        public void explode()
        {
            Vector3 currentPlayerPosition = mainscript.M.player.Tb.position;
            Rigidbody carBody = mainscript.M.player.Car.gameObject.GetComponent<Rigidbody>();
            carBody.AddExplosionForce(5f, currentPlayerPosition, 25f, 2.0F, ForceMode.VelocityChange);
            
            foreach (partslotscript carPartSlot in mainscript.M.player.Car.gameObject.GetComponentsInChildren<partslotscript>())
            {
                if (carPartSlot != null && carPartSlot.part != null)
                {
                    partscript activePart = carPartSlot.part;

                    foreach (partslotscript subPartSlot in activePart.tosaveitem.partslotscripts)
                    {
                        partscript subPart = subPartSlot.part;
                        if (subPart != null)
                        {
                            Rigidbody rb = carPartSlot.part.GetComponent<Rigidbody>();
                            subPart.FallOFf();
                            rb.AddExplosionForce(partPower, currentPlayerPosition, radius, 25.0F, ForceMode.Impulse);
                        }
                    }
                    activePart.FallOFf();
                    
                }
            }
            gamemodeActive = false;
            bombWatchingSpeed = false;
        }
    }
}
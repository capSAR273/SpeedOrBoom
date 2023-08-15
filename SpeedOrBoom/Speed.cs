using UnityEngine;
using TLDLoader;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;

namespace SpeedOrBoom
{
    public class SpeedOrBoom : Mod
    {
        public bool fastEnough;
        public bool activateBombFlag;
        public bool bombReset = false;
        public float minSpeed = 60.0f;
        public float radius = 150.0F;
        public float whimpPower = 100.0F;
        private GUIStyle redStyle = null;
        private GUIStyle greenStyle = null;
        private GUIStyle blueStyle = null;
        //To be used in the future so player can toggle the type of bomb
        public float crazyPower = 30.0F;

        public override string ID => "8723";
        public override string Name => "Speed or Boom";
        public override string Author => "Fat Rat";
        public override string Version => "0.1";

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
                100, 50, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Minimum Speed: {0}</size></color>", // format string
                mainscript.M.player.lastCar.speed)); // format arguments

                GUI.Label(new Rect(
                100, 100, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=35>Bomb Activated: {0}</size></color>", // format string
                activateBombFlag)); // format arguments

                GUI.Label(new Rect(
                100, 150, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Fast Enough: {0}</size></color>", // format string
                fastEnough)); // format arguments
            }
                GUI.Label(new Rect(
                100, 200, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Bomb Reset: {0}</size></color>", // format string
                bombReset)); // format arguments
#endif
            if (mainscript.M.player.lastCar != null)
            {
                if(!bombReset)
                {
                    GUI.Box(new Rect(25, 150, 25, 25), string.Format("<color=black><size=15>{0}</size></color>", // format string
                        (int)mainscript.M.player.lastCar.speed), redStyle); 
                }
                else if (bombReset && activateBombFlag)
                {
                    GUI.Box(new Rect(25, 150, 25, 25), string.Format("<color=yellow><size=15>{0}</size></color>", (int)mainscript.M.player.lastCar.speed), blueStyle);
                }
                else if(bombReset)
                {  
                    GUI.Box(new Rect(25, 150, 25, 25), string.Format("<color=black><size=15>{0}</size></color>",(int)mainscript.M.player.lastCar.speed), greenStyle);
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
            if(greenStyle == null)
            {
                greenStyle = new GUIStyle(GUI.skin.box);
                greenStyle.normal.background = MakeTex(2,2,new Color(0f, 1f, 0f, 1f));
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
                if (mainscript.M.player.lastCar != null)
                {
                    //Pissing resets the bomb activation if you are somewhat standstill in the car
                    if (mainscript.M.player.pissing && mainscript.M.player.lastCar.speed > 0 && mainscript.M.player.lastCar.speed < 5)
                    {
                        bombReset = true;
                    }
                    //Player has met the min speed requirement for the bomb to be enabled and to be going fast enough
                    if (mainscript.M.player.lastCar.speed > minSpeed)
                    {
                        activateBombFlag = true;
                        fastEnough = true;
                    }
                    //Car dipped under the minimum speed, might blow up
                    else if (mainscript.M.player.lastCar.speed < minSpeed - 1)
                    {
                        fastEnough = false;
                        checkExplode();
                    }
                }
                
            }
            catch { }
        }
        public void checkExplode()
        {
            if (activateBombFlag && !fastEnough && bombReset)
            {
                //Go Boom
                activateBombFlag = false;
                explode();
                bombReset = false;
            }
        }

        public void explode()
        {
            // Get the current position of the player
            Vector3 currentPlayerPosition = mainscript.M.player.Tb.position;
            
            Collider[] colliders = Physics.OverlapSphere(currentPlayerPosition, 150.0f);

            //Find all parts in the collision sphere
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //Detatch parts from the car and apply the explosion force to them
                    partscript part = mainscript.M.player.lastCar.gameObject.GetComponentInChildren<partscript>();
                    part.FallOFf();
                    //rb.AddExplosionForce(whimpPower, currentPlayerPosition, radius, 100.0F, ForceMode.Impulse);

                    rb.AddExplosionForce(crazyPower, currentPlayerPosition, radius, 2.0F, ForceMode.VelocityChange);
                }
            }
        }
    }
}
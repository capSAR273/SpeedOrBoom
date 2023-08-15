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
        public float minSpeed = 50.0f;
        public float radius = 100.0F;
        public float power = 10000.0F;

        public override string ID => "8723";
        public override string Name => "Speed or Boom";
        public override string Author => "Fat Rat";
        public override string Version => "0.1";

        public override void OnLoad()
        {
            Debug.Log("SOB: Mod Loaded");
            if (mainscript.M.load)
                return;
        }

        public override void OnGUI()
        {
            if ( mainscript.M.player.lastCar != null)
            {
                GUI.Label(new Rect(
                100, 100, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Minimum Speed: {0}</size></color>", // format string
                mainscript.M.player.lastCar.speed)); // format arguments

                GUI.Label(new Rect(
                100, 200, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=35>Bomb Activated: {0}</size></color>", // format string
                activateBombFlag)); // format arguments

                GUI.Label(new Rect(
                100, 300, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Fast Enough: {0}</size></color>", // format string
                fastEnough)); // format arguments

                GUI.Label(new Rect(
                100, 400, // offset from the top-left of the screen
                1000, 700), // maximum pixel size of the box
                string.Format("<color=red><size=25>Bomb Reset: {0}</size></color>", // format string
                bombReset)); // format arguments
            }
        }

        public override void Update()
        {
            try
            {
                if (mainscript.M.player.lastCar != null)
                {
                    if(mainscript.M.player.lastCar.speed < 1 && mainscript.M.player.lastCar.speed > 0)
                    {
                        bombReset = true;
                    }
                    if (mainscript.M.player.lastCar.speed > minSpeed)
                    {
                        activateBombFlag = true;
                        //Debug.Log("SOB: Bomb activated");
                        fastEnough = true;
                    }
                    else if (mainscript.M.player.lastCar.speed < minSpeed - .5f)
                    {
                        fastEnough = false;
                        //Debug.Log("SOB: Not fast enough!!!");
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
                Debug.Log("SOB: Calling explode()");
                activateBombFlag = false;
                explode();
                bombReset = false;
            }
            else
            {
                //Debug.Log("SOB: Safe for Now");
                return;
            }
        }

        public void explode()
        {
            // Get the current position of the player
            Vector3 currentPlayerPosition = mainscript.M.player.Tb.position;
            Debug.Log("SOB: EXPLODE TIME");
            
            Collider[] colliders = Physics.OverlapSphere(currentPlayerPosition, 100.0f);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                Debug.Log("SOB: Colliders exist");
                if (rb != null)
                {
                    Debug.Log("SOB: Adding Force/Explosion");
                    rb.AddExplosionForce(power, currentPlayerPosition, radius, 5.0F, ForceMode.VelocityChange);
                }
            }
        }
    }
}
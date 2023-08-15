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
        public carscript car;
        public bool fastEnough;
        public bool activateBombFlag;
        public float radius = 50.0F;
        public float power = 100.0F;

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
                string.Format("<color=red><size=25>Current Speed: {0}</size></color>", // format string
                mainscript.M.player.lastCar.speed)); // format arguments
            }
        }

        public override void Update()
        {
            try
            {
                if (mainscript.M.player.lastCar != null)
                {
                    if ((int)mainscript.M.player.lastCar.speed > 40)
                    {
                        activateBombFlag = true;
                        Debug.Log("SOB: Bomb activated");
                        while ((int)mainscript.M.player.lastCar.speed >= 40)
                        {
                            //Debug.Log("SOB: Fast enough!"); - Disabling this for now as its super spammy
                            fastEnough = true;
                        }
                        if((int)mainscript.M.player.lastCar.speed < 40)
                        {
                            fastEnough = false;
                            Debug.Log("SOB: Not fast enough!!!");
                        }
                        if (activateBombFlag && !fastEnough)
                        {
                            //Go Boom
                            Debug.Log("SOB: Calling explode()");
                            explode();
                            activateBombFlag = false;
                        }
                    }
                }
            }
            catch { }
        }

        public void explode()
        {
            // Get the current position of the player
            Vector3 currentPlayerPosition = mainscript.M.player.Tb.transform.position;
            Debug.Log("SOB: EXPLODE TIME");

            Collider[] colliders = Physics.OverlapSphere(currentPlayerPosition, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, currentPlayerPosition, radius, 3.0F);
            }
        }
    }
}
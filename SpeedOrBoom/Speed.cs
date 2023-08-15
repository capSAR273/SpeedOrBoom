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
        public float power = 1000.0F;

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
            GUI.Label(new Rect(
            100, 100, // offset from the top-left of the screen
            1000, 700), // maximum pixel size of the box
            string.Format("<color=black><size=25>Position: {0}</size></color>", // format string
            mainscript.M.Car.speed)); // format arguments

            GUI.Label(new Rect(
            300, 100, // offset from the top-left of the screen
            1000, 700), // maximum pixel size of the box
            string.Format("<color=black><size=25>Position: {0}</size></color>", // format string
            this.car.speed)); // format arguments
        }

        public override void Update()
        {
            try
            {
                if (this.car.ignition)
                {
                    if (this.car.speed > 40.00f)
                    {
                        activateBombFlag = true;
                        while (this.car.speed > 40.00f)
                        {
                            fastEnough = true;
                        }
                        fastEnough = false;
                        if (activateBombFlag && !fastEnough)
                        {
                            //Go Boom
                            // Get the current position of the player
                            Vector3 currentPlayerPosition = mainscript.M.player.transform.position;

                            Collider[] colliders = Physics.OverlapSphere(currentPlayerPosition, radius);
                            foreach (Collider hit in colliders)
                            {
                                Rigidbody rb = hit.GetComponent<Rigidbody>();

                                if (rb != null)
                                    rb.AddExplosionForce(power, currentPlayerPosition, radius, 3.0F);
                            }
                            activateBombFlag = false;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
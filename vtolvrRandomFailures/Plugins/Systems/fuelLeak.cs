using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;
using System.Collections;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using Valve.Newtonsoft.Json;

namespace vtolvrRandomFailures.Plugins
{
    class fuelLeak :BaseFailure
    {

        public FuelDump fuelDump;
        public float fuelLeakRate;
        public Actor playerActor;

        public int fuelLeakRunTime;
        public int fuelLeakRunTimeMin;
        public int fuelLeakRunTimeMax;

        public fuelLeak()
        {

            failureName = "Fuel Leak";
            failureDescription = "Test";
            failureCategory = "Systems";

            hourlyFailureRate = 1;
            fuelLeakRate = 1;
            failureEnabled = true;
            maxRunCount = 2;

            fuelLeakRunTimeMin = 30;
            fuelLeakRunTimeMax = 1800;

        }

        public override void Run()
        {
            base.Run();
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();

            playerActor = FlightSceneManager.instance.playerActor;

            fuelDump = currentVehicle.GetComponentInChildren<FuelDump>();

            fuelDump.particleSystems.SetEmission(true);
            System.Random rand = new System.Random();
            fuelLeakRunTime = rand.Next(fuelLeakRunTimeMin,fuelLeakRunTimeMax);
            running = true;
            StartCoroutine(leakFuel());
            StartCoroutine(unfailFuelLeak());
        }


        private IEnumerator leakFuel()
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            if (running)
            {
                while (fuelDump.fuelTank.RequestFuel(fuelLeakRate * Time.fixedDeltaTime) > 0f && running)
                {

                    fuelLeakRate = playerActor.flightInfo.playerGs;
                    if (fuelLeakRate > 2)
                    {
                        fuelLeakRate = 2;
                    } else if (fuelLeakRate > 0)
                    {
                        fuelDump.particleSystems.SetEmission(true);
                    } 
                    else if (fuelLeakRate < 0)
                    {
                        fuelLeakRate = 0;
                        fuelDump.particleSystems.SetEmission(false);
                    }
                    Debug.Log(fuelLeakRate);
                    Debug.Log(fuelDump.fuelTank.fuel);
                    yield return wait;
                }

            }

        }

        private IEnumerator unfailFuelLeak()
        {

            yield return new WaitForSeconds(fuelLeakRunTime);
            Debug.Log("Unfailing fuel leak");
            running = false;
            fuelDump.particleSystems.SetEmission(false);

        }
    }
}

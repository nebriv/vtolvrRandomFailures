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
    class engineFailure :BaseFailure
    {
        
        public engineFailure()
        {

            failureName = "Engine Failure";
            failureDescription = "Test";
            failureCategory = "Systems";
            maxRunCount = 2;
            hourlyFailureRate = 1;
            failureEnabled = false;

        }

        public override void Run()
        {
            base.Run();
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            ModuleEngine[] engines = currentVehicle.GetComponentsInChildren<ModuleEngine>();
            
            System.Random rand = new System.Random();
            int index = rand.Next(engines.ToList().Count());

            Debug.Log($"Total Engines: {engines.ToList().Count()}");
            
            if (!engines[index].failed)
            {
                Debug.Log($"Failing Engine {index}");
                engines[index].FailEngine();
                if (rand.Next(1, 100) < 80)
                {
                    Debug.Log($"Unfailing Engine {index}");
                    StartCoroutine(unfailEngine(index, engines));
                }
                else
                {
                    Debug.Log($"Engine {index} is permanently failed");
                }
            }
        }

        private IEnumerator unfailEngine(int index, ModuleEngine[] engines)
        {

            yield return new WaitForSeconds(30);

            engines[index].FullyRepairEngine();


        }
    }
}

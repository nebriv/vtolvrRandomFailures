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
using UnityEngine.UI.Extensions;
using System.IO;

namespace vtolvrRandomFailures.Plugins
{

    class alternatorFailure : BaseFailure
    {

        Actor playerActor;

        public alternatorFailure()
        {

            failureName = "Alternator Failure";
            failureDescription = "Test";
            failureCategory = "Systems";
            maxRunCount = 1;
            hourlyFailureRate = 3;
            failureEnabled = true;

        }


        public override void Run()
        {
            base.Run();
            running = true;

            playerActor = FlightSceneManager.instance.playerActor;

            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            ModuleEngine[] engines = currentVehicle.GetComponentsInChildren<ModuleEngine>();
            
            System.Random rand = new System.Random();
            int index = rand.Next(engines.ToList().Count());

            Debug.Log($"Total Engines: {engines.ToList().Count()}");
            //ModuleEngine engine = engines[index];

            Debug.Log("Creating Alternator Failure FlightWarning");
            FlightWarnings.FlightWarning alternatorFailureWarning = AddWarning("Alternator Failure", genericWarning);

            Debug.Log("Adding Alternator Failure to HUDWarning");
            HUDWarning.flightWarning = alternatorFailureWarning;

            UIImageToggle[] lights = currentVehicle.GetComponentsInChildren<UIImageToggle>();
            string leftOrRight;
            string leftOrRightCAPS;
            if (index == 0)
            {
                leftOrRight = "Left";
                leftOrRightCAPS = "LEFT";
            }
            else
            {
                leftOrRight = "Right";
                leftOrRightCAPS = "RIGHT";
            }


            foreach (ModuleEngine engine in engines)
            {
                engine.alternatorChargeRate = 0f;
            }



            Debug.Log("Adding warntext to HUDWarning");
            
            HUDWarning.setWarnText($"-[ ALTERNATOR FAILURE ]-");
            HUDWarning.runWarning = true;

        }

    }
}

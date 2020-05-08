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
    class dashGuageFailure :BaseFailure
    {
        public MFDManager manager;
        public dashGuageFailure()
        {
            failureName = "Dash Guage Failure";
            failureDescription = "Test";
            failureCategory = "Avionics";
            hourlyFailureRate = 1;
            failureEnabled = false;
        }

        public override void Run()
        {
            base.Run();
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            DashGauge[] dashGuages = currentVehicle.GetComponentsInChildren<DashGauge>();

            foreach (DashGauge guage in dashGuages)
            {
                Debug.Log(guage.name);
            }

        }
    }
}

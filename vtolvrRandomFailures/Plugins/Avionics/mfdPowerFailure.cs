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
    class mfdPowerFailure :BaseFailure
    {
        public MFDManager manager;
        public mfdPowerFailure()
        {
            failureName = "Random MFD Power Toggler";
            failureDescription = "Test";
            failureCategory = "Avionics";
            hourlyFailureRate = 1000;
            failureEnabled = false;
        }

        public override void Run()
        {
            base.Run();
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            manager = currentVehicle.GetComponentInChildren<MFDManager>();
            List<MFD> mfds = manager.mfds;
            System.Random rand = new System.Random();
            Debug.Log($"Found {mfds.Count.ToString()} MFDs");
            int index = rand.Next(mfds.Count);
            mfds[index].TogglePower();
            StartWarning();
        }
    }
}

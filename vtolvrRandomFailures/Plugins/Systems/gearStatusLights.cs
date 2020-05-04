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
    class gearStatusLights : BaseFailure
    {

        public GearAnimator gear;
        public VRInteractable gearInteractable;

        public gearStatusLights()
        {

            failureName = "Gear Status Lights";
            failureDescription = "Hmm something weird is going on with the gear status indicator.";
            failureCategory = "Systems";
            failureRate = 0.01;
            maxRunCount = 5;
            failureEnabled = false;
            running = false;

        }
        public override void Run()
        {
            running = true;

            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            gear = currentVehicle.GetComponentInChildren<GearAnimator>();

            foreach (VRInteractable interactable in currentVehicle.GetComponentsInChildren<VRInteractable>())
            {
                if (interactable.name == "GearInteractable")
                {
                    interactable.OnInteract.AddListener(stopFailure);
                    gearInteractable = interactable;
                    break;
                }

            }           

            StartCoroutine(FlashLightsRandom());

        }

        private IEnumerator FlashLightsRandom()
        {
            var colorList = new List<Color> { Color.black, Color.green, Color.red, Color.yellow };
            System.Random rand = new System.Random();
            int index = rand.Next(colorList.Count);
            Color randColor = colorList[index];

            double randWait = rand.NextDouble();

            if (gear != null && gear.statusLight != null)
            {
                gear.statusLight.toggleColor = randColor;
                gear.statusLight.Toggle();
            }
            else
            {
                Debug.Log("It seems gear or statuslight is null");
            }
            yield return new WaitForSeconds((float)randWait);
            if (running)
            {
                this.StartCoroutine(FlashLightsRandom());
            }
           
        }

        public override void stopFailure()
        {
            base.stopFailure();
            gearInteractable.OnInteract.RemoveListener(stopFailure);
        }
    }
}

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
    class gearStuckUp : BaseFailure
    {

        public GearAnimator gear;
        public VRInteractable gearInteractable;

        public gearStuckUp()
        {

            failureName = "Gear Stuck Up";
            failureDescription = "Gear Stuck Up.";
            failureCategory = "Systems";
            hourlyFailureRate = 1;
            maxRunCount = 5;
            failureEnabled = false;
            running = false;

        }
        public override void Run()
        {
            running = true;

            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            gear = currentVehicle.GetComponentInChildren<GearAnimator>();

            gear.RetractImmediate();
            Color red = new Color();
            gear.statusLight.SetColor(red);

            foreach (VRInteractable interactable in currentVehicle.GetComponentsInChildren<VRInteractable>())
            {
                if (interactable.name == "GearInteractable")
                {
                    interactable.OnInteract.AddListener(stopFailure);
                    gearInteractable = interactable;
                    break;
                }

            }

            StartCoroutine(FlashLightsRed());
        }

        private IEnumerator FlashLightsRed()
        {

            if (gear != null && gear.statusLight != null)
            {
                gear.statusLight.toggleColor = Color.red;
                gear.RetractImmediate();
                gear.statusLight.Toggle();
            }
            else
            {
                Debug.Log("It seems gear or statuslight is null");
            }
            yield return new WaitForSeconds(.5f);
            if (running)
            {
                this.StartCoroutine(FlashLightsRed());
            }
        }

        public override void stopFailure()
        {
            if (running)
            {
                System.Random rand = new System.Random();
                int randInt = rand.Next(1, 100);
                if (randInt < 80)
                {
                    Debug.Log("Unfailing Gear Stuck Up");
                    gear.statusLight.SetColor(Color.black);
                    running = false;
                    gearInteractable.OnInteract.RemoveListener(stopFailure);
                }
            }
        }
    }
}

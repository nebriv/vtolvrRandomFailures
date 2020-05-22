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
        public GameObject currentVehicle;

        public gearStuckUp()
        {

            failureName = "Gear Stuck Up";
            failureDescription = "Gear Stuck Up.";
            failureCategory = "Systems";
            hourlyFailureRate = 1;
            maxRunCount = 3;
            failureEnabled = false;
        }
        public override void Run()
        {

            Debug.Log("Creating Gear Stuck Up FlightWarning");
            AddWarning("GEAR STUCK UP", warningAudio);

            Debug.Log("Adding warntext to HUDWarning");
            SetHUDWarningText($"-[ GEAR FAILURE ]-");
            StartWarning();
            running = true;

            currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
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
            APURunning();
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


        public bool APURunning()
        {
            AuxilliaryPowerUnit apu = currentVehicle.GetComponentInChildren<AuxilliaryPowerUnit>();

            if (apu != null)
            {

                if (apu.rpm > 0)
                {
                    return true;
                }
            }
            else
            {
                Debug.Log("APU IS NULL");
                return true;
            }
            return false;
        }

        public override void stopFailure()
        {
            if (running)
            {
                System.Random rand = new System.Random();
                int randInt = rand.Next(1, 100);
                if (randInt < 50 || APURunning())
                {
                    Debug.Log("Unfailing Gear Stuck Up");
                    gear.statusLight.SetColor(Color.black);
                    HUDWarning.runWarning = false;
                    running = false;
                    gearInteractable.OnInteract.RemoveListener(stopFailure);
                }
            }
        }
    }
}

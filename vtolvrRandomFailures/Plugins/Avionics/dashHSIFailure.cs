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
    class dashHSIFailure :BaseFailure
    {
        public MFDManager manager;
        public dashHSIFailure()
        {
            failureName = "Dash HSI Failure";
            failureDescription = "Test";
            failureCategory = "Avionics";
            hourlyFailureRate = 1;
            failureEnabled = false;
        }

        public override void Run()
        {
            base.Run();
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            DashHSI hsi = currentVehicle.GetComponentInChildren<DashHSI>();

            Debug.Log(hsi.name);
            running = true;

            StartCoroutine(spinGuage(hsi));

        }
        private IEnumerator spinGuage(DashHSI hsi, float fakeHeading = 999)
        {
            
            if (fakeHeading == 999)
            {
                fakeHeading = hsi.flightInfo.heading + 1;
            }
            else
            {
                fakeHeading = fakeHeading + 1;
            }
            if (fakeHeading > 360)
            {
                fakeHeading = -180;
            }
            if (hsi != null)
            {
                Debug.Log($"Setting Heading to {fakeHeading}");

                Destroy(hsi.flightInfo);

                hsi.compassTf.localRotation = Quaternion.Slerp(hsi.compassTf.localRotation, Quaternion.Euler(0f, 0f, fakeHeading), hsi.compassSlerpRate * Time.deltaTime);
            }
            else
            {
                Debug.Log("It seems that the HSI is null");
            }
            yield return new WaitForSeconds(.0000001f);
            if (running)
            {
                this.StartCoroutine(spinGuage(hsi, fakeHeading));
            }

        }
    }
}

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
    class engineFire : BaseFailure
    {

        int fireDestroyTime = 45;
        int fireCounter = 0;

        public static AssetBundle engineFireAsset;
        public static AudioClip engineFireAlarm;
        public GameObject fireAndSmoke;


        public engineFire()
        {

            failureName = "Engine Fire";
            failureDescription = "Test";
            failureCategory = "Systems";
            hourlyFailureRate = 1000;
            failureEnabled = true;

            string assetPath = Path.Combine(Application.dataPath, "Managed", "enginefire.dll");

            engineFireAsset = AssetBundle.LoadFromFile(assetPath);

            engineFireAlarm = engineFireAsset.LoadAsset<AudioClip>("ttsw_fire");

            Debug.Log("Loading Fire Particle System");
            fireAndSmoke = engineFireAsset.LoadAsset<GameObject>("Fire");

        }
        public override void SetupSpecific()
        {

            Debug.Log($"TestVAR: {testVar}");
            testVar = "SetupSpecific";
            Debug.Log($"TestVAR: {testVar}");
            Debug.Log("In specific Setup");
            //Debug.Log("Creating Engine Fire FlightWarning");
            Debug.Log("()()()()()");
            Debug.Log(HUDWarning.warnText.color);
            Debug.Log(HUDWarning.warnText.transform.position);
            Debug.Log(HUDWarning.warnText.text);
            AddWarning("Engine Fire", engineFireAlarm);
            SetHUDWarningText("-[ ENGINE FIRE ]-");
            Debug.Log(HUDWarning.warnText.text);
            Debug.Log(HUDWarning.warnText.color);
            Debug.Log("Done with specific Setup");
        }

        // Was hoping to double up on effects in the engine, didn't work
        private IEnumerator addFX(EngineEffects.EngineParticleFX[] effects)
        {
            int counter = 0;
            yield return new WaitForSeconds(5);
            foreach (EngineEffects.EngineParticleFX efx in effects)
            {
                Debug.Log($"Adding fx {counter}");
                effects.Add(efx);
                yield return new WaitForSeconds(2);
            }
            
        }



        private IEnumerator fireBurnOut(ModuleEngine engine)
        {
            yield return new WaitForSeconds(fireDestroyTime);

            if (engine.engineEnabled)
            {
                StopWarning();
            
                engine.engineEffects.particleEffects[1].afterburnerOnly = true;
                engine.FailEngine();
            }
            else
            {
                engine.engineEffects.particleEffects[1].afterburnerOnly = true;
                StopWarning();
            }

        }

        public override void Run()
        {

            Debug.Log($"TestVAR: {testVar}");
            testVar = "Run";
            Debug.Log($"TestVAR: {testVar}");


            Debug.Log("*******");
            Debug.Log(HUDWarning.warnText.color);
            Debug.Log(HUDWarning.warnText.text);

            base.Run();
            running = true;

            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            ModuleEngine[] engines = currentVehicle.GetComponentsInChildren<ModuleEngine>();
            
            System.Random rand = new System.Random();
            int index = rand.Next(engines.ToList().Count());

            Debug.Log($"Total Engines: {engines.ToList().Count()}");
            ModuleEngine engine = engines[index];



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

            // Adding warning text to the HUD Warning
            if (engines.ToList().Count() > 0)
            {
                SetHUDWarningText($"-[ {leftOrRightCAPS} ENGINE FIRE ]-");
            }

            StartWarning();


            // THIS DOESN'T SET THE LIGHT TO ON. WHAT GIVES?
            foreach (UIImageToggle light in lights)
            {
                if (light.name.Contains($"EWIndicator_{leftOrRight}")) {

                    Traverse.Create(light).Field("color").SetValue(Color.red);
                    light.gameObject.SetActive(true);

                }
            }

            // This is a lazy way to set this. Need to find a better "fire" simulation.
            engine.engineEffects.particleEffects[1].afterburnerOnly = false;



            StartCoroutine(fireBurnOut(engine));

        }

        private IEnumerator unfailEngine(int index, ModuleEngine[] engines)
        {

            yield return new WaitForSeconds(30);

            engines[index].FullyRepairEngine();


        }
    }
}

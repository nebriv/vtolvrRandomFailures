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
using System.IO;

namespace vtolvrRandomFailures
{
    static class Globals
    {

        public static string projectName = "vtolvrRandomFailures";
        public static string projectAuthor = "nebriv";
        public static string projectVersion = "v1.0";

    }

    public class Main : VTOLMOD
    {
        private int iterator;
        private Dictionary<string, List<BaseFailure>> failures;

        public Settings settings;
        private float failureRateMultiplier = 1;
        private float failureRateHullDamageMultiplier = 6;
        public UnityAction<float> multiplierChanged;
        public Actor playerActor;

        public bool failuresSetup;

        private bool runFailures;

        public void failureRateMultiplierChanged(float amount)
        {
            failureRateMultiplier = amount;
        }

        public void failureCategoryChange(bool status)
        {

        }

        public override void ModLoaded()
        {
            base.ModLoaded();
            multiplierChanged += failureRateMultiplierChanged;

            failures = GetFailures();

            List<String> failureCategories = new List<string>(failures.Keys);

            settings = new Settings(this);

            settings.CreateCustomLabel("Failure rate multiplier changes how often failures happen.");
            settings.CreateCustomLabel("Default = 1");
            settings.CreateFloatSetting("Failure multiplier:", multiplierChanged, failureRateMultiplier);

            settings.CreateCustomLabel("Failure Categories (Currently Read Only)");

            Dictionary<String, Boolean> failureCategoriesEnabled = new Dictionary<string, bool>();

            foreach (String category in failureCategories)
            {
                failureCategoriesEnabled.Add(category, true);
                settings.CreateBoolSetting(category, failureCategoryChange, true);
            }


            VTOLAPI.CreateSettingsMenu(settings);

            Debug.Log($"{Globals.projectName} - {Globals.projectVersion} by {Globals.projectAuthor} loaded!");
        }

        public void Start()
        {
            Log("Running Startup and Waiting for map load");

            StartCoroutine(WaitForMap());

        }

        IEnumerator WaitForMap()
        {
            while (SceneManager.GetActiveScene().buildIndex != 7 || SceneManager.GetActiveScene().buildIndex == 12)
            {
                //Pausing this method till the loader scene is unloaded
                yield return null;
            }

            yield return new WaitForSeconds(2);

            // Run the setup method on each failure to build out various components
            if (!failuresSetup)
            {
                StartCoroutine(setupFailures());
                failuresSetup = true;
            }
            

            
            playerActor = FlightSceneManager.instance.playerActor;

        }

        IEnumerator setupFailures()
        {
            foreach (KeyValuePair<string, List<BaseFailure>> entry in failures)
            {
                foreach (BaseFailure failure in entry.Value)
                {
                    failure.Setup();
                    yield return new WaitForFixedUpdate();
                }
            }
            runFailures = true;
            StartCoroutine(RunEverySecond(1));
        }

        IEnumerator RunEverySecond(float seconds)
        {
            while (runFailures)
            {
                yield return new WaitForSeconds(seconds);
                if (SceneManager.GetActiveScene().buildIndex != 7 && SceneManager.GetActiveScene().buildIndex != 12)
                {
                    runFailures = false;
                    Start();
                }

                foreach (KeyValuePair<string, List<BaseFailure>> entry in failures)
                {
                    foreach (BaseFailure failure in entry.Value)
                    {
                        yield return new WaitForFixedUpdate();
                        Random rand = new Random();
                        double chance = rand.NextDouble();

                        // TODO: It sort of seems like the chance isn't being regenerated for each failure. Fix?

                        Health health = Traverse.Create(playerActor).Field("h").GetValue() as Health;
                        float lostHealth = 100 - health.currentHealth;

                        float failureRateHullDamageMultiplierResult = (lostHealth / 100) * failureRateHullDamageMultiplier;

                        if (failureRateHullDamageMultiplierResult == 0)
                        {
                            failureRateHullDamageMultiplierResult = 1;
                        }

                        double chanceMultiplied = chance * failureRateMultiplier * failureRateHullDamageMultiplierResult;
                        //Log($"{chance} * {failureRateMultiplier} * {failureRateHullDamageMultiplierResult} = {chanceMultiplied}");

                        if (chanceMultiplied <= failure.failureRate)
                        {
                            //Log($"{chanceMultiplied} <= {failure.failureRate}");
                            failure.runFailure(failures);
                        }
                    }
                }
            }

        }


        private Dictionary<string, List<BaseFailure>> GetFailures()
        {
            // Getting standard asset bundle which contains some warning sounds and etc
            try
            {
                string assetPath = Path.Combine(Application.dataPath, "Managed", "basefailure.dll");
                AssetBundle baseFailureAssetBundle = AssetBundle.LoadFromFile(assetPath);
                AudioClip genericWarning = baseFailureAssetBundle.LoadAsset<AudioClip>("ttsw_warning");
            } catch (Exception err)
            {
                LogError("Caught exception while loading basefailure.dll");
                LogError(err.ToString());
            }


            // The parent object we want to get the objects from
            Type parentType = typeof(BaseFailure);

            // Getting list of all subtypes of BaseFailure
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.BaseType == parentType);
            
            // List of failures after theyre created 
            Dictionary<string, List<BaseFailure>> failures = new Dictionary<string, List<BaseFailure>>();

            Log("Getting available failures...");

            foreach (Type source in subclasses)
            {
                Log($"Creating Game Object {source.Name}");
                GameObject newFailure = new GameObject(source.Name, source);

                Log($"Creating BaseFailure");
                BaseFailure failure = newFailure.GetComponent<BaseFailure>();
                failure.genericWarning = genericWarning;
                failure.baseFailureAssetBundle = baseFailureAssetBundle;

                DontDestroyOnLoad(newFailure);
                Log($"Done loading.");

                //VTOLMOD mod = newModGo.GetComponent<VTOLMOD>();


                //BaseFailure failure = (BaseFailure)Activator.CreateInstance(type);

                //Log(failure.ToString());

                if (failures.ContainsKey(failure.failureCategory))
                {
                    failures[failure.failureCategory].Add(failure);
                }
                else
                {
                    List<BaseFailure> newlist = new List<BaseFailure>(1) { failure };
                    failures.Add(failure.failureCategory, newlist);
                }
            }

            Log("Done getting Failures!");

            return failures;
        }

    }
}

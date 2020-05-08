using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.IO;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class ObjectCopier
{
    /// <summary>
    /// Perform a deep Copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", nameof(source));
        }

        // Don't serialize a null object, simply return the default for that object
        if (System.Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}

class BaseFailure : MonoBehaviour
{
    public string failureName;
    public string failureDescription;
    public string failureCategory; // Systems, Avionics, Environment
    public double failureRate;
    public double hourlyFailureRate;

    public BaseFailure thisFailure { private set; get; } = null;

    public bool failureEnabled;
    public bool running;

    public int maxRunCount; // Maximum times the failure is allowed to run. Setting to 0 allows it to run infinitely. >:)
    private int runCount;

    public Dictionary<string, List<BaseFailure>> otherFailures = null;

    public FlightWarnings flightWarningSystem;
    public HUDMessages hudMessages;

    public AssetBundle baseFailureAssetBundle;
    public AudioClip genericWarning;

    public GameObject playersVehicle;

    public HUDWarningMessage HUDWarning;

    public GameObject test;

    public void Setup()
    {

        Debug.Log($"Running Setup for {failureName}");
        if (hourlyFailureRate != 0.0)
        {
            failureRate = hourlyFailureRate / 3600;
            Debug.Log($"Hourly failure rate {hourlyFailureRate} for {failureName} is {failureRate}");
        }

        playersVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
        hudMessages = playersVehicle.GetComponentInChildren<HUDMessages>();

        // Getting the hud
        GameObject hud = GameObject.Find("CollimatedHud");

        // Getting the gear warning so we can copy some parameters
        GameObject gearWarning = GameObject.Find("GearWarning");

        // Adding our HUD Warning message to the vehicle's hud
        hud.gameObject.AddComponent<HUDWarningMessage>();

        HUDWarning = hud.gameObject.GetComponent<HUDWarningMessage>();

        // Getting the text object from the hud warning
        Text hudText = HUDWarning.GetComponentInChildren<Text>();

        // Getting the flight warnings from the vehicle
        HUDWarning.flightWarnings = playersVehicle.GetComponentInChildren<FlightWarnings>();


        // Copying the HUDGearWarning text settings
        if (hudText != null)
        {
            hudText = gearWarning.GetComponent<HUDGearWarning>().gameObject.GetComponentInChildren<Text>();
        }
        else
        {
            Debug.Log("hudtext is null???");
        }

        // Setting the warn text params
        HUDWarning.warnText = hudText;

        // Shifting it up a bit
        HUDWarning.warnText.transform.position = new Vector3(HUDWarning.warnText.transform.position.x, HUDWarning.warnText.transform.position.y + 15, HUDWarning.warnText.transform.position.z);

        // Add the clear action
        HUDWarning.flightWarnings.OnClearedWarnings.AddListener(new UnityAction(HUDWarning.OnCleared));
        
        HUDWarning.gameObject.SetActive(true);
      
    }

    public FlightWarnings.FlightWarning AddWarning(string warningName, AudioClip warningAudio = null)
    {

        FlightWarnings.FlightWarning newWarning = new FlightWarnings.FlightWarning(warningName, warningAudio);


        return newWarning;
    }

    public void addContinuousWarning(FlightWarnings.FlightWarning warning)
    {
        
        flightWarningSystem.AddContinuousWarning(warning);
        
    }

    public void addHUDMessage(string messageKey, string message)
    {
        if (hudMessages != null)
        {
            hudMessages.SetMessage(messageKey, message);
        }
        else
        {
            Debug.Log("HUD Messages IS NULL");
        }
    }

    public void removeHUDMessage(string messageKey)
    {
        hudMessages.RemoveMessage(messageKey);
    }

    public void SetFailureInfo(BaseFailure thisFailure)
    {
        if (this.thisFailure == null)
        {
            this.thisFailure = thisFailure;
        }
    }

    public virtual void Run()
    {
    }

    public override string ToString()
    {
        return "Name: " + failureName + " - Category: " + failureCategory + " - Failure Rate: " + failureRate;
    }

    public void runFailure(Dictionary<string, List<BaseFailure>> failures = null, Boolean ignoreDisabled = false)
    {
        otherFailures = failures;
        if ((failureEnabled || ignoreDisabled ) && !running)
        {
            Debug.Log($"Running Failure: {failureName}");

            if ((runCount < maxRunCount || maxRunCount == 0))
            {
                Run();
                runCount++;
            }
            else
            {
                Debug.Log($"{failureName} reached max run count: {maxRunCount}");
            }
        }   
    }

    public virtual void stopFailure()
    {
        Debug.Log($"Stopping {failureName}.");
        running = false;
    }

    public virtual void resetFailure()
    {

    }

}

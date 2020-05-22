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

    public FlightWarnings flightWarnings;
    public FlightWarnings.FlightWarning flightWarning;

    public HUDMessages hudMessages;

    public AssetBundle baseFailureAssetBundle;
    public AudioClip warningAudio;

    public GameObject playersVehicle;

    public HUDWarningMessage HUDWarning;
    public string testVar = "no";


    public void SetupHUDWarning()
    {
        
        hudMessages = playersVehicle.GetComponentInChildren<HUDMessages>();
        try
        {
            Debug.Log($"TestVAR: {testVar}");
            testVar = "SetupHUDWarning";
            Debug.Log($"TestVAR: {testVar}");
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
            //Debug.Log("Creating Engine Fire FlightWarning");


            // Copying the HUDGearWarning text settings
            if (hudText != null)
            {
                hudText = gearWarning.GetComponent<HUDGearWarning>().gameObject.GetComponentInChildren<Text>();
            }
            else
            {
                Debug.Log("hudtext is null???");
            }

            Debug.Log(hudText.color);
            Debug.Log(hudText.text);

            // Setting the warn text params
            HUDWarning.warnText = hudText;

            Debug.Log(HUDWarning.warnText.color);

            // Shifting it up a bit
            HUDWarning.warnText.transform.position = new Vector3(HUDWarning.warnText.transform.position.x, HUDWarning.warnText.transform.position.y + 15, HUDWarning.warnText.transform.position.z);
            HUDWarning.warnText.enabled = true;
            // Add the clear action
            HUDWarning.flightWarnings.OnClearedWarnings.AddListener(new UnityAction(HUDWarning.OnCleared));

            HUDWarning.gameObject.SetActive(true);
        }
        catch (Exception err)
        {
            Debug.Log("Error adding HUD?");
        }
    }

    public void Setup()
    {
        if (failureEnabled)
        {
            Debug.Log($"TestVAR: {testVar}");
            testVar = "Setup";
            Debug.Log($"TestVAR: {testVar}");
            playersVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            Debug.Log($"Running Setup for {failureName}");
            if (hourlyFailureRate != 0.0)
            {
                failureRate = hourlyFailureRate / 3600;
                Debug.Log($"Hourly failure rate {hourlyFailureRate} for {failureName} is {failureRate}");
            }
            
            flightWarnings = playersVehicle.GetComponentInChildren<FlightWarnings>();


            Debug.Log("Setting up HUD");
            SetupHUDWarning();

            flightWarning = AddWarning($"{failureName}", warningAudio);
            //SetHUDWarningText($"{failureName}");

            if (flightWarning == null)
            {
                Debug.Log("Flight Warning Not Created");
            }
            Debug.Log(HUDWarning.warnText.text);
            Debug.Log("Running specific setup");
            SetupSpecific();
        }
    }

    public virtual void SetupSpecific()
    {

    }


    public void StartWarning()
    {
        Debug.Log($"TestVAR: {testVar}");
        testVar = "StartWarning";
        Debug.Log($"TestVAR: {testVar}");
        Debug.Log($"Starting Warning for {failureName}");
        if (!HUDWarning)
        {
            Debug.Log("Trying to setup the hud again...");
            //SetupHUDWarning();
        }

        if (!HUDWarning)
        {
            Debug.Log("Unable to start HUD warning, no HUD?");
            if (flightWarning == null)
            {
                Debug.Log("Flight Warning Not Created");
            }
            else
            {
                flightWarnings.AddContinuousWarning(flightWarning);
            }
        }
        else
        {
            HUDWarning.runWarning = true;
            Debug.Log(HUDWarning.warnText.color);
            Debug.Log(HUDWarning.warnText.text);
        }

    }

    public void OneShotWarning()
    {
        flightWarnings.AddOneShotWarning(flightWarning);
        if (HUDWarning)
        {
            StartCoroutine(flashWarningText());
        }
        else
        {
            Debug.Log("No HUD Warning");
        }

    }

    IEnumerator flashWarningText()
    {
        HUDWarning.warnText.enabled = true;
        yield return new WaitForSeconds(1.5f);
        HUDWarning.warnText.enabled = false;
    }

    public void StopWarning()
    {
        Debug.Log($"Stopping Warning for {failureName}");
        if (HUDWarning)
        {
            HUDWarning.runWarning = false;
            HUDWarning.warnCleared = true;
        }
        else
        {
            flightWarnings.RemoveContinuousWarning(flightWarning);
        }
    }


    public FlightWarnings.FlightWarning AddWarning(string warningName, AudioClip warningAudio = null)
    {
        Debug.Log("Creating new FlightWarning warning");
        FlightWarnings.FlightWarning newWarning = new FlightWarnings.FlightWarning(warningName, warningAudio);
        flightWarning = newWarning;
        if (HUDWarning)
        {
            HUDWarning.flightWarning = newWarning;
        }
        else
        {
            Debug.Log("No HUD warning to add new warning to?");
        }
        return newWarning;
    }

    public void SetHUDWarningText(string warningText)
    {
        if (HUDWarning)
        {
            HUDWarning.warnText.text = warningText;
            Debug.Log("!!!!");
            Debug.Log(HUDWarning.warnText.color);
            Debug.Log(HUDWarning.warnText.transform.position);
        }
        else
        {
            Debug.LogError("No HUDWarning to add text to!");
        }
    }

    public void addContinuousWarning(FlightWarnings.FlightWarning warning)
    {

        flightWarnings.AddContinuousWarning(warning);
        
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
        Debug.Log($"TestVAR: {testVar}");
        testVar = "RunFailure";
        Debug.Log($"TestVAR: {testVar}");

        otherFailures = failures;

        if ((failureEnabled || ignoreDisabled ) && !running)
        {
            Debug.Log($"Running Failure: {failureName}");

            if ((runCount < maxRunCount || maxRunCount == 0))
            {
                Debug.Log("$$$$$");
                Debug.Log(HUDWarning.warnText.color);
                Debug.Log(HUDWarning.warnText.text);
                Run();
                runCount++;
            }
            else
            {
                Debug.Log($"{failureName} reached max run count: {maxRunCount}");
            }
        }
        else
        {
            Debug.Log($"{failureName} Failure enabled: {failureEnabled}, Ignore Disabled: {ignoreDisabled}, Running: {running}");
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

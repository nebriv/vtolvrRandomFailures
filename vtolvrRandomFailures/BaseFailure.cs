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

    public void Setup()
    {
        if (hourlyFailureRate != 0.0)
        {
            failureRate = hourlyFailureRate / 3600;
            Debug.Log($"Hourly failure rate {hourlyFailureRate} for {failureName} is {failureRate}");
        }
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

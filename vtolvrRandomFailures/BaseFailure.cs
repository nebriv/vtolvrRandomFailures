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

    public BaseFailure thisFailure { private set; get; } = null;


    public bool failureEnabled;
    public bool running;

    public int maxRunCount; // Maximum times the failure is allowed to run. Setting to 0 allows it to run infinitely. >:)
    private int runCount;

    Dictionary<string, List<BaseFailure>> otherFailures = null;

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

    public void runFailure(Dictionary<string, List<BaseFailure>> failures = null)
    {
        otherFailures = failures;
        if (failureEnabled && !running)
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

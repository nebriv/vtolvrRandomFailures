using UnityEngine;
using UnityEngine.UI;

class HUDWarningMessage : MonoBehaviour
{
    public Text warnText;

    public float warnInterval;
    public FlightWarnings flightWarnings;

    public bool useCommonWarnings = false;

    public FlightWarnings.FlightWarning flightWarning;

    private float timeWarned;

    private bool wasWarn;

    public bool warnCleared;

    public bool runWarning;

    private void Start()
    {
        warnText = gameObject.GetComponentInChildren<Text>();
        warnText.gameObject.SetActive(true);
        warnInterval = 1.0f;
    }

    public void Update()
    {
        if (runWarning)
        {
            if (!warnCleared)
            {
                if (Time.time - timeWarned > warnInterval)
                {
                    timeWarned = Time.time;
                }
                if (Time.time - this.timeWarned < warnInterval * 0.75f)
                {
                    warnText.enabled = true;
                }
                else
                {
                    warnText.enabled = false;
                }
                if (!wasWarn)
                {

                    flightWarnings.AddContinuousWarning(flightWarning);

                    wasWarn = true;
                    return;
                }
            }
            else
            {
                if (wasWarn)
                {
                    flightWarnings.RemoveContinuousWarning(flightWarning);
                    wasWarn = false;
                    runWarning = false;
                }
                warnText.enabled = false;
                runWarning = false;
            }
        }

    }

    public void OnCleared()
    {
        Debug.Log("Clearing Warning");
        warnCleared = true;
    }

    public void setWarnText(string warntext)
    {
        Debug.Log("Setting warning text....");
        warnText.text = warntext;
    }
}
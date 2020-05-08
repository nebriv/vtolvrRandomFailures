using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Harmony;



namespace vtolvrRandomFailures.Plugins
{
    class canopyRelease :BaseFailure
    {
        
        public canopyRelease()
        {

            failureName = "Canopy Release";
            failureDescription = "Freddy forgot the secure the electrical system tied to the canopy! Wooosh there it goes!";
            failureCategory = "Systems";
            hourlyFailureRate = 1;
            maxRunCount = 1;
            failureEnabled = true;

        }
        public override void Run()
        {
            GameObject currentVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();

            EjectionSeat seat = currentVehicle.GetComponentInChildren<EjectionSeat>();
            Traverse.Create(seat).Field("seatObject").SetValue(seat.gameObject);
            Traverse.Create(seat).Method("FixedUpdateDelayedCanopyJett").GetValue();

        }
    }
}

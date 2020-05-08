# VTOLVR Random Failure Mod

This mod has numerous features to add a little excitement to your flights! This mod may result in instability of your VTOL VR game. Failures may result in the inability to accomplish game missions.

It is currently a work in progress. Failures and their recovery scenarios are currently a work in progress.

## Failure Modes
### Environment

 - Bird Strikes
	 - **Criteria**
		 - Must be between 500 and 5000 feet above the ground.
	 - **Default Failure Rate**
		 - 4x Hour (Due to the above criteria, failure rate is increased)
	 - **Description**
		 - Generates "birds" (currently spheres, WIP) in front of your plane if you are between 500 and 5000 feet above the ground. Collision with a bird may result in any system failures. There is a higher likelihood of an engine failure.
	 - **Recovery**
		 - Sub Failure Dependent, see below.


### Systems
- Canopy Release System
	 -  **Criteria**
		 - Must be in a plane that has an electable canopy. (Only tested on F/A-26)
	 - **Default Failure Rate**
		 - 1x Hour
	 - **Description**
		 - Triggers the ejection sequence associated with the planes canopy.
	 - **Recovery**
		 - Return to base
- Fuel Leak
	 -  **Criteria**
		 - None
	 - **Default Failure Rate**
		 - 1x Hour
	 - **Description**
		 - Adds a fuel drain to your fuel tanks. This drain lasts anywhere between 30 seconds to 5 minutes.
	 - **Recovery**
		 - Fuel drain rate is dependent on current G Load. Generating negative G Forces temporarily stops the fuel leak.
		
- Engines
	- Engine Fire
		 -  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Simulates an engine fire in one or more engines. Failure to recovery from the fire results in permanent failure of the engine.
		 - **Recovery**
			 - Shut down the affected engine.
	- Alternator(s)
		 -  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Causes the alternators associated with your engine(s) to fail resulting a rapid decrease in battery power available.
		 - **Recovery**
			 - Turn on the APU.
	- Engine Failure
	 	 -  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Causes a random failure in one or more engines. 
		 - **Recovery**
			 - There is an 80% chance that switching off and back on the engine will work.
- Gear
	- Stuck Retracted
		 -  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Causes the landing gear to be stuck retracted within your plane. Currently if they are not retracted, they will be.
		 - **Recovery**
			 - Turning on the APU generates enough power to lower the gear.
	- Stuck Deployed
		-  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Causes the landing gear to be stuck deployed. Currently if they are not already deployed, they will be.
		 - **Recovery**
			 - There is currently a 50 percent chance that the gear will be able to be retracted.
	- Status Lights Failure
	 	 -  **Criteria**
			 - None
		 - **Default Failure Rate**
			 - 1x Hour
		 - **Description**
			 - Bob messed up the gear status lights. They might be a bit wonky - everything *should* be fine though.
		 - **Recovery**
			 - Toggle the gear state to resolve the lights.
### Avionics
- Dash HSI (Compass)
	 -  **Criteria**
		 - None
	 - **Default Failure Rate**
		 - 1x Hour
	 - **Description**
		 - Causes the Steam Gauge Compass to spin uncontrollably.
	 - **Recovery**
		 - None
- Loose Wiring in MultiFunction Displays
	 -  **Criteria**
		 - None
	 - **Default Failure Rate**
		 - 1x Hour
	 - **Description**
		 - Results in an MFD being toggled off.
	 - **Recovery**
		 - Turn it back on.


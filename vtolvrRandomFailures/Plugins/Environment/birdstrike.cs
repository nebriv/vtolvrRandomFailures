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
using Random = UnityEngine.Random;
using Valve.Newtonsoft.Json;
using NAudio;
using Unity;
using System.IO;
using MFlight.Demo;

namespace vtolvrRandomFailures.Plugins
{

    class birdStrike : BaseFailure
    {

        //This seems to be a semi-realistic number that visually looks alright
        int birdCount = 200;
        int curCount = 0;

        // Min and Max Altitude to spawn birds at
        float minAltitude = 100;
        float maxAltitude = 1524;
        //float maxAltitude = 10000;

        public GameObject[] birdList;

        public static AssetBundle birdAsset;
        public static AudioClip notabirdsound;
        public static AudioClip honking;
        public static AudioClip birdBump;


        public birdStrike()
        {
            // Setting up the failure variables
            failureName = "Bird Strike";
            failureDescription = "Test";

            failureCategory = "Environment";

            hourlyFailureRate = 4;
            failureEnabled = false;
            maxRunCount = 1;

            string birdPath = Path.Combine(Application.dataPath, "Managed", "birdstrike.dll");

            birdAsset = AssetBundle.LoadFromFile(birdPath);

            notabirdsound = birdAsset.LoadAsset<AudioClip>("smg1fire");

            honking = birdAsset.LoadAsset<AudioClip>("honk-honk");
            birdBump = birdAsset.LoadAsset<AudioClip>("doorbump");

        }

        public override void Run()
        {
            base.Run();
            Actor playeractor = FlightSceneManager.instance.playerActor;
            GameObject playersVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            Transform playerLocation = playeractor.transform;

            //addContinuousWarning(birdWarning);

            if (playeractor.flightInfo.radarAltitude >= minAltitude && playeractor.flightInfo.radarAltitude <= maxAltitude)
            {
                Debug.Log("Spawning Birds");

                running = true;

                // Add the audio emitter
                AudioSource honker = gameObject.AddComponent<AudioSource>();

                // Set the bird clip audio (loaded in birdStrike)
                honker.clip = birdStrike.honking;

                // Set the volume UNKNOWN scale...
                honker.volume = .5f;
                honker.Play();

                Debug.Log(otherFailures);

                while (curCount < birdCount)
                {
                    //Spawn in bird spheres
                    GameObject bird = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    bird.AddComponent<Bird>();
                    //Add the above bird class to them

                    bird.GetComponent<Bird>().otherFailures = this.otherFailures;


                    //Keep track of the birds so they don't fly away
                    birdList.Add(bird);
                    curCount++;
                }

                //Start our coroutine to delete the birds eventually
                StartCoroutine(DeleteBirds());
                running = false;
            }

        }



        private IEnumerator DeleteBirds()
        {

            yield return new WaitForSeconds(60);

            foreach (GameObject bird in birdList)
            {
                // I think null birds might be dead?
                if (bird != null) {
                    GameObject.Destroy(bird);
                }
            }
        }
    }

    class Bird : MonoBehaviour
    {

        public GameObject bird;
        public Transform playerLocation;
        public GameObject playersVehicle;
        public Actor playeractor;
        public Actor birdActor;

        // Used to control the bird's bobbing
        private bool movingDown = true;
        private float spawnY;
        private float maxDistance = 100;
        private float speed = 1;


        // Bird Spawning Configuration.
        // XYZ = Bird spawning distance from centered point ahead of player 
        // (creating a cube that gets filled with birds)
        private float minX = -300;
        private float maxX = 300;
        private float minY = -300;
        private float maxY = 300;
        private float minZ = -10;
        private float maxZ = 10;

        // Min/Max distance that bird filled cube spawns
        private float minDistanceFromPlayer = 900;
        private float maxDistanceFromPlayer = 2000;

        // Damage/Failure Rates
        private float birdColliderRadius = 30f;
        private float birdDamage = .25f;
        private float engineFailureRate = .40f; // Read as Percent
        private float failureRateMultiplier = 3f;


        // The audio emitter
        private AudioSource squawk;

        // A list of all other failures available.
        public Dictionary<string, List<BaseFailure>> otherFailures = null;

        public void Awake()
        {
            // Getting env variables. This is probably pretty costly and 
            // should be moved out of this as it'll be run every time a bird is created.
            playeractor = FlightSceneManager.instance.playerActor;
            playersVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();
            playerLocation = playeractor.transform;


            birdActor = gameObject.AddComponent<Actor>();
            birdActor.name = "Bird";
            birdActor.team = Teams.Enemy;
            birdActor.enabled = false;

            // Make a sphere collider and set it to trigger. 50 units seems big enough for testing
            // Might go down in size a bit for real life to allow a player to have a chance to dodge.
            SphereCollider birdCollider = gameObject.GetComponent<SphereCollider>();
            birdCollider.isTrigger = true;
            birdCollider.radius = birdColliderRadius;
            gameObject.transform.rotation = playerLocation.rotation;

            // Black birds are easier to see.
            gameObject.GetComponent<Renderer>().material.color = Color.black;

            // Does some sort of voodoo magic to stop the object from popping around the player's plane.
            gameObject.AddComponent<FloatingOriginTransform>();

            // Distance to place the birds in front of the player
            float SpacingZ = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 postion1 = playerLocation.transform.position + playerLocation.transform.forward * SpacingZ;

            // -300x300 seems to be a decent size to place the birds. This will randomly place them within this box below (length 600, width 600, depth 10)
            Vector3 position2 = new Vector3(postion1.x + Random.Range(minX, maxX), postion1.y + Random.Range(minY, maxY), postion1.z + Random.Range(minZ, maxZ));

            // Put the bird there.
            gameObject.transform.position = position2;

            // Used for the bobbing routine.
            spawnY = transform.position.y;

            // Wakey wakey.
            gameObject.SetActive(true);


            // Add the audio emitter
            squawk = gameObject.AddComponent<AudioSource>();

            // Set the bird clip audio (loaded in birdStrike)
            squawk.clip = birdStrike.birdBump;

            // Set the volume UNKNOWN scale...
            squawk.volume = .5f;

        }

        public void Update()
        {
            //Marsh's attempt
            //Debug.Log(transform.position);
            //if (movingDown)
            //{
            //    gameObject.transform.position += Vector3.down * Time.deltaTime * speed;

            //    if (gameObject.transform.position.y < spawnY - maxDistance)
            //    {
            //        movingDown = false;
            //    }
            //}
            //else
            //{
            //    gameObject.transform.position += Vector3.up * Time.deltaTime * speed;

            //    if (gameObject.transform.position.y > spawnY + maxDistance)
            //    {
            //        movingDown = true;
            //    }
            //}


            //Nebriv's first attempt
            //Vector3 curPosition = this.gameObject.transform.position;
            //this.gameObject.transform.position = new Vector3(curPosition.x + Random.Range(-1, 1), curPosition.y + Random.Range(-2, 2), curPosition.z + Random.Range(-1, 1));
            //Debug.Log($"Old Bird Location: {curPosition} New Bird Location: {this.gameObject.transform.position}");
            //Debug.Log("Update");
        }

        public void OnTriggerEnter(Collider thingICollidedWith)
        {
            if (thingICollidedWith.transform.root == playersVehicle.transform)
            {

                Health health = Traverse.Create(playeractor).Field("h").GetValue() as Health;
                if (health.currentHealth > 10)
                {
                    playeractor.health.Damage(birdDamage, new Vector3(0, 0, 0), Health.DamageTypes.Impact, birdActor, "Bird Strike");
                }

                Debug.Log("Bird Strike!!");
                squawk.Play();
                BaseFailure fail = getRandomFailure();
                System.Random rand = new System.Random();
                double chance = rand.NextDouble();

                if (fail.failureName.Contains("Engine Failure") && chance <= engineFailureRate)
                {
                    fail.runFailure();
                }
                else
                {
                    Debug.Log($"Is {chance} <= {fail.failureRate * failureRateMultiplier}?");
                    if (chance <= fail.failureRate * failureRateMultiplier)
                    {
                        Debug.Log($"Triggering failure {fail.failureName}");
                        fail.runFailure(null, true);
                    }
                }

            }
        }

        public BaseFailure getRandomFailure()
        {

            foreach (KeyValuePair<string, List<BaseFailure>> entry in otherFailures)
            {
                if (entry.Key == "Systems")
                {
                    return entry.Value[Random.Range(0, entry.Value.Count)];
                }
            }
            return null;
        }
    }

}

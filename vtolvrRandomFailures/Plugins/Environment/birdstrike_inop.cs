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

namespace vtolvrRandomFailures.Plugins
{

    class Bird : MonoBehaviour
    {

        public GameObject bird;
        public Transform myTrans;
        public Transform playerLocation;
        public GameObject playersVehicle;
        private bool movingDown = true;
        private float spawnY;
        private float maxDistance = 100;
        private float speed = 1;
        public Bird(Transform playerLoc)
        {
            playerLocation = playerLoc;
            Debug.Log("Bird Constructor");
        }

        public void Awake()
        {
            myTrans = gameObject.transform;
            Actor playeractor = FlightSceneManager.instance.playerActor;
            playersVehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();

            print(playersVehicle);

            playerLocation = playeractor.transform;
            SphereCollider birdCollider = gameObject.GetComponent<SphereCollider>();
            birdCollider.isTrigger = true;
            birdCollider.radius = 50f;
            gameObject.transform.rotation = playerLocation.rotation;
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            gameObject.AddComponent<FloatingOriginTransform>();

            float SpacingZ = 1500.0f;
            Vector3 postion1 = playerLocation.transform.position + playerLocation.transform.forward * SpacingZ;
            Vector3 position2 = new Vector3(postion1.x + Random.Range(-300, 300), postion1.y + Random.Range(-300, 300), postion1.z + Random.Range(0, 10));
            gameObject.transform.position = position2;
            spawnY = transform.position.y;
            gameObject.SetActive(true);
            

            //Debug.Log("Bird is awake");
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
            Debug.Log($"Collided with {thingICollidedWith.name} owned by {thingICollidedWith.transform.parent.gameObject.activeInHierarchy} - root {thingICollidedWith.transform.root}");
            Debug.Log(playersVehicle);
            if (thingICollidedWith.transform.root == playersVehicle.transform)
            {
                Debug.Log("OH GOD I COLLIDED WITH THE PLAYER!?");
            }

        }
    }

    class birdStrike : BaseFailure
    {

        int birdCount = 200;
        int curCount = 0;

        public GameObject[] birdList;

        public birdStrike()
        {
            // Setting up the failure variables
            failureName = "Bird Strike";
            failureDescription = "Test";

            failureCategory = "Environment";

            failureRate = 0.5;
            failureEnabled = true;
            maxRunCount = 1;

        }

        public override void Run()
        {
            base.Run();
            Debug.Log("Spawning Birds");

            running = true;

            while (curCount < birdCount)
            {
                //Spawn in bird spheres
                GameObject bird = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                //Add the above bird class to them
                bird.AddComponent<Bird>();

                //Keep track of the birds so they don't fly away
                birdList.Add(bird);
                curCount++;
            }

            //Start our coroutine to delete the birds eventually
            StartCoroutine(DeleteBirds());

        }

        private IEnumerator DeleteBirds()
        {

            yield return new WaitForSeconds(60);

            foreach (GameObject bird in birdList)
            {
                GameObject.Destroy(bird);
            }
        }
    }
}

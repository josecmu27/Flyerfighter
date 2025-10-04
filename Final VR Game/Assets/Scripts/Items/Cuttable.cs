using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    public int uncutDetectors;
    
    private Rigidbody fallingRigidbody;

    public GameObject[] fallingObjects;
    public GameObject[] Cutzones;

    void Start()
    {
        foreach (GameObject obj in fallingObjects)
        {
            // get the rigidbody of the object you want to make fall
            fallingRigidbody = obj.GetComponent<Rigidbody>();

            // gravity must be off initially
            fallingRigidbody.useGravity = false;
            fallingRigidbody.isKinematic = true;
        }

        uncutDetectors = Cutzones.Length;
    }

        void FixedUpdate()
        {
            // player has cut the object
            if (uncutDetectors == 0)
            {
                foreach (GameObject fallingObject in fallingObjects)
                {
                    fallingObject.GetComponent<Rigidbody>().useGravity = true;
                    fallingObject.GetComponent<Rigidbody>().isKinematic = false;
                    fallingObject.gameObject.layer = 10; //set physics so player can walk through it
                    Destroy(this.gameObject, 15); //Destroy wood segments after time passes
                }

                foreach(GameObject obj in Cutzones)
                {
                    Destroy(obj.gameObject); //Destroy cutting areas immediately
                }
            }
        }
    }

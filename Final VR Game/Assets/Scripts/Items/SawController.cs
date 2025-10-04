using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was the original idea for the player's tool to cut objects
public class SawController : MonoBehaviour
{   
    public bool isRightSaw = false;
    public bool isSpinning = false;
    private bool isInWood = false;
    
    // saw paramters
    public float rotationSpeed;
    private float initRotationSpeed;
    public float maxRotationSpeed = 450f;

    public float vibrationAmplitude = 0.3f;
    
    public ParticleSystem emitter;
    public Material cutMaterial;

    // sound
    private AudioSource source;
    public AudioClip on;
    public AudioClip off;

    void Start()
    {
        initRotationSpeed = rotationSpeed;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        // right saw
        if (isRightSaw && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            SpinBlade();
        }
        // left saw
        else if (!isRightSaw && OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
        {
            SpinBlade();
        }
        else
        {
            StopBlade();
        }
    }

    private void SpinBlade()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            rotationSpeed = initRotationSpeed;

            source.PlayOneShot(on); //play on sound
        }

        // rotates around pivot (y-axis)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // set haptic feedback
        if (isRightSaw)
        {
            OVRInput.SetControllerVibration(1f, vibrationAmplitude, OVRInput.Controller.RTouch);
        }
        else
        {
            OVRInput.SetControllerVibration(1f, vibrationAmplitude, OVRInput.Controller.LTouch);
        }

        // increase saw rotation as player hold the trigger
        if (rotationSpeed < maxRotationSpeed)
        {
            rotationSpeed += rotationSpeed;
        }
    }

    private void StopBlade()
    {
        if (isSpinning) source.PlayOneShot(off); //turn off sound
        
        // player is no longer pressing the saw's trigger
        isSpinning = false;
       
        if (rotationSpeed > 0f) rotationSpeed -= 10;
        
        if (rotationSpeed < 0f) rotationSpeed = 0f;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        emitter.Stop(); // turn of particles

        // stop haptic feedback
        if (isRightSaw)
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
        }
        else
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Uncut" && isSpinning)
        {
            
            emitter.Play();

            if (other.gameObject.GetComponent<CutZone>().isCut == false) 
            {
                other.gameObject.transform.parent.GetComponent<Cuttable>().uncutDetectors--;
                other.gameObject.GetComponent<MeshRenderer>().material = cutMaterial; //set cut area material to the cut material 
                other.gameObject.GetComponent<CutZone>().isCut = true;
            }

            // strengthen controller vibration
            vibrationAmplitude *= 1.5f;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        emitter.Stop();
        source.Stop();
        isInWood = false;

        // set controller vibration back to default
        vibrationAmplitude /= 1.5f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Uncut" && isSpinning)
        {
            if (!isInWood)
            {
                source.Play(); //turn on cutting sound
                isInWood = true;
            }
        }
    }
}

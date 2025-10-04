using Unity.VisualScripting;
using UnityEngine;

// this is the final version of the player's tool to cut objects
public class PlasmaBlade : MonoBehaviour
{
    public bool isRight = false;
    public bool isOn = false;

    public float vibrationAmplitude = 0.3f;
    
    public ParticleSystem[] emitters;

    public Material cutMaterial;
    public Material EmissionMaterial1;
    public Material EmissionMaterial2;

    //sound
    private AudioSource source;
    public AudioClip on;
    public AudioClip off;

    public bool isInWood = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // right blade
        if (isRight && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            PowerOn();
        }
        // left blade
        else if (!isRight && OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
        {
            PowerOn();
        }
        else
        {
            PowerOff();
        }
    }

    private void PowerOn()
    {
        if (!isOn)
        {
            isOn = true;
            source.PlayOneShot(on); //play on sound
            EmissionMaterial1.EnableKeyword("_EMISSION");
            EmissionMaterial2.EnableKeyword("_EMISSION");
        }
        // set haptic feedback
        if (isRight)
        {
            OVRInput.SetControllerVibration(1f, vibrationAmplitude, OVRInput.Controller.RTouch);
        }
        else
        {
            OVRInput.SetControllerVibration(1f, vibrationAmplitude, OVRInput.Controller.LTouch);
        }
    }
    
    private void PowerOff()
    {
        if (isOn)
        {
            source.PlayOneShot(off);
            EmissionMaterial1.DisableKeyword("_EMISSION");
            EmissionMaterial2.DisableKeyword("_EMISSION");
        }

        isOn = false;

        foreach (ParticleSystem emitter in emitters)
        {
            emitter.Stop(); // turn of particles
        }
        
        // stop haptic feedback
        if (isRight)
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
        if (other.gameObject.tag == "Uncut" && isOn)
        {
            foreach (ParticleSystem emitter in emitters)
            {
                emitter.Play(); // turn on particles
            }

            if (other.gameObject.GetComponent<CutZone>().isCut == false)
            {
                Debug.Log("Cutting wood!");
                other.gameObject.transform.parent.GetComponent<Cuttable>().uncutDetectors--;
                other.gameObject.GetComponent<MeshRenderer>().material = cutMaterial; //set cut area material to the cut material 
                other.gameObject.GetComponent<CutZone>().isCut = true;
            }
            vibrationAmplitude *= 1.5f;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "Uncut" && isOn)
        {
            if (!isInWood)
            {
                source.Play(); //turn on cutting sound
                isInWood = true;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (ParticleSystem emitter in emitters)
        {
            emitter.Stop(); // turn of particles
        }
        source.Stop(); //turn off cutting sound
        isInWood = false;

        // set controller vibration back to default
        vibrationAmplitude /= 1.5f;
    }
}

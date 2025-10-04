using UnityEngine;
using System.Collections;

public class FallingFloor : MonoBehaviour
{
    private float timeStayed = 0;
    public float fallTimer;

    private void OnCollisionStay(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            timeStayed += Time.deltaTime;
            if (timeStayed > fallTimer)
            {
                Debug.Log("Object should be falling");
                Fall();
            }
        }
    }

    private void Fall()
    {
        Rigidbody floorRigidbody = gameObject.GetComponent<Rigidbody>();
        floorRigidbody.useGravity = true;
        floorRigidbody.isKinematic = false;
    }
}

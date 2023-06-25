using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    Vector3 origin;
    Rigidbody rb;

    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        active = true;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bottom"))
        {
            if (active)
            {
                active = false;
                StartCoroutine(reset());
            }
        }
    }

    IEnumerator reset()
    {
        yield return new WaitForSeconds(3);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = origin;

        active = true;
    }


}

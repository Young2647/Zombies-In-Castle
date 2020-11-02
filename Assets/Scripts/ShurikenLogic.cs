using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenLogic : MonoBehaviour
{
    const float SHURIKEN_LIFE_TIME = 1.5f;
    Rigidbody m_rigidBody;
    const float SPEED = 10.0f;
    Collider m_collider;


    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.velocity = transform.forward * SPEED;
        m_collider = GetComponent<Collider>();

        Destroy(gameObject,SHURIKEN_LIFE_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Zombie")
            IsHit();
    }

    void IsHit()
    {

        Destroy(gameObject);

    }
}

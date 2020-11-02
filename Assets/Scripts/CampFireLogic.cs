using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFireLogic : MonoBehaviour
{
    ZombieLogic[] m_zombieLogics;

    ParticleSystem[] m_particleSystems;

    [SerializeField]
    GameObject m_explosionObject;

    [SerializeField]
    Transform m_explosionPlace;

    FireState m_state;
    MeshRenderer m_meshRenderer;

    Collider m_collider;

    AudioSource m_audioSource;

    [SerializeField]
    AudioClip m_explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<Collider>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, -0.5f);
    }

    void OnTriggerEnter(Collider other){
        m_zombieLogics = FindObjectsOfType<ZombieLogic>();
        if (other.tag == "Player"){
            Explode();
            for(int index = 0; index < m_zombieLogics.Length; ++index)
            {
                m_zombieLogics[index].TakeDamage(100);
            }
            SetState(FireState.Inactive);
        }
    }

    void Explode()
    {
        m_audioSource.PlayOneShot(m_explosionSound);
        Instantiate(m_explosionObject,m_explosionPlace.position,Quaternion.Euler(new Vector3(0,0,0)));
    }

    public void SetState(FireState state){
        if (state == FireState.Inactive){
            m_state = FireState.Inactive;
            m_meshRenderer.enabled = false;
            m_collider.enabled = false;
        }
        else if (state == FireState.Active){
            m_state = FireState.Active;
            m_meshRenderer.enabled = true;
            m_collider.enabled = true;
        }
    }
}

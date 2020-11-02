using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireState{
    Active,
    Inactive
}
public class NewCampFireLogic : MonoBehaviour
{
    
    const float MAX_COOL_DOWN = 5.0f;

    float m_cooldown = 0.0f;
    ZombieLogic m_zombieLogic;

    ParticleSystem[] m_particleSystems;

    [SerializeField]
    GameObject m_explosionObject;

    [SerializeField]
    Transform m_explosionPlace;

    GameObject m_bigZombie;

    FireState m_state = FireState.Active;
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
        if(!CheckActive()){
            m_meshRenderer.enabled = false;
            m_collider.enabled = false;
        }
        transform.Rotate(Vector3.forward, -0.5f);
    }

    void OnTriggerEnter(Collider other){
        m_zombieLogic = FindObjectOfType<ZombieLogic>();
        if (other.tag == "Player"){
            Explode();
            m_zombieLogic.TakeDamage(100);
            SetInactive();
        }
    }

    bool CheckActive(){
        if (m_cooldown <= 0){
            m_state = FireState.Active;
            m_meshRenderer.enabled = true;
            m_collider.enabled = true;
            return true;
        }
        if (m_state == FireState.Inactive){
            m_cooldown -= Time.deltaTime;
            return false;
        }
        if (m_cooldown <= 0){
            m_state = FireState.Active;
            m_meshRenderer.enabled = true;
            m_collider.enabled = true;
            return true;
        }
        return true;
    }
    void SetInactive(){
        m_state = FireState.Inactive;
        m_cooldown = MAX_COOL_DOWN;
    }
    void Explode()
    {
        m_audioSource.PlayOneShot(m_explosionSound);
        Instantiate(m_explosionObject,m_explosionPlace.position,Quaternion.Euler(new Vector3(0,0,0)));
    }
}

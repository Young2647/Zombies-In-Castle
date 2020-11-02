using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    PlayerLogic m_playerLogic;
    GameManager m_gameManager;
    AudioSource m_audioSource;

    [SerializeField]
    AudioClip m_dooropenSound;
    // Start is called before the first frame update
    void Start()
    {
        m_playerLogic = FindObjectOfType<PlayerLogic>();
        m_gameManager = FindObjectOfType<GameManager>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Player" && (m_playerLogic.passStage3||!m_playerLogic.passStage1)){
            m_audioSource.PlayOneShot(m_dooropenSound);
            m_gameManager.Win();
        }
    }
}

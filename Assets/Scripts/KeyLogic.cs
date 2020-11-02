using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLogic : MonoBehaviour
{
    GameManager m_gameManager;
    AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, -1.0f);
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){
            m_audioSource.PlayOneShot(m_audioSource.GetComponent<AudioClip>());
            m_gameManager.key_num += 1;
            Destroy(gameObject);
            m_gameManager.SaveGame();
        }

    }
}

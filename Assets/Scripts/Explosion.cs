using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    PlayerLogic m_playerLogic;
    
    float m_lifeTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        m_playerLogic = FindObjectOfType<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0)
            Destroy(gameObject);
    }


}
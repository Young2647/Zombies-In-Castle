using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public int zombieCount = 0;
    public bool zombieKilled = false;
    const float MAX_COOL_DOWN = 1.0f;

    public int key_num = 0;
    float m_cooldown = 0.0f;
    bool instage = false;
    int m_currentStage = 0;
    PlayerLogic m_playerLogic;
    CameraLogic m_cameraLogic;

    CampFireLogic m_campFireLogic;

    [SerializeField]
    GameObject m_player;
    
    [SerializeField]
    GameObject m_zombiePrefab;

    [SerializeField]
    GameObject m_bigzombiePrefab;

    [SerializeField]
    GameObject m_keyPrefab;

    [SerializeField]

    Transform m_keySpawnPlace1;

    [SerializeField]

    Transform m_keySpawnPlace2;

    [SerializeField]

    Transform m_keySpawnPlace3;

    [SerializeField]

    Transform m_zombieSpawnPlace1;
    [SerializeField]

    Transform m_zombieSpawnPlace2;
    [SerializeField]

    Transform m_zombieSpawnPlace3;
    [SerializeField]

    Transform m_zombieSpawnPlace4;
    [SerializeField]

    Transform m_zombieSpawnPlace5;
     [SerializeField]

    Transform m_zombieSpawnPlace6;
     [SerializeField]

    Transform m_zombieSpawnPlace7;
    [SerializeField]
    Transform m_bigZombieSpawnPlace;


    [SerializeField]
    TextMeshProUGUI m_WinUi;

    [SerializeField]
    TextMeshProUGUI m_SaveUi;
    
    [SerializeField]
    TextMeshProUGUI m_KeyUi;


    AudioSource m_audioSource;
    [SerializeField]
    AudioClip m_saveSound;

    // Start is called before the first frame update
    void Start()
    {
        m_playerLogic = FindObjectOfType<PlayerLogic>();
        m_cameraLogic = FindObjectOfType<CameraLogic>();
        m_campFireLogic = FindObjectOfType<CampFireLogic>();
        m_audioSource = GetComponent<AudioSource>();
        SaveGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieCount == 0 && zombieKilled && instage)
            OutStage(m_currentStage,"");
        if(m_cooldown > 0 )
            m_cooldown -= Time.deltaTime;
        else
            UpdateSaveUI("");
        UpdateKeyUI(key_num);

        if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Joystick1Button12)){
            Application.Quit();
        }
        if(m_WinUi.text != "" && (Input.GetButtonDown("Fire2")||Input.GetKeyDown(KeyCode.Joystick1Button3))){
            m_playerLogic.Reborn();
            UpdateWinUI("");
        }
    }
    public void StageConstruct(int stage_num){
        if (stage_num == 1){
            Instage(1);
            Mathf.Clamp(m_player.transform.position.x,-23.0f,-9.7f);
            SpawnZombie(m_zombieSpawnPlace1);
            zombieCount ++;
        }
        if (stage_num == 2){
            Instage(2);
            Mathf.Clamp(m_player.transform.position.x,-1.32f,14.6f);
            SpawnZombie(m_zombieSpawnPlace2);
            SpawnZombie(m_zombieSpawnPlace3);
            SpawnZombie(m_zombieSpawnPlace4);
            SpawnZombie(m_zombieSpawnPlace5);
            SpawnZombie(m_zombieSpawnPlace6);
            SpawnZombie(m_zombieSpawnPlace7);
            m_campFireLogic.SetState(FireState.Active);
            zombieCount += 6;

        }
        if (stage_num == 3){
            Instage(3);
            Mathf.Clamp(m_player.transform.position.x,27.79f,41.2f);
            SpawnBigZombie(m_bigZombieSpawnPlace);
            zombieCount += 1;
        }
        
    }

    void SpawnZombie(Transform m_spawnplace)
    {
        Instantiate(m_zombiePrefab, m_spawnplace.position,Quaternion.Euler(new Vector3(0,-90,0)));
    }
    void SpawnBigZombie(Transform m_spawnplace){
        Instantiate(m_bigzombiePrefab, m_spawnplace.position,Quaternion.Euler(new Vector3(0,-90,0)));
    }

    void SpawnKey(Transform m_spawnplace)
    {
        Instantiate(m_keyPrefab, m_spawnplace.position,Quaternion.Euler(new Vector3(0,90,90)));
    }
    public void SaveGame(){
        m_playerLogic.Save();
        PlayerPrefs.Save();
        UpdateSaveUI("Save");
        PlaySound(m_saveSound);
    }

    public void LoadGame(){
        m_playerLogic.Load();
        OutStage(m_currentStage-1,"n");
        UpdateSaveUI("Load");
    }

    void Instage(int stage_num){
        instage = true;
        m_currentStage = stage_num;
        m_cameraLogic.SetStage(true);
        m_playerLogic.SetStage(true);
        zombieKilled = false;
    }

    void OutStage(int stage_num,string key_state){
        instage = false;
        zombieKilled = false;
        zombieCount = 0;
        if (stage_num == 1){
            m_playerLogic.passStage1 = true;
            if(key_state != "n")
                SpawnKey(m_keySpawnPlace1);
        }
        if(stage_num == 2){
            m_playerLogic.passStage2 = true;
            if(key_state != "n")
                SpawnKey(m_keySpawnPlace2);
        }
        if(stage_num == 3){
            m_playerLogic.passStage3 = true;
            if(key_state != "n")
                SpawnKey(m_keySpawnPlace3);
        }
        m_cameraLogic.SetStage(false);
        m_playerLogic.SetStage(false);
    }

    public void Win()
    {
        Time.timeScale = 0;
        UpdateWinUI("Win");
    }
    
    void UpdateWinUI(string state){
        if (state == "Win"){
            if (!m_playerLogic.passStage1)
                m_WinUi.text = "You win!\n(the right not always means right.)";
            else
                m_WinUi.text = "You win!\n(press esc to quit)";
        }
        else{
            m_WinUi.text = "";
        }
    }

    void UpdateSaveUI(string state){
        m_cooldown = MAX_COOL_DOWN;
        if (state == "Save")
            m_SaveUi.text = "Game Saved!";
        else if (state == "Load")
            m_SaveUi.text = "Game Loaded!";   
        else
            m_SaveUi.text = ""; 
    }

    public void UpdateKeyUI(int key_num){
        m_KeyUi.text = "Keys:" + key_num + "/3";
    }
    
    void PlaySound(AudioClip sound){
        if(m_audioSource && sound){
            m_audioSource.PlayOneShot(sound);
        }
    }
}

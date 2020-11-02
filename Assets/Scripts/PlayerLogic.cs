using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLogic : MonoBehaviour
{
    const float MOVEMENT_SPEED = 2.72f;
    const float JUMP_HEIGHT = 0.25f;
    const float GRAVITY = 0.8f;
    const float MAX_COOLDOWN = 0.25f;
    const float SHURIKEN_LIFE_TIME = 1.5f;
    int m_health = 100;
    int m_jumpCount = 2;
    float m_horizontalInput;
    float m_verticalInput;
    float m_cooldown = 0.0f;
    Vector3 m_movementInput;
    Vector3 m_movement;
    Vector3 m_heightMovement;

    bool m_isJumping = false;
    float m_jumpStart;

    public bool instage = false;
    public bool passStage1 = false;
    public bool passStage2 = false;
    public bool passStage3 = false;

    CharacterController m_characterController;
    Animator m_animator;

    ZombieLogic[] m_zombieLogics;
    
    [SerializeField]
    Transform m_shurikenSpawn;

    [SerializeField]
    GameObject m_shurikenPrefab;
    GameManager m_gameManager;
    
    [SerializeField]
    TextMeshProUGUI m_healthTMP;

    [SerializeField]
    TextMeshProUGUI m_DieUi;

    [SerializeField]
    AudioClip m_shootSound;

    [SerializeField]
    AudioClip m_hitSound;

    AudioSource m_audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_gameManager = FindObjectOfType<GameManager>();
        m_audioSource = GetComponent<AudioSource>();
        UpdateHealthUI();
    }
    
    // Update is called once per frame
    void Update()
    {
        m_verticalInput = Input.GetAxisRaw("Vertical");
        m_horizontalInput = Input.GetAxisRaw("Horizontal");
        m_movementInput = new Vector3(m_horizontalInput,0,m_verticalInput);
        
        if (!instage)
            m_gameManager.StageConstruct(SetStage(transform.position.x));

        m_animator.SetFloat("HorizontalInput", Mathf.Abs(m_horizontalInput));
        m_animator.SetFloat("VerticalInput", m_verticalInput);
        
        if(m_characterController.isGrounded)
            m_jumpCount = 2;
        if (Input.GetButtonDown("Jump")||Input.GetKeyDown(KeyCode.Joystick1Button1) && !m_isJumping  ){
            if (m_jumpCount > 0){
                m_jumpStart = transform.position.y;
                m_isJumping = true;
                m_jumpCount--;
            }
        }
        if(!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Up") && Time.timeScale != 0){
            if (m_horizontalInput > 0.0f){
                transform.rotation = Quaternion.Euler(new Vector3(0,100,0));   
            }
            else if (m_horizontalInput < 0.0f){
                transform.rotation = Quaternion.Euler(new Vector3(0,-100,0));   
            }
        }
        if ((Input.GetKeyDown(KeyCode.J)||Input.GetKeyDown(KeyCode.Joystick1Button2)) && m_cooldown <= 0.0f){
            ShootShuriken();
        }
        
        if(m_cooldown > 0.0f)
            {
                m_cooldown -= Time.deltaTime;
            }
        if ((Input.GetButtonDown("Fire2")||Input.GetKeyDown(KeyCode.Joystick1Button3)) && m_health <= 0){
           Reborn();
        }
    }

    void FixedUpdate(){
        if ( Time.timeScale != 0){
            m_movement = m_movementInput * MOVEMENT_SPEED * Time.deltaTime;

            if(m_isJumping){
                m_heightMovement.y = JUMP_HEIGHT;
                m_isJumping = false; 
            }
            m_heightMovement.y -= GRAVITY * Time.deltaTime;
            if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Up"))
                m_characterController.Move(m_movement + m_heightMovement);

        }
    }

    void ShootShuriken()
    {
        if(m_animator)
        {
            m_animator.SetTrigger("ShootShuriken");      
            m_cooldown = MAX_COOLDOWN;
        }   
    }

    void PlayShootSound(){
        PlaySound(m_shootSound);
    }
    public void SpawnShuriken()
    {
        Instantiate(m_shurikenPrefab, m_shurikenSpawn.position, transform.rotation);
    }
    public void Save()
    {
        Debug.Log("Saving Game!");

        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);

        PlayerPrefs.SetFloat("PlayerRotX", transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("PlayerRotY", transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("PlayerRotZ", transform.rotation.eulerAngles.z);
    }

    public void Load()
    {
        m_animator.SetTrigger("Reborn");
        Debug.Log("Loading Game!");

        float playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
        float playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
        float playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ");

        float playerRotX = PlayerPrefs.GetFloat("PlayerRotX");
        float playerRotY = PlayerPrefs.GetFloat("PlayerRotY");
        float playerRotZ = PlayerPrefs.GetFloat("PlayerRotZ");

        m_characterController.enabled = false;
        transform.position = new Vector3(playerPosX, playerPosY, playerPosZ);
        transform.rotation = Quaternion.Euler(playerRotX, playerRotY, playerRotZ);
        m_characterController.enabled = true;
    }

    public void TakeDamage(int damage){
        PlaySound(m_hitSound);
        m_animator.SetTrigger("Hit");
        m_health -= damage;
        UpdateHealthUI();
        if (m_health <= 0)
            Die();
    }

    public int SetStage(float x_pos){
        if (Mathf.Abs(x_pos + 23.0f) < 0.25f && !passStage1){
            return 1;
        }  

        if (Mathf.Abs(x_pos + 1.32f) < 0.25f && !passStage2){
            return 2;
        } 

        if (Mathf.Abs(x_pos - 27.79f) < 0.25f && !passStage3){
            return 3;
        }
        return 0;
    }
    public void SetStage(bool stage){
        instage = stage;
    }


    void Die(){
        m_animator.SetTrigger("Die");
        m_zombieLogics = FindObjectsOfType<ZombieLogic>();
        for (int index = 0; index < m_zombieLogics.Length; index++){
            m_zombieLogics[index].DestroyZombie();
        }
    }

    void Dead(){
        if (m_health <= 0){
            Time.timeScale = 0;
            UpdateDieUI("Dead");
        }
    }
    public void Reborn(){
        m_gameManager.LoadGame();
        Time.timeScale = 1;
        m_health = 100;
        m_animator.SetTrigger("Reborn");
        UpdateHealthUI();
        UpdateDieUI("");
    }
    void UpdateHealthUI(){
        if(m_healthTMP){
            if(m_health >= 0)
                m_healthTMP.text = "Health :" + m_health;
            else
                m_healthTMP.text = "Health :" + 0;
        }
    }
    
    void UpdateDieUI(string state){
        if (state == "Dead")
            m_DieUi.text = " You died!\n(click right button to load save point!)";
        else 
            m_DieUi.text = "";
    }


    void PlaySound(AudioClip sound){
        if(m_audioSource && sound){
            m_audioSource.PlayOneShot(sound);
        }
    }
}

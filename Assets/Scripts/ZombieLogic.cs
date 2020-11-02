using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState{
    WakeUp,
    Walk,
    Run,
    Attack,
    Dead,
    Hit
    
}
public class ZombieLogic : MonoBehaviour
{
    public bool ifBig;
    int m_health = 100;
    NavMeshAgent m_navMeshAgent;
    Transform m_targetDestination;
    Vector3 m_previousDestination;
    ZombieState m_zombieState = ZombieState.WakeUp;
    GameObject m_player;


    const float AGGRO_RADIUS = 8.0f;
    const float S_ATTACK_RADIUS = 1.8f;
    const float B_ATTACK_RADIUS = 2.0f;
    const float MAX_ATTACK_COOLDOWN = 1.0f;

    const float S_MAX_STOP_TIME = 0.7f;

    const float B_MAX_STOP_TIME = 2.0f;
    float m_attackCooldown = 0.0f;
    float m_stoptime = 0.0f;
    PlayerLogic m_playerLogic;
    GameManager m_gameManager;
    Animator m_animator;
    AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if(!ifBig)
            m_health = 100;
        else
            m_health = 380;
        
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        
        m_playerLogic = FindObjectOfType<PlayerLogic>();

        m_animator = GetComponent<Animator>();

        m_player = GameObject.Find("mainNinja");

        m_gameManager = FindObjectOfType<GameManager>();

        m_audioSource = GetComponent<AudioSource>();

    }

    void OnDrawGizmos()
    {
        // Set the color of the Gizmos to transparant red
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);

        // Draw a sphere with center of this object with radius of aggro radius (defined in code)
        Gizmos.DrawSphere(transform.position, AGGRO_RADIUS);

        // Set the color of the Gizmos to transparant blue
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.25f);

        // Draw a sphere with center of this object with radius of attack radius (defined in code)
        if (!ifBig)
            Gizmos.DrawSphere(transform.position, S_ATTACK_RADIUS);
        else
            Gizmos.DrawSphere(transform.position, B_ATTACK_RADIUS);
    }
    void SetState(ZombieState zombieState){
        m_zombieState = zombieState;
    }
    bool CheckIsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, m_targetDestination.position) < range;
    }
    void SetTargetDestination()
    {
        // If we have a NavMeshAgent and the previous destination is different from the current one
        if (m_navMeshAgent && m_previousDestination != m_targetDestination.position)
        {
            // Then set the destination to the new target and store it as the previous one
            m_navMeshAgent.isStopped = false;
            m_navMeshAgent.SetDestination(m_targetDestination.position);
            m_previousDestination = m_targetDestination.position;
        }
    }
    // Update is called once per frame

    void Update()
    {
        if(ifBig)
            Debug.Log(m_stoptime);
        m_targetDestination = m_player.transform;
        switch(m_zombieState)
        {
            case (ZombieState.WakeUp):
                    m_animator.SetFloat("MovementInput",0.0f);// Check if the player is nearby
                    AnimatorStateInfo info =m_animator.GetCurrentAnimatorStateInfo(0);
                    if ( info.normalizedTime >= 1.0f)
                        SetState(ZombieState.Walk);
                //CheckIsPlayerInAggroRange();
                break;

            case (ZombieState.Walk):
                // Chase the player by setting the destination to the player's position
                if (m_stoptime > 0){
                    m_navMeshAgent.isStopped = true;
                    m_stoptime -= Time.deltaTime;
                }
                else
                    m_navMeshAgent.isStopped = false;
                SetTargetDestination();
                m_animator.SetFloat("MovementInput",1);
                if(!ifBig){
                    if (CheckIsPlayerInRange(S_ATTACK_RADIUS) && !m_navMeshAgent.isStopped)
                    {
                        SetState(ZombieState.Attack);
                    }
                }else{
                    if (CheckIsPlayerInRange(B_ATTACK_RADIUS) && !m_navMeshAgent.isStopped)
                    {
                        SetState(ZombieState.Attack);
                    }
                }
                //CheckIsPlayerInAttackRange();
                break;

            case (ZombieState.Attack):
                // Check if we are in range of the player
                if(!ifBig){
                    if (CheckIsPlayerInRange(S_ATTACK_RADIUS))
                    {
                        // If we are in range stop moving and attack
                        m_navMeshAgent.isStopped = true;
                        UpdateAttack();
                    }
                    else
                    {
                        // If we are out of range chase the player again
                        SetState(ZombieState.Walk);
                    }
                }else{
                    if (CheckIsPlayerInRange(B_ATTACK_RADIUS))
                    {
                        // If we are in range stop moving and attack
                        m_navMeshAgent.isStopped = true;
                        UpdateAttack();
                    }
                    else
                    {
                        // If we are out of range chase the player again
                        SetState(ZombieState.Walk);
                    }
                }
                break;

            case (ZombieState.Dead):
                m_navMeshAgent.isStopped = true;
                m_animator.SetTrigger("Dead");
                break;
            case (ZombieState.Hit):
                m_navMeshAgent.isStopped = true;
                m_animator.SetTrigger("Hit");
                if (m_stoptime > 0)
                    m_stoptime -= Time.deltaTime;
                else
                    m_navMeshAgent.isStopped = false;
                SetState(ZombieState.Walk);
                break;
        }
    }
    void UpdateAttack(){
        m_attackCooldown -= Time.deltaTime;
        if(m_attackCooldown <= 0.0f)
        {
            m_animator.SetTrigger("Attack");
            m_attackCooldown = MAX_ATTACK_COOLDOWN;
        }
    }
    void Attack(){
        m_playerLogic.TakeDamage(25);
    }

    public void TakeDamage(int damage){
            m_health -= damage;
        m_audioSource.PlayOneShot(m_audioSource.clip);
        if (m_health > 0){
            if (ifBig && damage >= 50)
                m_stoptime = B_MAX_STOP_TIME;
            else
                m_stoptime = S_MAX_STOP_TIME;
            SetState(ZombieState.Hit);
        }
        else 
            SetState(ZombieState.Dead);
    }

    void ZombieDie(){
        DestroyZombie();
        m_gameManager.zombieCount -= 1;
        m_gameManager.zombieKilled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Shuriken"){
            Destroy(collision.collider);
            TakeDamage(30);
        }

    }

    public void DestroyZombie(){
        Destroy(gameObject);
    }

}

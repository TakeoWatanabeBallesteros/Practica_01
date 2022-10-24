using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;
public class DronBehaviour : MonoBehaviour,IDamageable
{
    [SerializeField] private string currentState;
    [SerializeField] private Transform enemyEyes;
    [SerializeField] private Transform playerHead;
    NavMeshAgent m_NavMeshAgent;
    [SerializeField] private List<Transform> m_PartolTargets;
    private int m_CurrentPatrolTargetId = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hearDistance;
    [SerializeField] private float sightDistance;
    [SerializeField] private float maxAttackDistance;
    [SerializeField] private float sightAngle;
    [SerializeField] private float headHeight;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private int maxHealth;
    private int health;
    private StateMachine fsm;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        fsm = new StateMachine();
        
        fsm.AddState("Idle", new State(
            onLogic: (state) => { fsm.RequestStateChange("Patrol"); }));
        fsm.AddState("Patrol", new State(
            onEnter: (state) =>
            {
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.destination = m_PartolTargets[m_CurrentPatrolTargetId].position;
            },
            onLogic: (state) =>
            {
                if (PatrolTargetPositionArrived())
                    MoveToNextPatrolPosition();
            },
            onExit: (state) =>
            {
                m_NavMeshAgent.isStopped = true;
            }
            ));
        fsm.AddState("Alert", new State(
            onLogic: (state) =>
            {
                if (state.timer.Elapsed > 5)
                    fsm.RequestStateChange("Patrol");
                if(SeesPlayer() && PlayerOnAttackDistance())
                    fsm.RequestStateChange("Attack");
                else if(SeesPlayer() && !PlayerOnAttackDistance())
                    fsm.RequestStateChange("Chase");
                Debug.DrawLine(enemyEyes.position, playerHead.position, SeesPlayer() ?Color.green:Color.red);
                    
                RotateAtSpeed(rotationSpeed);
            }));
        fsm.AddState("Chase", new State(
            onEnter: (state) =>
            {
                m_NavMeshAgent.isStopped = false;
            },
            onLogic: (state) =>
            {
                MoveTowardsPlayer(moveSpeed);
            },
            onExit: (state) =>
            {
                m_NavMeshAgent.isStopped = true;
            }
            ));
        fsm.AddState("Attack", new State(
            onLogic: (state) =>
            {
                enemyEyes.LookAt(playerHead);
                transform.rotation = Quaternion.Euler(0.0f, enemyEyes.rotation.eulerAngles.y, 0.0f);
            }
            ));
        fsm.AddState("Hit", new State());
        fsm.AddState("Die", new State());
        
        fsm.AddTransition(new Transition(
            "Patrol",
            "Alert",
            (transition) => HearsPlayer()
        ));
        
        fsm.SetStartState("Idle");
        fsm.Init();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.OnLogic();
        currentState = fsm.ActiveState.name;
    }
    
    private bool PatrolTargetPositionArrived()
    {
        return !m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
    }
    
    private void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolTargetId;
        if (m_CurrentPatrolTargetId >= m_PartolTargets.Count)
            m_CurrentPatrolTargetId = 0;
        m_NavMeshAgent.destination = m_PartolTargets[m_CurrentPatrolTargetId].position;
    }

    bool PlayerOnAttackDistance()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= maxAttackDistance;
    }
    
    bool HearsPlayer()
    {
        Vector3 l_PlayerPosition = playerHead.position;
        return Vector3.Distance(l_PlayerPosition, enemyEyes.position) <= hearDistance;
    }
    
    bool SeesPlayer()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_DirectionPlayerXZ = PlayerPos - transform.position;
        l_DirectionPlayerXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.Normalize();

        Vector3 l_Direction = playerHead.position - enemyEyes.position;
        float l_Lenght = l_Direction.magnitude;
        l_Direction /= l_Lenght;

        Ray l_Ray = new Ray(enemyEyes.position, l_Direction);
        //if(!)
        return Vector3.Distance(PlayerPos, transform.position) < sightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionPlayerXZ) > Mathf.Cos(sightAngle * Mathf.Deg2Rad / 2.0f) &&
               !Physics.Raycast(l_Ray, l_Lenght, playerLayerMask.value);
    }
    
    void RotateAtSpeed(float speed)
    {
        transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime);
    }
    
    void MoveTowardsPlayer(float speed)
    {
        m_NavMeshAgent.destination = playerHead.transform.position;
    }

    public void TakeDamage(int damage)
    {
        
    }
}

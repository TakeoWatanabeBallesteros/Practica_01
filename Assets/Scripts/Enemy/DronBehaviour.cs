using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;
public class DronBehaviour : MonoBehaviour
{
    
    NavMeshAgent m_NavMeshAgent;
    [SerializeField] private List<Transform> m_PartolTargets;
    private int m_CurrentPatrolTargetId = 0;
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
                m_NavMeshAgent.destination = m_PartolTargets[m_CurrentPatrolTargetId].position;
            },
            onLogic: (state) =>
            {
                if (PatrolTargetPositionArrived())
                    MoveToNextPatrolPosition();
            }
            ));
        fsm.AddState("Alert", new State());
        fsm.AddState("Chase", new State());
        fsm.AddState("Attack", new State());
        fsm.AddState("Hit", new State());
        fsm.AddState("Die", new State());
        
        
        
        fsm.SetStartState("Idle");
        fsm.Init();
    }

    // Update is called once per frame
    void Update()
    {
        fsm.OnLogic();
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;
using UnityEngine.UI;
public class DronBehaviour : MonoBehaviour,IDamageable
{
    [SerializeField] private string currentState;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform enemyEyes;
    [SerializeField] private Transform playerHead;
    NavMeshAgent m_NavMeshAgent;
    [SerializeField] private List<Transform> m_PartolTargets;
    private int m_CurrentPatrolTargetId = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hearDistance;
    [SerializeField] private float sightDistance;
    [SerializeField] private float maxChaseDistance;
    [SerializeField] private float maxAttackDistance;
    [SerializeField] private float minAttackDistance;
    [SerializeField] private float sightAngle;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private int maxHealth;
    [SerializeField] private float damage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform muzzle_1;
    [SerializeField] private Transform muzzle_2;
    [SerializeField] private ParticleSystem muzzleFlash_1;
    [SerializeField] private ParticleSystem muzzleFlash_2;
    [SerializeField] private BulletBehavior bullet;
    [SerializeField] private Slider lifeBar;
    [SerializeField] private ParticleSystem sparks_1;
    [SerializeField] private ParticleSystem sparks_2;
    [SerializeField] private ParticleSystem explosion;
    
    private float health;
    private StateMachine fsm;
    private bool canBeDamaged = true;
    private string lastState;
    private int hitAnimID;
    private int dieAnimID;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        hitAnimID = Animator.StringToHash("Hit");
        dieAnimID = Animator.StringToHash("Die");
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
                lastState = fsm.ActiveState.name;
                m_NavMeshAgent.isStopped = true;
            }
            ));
        fsm.AddState("Alert", new State(
            onLogic: (state) =>
            {
                if (state.timer.Elapsed > 360/rotationSpeed)
                    fsm.RequestStateChange("Patrol");
                    
                RotateAtSpeed(rotationSpeed);
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
            }
            ));
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
                lastState = fsm.ActiveState.name;
                m_NavMeshAgent.isStopped = true;
            }
            ));
        fsm.AddState("Attack", new CoState(
            this,
            onLogic: Shoot,
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
            }
            ));
        fsm.AddState("Hit", new State(
            onEnter: (state) =>
            {
                StartCoroutine(Hit());
                canBeDamaged = false;
            },
            onLogic: (state) =>
            {
                if (state.timer.Elapsed >= 2.0f)
                {
                    fsm.RequestStateChange(lastState == "Patrol" ? "Alert" : lastState);
                }
            },
            onExit: (state) =>
            {
                lastState = fsm.ActiveState.name;
                StartCoroutine(CanBeAttacked());
            }
            ));
        fsm.AddState("Die", new CoState(
            this,
            onEnter: (state) =>
            {
                canBeDamaged = false;
            },
            onLogic: Die
            ));
        
        fsm.AddTransition(new Transition(
            "Patrol",
            "Alert",
            (transition) => HearsPlayer()
        ));
        fsm.AddTransition(new Transition(
            "Alert",
            "Chase",
            (transition) => SeesPlayer() && !CanStartAttack()
        ));
        fsm.AddTransition(new Transition(
            "Alert",
            "Attack",
            (transition) => SeesPlayer() && CanStartAttack()
        ));
        fsm.AddTransition(new Transition(
            "Chase",
            "Attack",
            (transition) => CanStartAttack() && SeesPlayer()
        ));
        fsm.AddTransition(new Transition(
            "Chase",
            "Patrol",
            (transition) => !CanChase()
        ));
        fsm.AddTransition(new Transition(
            "Attack",
            "Chase",
            (transition) => !PlayerOnAttackDistance()||!SeesPlayer()
        ));
        fsm.AddTriggerTransitionFromAny(
            "OnHit",
            new Transition("", "Hit", t => (health > 0))
        );
        fsm.AddTransitionFromAny(
            new Transition("", "Die", t => (health <= 0))
        );
        
        fsm.SetStartState("Idle");
        fsm.Init();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.OnLogic();
        if(fsm.ActiveState.name == "Attack")
        {
            transform.rotation = Quaternion.Euler(0.0f, enemyEyes.rotation.eulerAngles.y, 0.0f);
            enemyEyes.LookAt(playerHead);
        }
        currentState = fsm.ActiveState.name;
        lifeBar.transform.forward = -(GameManager.GetGameManager().GetPlayer().transform.position - transform.position).normalized;
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

    bool CanChase()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= maxChaseDistance;
    }
    
    bool PlayerOnAttackDistance()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= maxAttackDistance;
    }

    bool CanStartAttack()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= minAttackDistance;
    }
    
    bool HearsPlayer()
    {
        Vector3 PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        return Vector3.Distance(PlayerPos, transform.position) <= hearDistance;
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
        return Vector3.Distance(PlayerPos, transform.position) < sightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionPlayerXZ) > Mathf.Cos(sightAngle * Mathf.Deg2Rad / 2.0f) &&
               !Physics.Raycast(l_Ray, l_Lenght, playerLayerMask.value);
    }
    
    private void RotateAtSpeed(float speed)
    {
        transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime);
    }
    
    private void MoveTowardsPlayer(float speed)
    {
        m_NavMeshAgent.destination = playerHead.transform.position;
    }

    public void TakeDamage(float damage)
    {
        if (canBeDamaged)
        {
            fsm.Trigger("OnHit");
            health -= damage;
            if(health > 0) animator.SetTrigger(hitAnimID);
        }
        else
        {
            health -= damage * 0.6f;
        }

        if (!lifeBar.enabled) lifeBar.enabled = true;
        lifeBar.value = health / 100;
    }
    
    private IEnumerator Shoot(CoState<string, string> state)
    {
        // Shoot muzzle_1
        Vector3 destination = playerHead.position;
        var b = Instantiate(bullet, muzzle_1.position, Quaternion.identity);
        b.destination = destination;
        b.damage = damage;
        b.velocity = bulletSpeed;
        b.layerMask = ~LayerMask.NameToLayer("Enemy");
        muzzleFlash_1.Play();
        yield return new WaitForSeconds(0.8f);
        
        // Shoot muzzle_2
        destination = playerHead.position;
        var v = Instantiate(bullet, muzzle_1.position, Quaternion.identity);
        v.destination = destination;
        v.damage = damage;
        v.velocity = bulletSpeed;
        v.layerMask = ~LayerMask.NameToLayer("Enemy");
        muzzleFlash_2.Play();
        
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Hit()
    {
        sparks_1.Play();

        yield return new WaitForSeconds(1f);
        
        sparks_2.Play();
    }
    
    private IEnumerator Die(CoState<string, string> state)
    {
        animator.SetTrigger(dieAnimID);

        yield return new WaitForSeconds(1f);
        
        explosion.Play();

        yield return new WaitForSeconds(0.5f);
        
        state.timer.Reset();
        while (state.timer.Elapsed < 1.5f)
        {
            enemyEyes.transform.position += Vector3.down*Time.deltaTime*.70f/1.5f;
            yield return null;
        }

        Destroy(gameObject, 3f);
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator CanBeAttacked()
    {
        yield return new WaitForSeconds(1.5f);
        canBeDamaged = true;
    }
}

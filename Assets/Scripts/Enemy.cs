using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static UnityEngine.Audio.GeneratorInstance;
using static UnityEngine.GraphicsBuffer;

public class Enemy : LivingEntity
{
    public enum status
    {
        Idle,
        Trace,
        Attack,
        Die
    }

    public EnemyData enemyData;
    public LayerMask targetLayer;

    private Transform target;
    private Collider enemyCollider;
    private AudioSource enemyAdioSource;
    private Animator enemyAnimator;
    private NavMeshAgent agent;
    private ParticleSystem hitEffect;
    private Rigidbody rb;

    private float traceDistance = 50f;
    private float attckDistance = 2f;
    private float damage;
    private float lastAttackTime;
    private bool isSinking = false;
    private float sinkSpeed = 0.5f;

    private status state;
    public status State
    {
        get {  return state; }
        set
        {
            state = value;
            switch (state)
            {
                case status.Idle:
                    enemyAnimator.SetBool("isTrace", false);
                    agent.isStopped = true;
                    break;
                case status.Trace:
                    enemyAnimator.SetBool("isTrace", true);
                    agent.isStopped = false;
                    break;
                case status.Attack:
                    break;
                case status.Die:
                    enemyAnimator.SetTrigger("Die");
                    agent.isStopped = true;
                    agent.enabled = false;
                    break;
            }
        }
    }
    private void Awake()
    {
        enemyAdioSource = GetComponent<AudioSource>();
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        hitEffect = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody>();

    }

    public void Setup(EnemyData enemyData)
    {
        startingHealth = enemyData.maxHealth;
        damage = enemyData.damage;
        agent.speed = enemyData.speed;
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        OnDead.AddListener(StartDeathSequence);

        Setup(enemyData);
        isSinking = false;
        rb.isKinematic = false;

        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, traceDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        enemyCollider.enabled = true;
        State = status.Idle;
    }
    public void OnDisable()
    {
        OnDead.RemoveListener(StartDeathSequence);
        if (agent != null)
        {
            if (agent.gameObject.activeSelf && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            agent.enabled = false;
        }
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void Update()
    {
        if (isSinking)
        {
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
            return;
        }
        if (isDead) return;

        switch (state)
        {
            case status.Idle:
                UpdateIdle();
                break;
            case status.Trace:
                UpdateTrace();
                break;
            case status.Attack:
                UpdateAttack();
                break;
            case status.Die:
                OnDie();
                break;
        }
    }

    private void UpdateAttack()
    {
        if (target == null || Vector3.Distance(target.position, transform.position) > attckDistance)
        {
            State = status.Trace;
            return;
        }
        var lookat = target.position;
        lookat.y = transform.position.y;
        transform.LookAt(lookat);

        if (Time.time > lastAttackTime + enemyData.attackInterval)
        {
            lastAttackTime = Time.time;
            var livingEntity = target.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                livingEntity.OnDamage(enemyData.damage, transform.position, -transform.forward);
            }
        }
        agent.SetDestination(target.position);
    }

    private void UpdateTrace()
    {
        if (target != null && Vector3.Distance(target.position, transform.position) > traceDistance)
        {
            target = null;
            State = status.Idle;
            return;
        }
        if (target != null && Vector3.Distance(target.position, transform.position) < attckDistance)
        {
            State = status.Attack;
        }
        agent.SetDestination(target.position);
    }

    private void UpdateIdle()
    {

        if (target != null && Vector3.Distance(target.position, transform.position) < traceDistance)
        {
            State = status.Trace;
        }
        target = Findtarget(traceDistance);
    }
    private Transform Findtarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        if (colliders.Length == 0)
        {
            return null;
        }
        var target = colliders.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();
        return target.transform;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return;
      
        hitEffect.transform.position = hitPoint;
        hitEffect.transform.forward = hitNormal;
        hitEffect.Play();
        enemyAdioSource.PlayOneShot(enemyData.hurtClip);
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void OnDie()
    {
        if (isDead) return;

        GameObject uiObj = GameObject.Find("UiManager");
        if (uiObj != null)
        {
            UiManager uiManager = uiObj.GetComponent<UiManager>();

            if (uiManager != null)
            {
                uiManager.AddScore(enemyData.score); 
            }
        }
        base.OnDie();
    }

    private void StartDeathSequence() //사망 이펙트 이벤트
    {
        enemyAdioSource.PlayOneShot(enemyData.deathClip);
        State = status.Die;
    }
    public void StartSinking()
    {
        if (enemyCollider != null)  enemyCollider.enabled = false;

        rb.isKinematic = true;
        isSinking = true;

        Destroy(gameObject, 2f);
    }
}

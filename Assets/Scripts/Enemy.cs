using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
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

    private float traceDistance = 20f;
    private float attckDistance = 2f;
    private float damage;
    private float lastAttackTime;

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

    public void Update()
    {
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
        if (hitEffect == null)
        {
            // 만약 이 로그가 찍힌다면 Awake에서 자식을 못 찾은 겁니다.
            Debug.LogError($"{gameObject.name}: hitEffect가 null입니다!");
            return;
        }
        hitEffect.transform.position = hitPoint;
        hitEffect.transform.forward = hitNormal;
        hitEffect.Play();
        enemyAdioSource.PlayOneShot(enemyData.hurtClip);
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void OnDie()
    {
        if (isDead)
        {
            return;
        }
        enemyAdioSource.PlayOneShot(enemyData.deathClip);
        state = status.Die;
        base.OnDie();
    }
}

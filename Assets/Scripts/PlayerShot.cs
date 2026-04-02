using System;
using System.Collections;
using UnityEngine;


//플레이어 공격
public class PlayerShot : MonoBehaviour
{
    public ParticleSystem gunParticle;
    public AudioClip gunShotClip;
    public Transform fireTransform;
    public LayerMask targetLayer;

    private AudioSource gunAudioSource;
    private LineRenderer bulletLineEffect;
    //private PlayerInput playerInput;

    public int damage = 20;
    private float fireDistance = 50;
    private float timeBetFire = 0.12f;
    private float lastFireTime;

    private Coroutine CoShot;


    private void Awake()
    {
        gunAudioSource = GetComponent<AudioSource>();
        bulletLineEffect = GetComponent<LineRenderer>();
        //playerInput = GetComponent<PlayerInput>();

        bulletLineEffect.positionCount = 2;
        bulletLineEffect.enabled = false;
    }

    public void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    private void Shot()
    {
        Vector3 hitPosition = Vector3.zero;
        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;
            var target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        if (CoShot != null)
        {
            StopCoroutine(CoShot);
            CoShot = null;
        }

        CoShot = StartCoroutine(CoShotEffect(hitPosition));
    }

    private IEnumerator CoShotEffect(Vector3 hitPoint)
    {
        gunParticle.Play();
        gunAudioSource.PlayOneShot(gunShotClip);

        bulletLineEffect.SetPosition(0, fireTransform.position);
        bulletLineEffect.SetPosition(1, hitPoint);
        bulletLineEffect.enabled = true;

        yield return new WaitForSeconds(0.03f);
        bulletLineEffect.enabled = false;

    }
}

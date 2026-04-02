using System;
using System.Collections;
using UnityEngine;


//플레이어 공격
public class PlayerController : MonoBehaviour
{
    public ParticleSystem gunParticle;
    public AudioClip gunShotClip;
    public Transform fireTransform;
    public LayerMask targetLayer;

    private AudioSource gunAudioSource;
    private Animator playerAnimator;
    private LineRenderer bulletLineEffect;
    //private PlayerInput playerInput;

    public int damage = 20;
    private float fireDistance = 50;

    private Coroutine CoShot;


    private void Awake()
    {
        gunAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        bulletLineEffect = GetComponent<LineRenderer>();
        //playerInput = GetComponent<PlayerInput>();

        bulletLineEffect.positionCount = 2;
        bulletLineEffect.enabled = false;
    }
    private void OnEnable()
    {
        playerAnimator.SetBool("IsMove", false);
    }

    public void Update()
    {
        if (Input.GetButton("Fire1"))
        {
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
            target.OnDamege(damage, hit.point, hit.normal);
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

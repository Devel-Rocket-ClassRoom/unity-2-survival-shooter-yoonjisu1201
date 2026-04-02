using UnityEngine;

public class PlayerHealth : LivingEntity
{
    public AudioClip playerDeathClip;
    public AudioClip playerHurtClip;

    private Animator playerAnimator;
    private AudioSource playerAudioSource;
    private PlayerMovement playerMovement;
    private PlayerShot playerShot;

    private void Awake()
    {
        startingHealth = 300f;
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShot = GetComponent<PlayerShot>();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        //ui 체력바 활성화

        playerMovement.enabled = true;
        playerShot.enabled = true;
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        playerAudioSource.PlayOneShot(playerHurtClip);
        //ui 체력바 업데이트
    }
    public override void OnDie()
    {
        if (isDead)
        {
            return;
        }
        base.OnDie();
        playerAnimator.SetTrigger("Die");
        playerAudioSource.PlayOneShot(playerDeathClip);
        playerMovement.enabled = false;
        playerShot.enabled = false;
        //gameover ui 업데이트
    }
    //public virtual void OnHeal(float add)
    //{
    //    base.OnHeal(add);
    //    //ui 체력바 업데이트
    //}
}

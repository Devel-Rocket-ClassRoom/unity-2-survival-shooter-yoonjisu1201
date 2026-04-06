using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public AudioClip playerDeathClip;
    public AudioClip playerHurtClip;
    public GameObject gameOverUI;
    public GameObject SoreText;
    public Slider healthSlider;

    private Animator playerAnimator;
    private AudioSource playerAudioSource;
    private PlayerMovement playerMovement;
    private PlayerShot playerShot;
    private Rigidbody rb;

    private void Awake()
    {
        startingHealth = 200f;
        currentHealth = startingHealth;

        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShot = GetComponent<PlayerShot>();
        rb = GetComponent<Rigidbody>();
    }
 
    protected override void OnEnable()
    {
        base.OnEnable();
        healthSlider.gameObject.SetActive(true);
        healthSlider.value = currentHealth / startingHealth;
        SoreText.SetActive(true);
        gameOverUI.SetActive(false);
        OnDead.AddListener(HandlePlayerDeath);

        playerMovement.enabled = true;
        playerShot.enabled = true;
    }
    public void OnDisable()
    {
        OnDead.RemoveListener(HandlePlayerDeath);
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        playerAudioSource.PlayOneShot(playerHurtClip);
        healthSlider.value = currentHealth / startingHealth;
    }


    public override void OnDie()
    {
        if (isDead)
        {
            return;
        }
        base.OnDie();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        StartCoroutine(RestartSceneAfterDelay(3f));
    }
    
    public void HandlePlayerDeath()
    {
        playerAnimator.SetTrigger("Die");
        playerAudioSource.PlayOneShot(playerDeathClip);

        playerMovement.enabled = false;
        playerShot.enabled = false;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        healthSlider.gameObject.SetActive(false);
    }

    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

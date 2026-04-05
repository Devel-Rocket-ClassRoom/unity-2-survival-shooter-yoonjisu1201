using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth { get; protected set; } 
    public float currentHealth { get; protected set; }
    public bool isDead {  get; protected set; }
    public UnityEvent OnDead;

    protected virtual void OnEnable()
    {
        isDead = false;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDie();
        }
    }
    public virtual void OnDie()
    {
        isDead = true;
        OnDead?.Invoke();
    }
    //public virtual void OnHeal(float add)
    //{
    //    currentHealth += add;
    //    if (currentHealth > startingHealth )
    //    {
    //        currentHealth = startingHealth;
    //    }
    //}
}

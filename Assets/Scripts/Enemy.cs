using UnityEngine;

public class Enemy : LivingEntity
{

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDamege(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamege(damage, hitPoint, hitNormal);
    }
    public override void OnDie()
    {
        if (isDead)
        {
            return;
        }
        base.OnDie();
    }
}

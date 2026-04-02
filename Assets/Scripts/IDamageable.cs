using Unity.Collections;
using UnityEngine;

public interface IDamageable
{
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
 
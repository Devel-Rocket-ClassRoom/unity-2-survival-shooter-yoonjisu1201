using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    AudioClip deathClip;
    AudioClip hurtClip;

    public float maxHealth;
    public float damage;
    public float attackTimer = 0.5f;
}

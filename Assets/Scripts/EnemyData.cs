using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public AudioClip deathClip;
    public AudioClip hurtClip;

    public float maxHealth;
    public float damage;
    public float attackInterval = 0.5f;
    public float speed;
}

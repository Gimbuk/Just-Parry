using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public int damage = 25;

    private Collider hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false; // ±âº» OFF
    }

    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }
}

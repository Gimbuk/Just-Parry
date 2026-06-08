using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 20;

    private Collider hitCollider;

    void Awake()
    {
        hitCollider = GetComponent<Collider>();
        hitCollider.enabled = false; // Æò¼Ò¿£ ²¨µÒ
    }

    public void EnableHitbox()
    {
        hitCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}

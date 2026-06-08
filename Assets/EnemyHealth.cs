using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 50f;
    private float currentHP;

    Animator anim;
    NavMeshAgent agent;
    Collider col;

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
    }

    public void TakeDamage(float damage)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        anim.SetTrigger("Hit");

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");
        agent.enabled = false;
        col.enabled = false;
        Destroy(gameObject, 5f);
    }
}

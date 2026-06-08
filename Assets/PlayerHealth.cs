using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    Animator anim;

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        Debug.Log("Player HP: " + currentHP);

        anim?.SetTrigger("Hit");

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        anim?.SetTrigger("Die");
        // 여기서 이동 차단, 게임오버 처리 가능
    }
}

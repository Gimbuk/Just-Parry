using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 50f;
    public float attackDamage = 10f;
    public float detectionRange = 10f;
    public float attackRange = 1.6f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Transform attackPoint;

    float currentHP;
    float lastAttackTime;
    bool isAttacking;

    Transform player;
    NavMeshAgent agent;
    Animator anim;
    Collider col;

    void Awake()
    {
        currentHP = maxHP;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();

        // ⭐ 핵심
        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            anim.SetBool("IsMoving", false);
            return;
        }

        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            player = null;
            return;
        }

        if (distance > attackRange)
            Chase();
        else
            TryAttack();
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) return;

        if (Vector3.Distance(transform.position, p.transform.position) <= detectionRange)
            player = p.transform;
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        anim.SetBool("IsMoving", true);
        anim.SetBool("IsAttacking", false);
    }

    void TryAttack()
    {
        agent.isStopped = true;
        anim.SetBool("IsMoving", false);

        transform.LookAt(new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z));

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetBool("IsAttacking", true);
            lastAttackTime = Time.time;
            isAttacking = true;

            Invoke(nameof(EndAttack), 0.8f); // ⭐ 공격 애니 길이에 맞게
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", false);
    }

    // 🔥 애니메이션 이벤트
    public void AttackHit()
    {
        if (!player) return;

        float dist = Vector3.Distance(attackPoint.position, player.position);
        if (dist <= attackRange)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(attackDamage);
        }
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

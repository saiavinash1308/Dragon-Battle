using UnityEngine;

public class DragonAIController : MonoBehaviour
{
    public enum AggressionLevel { Passive, Normal, Aggressive, HyperAggressive }

    [Header("AI Settings")]
    public float detectionRange = 30f;
    public float attackRange = 10f;
    public float moveSpeed = 4f;
    public float turnSpeed = 80f;
    public float flySpeed = 6f;
    public float maxFlyHeight = 20f;
    public float attackCooldown = 2f;
    public bool isFlyingEnemy = false; 

    [Header("References")]
    public Transform player;
    public GameObject fireBreathPrefab;
    public Transform fireSpawnPoint;

    private Animator animator;
    private Rigidbody rb;
    private AggressionLevel aggression = AggressionLevel.Normal;
    private float nextAttackTime = 0f;
    private bool isFlying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (isFlyingEnemy)
        {
            isFlying = true;
            animator.SetBool("isFlying", true);
        }

        animator.Play("Idle");
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }

        AdjustFlyingState();
    }

    public void SetAggressionLevel(int level)
    {
        aggression = (AggressionLevel)level;

        switch (aggression)
        {
            case AggressionLevel.Passive:
                moveSpeed = 3f;
                attackCooldown = 3f;
                detectionRange = 20f;
                break;
            case AggressionLevel.Normal:
                moveSpeed = 5f;
                attackCooldown = 2f;
                detectionRange = 30f;
                break;
            case AggressionLevel.Aggressive:
                moveSpeed = 7f;
                attackCooldown = 1.5f;
                detectionRange = 40f;
                break;
            case AggressionLevel.HyperAggressive:
                moveSpeed = 10f;
                attackCooldown = 0.8f;
                detectionRange = 50f;
                break;
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent unwanted vertical movement

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if (!isFlying)
            animator.Play("Run Jump");
        else
            animator.Play("Fly Forward");
    }

    void AttackPlayer()
    {
        if (Time.time < nextAttackTime) return;

        int attackType = Random.Range(0, 3);
        float damage = 0f;

        if (attackType == 0)
        {
            FireAttack();
            damage = 15f;
        }
        else if (attackType == 1)
        {
            TailAttack();
            damage = 10f;
        }
        else
        {
            BiteAttack();
            damage = 20f;
        }

        player.GetComponent<HealthSystem>().TakeDamage(damage);
        nextAttackTime = Time.time + attackCooldown;
    }


    void FireAttack()
    {
        animator.Play("Attack FireBall");
        Instantiate(fireBreathPrefab, fireSpawnPoint.position, fireSpawnPoint.rotation);
    }

    void TailAttack()
    {
        animator.Play("Attack Tail");
    }

    void BiteAttack()
    {
        animator.Play("Attack Bite");
    }

    void Patrol()
    {
        animator.Play("Idle");
    }

    void AdjustFlyingState()
    {
        float playerHeight = player.position.y;
        float terrainHeight = GetTerrainHeight();

        if (isFlyingEnemy)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, terrainHeight + 5f, terrainHeight + maxFlyHeight), transform.position.z);
        }
        else
        {
            // If it's a ground enemy, determine if it should start flying
            if (playerHeight > terrainHeight + 2f && !isFlying)
            {
                isFlying = true;
                animator.SetBool("isFlying", true);
            }
            else if (playerHeight <= terrainHeight + 1.5f && isFlying)
            {
                isFlying = false;
                animator.SetBool("isFlying", false);
            }

            if (isFlying)
            {
                float targetHeight = Mathf.Clamp(playerHeight, terrainHeight + 1f, terrainHeight + maxFlyHeight);
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            }
        }
    }

    float GetTerrainHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return 0f;
    }
}

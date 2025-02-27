using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 100f;
    public float flySpeed = 7f;
    public float glideSpeed = 3f;
    public float maxFlyHeight = 20f;

    [Header("Attack Settings")]
    public GameObject fireBreathPrefab;
    public Transform fireSpawnPoint;
    public float attackCooldown = 2f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip fireAttackSound;
    public AudioClip tailAttackSound;
    public AudioClip groundAttackSound;
    public AudioClip flySound;
    public AudioClip landingSound;

    private Animator animator;
    private Rigidbody rb;
    public bool isFlying = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); 
        animator.Play("Idle");
        InvokeRepeating(nameof(HandleAttacks), 0f, 0.1f);
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFlying = !isFlying;
            animator.SetBool("isFlying", isFlying);

            if (isFlying && flySound)
                audioSource.PlayOneShot(flySound);
        }

        if (isFlying)
        {
            FlyMovement(move, turn);
        }
        else
        {
            WalkMovement(move, turn);
        }
    }

    void WalkMovement(float move, float turn)
    {
        Vector3 moveDirection = transform.forward * move * moveSpeed * Time.deltaTime;
        transform.position += moveDirection;
        transform.Rotate(0, turn, 0);

        if (move > 0.1f)
            animator.Play("Run Jump");
        else
            animator.Play("Idle");
    }

    void FlyMovement(float move, float turn)
    {
        Vector3 moveDirection = transform.forward * move * flySpeed * Time.deltaTime;

        float terrainHeight = GetTerrainHeight();
        float currentHeight = transform.position.y;
        float targetHeight = currentHeight;

        if (Input.GetKey(KeyCode.E))
        {
            animator.Play("Fly Glide");
            targetHeight -= glideSpeed * Time.deltaTime;
        }
        else if (move > 0.1f)
        {
            animator.Play("Fly Forward");
            targetHeight += flySpeed * Time.deltaTime;
        }
        else
        {
            animator.Play("Fly Idle");
            targetHeight += flySpeed * 0.5f * Time.deltaTime;
        }

        targetHeight = Mathf.Clamp(targetHeight, terrainHeight + 1f, terrainHeight + maxFlyHeight);

        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z) + moveDirection;
        transform.Rotate(0, turn, 0);

        if (targetHeight <= terrainHeight + 1.1f)
        {
            isFlying = false;
            animator.SetBool("isFlying", false);
            animator.Play("Idle");

            if (landingSound)
                audioSource.PlayOneShot(landingSound);
        }
    }

    void HandleAttacks()
    {
        if (Time.time < nextAttackTime) return;

        if (isFlying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireAttack();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                TailAttack();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.X))
            {
                GroundAttack();
            }
            else if (Input.GetKey(KeyCode.B))
            {
                GroundAttack2();
            }
        }
    }

    void FireAttack()
    {
        animator.Play("Attack FireBall");

        GameObject fire = Instantiate(fireBreathPrefab, fireSpawnPoint.position, fireSpawnPoint.rotation);
        fire.GetComponent<FireProjectile>().Initialize(20f, 5f);

        ParticleSystem fireEffect = fire.GetComponentInChildren<ParticleSystem>();
        if (fireEffect != null)
        {
            fireEffect.Play();
        }

        if (fireAttackSound)
            audioSource.PlayOneShot(fireAttackSound);
    }

    void TailAttack()
    {
        animator.Play("Attack Tail");
        DealMeleeDamage(10f);

        if (tailAttackSound)
            audioSource.PlayOneShot(tailAttackSound);
    }

    void GroundAttack()
    {
        animator.Play("Attack Paw L");
        DealMeleeDamage(15f);

        if (groundAttackSound)
            audioSource.PlayOneShot(groundAttackSound);
    }

    void GroundAttack2()
    {
        animator.Play("Attack Paw R");
        DealMeleeDamage(15f);

        if (groundAttackSound)
            audioSource.PlayOneShot(groundAttackSound);
    }

    void DealMeleeDamage(float damage)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 2f, 2f);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<HealthSystem>().TakeDamage(damage);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isFlying && collision.gameObject.CompareTag("Terrain"))
        {
            isFlying = false;
            animator.SetBool("isFlying", false);
            animator.Play("Idle");

            if (landingSound)
                audioSource.PlayOneShot(landingSound);
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

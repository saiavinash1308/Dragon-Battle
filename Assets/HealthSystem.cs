using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private Animator animator;
    private bool isDead = false;
    public Text damageText;

    [Header("Audio Settings")]
    public AudioSource audioSource;  
    public AudioClip damageSound;    

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP: " + currentHealth);

        if (damageText != null)
        {
            damageText.text = gameObject.name + " took " + damage + " damage. HP: " + currentHealth;
        }

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.Play("Stun");
        //Destroy(gameObject, 3f); 
    }
}

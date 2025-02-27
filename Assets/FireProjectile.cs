using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    private float damage;
    private float speed;

    public void Initialize(float dmg, float spd)
    {
        damage = dmg;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            Destroy(gameObject);
        }
    }
}


using UnityEngine;

public class TorpedoControl : MonoBehaviour
{

    public GameObject ExplosionPrefab;

    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor") || other.CompareTag("Player") || other.CompareTag("Torpedo"))
        {
            Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

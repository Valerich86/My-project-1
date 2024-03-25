using System;
using UnityEngine;

public class ExplosionControl : MonoBehaviour
{
    public AudioClip Explose;

    private AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.PlayOneShot(Explose);
        Destroy(gameObject, 1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Coin") || other.CompareTag("Heart") || other.CompareTag("Hourglass") || other.CompareTag("Jaws"))
        {
            other.GetComponent<Rigidbody>().AddExplosionForce(100f, transform.position, 3f);
        }
    }
}

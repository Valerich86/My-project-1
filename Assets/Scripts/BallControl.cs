
using System;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public PlaneControl Plane;
    public GameObject BallBloodPrefab;
    public ParticleSystem CoinEffect;
    public AudioClip PickCoin;
    public AudioClip Bonus;
    public event Action<int> CoinPickedUp;
    public event Action HourglassPickedUp;
    public event Action HeartPickedUp;
    public event Action ScorePickedUp;
    public event Action TorpedoAttack;

    private Vector3 _startPosition;
    private Vector3 _pos;
    private Quaternion _startRotation;
    private AudioSource _audioSource;
    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        Plane.BallFell += Respawn;
        _audioSource = GetComponent<AudioSource>();
        
    }

    void Update()
    {
        _pos = transform.position;
        if (_pos.y < -15 && Plane.StopGame == false)
        {
            Plane.TakeDamage();
            Respawn();
        } 
    }

    private void Respawn()
    {
        transform.position = _startPosition;  
        transform.rotation = _startRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            _audioSource.PlayOneShot(PickCoin);
            Instantiate(CoinEffect, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            CoinPickedUp?.Invoke(1);
            Plane.CoinsOnLevel.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Hourglass"))
        {
            _audioSource.PlayOneShot(Bonus);
            HourglassPickedUp?.Invoke();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Heart"))
        {
            _audioSource.PlayOneShot(Bonus);
            HeartPickedUp?.Invoke();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Score"))
        {
            _audioSource.PlayOneShot(PickCoin);
            ScorePickedUp?.Invoke();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Torpedo") && _pos.y < 5)
        {
            TorpedoAttack?.Invoke();
        }
        if (collision.gameObject.CompareTag("Jaws"))
        {
            Instantiate(BallBloodPrefab, transform.position, transform.rotation);
            Plane.TakeDamage();
        }
    }
}

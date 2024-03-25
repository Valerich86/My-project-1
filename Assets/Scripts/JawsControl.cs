using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JawsControl : MonoBehaviour
{
    private GameObject _target;
    private NavMeshAgent _agent;
    private Vector3 _position;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player");
        _agent.enabled = false;
        StartCoroutine(Activation());
    }

    void Update()
    {
        _position = transform.position;
        if (_position.y < - 5) Destroy(gameObject);
        if(_target != null && _agent.enabled)
        {
            _agent.SetDestination(_target.transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin") || collision.gameObject.CompareTag("Hourglass") || collision.gameObject.CompareTag("Heart"))
        {
            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(500, transform.position, 3f);
        }
    }

    private IEnumerator Activation()
    {
        yield return new WaitForSeconds(5f);
        _agent.enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}


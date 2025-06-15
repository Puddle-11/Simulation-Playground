using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(SphereCollider))]
public class Boid : MonoBehaviour
{
    public List<Boid> Flock = new List<Boid>();
    private SphereCollider sp;
    [SerializeField] private float flockRadius;
    private void Awake()
    {
        sp = GetComponent<SphereCollider>();
        sp.radius = flockRadius;
        sp.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Boid BRef))
        {
            Flock.Add(BRef);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Boid BRef))
        {
            Flock.Remove(BRef);
        }
    }

}

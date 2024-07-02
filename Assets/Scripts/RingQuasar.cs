using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingQuasar : MonoBehaviour
{
    [HideInInspector] private SystemManager manager;
    public GameObject PlanetPrefab;
    public Transform Parent;
    [Range(5, 100)]
    [SerializeField] private  float radius = 15f;
    [SerializeField] private float spawnRate;
    private float Stimer;
    private void Awake()
    {
        manager = GetComponentInParent<SystemManager>();
    }

    private void Update()
    {
        spawnRate = Mathf.Clamp(spawnRate, 0, Mathf.Infinity);
        if(Stimer <= 0)
        {
            Spawn();
            Stimer = spawnRate;
        }
        else
        {
            Stimer -= Time.deltaTime;

        }
    }
    private void Spawn()
    {
        float sample = Random.Range(0f, Mathf.PI*2);
        GameObject x = Instantiate(PlanetPrefab, new Vector3(Mathf.Sin(sample), 0, Mathf.Cos(sample)) * radius + transform.position, Quaternion.identity, Parent);
        Gravity scrRef;
        x.TryGetComponent<Gravity>(out scrRef);

        if(scrRef != null)
        {
            scrRef.StartVelocity = Vector3.up;
        }
        if (manager != null) manager.updateBodies();
    }




}

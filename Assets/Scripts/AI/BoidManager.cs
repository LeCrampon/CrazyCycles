using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager _instance;
    public List<GameObject> _boids = new List<GameObject>();

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
    }

    public List<GameObject> GetClosestBoids(GameObject center, float range)
    {
        List<GameObject> closestBoids = new List<GameObject>();
        foreach(GameObject boid in _boids)
        {
            //On ignore soit même
            if(boid == center)
            {
                continue;
            }

            if(Vector3.Distance(boid.transform.position, center.transform.position) <= range)
            {
                closestBoids.Add(boid);
            }
        }
        return closestBoids;
    }

}

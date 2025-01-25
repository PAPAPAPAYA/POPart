using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornScript : MonoBehaviour
{
    public float spd;
    public float lifespan;
    private float lifeSpanTimer;
    public float destroyRadius;

    private void Start()
    {
        transform.Rotate(0f, 0f, Random.Range(0f, 360f));
        lifeSpanTimer = lifespan;
    }
    private void Update()
    {
        transform.position += spd * Time.deltaTime * transform.right;

        // self destroy if lifespan reached
        if (lifeSpanTimer > 0)
        {
            lifeSpanTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        // destroy bubble with similar x & y
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            if (bs.active &&
                Vector2.Distance (bubble.transform.position, transform.position) <= destroyRadius)
            {
                bubble.GetComponentInChildren<BubbleScript>().hp--;
            }
        }
    }
}

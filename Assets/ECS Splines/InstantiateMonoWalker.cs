using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMonoWalker : MonoBehaviour
{
    public GameObject prefabdude;

    private const float TICK_TIMER_MAX = 0.5f;

    private int tick;
    private float tickTimer;
    // Start is called before the first frame update
    void Start()
    {
        tick = 0;
        tickTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= TICK_TIMER_MAX)
        {

            //Instantiate(prefabdude, new Vector3(0, 0, 0), Quaternion.identity);
            tickTimer -= TICK_TIMER_MAX;
            tick++;
        }
    }
}


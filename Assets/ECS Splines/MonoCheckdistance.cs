using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCheckdistance : MonoBehaviour
{
    public GameObject firstpoint;
    public GameObject secondpoint;

    // Start is called before the first frame update
    void Start()
    {
        var Temperval = Vector3.Distance(firstpoint.transform.position, secondpoint.transform.position);

        //Debug.Log("The mono distance is " + Temperval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

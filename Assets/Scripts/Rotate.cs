using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject parent;
    public float speed = 10f;
    private Vector3 axis;

    // This script is used to rotate all the bodies in the game 
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
    }
    void Update()
    {

        transform.RotateAround(parent.transform.position, parent.transform.up, speed * Time.deltaTime);
    }

}

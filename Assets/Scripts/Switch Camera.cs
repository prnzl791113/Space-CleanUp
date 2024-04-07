using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    // This class performs the switch when back camera button is pressed 
    public GameObject cam1;
    public GameObject cam2;
    private void Start()
    {
        cam2.SetActive(false);
        cam1.SetActive(true);
    }

    public void manager()
    {

        Cam_1();
    }


    void Cam_1()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
    }

    public void Cam_2()
    {
        cam1.SetActive(false);
        cam2.SetActive(true);
    }
}

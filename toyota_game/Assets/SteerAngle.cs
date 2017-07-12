using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class SteerAngle : MonoBehaviour
{

    public float steerAngle = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        steerAngle = CrossPlatformInputManager.GetAxis("Horizontal");
        this.GetComponent<TextMesh>().text = steerAngle.ToString();
    }


}

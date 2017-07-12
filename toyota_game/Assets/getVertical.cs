using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class getVertical : MonoBehaviour
{

    public float Vertical = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vertical = CrossPlatformInputManager.GetAxis("Vertical");
        this.GetComponent<TextMesh>().text = Vertical.ToString();
    }


}

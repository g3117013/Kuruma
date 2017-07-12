using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.IO;



public class CSVReader : MonoBehaviour {

    public string guide = "";
    public string[] GuideArray;
    public string[] SteerAndPos;
    public List<Vector3> SteerVec3 = new List<Vector3>();
    public List<Vector3> PosVec3 = new List<Vector3>();

    private string guidefile = "7_12_2017_4_54_36_PM";

    // Use this for initialization
    void Start () {
        TextAsset SteerGuide = Resources.Load(guidefile) as TextAsset;
        // public string Guide[] = SteerGuide.Split(',');

        guide =SteerGuide.text;
        SteerAndPos = guide.Split('?');

        Debug.Log("SteerAndPos.Length = " + SteerAndPos.Length);
        Debug.Log("Read Otehon Data.........");

        for (int i = 0; i < SteerAndPos.Length-1; i++)
        {
            string[] tmp = SteerAndPos[i].Split(':');
            string[] tmpVec3 = tmp[0].Split(',');
            string[] tmpPos3 = tmp[1].Split(',');

            Debug.Log(float.Parse(tmpVec3[0]) + "," + float.Parse(tmpVec3[1]) + "," + float.Parse(tmpVec3[2]));
            Vector3 sv = new Vector3(float.Parse(tmpVec3[0]), float.Parse(tmpVec3[1]), float.Parse(tmpVec3[2]));
            SteerVec3.Add(sv);

            Vector3 pv = new Vector3(float.Parse(tmpPos3[0]), float.Parse(tmpPos3[1]), float.Parse(tmpPos3[2]));
            PosVec3.Add(pv);
        }



        GuideArray = guide.Split(',');

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

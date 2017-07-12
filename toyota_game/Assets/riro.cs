using UnityEngine;

public class riro : MonoBehaviour {
	    public KeyCode debugKey = KeyCode.S;

     void Update() {

        if (Input.GetKeyDown(this.debugKey)) {
			

            Application.LoadLevel("Car"); // シーンの名前かインデックスを指定
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	// Use this for initialization
	public float speed = 1.5f;
	public int unitID;
	public bool isPlayersUnit;
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isPlayersUnit){
			Vector3 pos = transform.position;

			if (Input.GetKey ("w")) {
				pos.z += speed * Time.deltaTime;
			}
			if (Input.GetKey ("s")) {
				pos.z -= speed * Time.deltaTime;
			}
			if (Input.GetKey ("d")) {
				pos.x += speed * Time.deltaTime;
			}
			if (Input.GetKey ("a")) {
				pos.x -= speed * Time.deltaTime;
			}

			transform.position = pos;

			if(Input.anyKey){
				PlayerControls.client.Send("Moving|" + unitID + "|" + pos.x + "|" + pos.y + "|" + pos.z + "|");
			}
		}
	}
}

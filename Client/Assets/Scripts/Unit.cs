using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    public float speed = 10.0f;
    public int unitID;
    public bool isPlayersUnit;

    void Update()
    {
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

    public void MoveTo(Vector3 pos)
    {
        transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
    }
}

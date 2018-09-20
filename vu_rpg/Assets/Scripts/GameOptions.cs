using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour {

    public GameObject chatButton;
    public GameObject mapButton;
    public GameObject exitButton;

	// Update is called once per frame
	void Update () {
	    Player player = Utils.ClientLocalPlayer();
	    chatButton.SetActive(player != null);
	    mapButton.SetActive(player != null);
	    exitButton.SetActive(player != null);
    }
}

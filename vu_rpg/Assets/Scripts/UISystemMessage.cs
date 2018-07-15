using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemMessage : MonoBehaviour {

    public GameObject message;

    public void NewTextAndDisplay(string text) {
        message.GetComponent<Text>().text = text;
        message.SetActive(true);
        StartCoroutine(Display());
    }

    public IEnumerator Display() {
        yield return new WaitForSeconds(2.5f);
        message.SetActive(false);
    }
}

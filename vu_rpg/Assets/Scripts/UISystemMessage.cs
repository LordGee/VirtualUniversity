using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemMessage : MonoBehaviour {

    public Text message;

    public void NewTextAndDisplay(string text) {
        message.text = text;
        this.gameObject.SetActive(true);
        StartCoroutine("Display");
    }

    public IEnumerable Display() {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }
}

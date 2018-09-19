using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Help function to display on screen messages
/// </summary>
public class UISystemMessage : MonoBehaviour {

    public GameObject message;

    /// <summary>
    /// Sets up the message game object with the message
    /// Starts co-routine to display.
    /// </summary>
    /// <param name="text">Text Message to Display</param>
    public void NewTextAndDisplay(string text) {
        message.GetComponent<Text>().text = text;
        message.SetActive(true);
        StartCoroutine(Display());
    }

    /// <summary>
    /// Returns the message game object to NOT active after a short period of time
    /// </summary>
    /// <returns>Waits 2.5 seconds</returns>
    public IEnumerator Display() {
        yield return new WaitForSeconds(2.5f);
        message.SetActive(false);
    }
}

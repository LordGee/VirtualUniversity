using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour {

    public Text welcomeMessage;
    public Text Score1;
    public Text Score2;
    public Text Score3;

    [System.Serializable]
    public struct ButtonPlayerPrefs {
        public GameObject gameObject;
        public string playerPrefsKey;
    }

    public ButtonPlayerPrefs[] buttons;

    void Awake() {
        if (PlayerPrefs.GetInt("PlayerID", 0) == 0 || PlayerPrefs.GetString("PlayerName", "") == "") {
            SceneManager.LoadScene("LoginScene");
        }
        welcomeMessage.text = "Welcome " + PlayerPrefs.GetString("PlayerName");
    }

    void Start() {
        for (int i = 0; i < buttons.Length; i++) {
            int score = PlayerPrefs.GetInt(buttons[i].playerPrefsKey, 0);
            for (int starIndex = 1; starIndex <= 3; starIndex++) {
                Transform star = buttons[i].gameObject.transform.Find("Star" + starIndex);
                if (starIndex <= score) {
                    star.gameObject.SetActive(true);
                } else {
                    star.gameObject.SetActive(false);
                }
            }
        }
    }
 
    public void OneButtonPress(string levelName) {
        SceneManager.LoadScene(levelName);
    }

    public void SignOut() {
        PlayerPrefs.SetInt("PlayerID", 0);
        PlayerPrefs.SetString("PlayerName", "");
        SceneManager.LoadScene("LoginScene");
    }

}

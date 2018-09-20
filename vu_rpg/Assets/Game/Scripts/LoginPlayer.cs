using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPlayer : MonoBehaviour {

    public Text emailAddress;
    public GameObject password;

    public Text errorMessage;

    public GameObject registerPanel;

    public void RegisterNewPlayer() {
        registerPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void LoginPlayerCheck() {
        string pw = password.GetComponent<InputField>().text; // UtilityScript.EncryptPassword(password.text);

        GetComponent<DB_GetPlayer>().GetPlayer(emailAddress.text, pw);
    }

    public void ReportLoginError() {
        errorMessage.text = "There was an issue retrieving your details please try again";
        emailAddress.text = "";
        password.GetComponent<InputField>().text = "";
    }

    public void LoginSuccessful() {
        if (PlayerPrefs.GetInt("PlayerID", 0) != 0) {
            Debug.Log(PlayerPrefs.GetInt("PlayerID"));
            SceneManager.LoadScene("LevelSelect");
        }
    }
}

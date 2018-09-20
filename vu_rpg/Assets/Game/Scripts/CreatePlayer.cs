using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayer : MonoBehaviour {

    public Text displayName;
    public Text emailAddress;
    public GameObject password;
    public GameObject password2;
    public Text errorMessage;

    public GameObject loginPanel;

    private string message;

    public void CheckForm() {
        message = "";
        errorMessage.color = Color.red;
        if (displayName.text.Length < 3) {
            message += "bad display name (must be at least 3 characters long) ";
        }
        if (!emailAddress.text.Contains("@") || !emailAddress.text.Contains(".") || emailAddress.text.Length < 7) {
            message += "bad email address (must be a valid email address) ";
        }
        if (password.GetComponent<InputField>().text.Length < 8) {
            message += "bad password (minimum length = 7) ";
        }
        if (password.GetComponent<InputField>().text != password2.GetComponent<InputField>().text) {
            message += "bad passwords (Don't match) ";
        }
        if (message != "") {
            errorMessage.text = message;
        } else {
            errorMessage.color = Color.green;
            errorMessage.text = "All GOOD!";

            string result = password.GetComponent<InputField>().text; //  UtilityScript.EncryptPassword(password.text);
            
            GetComponent<DB_AddPlayer>().InsertNewPlayer(displayName.text, emailAddress.text, result);
        }
        
    }

    public void GoBack() {
        loginPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }


}

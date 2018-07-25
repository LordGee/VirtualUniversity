using System;
using System.Collections.Generic;
using UnityEngine;

public partial class UILogin : MonoBehaviour {

    public GameObject nextButton;
    public GameObject backButton;

    private string username = "", password = "";
    private int server = 0;

    public enum UIState {
        USERNAME,
        PASSWORD,
        SERVER
    };

    private UIState currentState;

    private void InitStart() {
        currentState = UIState.USERNAME;
        statusText.text = "Login - Enter Username";
    }

    public void NextButtonChecked() {
        if (currentState == UIState.USERNAME) {
            username = accountInput.text;
            accountInput.gameObject.SetActive(false);
            passwordInput.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
            currentState = UIState.PASSWORD;
            statusText.text = "Enter Password";
        } else if (currentState == UIState.PASSWORD) {
            password = passwordInput.text;
            passwordInput.gameObject.SetActive(false);
            serverDropdown.gameObject.SetActive(true);
            statusText.text = "Select Server";
            currentState = UIState.SERVER;
            nextButton.gameObject.SetActive(false);
            hostButton.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
        } else if (currentState == UIState.SERVER) {
            server = serverDropdown.value;
        }
    }

    public void BackButtonChecked() {
        accountInput.gameObject.SetActive(true);
        passwordInput.gameObject.SetActive(false);
        serverDropdown.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);
        currentState = UIState.USERNAME;
    }
}

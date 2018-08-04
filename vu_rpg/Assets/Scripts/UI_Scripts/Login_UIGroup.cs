using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILogin : MonoBehaviour {

    public GameObject nextButton;
    public GameObject backButton;
    public GameObject regButton;
    public Dropdown courseDropdown;
    public Dropdown characterDropdown;

    private string username = "", password = "", course = "";
    private int server = 0;
    private bool registration;

    public enum UIState {
        USERNAME,
        PASSWORD,
        SERVER,
        COURSE,
        CHARACTER,
        COUNT

    };

    private UIState currentState;

    private void InitStart() {
        currentState = UIState.USERNAME;
        registration = false;
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

    private void ProceedRegistration() {
        if (accountInput.text != "") {
            currentState = UIState.USERNAME;
            registration = true;
            RegNextButtonChecked();
        } else {
            statusText.text = "Registration - Enter Desired Username";
        }
    }

    public void RegNextButtonChecked() {
        if (currentState == UIState.USERNAME) {
            username = accountInput.text;
            accountInput.gameObject.SetActive(false);
            passwordInput.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(false);
            regButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
            currentState = UIState.PASSWORD;
            statusText.text = "Enter Desired Password";
        } else if (currentState == UIState.PASSWORD) {
            password = passwordInput.text;
            passwordInput.gameObject.SetActive(false);
            courseDropdown.gameObject.SetActive(true);
            statusText.text = "Select Your Course";
            currentState = UIState.COURSE;
            List<String> content = Database.GetCourseNames();
            PopulateDropbox.Run(ref courseDropdown, content, "Select your course");
        } else if (currentState == UIState.COURSE) {
            course = courseDropdown.options[courseDropdown.value].text;
            courseDropdown.gameObject.SetActive(false);
            serverDropdown.gameObject.SetActive(true);
            statusText.text = "Select Server";
            currentState = UIState.SERVER;
            hostButton.gameObject.SetActive(true);
        } else if (currentState ==  UIState.SERVER) {
            manager.StartClient();
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

    private void Activate(GameObject game, bool active) { game.SetActive(active); }
}

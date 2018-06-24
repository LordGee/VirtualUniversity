using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreateQuestion : MonoBehaviour {

    public NetworkManagerMMO manager;
    public GameObject panel;
    public Text question;
    public Text[] answers;
    public Toggle[] results;

	void Start () {
		
	}

	void Update () {
		
	}

    public void AddQuestion() {
        string[] answers = {this.answers[0].text, this.answers[1].text, this.answers[2].text};
        int result = -1;
        for (int i = 0; i < results.Length; i++) {
            if (results[i].isOn) {
                result = i;
            }
        }
        if (result == -1) {
            Debug.LogWarning("No correct answer");
            return;
        } 
        
        Database.AddNewQuestionAndAnswer(question.text, answers, result);
        
    }

    public void Hide() { panel.SetActive(false); }
    public void Show() { panel.SetActive(true); }
    public bool IsVisible() { return panel.activeSelf; }
}

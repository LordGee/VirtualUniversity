using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DB_GetHighScores : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_GET_HIGH_SCORE = "api_code=get_high";

    [System.Serializable]
    public class Results {
        public List<ResultData> results;
    }

    [System.Serializable]
    public class ResultData {
        public int result_id;
        public int score;
        public int fk_user_id;
        public int fk_level_id;
        public string display_name;
    }

    private Results result;

    public Text[] scoreText;
    public GameObject scorePanel;
    public Text headingText;

    public struct Scores {
        public int score;
        public string name;
    };

    private List<Scores> scores = new List<Scores>();

    public void GetScoresForLevel(int level) {
        scorePanel.SetActive(true);
        headingText.text = "Top Scores - Level " + level;
        StartCoroutine(GetHighScores(level));
    }

    public void ClosePanel() {
        ClearText();
        headingText.text = "Loading please wait...";
        scorePanel.SetActive(false);
        scores.Clear();
        result = new Results();
    }

    private IEnumerator GetHighScores(int level) {
        string uri = API_URL + API_GET_HIGH_SCORE + "&level_id=" + level;

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            result = JsonUtility.FromJson<Results>("{\"results\": " + www.downloadHandler.text + "}");
            UpdateResults();
        }
    }

    private void UpdateResults() {
        for (int i = 0; i < result.results.Count; i++) {
            if (scores.Count > 0) {
                bool playerExists = false;
                for (int j = 0; j < scores.Count; j++) {
                    if (scores[j].name == result.results[i].display_name) {
                        playerExists = true;
                    }
                }
                if (!playerExists) {
                    string playerName = result.results[i].display_name;
                    int tempScore = 0;
                    for (int k = 0; k < result.results.Count; k++) {
                        if (result.results[k].display_name == playerName) {
                            if (result.results[k].score > tempScore) {
                                tempScore = result.results[k].score;
                            }
                        }
                    }
                    Scores temp;
                    temp.score = tempScore;
                    temp.name = playerName;
                    scores.Add(temp);
                }
            } else {
                string playerName = result.results[i].display_name;
                int    tempScore  = 0;
                for (int k = 0; k < result.results.Count; k++) {
                    if (result.results[k].display_name == playerName) {
                        if (result.results[k].score > tempScore) {
                            tempScore = result.results[k].score;
                        }
                    }
                }
                Scores temp;
                temp.score = tempScore;
                temp.name = playerName;
                scores.Add(temp);
            }
        }
        UpdateText();
    }

    private void UpdateText() {
        for (int i = 0; i < scoreText.Length; i++) {
            string temp = (i + 1) + ". ";
            if (i < scores.Count) {
                temp += scores[i].name + "\t " + scores[i].score.ToString();
            }
            scoreText[i].text = temp;
        }
    }

    private void ClearText() {
        for (int i = 0; i < scoreText.Length; i++) {
            scoreText[i].text = (i + 1) + ". ";
        }
    }
}

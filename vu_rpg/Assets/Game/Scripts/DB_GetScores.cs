using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DB_GetScores : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";

    private const string API_GET_SCORE = "api_code=get_best";

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
    }

    private Results result;

    void Start() {
        GetScoresForPlayer(PlayerPrefs.GetInt("PlayerID"));
    }

    public void GetScoresForPlayer(int player) {
        StartCoroutine(GetScores(player));
    }

    private IEnumerator GetScores(int player) {
        //string uri = API_URL + API_GET_SCORE + "&level_id=" + level;
        string uri = API_URL + API_GET_SCORE + "&user_id=" + player;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            result = JsonUtility.FromJson<Results>("{\"results\": " + www.downloadHandler.text + "}");
            UpdateLevelResults();
        }
    }

    private void UpdateLevelResults() {
        int[] score = { 0, 0, 0 };
        for (int l = 1; l <= 3; l++) {
            int tempScore = 0;
            for (int i = 0; i < result.results.Count; i++) {
                if (result.results[i].fk_level_id == l) {
                    if (result.results[i].score > tempScore) {
                        tempScore = result.results[i].score;
                    }
                }
            }
            score[l - 1] = tempScore;
        }
        GetComponent<SelectLevel>().Score1.text = score[0].ToString();
        GetComponent<SelectLevel>().Score2.text = score[1].ToString();
        GetComponent<SelectLevel>().Score3.text = score[2].ToString();
    }
}

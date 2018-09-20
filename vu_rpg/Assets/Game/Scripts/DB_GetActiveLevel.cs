using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DB_GetActiveLevel : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";

    private const string API_GET_AVALIABLE_LEVELS = "api_code=get_available_levels";

    [System.Serializable]
    public class Results {
        public List<ResultData> results;
    }

    [System.Serializable]
    public class ResultData {
        public int season_id;
        public int level;
    }

    private Results result;

    void Start() {
        GetAvailableLevels();
    }

    public void GetAvailableLevels() {
        StartCoroutine(GetLevels());
    }

    private IEnumerator GetLevels() {
        //string uri = API_URL + API_GET_SCORE + "&level_id=" + level;
        string uri = API_URL + API_GET_AVALIABLE_LEVELS;
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
        for (int l = 1; l <= 3; l++) {
            bool test = false;
            GameObject thisButton = GetComponent<SelectLevel>().buttons[l - 1].gameObject;
            for (int i = 0; i < result.results.Count; i++) {
                if (result.results[i].level == l) {
                     test = true;
                }
            }
            thisButton.GetComponent<Button>().enabled = test;
        }
    }
}

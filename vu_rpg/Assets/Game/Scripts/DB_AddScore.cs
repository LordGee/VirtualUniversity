using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DB_AddScore : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_ADD_SCORE = "api_code=add_score";

    public void InsertNewScore(int score, int user, int level) {
        StartCoroutine(InsertScore(score, PlayerPrefs.GetInt("PlayerID"), level));
    }

    private IEnumerator InsertScore(int score, int user, int level) {
        string uri = API_URL + API_ADD_SCORE + "&score=" + score + "&user_id=" + user + "&level_id=" + level;

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } 
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DB_AddStats : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_ADD_STATS = "api_code=add_stats";

    public void InsertNewStats(int move, int three, int four, int five, int score) {
        StartCoroutine(InsertStats(PlayerPrefs.GetInt("PlayerID"), move, three, four, five, score));
    }

    private IEnumerator InsertStats(int player, int move, int three, int four, int five, int score) {
        string uri = API_URL + API_ADD_STATS + "&player=" + player + "&move=" + move + "&three=" + three + "&four=" + four + "&five=" + five + "&score=" + score;

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
    }
}

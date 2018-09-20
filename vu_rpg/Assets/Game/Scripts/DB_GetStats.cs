using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DB_GetStats : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_GET_STATS = "api_code=get_stats";

    public GameObject statsPanel;

    public Text playedText;
    public Text movesText;
    public Text threeText;
    public Text fourText;
    public Text fiveText;
    public Text scoreText;

    [System.Serializable]
    public class Statistics {
        public List<StatisticData> statistics;
    }

    [System.Serializable]
    public class StatisticData {
        public int played;
        public int moves;
        public int three;
        public int four;
        public int five;
        public int score;
        public int statistic_id;
        public int fk_player_id;
    }

    private Statistics stats;

    void Start() {
        
    }

    public void GetStatisticsForPlayer() {
        StartCoroutine(GetStatistics(PlayerPrefs.GetInt("PlayerID")));
    }

    private IEnumerator GetStatistics(int player) {
        string uri = API_URL + API_GET_STATS + "&user_id=" + player;
        Debug.Log(uri);

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            stats = JsonUtility.FromJson<Statistics>("{\"statistics\": " + www.downloadHandler.text + "}");
            UpdateStatisticView();
        }
    }

    private void UpdateStatisticView() {
        scoreText.text = stats.statistics[0].score.ToString();
        movesText.text = stats.statistics[0].moves.ToString();
        threeText.text = stats.statistics[0].three.ToString();
        fourText.text = stats.statistics[0].four.ToString();
        fiveText.text = stats.statistics[0].five.ToString();
        playedText.text = stats.statistics[0].played.ToString();
    }

    public void OpenPanel() {
        GetStatisticsForPlayer();
        statsPanel.SetActive(true);
    }

    public void ClosePanel() {
        statsPanel.SetActive(false);
    }
}

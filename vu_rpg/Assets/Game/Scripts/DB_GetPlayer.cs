using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DB_GetPlayer : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_GET_PLAYER = "api_code=get_player";

    [System.Serializable]
    public class Players {
        public List<PlayerData> players;
    }

    [System.Serializable]
    public class PlayerData {
        public int player_id;
        public string display_name;
    }

    private int player_id;

    public void GetPlayer(string email, string password) {
        StartCoroutine(AccessPlayer(email, password));
    }

    private IEnumerator AccessPlayer(string email, string password) {
        string uri = API_URL + API_GET_PLAYER + "&email=" + email + "&pw=" + password;
        Debug.Log(uri);
        UnityWebRequest www = UnityWebRequest.Get(uri);
        
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else {
            if (www.downloadHandler.text == "error") {
                GetComponent<LoginPlayer>().ReportLoginError();
            }
            else {
                Players player = JsonUtility.FromJson<Players>("{\"players\": " + www.downloadHandler.text + "}");

                Debug.Log(player.players[0].player_id + " " + player.players[0].display_name);
                int playerID = player.players[0].player_id;
                PlayerPrefs.SetInt("PlayerID", playerID);
                PlayerPrefs.SetString("PlayerName", player.players[0].display_name);

                GetComponent<LoginPlayer>().LoginSuccessful();
            }
        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DB_AddPlayer : MonoBehaviour {

    private const string API_URL = "https://summer.mychaos.co.uk/api/freeme_api.php?";
    private const string API_ADD_PLAYER = "api_code=add_player";

    private int player_id;

    public void InsertNewPlayer(string name, string email, string password) {
        StartCoroutine(InsertPlayer(name, email, password));
    }

    private IEnumerator InsertPlayer(string name, string email, string password) {
        string uri = API_URL + API_ADD_PLAYER + "&display=" + name + "&email=" + email + "&pw=" + password;

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            PlayerPrefs.SetInt("PlayerID", int.Parse(www.downloadHandler.text));
            PlayerPrefs.SetString("PlayerName", name);
            SceneManager.LoadScene("LevelSelect");
        }
    }
}

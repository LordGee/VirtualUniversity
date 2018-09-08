using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class Crud : MonoBehaviour {

    [System.Serializable]
    public class JsonResult {
        public ModelQuestion[] json_result;
    }

    [System.Serializable]
    public class ModelQuestion {
        public string question_id { get; set; }
        //public string question;
        //public int fk_quiz_id;
        //public int fk_break_id;
    }

    private JsonResult value;
    private string JsonString;

    public void DbCreate(string sql) {
        StartCoroutine(Create(sql));
    }

    private IEnumerator Create(string sql) {
        string          uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        }
    }

    public string DbRead(string sql) {
        JsonString = "";
        StartCoroutine(Read(sql));
        return JsonString;
    }


    private IEnumerator Read(string sql) {
        string          uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            string test = www.downloadHandler.text.Trim();
            JsonString = "{\"json_result\":" + test + "}";
            Debug.Log(JsonString);
            Debug.Log(test);
            value = JsonUtility.FromJson<JsonResult>(JsonString);

        }
    }
}

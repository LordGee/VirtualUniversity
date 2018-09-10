using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public partial class DatabaseCrud : MonoBehaviour {

    private static JsonResult value;
    private static string JsonString;

    public void DbCreate(string sql) {
        StartCoroutine(Create(sql));
    }

    private IEnumerator Create(string sql) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error + "\n" + sql);
        } else {
#if UNITY_EDITOR
            Debug.Log("Create Result: " + www.downloadHandler.text + " SQL: " + sql);
#endif
        }
    }

    public IEnumerator Read(string sql, string model) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            JsonString = ConvertJson(model, www.downloadHandler.text);
#if UNITY_EDITOR
            Debug.Log("JSON: " + JsonString + "\nSQL: " + sql);
#endif
            yield return JsonString;
        }
    }
 
    private string ConvertJson(string model, string json) {
        json.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work
        model.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work
        string result = "{\"" + model + "\":" + json.Trim() + "}";
        result.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work

        return RemoveBadChar(result);
    }

    /// <summary>
    /// Todo Ref: https://stackoverflow.com/questions/1317700/strip-byte-order-mark-from-string-in-c-sharp
    /// Removes a bad character from the string, the above trim methods do not work as presented in the ref
    /// This still gave me the idea of what to look for and manually stripe out the character.
    /// </summary>
    /// <param name="value">string value that may or may not have an issue</param>
    /// <returns>A fixed string with no \uFEFF char</returns>
    private string RemoveBadChar(string value) {
        char[] test = value.ToCharArray();
        int    badCharIndex = -1;
        for (int i = 0; i < test.Length; i++) {
            if (test[i] == '\uFEFF') {
                badCharIndex = i;
            }
        }
        StringBuilder newValue = new StringBuilder();
        if (badCharIndex != -1) {
            for (int i = 0; i < test.Length; i++) {
                if (badCharIndex != i) {
                    newValue.Append(test[i]);
                }
            }
        } else {
            newValue.Append(value);
        }

        return newValue.ToString();
    }

    private List<string> VarToStringList(List<string> value) {
        List<string> result = new List<string>();
        for (int i = 0; i < value.Count; i++) {
            result.Add(value[i]);
        }
        return result;
    }
}

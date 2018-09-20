using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This class is dedicated to the execution of the sql query's via the web api 
/// </summary>
public partial class DatabaseCrud : MonoBehaviour {

    // holds the converted json after conversion
    private static string jsonString;

    /// <summary>
    /// Public entry point for executing Create, Update or Delete queries
    /// These commands to not normally require a return value.
    /// </summary>
    /// <param name="sql">A prepared SQL statement</param>
    public void DbCreate(string sql) {
        StartCoroutine(Create(sql));
    }

    /// <summary>
    /// Prepares and execute the given SQL statement 
    /// </summary>
    /// <param name="sql">A prepared SQL statement</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator Create(string sql) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error + "\n" + sql);
        } else {
#if UNITY_EDITOR
            Debug.Log("Create Result: " + www.downloadHandler.text 
                                        + " SQL: " + sql);
#endif
        }
    }

    /// <summary>
    /// Dedicated to Read (SELECT) statements that will always return a value.
    /// In this instance a formated json string is returned which will require
    /// to be dealt with on an individual basis. 
    /// </summary>
    /// <param name="sql">A prepared SQL statement</param>
    /// <param name="model">the name of the model value for the required class</param>
    /// <returns>JSON String as an ASYNC Method</returns>
    public IEnumerator Read(string sql, string model) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            jsonString = ConvertJson(model, www.downloadHandler.text);
#if UNITY_EDITOR
            Debug.Log("JSON: " + jsonString + "\nSQL: " + sql);
#endif
            yield return jsonString;
        }
    }
 
    /// <summary>
    /// Prepares the Json string with the model as a prefix
    /// </summary>
    /// <param name="model">Reference to the variable name of the desired model</param>
    /// <param name="json">Un-converted json string</param>
    /// <returns>Returns the converted json string to be used with JsonUtility</returns>
    private string ConvertJson(string model, string json) {
        json.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work
        model.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work
        string result = "{\"" + model + "\":" + json.Trim() + "}";
        result.Trim(new char[] { '\uFEFF', '\u200B' }); // don't work
        return RemoveBadChar(result);
    }

    /// <summary>
    /// Ref: https://stackoverflow.com/questions/1317700/strip-byte-order-mark-from-string-in-c-sharp
    /// Removes a bad character from the string, the above trim methods do not work as presented in the ref
    /// This still gave me the idea of what to look for and manually stripe out the character.
    /// </summary>
    /// <param name="value">string value that may or may not have an issue</param>
    /// <returns>A fixed string with no \uFEFF char</returns>
    private string RemoveBadChar(string value) {
        char[] test = value.ToCharArray();
        int badCharIndex = -1;
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
}

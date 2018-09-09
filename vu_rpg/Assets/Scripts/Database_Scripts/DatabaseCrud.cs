using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;
using Task = UnityEditor.VersionControl.Task;

public partial class DatabaseCrud : MonoBehaviour {

    [Serializable]
    public class JsonResult {
        public List<ModelQuestion> questionResult;
        public List<ModelQuiz> quizResult;
        public List<ModelLecture> lectureResult;
        public List<ModelLectureBreak> lectureBreakResult;
        public List<ModelLectureAttend> lectureAttendResult;
        public List<ModelCourses> courseResult;

        //public JsonResult() {
        //    questionResult = new List<ModelQuestion>();
        //    quizResult = new List<ModelQuiz>();
        //    lectureResult = new List<ModelLecture>();
        //    lectureBreakResult = new List<ModelLectureBreak>();
        //    lectureAttendResult = new List<ModelLectureAttend>();
        //    courseResult = new List<ModelCourses>();
        //}
    }

    [Serializable]
    public class ModelQuestion {
        public int question_id;
        public string question;
        public int fk_quiz_id;
        public int fk_break_id;
    }

    [Serializable]
    public class ModelQuiz {
        public int quiz_id;
    }

    [Serializable]
    public class ModelLecture {
        public int lecture_id;
    }

    [Serializable]
    public class ModelLectureBreak {
        public int break_id;
    }

    [Serializable]
    public class ModelLectureAttend {
        public int attend_id;
    }

    [Serializable]
    public class ModelCourses {
        public string course_name;
    }

    private static JsonResult value;
    private static string JsonString;

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

    public void DbRead(string sql) {
        StartCoroutine(Read(sql));
    }

    private IEnumerator Read(string sql) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            JsonString = ConvertJson("json_result", www.downloadHandler.text);
            Debug.Log(JsonString);
            value = JsonUtility.FromJson<JsonResult>(JsonString);
        }
    }

    public void UpdateID(string sql, int index, string model) {
        StartCoroutine(GetNewIDs(sql, index, model));        
    }
    private IEnumerator GetNewIDs(string sql, int index, string model) {
        string uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            JsonString = ConvertJson(model, www.downloadHandler.text);
            Debug.Log(JsonString);
            value = JsonUtility.FromJson<JsonResult>(JsonString);
            if (index == (int) Database.Table.Questions && value.questionResult.Count > 0) {
                Database.NextID[index] = value.questionResult[0].question_id + 1;
            } else if (index == (int) Database.Table.Quizzes && value.quizResult.Count > 0) {
                Database.NextID[index] = value.quizResult[0].quiz_id + 1;
            } else if (index == (int) Database.Table.Lectures && value.lectureResult.Count > 0) {
                Database.NextID[index] = value.lectureResult[0].lecture_id + 1;
            } else if (index == (int)Database.Table.LectureBreakPoints && value.lectureBreakResult.Count > 0) {
                Database.NextID[index] = value.lectureBreakResult[0].break_id + 1;
            } else if (index == (int)Database.Table.LectureAttend && value.lectureAttendResult.Count > 0) {
                Database.NextID[index] = value.lectureAttendResult[0].attend_id + 1;
            }
        }
    }

    private string stringResults;

    public async Task<List<string>> GetCourseNames_Go(string table, string key, string model) {
        string sql = "SELECT " + key + " FROM " + table + " ORDER BY " + key + " ASC";
        var newValue = await GetCourseNames_Callback(sql, model);
        //return VarToStringList((List<string>)newValue);
        return (List<string>)newValue;
    }

    private IEnumerator GetCourseNames_Callback(string sql, string model) {
        string          uri = _CONST.API_URL + sql;
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            JsonString = ConvertJson(model, www.downloadHandler.text);
            value = JsonUtility.FromJson<JsonResult>(JsonString);
            List<string> result = new List<string>();
            for (int i = 0; i < value.courseResult.Count; i++) {
                result.Add(value.courseResult[i].course_name);
            }
            yield return result;
        }
    }


    private string ConvertJson(string model, String json) {
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

/**
* GoogleTranslate.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 31/10/18 (dd/mm/yy)
* Revised on: 10/11/18 (dd/mm/yy)
*/
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We need this for parsing the JSON, unless you use an alternative.
// You will need SimpleJSON if you don't use alternatives.
// It can be gotten hither. http://wiki.unity3d.com/index.php/SimpleJSON
public class GoogleTranslate : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static GoogleTranslate Instance { get; private set; }
    /// <summary>
    /// Callback function for <see cref="TranslateProcess(string, string, FinishTranslate)"/>
    /// </summary>
    /// <param name="text">translated text</param>
    public delegate void FinishTranslate(string text);
    /// <summary>
    /// Callback function for <see cref="GetEnglishCodeTranslations(FinishGetEnglishCodes)"/>
    /// </summary>
    /// <param name="languages">list of supported languages</param>
    public delegate void FinishGetEnglishCodes(List<Language> languages);
    /// <summary>
    /// Key api of tech.yandex.com (translator)
    /// </summary>
    string keyApi = "trnsl.1.1.20181102T103438Z.232e4e5682fafc32.40033984454201403c21ace9691bbfd5da83a2e1";

    // This is only called when the scene loads.
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        // Strictly for debugging to test a few words!
    }

    /// <summary>
    /// Get all "en-" codes for supported languages (calling <see cref="GetEnglishCodeProcess(FinishTranslate)"/>)
    /// </summary>
    /// <param name="finishGetEnglishCodes">delegate for receive "en-" codes</param>
    public void GetEnglishCodeTranslations(FinishGetEnglishCodes finishGetEnglishCodes)
    {
        StartCoroutine(GetEnglishCodeProcess(finishGetEnglishCodes));
    }

    /// <summary>
    /// Calls <see cref="TranslateProcess(string, string, FinishTranslate)"/> to translate <paramref name="sourceText"/>
    /// </summary>
    /// <param name="targetLang">languages, format "from-to" (en-pt)</param>
    /// <param name="sourceText">text to translate</param>
    /// <param name="finishTranslate">callback function</param>
    /// <returns>translated text</returns>
    public void Translate(string targetLang, string sourceText, FinishTranslate finishTranslate)
    {
        StartCoroutine(TranslateProcess(targetLang, sourceText, finishTranslate));
    }

    /// <summary>
    /// Translate <paramref name="sourceText"/>
    /// </summary>
    /// <param name="targetLang">languages, format "from-to" (en-pt)</param>
    /// <param name="sourceText">text to translate</param>
    /// <param name="finishTranslate">callback function</param>
    /// <returns>translated text</returns>
    IEnumerator TranslateProcess(string targetLang, string sourceText, FinishTranslate finishTranslate)
    {
        /* old google translate
        string sourceLang = "auto";
        targetLang = "pt";
        string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
        + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);
        */
        string url = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=" +
            keyApi + "&lang=" + targetLang + "&text=" + WWW.EscapeURL(sourceText);
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                var TableReturned = JSONNode.Parse(www.text);
                sourceText = TableReturned[2][0];
                finishTranslate(sourceText);
            }
            else
            {
                print(www.error);
                finishTranslate(null);
            }
        }
    }

    IEnumerator GetEnglishCodeProcess(FinishGetEnglishCodes finishGetEnglishCodes)
    {
        string url = "https://translate.yandex.net/api/v1.5/tr.json/getLangs?key=" + keyApi + "&ui=en";
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                var json = JSONNode.Parse(www.text);
                List<Language> languages = new List<Language>();
                List<string> codes = new List<string>();
                int codesSize = json[0].Count;
                for (int i = 0; i < codesSize; i++)
                {
                    string code = json[0][i].Value;
                    if (code.StartsWith("en-"))
                    {
                        codes.Add(code);
                    }
                }
                string secondList = json[1].ToString();
                secondList = secondList.Replace(":", ",").Replace("{", "").Replace("}", "").Replace("\"", "");
                string[] list = secondList.Split(","[0]);
                int languagesSize = list.Length;
                for (int i = 0; i < languagesSize; i+=2)
                {
                    string key = list[i];
                    string value = list[i+1];
                    if (codes.Contains("en-" + key))
                    {
                        Language lang = new Language(key, value);
                        languages.Add(lang);
                    }
                }
                finishGetEnglishCodes(languages);
            }
            else
            {
                print(www.error);
                finishGetEnglishCodes(null);
            }
        }
    }
}

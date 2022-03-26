using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LanguageSelection : MonoBehaviour
{

    // Unser Language Button
    public Button languageButtonOben;
    public Button languageButtonUnten;
    private List<LanguageData> languageData = new List<LanguageData>();
    public int languageDataLength;
    private Information information;
    private int index = 0;

    void Start()
    {
        LoadResourceFiles();

        information = GameObject.Find("Information").GetComponent<Information>();
        nextLanguage();
    }

    public void nextLanguage()
    {
        LanguageData selected = languageData[index % languageDataLength];
        languageButtonUnten.GetComponent<Image>().sprite = selected.languageFlag;
        languageButtonOben.GetComponent<Image>().sprite = selected.languageFlag;
        information.UpdateLanguage(selected.languageFile);
        index++;
    }

    public void LoadResourceFiles()
    {
        string[] langs = { "de", "en" };
        foreach (string language in langs)
        {
            LanguageData lData = new LanguageData();
            lData.languageKey = language;
            TextAsset json = Resources.Load<TextAsset>("Languages/" + language);
            lData.languageFile = JsonUtility.FromJson<LanguageFile>(json.text);
            Texture2D flagTexture = Resources.Load<Texture2D>("Languages/" + language);
            lData.languageFlag = Sprite.Create(flagTexture, new Rect(0.0f, 0.0f, flagTexture.width, flagTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            languageData.Add(lData);
        }
        languageDataLength = languageData.Count;
    }

}

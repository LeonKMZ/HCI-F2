using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanguageData
{
    public string languageKey;
    public LanguageFile languageFile;
    public Sprite languageFlag;
}

[System.Serializable]
public class LanguageFile
{
    public string title;
    public string biography;
    public string history;
    public string writings;
    public string journeys;
    public YearData[] items;
}

[System.Serializable]
public class YearData
{
    public string year;
    public string biography;
    public string history;
    public string writings;
    public string journeys;
    public double[] waypoints;
}


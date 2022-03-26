using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Information : MonoBehaviour, YearUpdateListener
{
    public Text TitleTop;
    public Text TitleBottom;
    public Text DateTop;
    public Text DateBottom;
    public Text Biography;
    public Text BiographyInformation;
    public Text History;
    public Text HistoryInformation;
    public Text Writings;
    public Text WritingsInformation;
    public Text Journeys;
    public Text JourneysInformation;

    private static YearData placeholder;
    private static Dictionary<string, YearData> data = new Dictionary<string, YearData>();

    // Start is called before the first frame update
    void Start()
    {
        placeholder = new YearData();
        placeholder.biography = "";
        placeholder.history = "";
        placeholder.journeys = "";
        placeholder.writings = "";
        placeholder.waypoints = new double[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateYear(string year)
    {
        DateTop.text = year;
        DateBottom.text = year;
        YearData yearData = data.ContainsKey(year) ? data[year] : placeholder;

        BiographyInformation.text = yearData.biography;
        Biography.gameObject.SetActive(yearData.biography.Length != 0);
        HistoryInformation.text = yearData.history;
        History.gameObject.SetActive(yearData.history.Length != 0);
        WritingsInformation.text = yearData.writings;
        Writings.gameObject.SetActive(yearData.writings.Length != 0);
        JourneysInformation.text = yearData.journeys;
        Journeys.gameObject.SetActive(yearData.journeys.Length != 0);
    }

    public void UpdateLanguage(LanguageFile language)
    {
        TitleTop.text = language.title;
        TitleBottom.text = language.title;
        Biography.text = language.biography;
        History.text = language.history;
        Journeys.text = language.journeys;
        Writings.text = language.writings;

        data.Clear();
        foreach(YearData yearData in language.items)
        {
            data.Add(yearData.year, yearData);
        }
        UpdateYear(DateTop.text);
    }

    public static YearData GetData(string year)
    {
        return data.ContainsKey(year) ? data[year] : placeholder;
    }
}

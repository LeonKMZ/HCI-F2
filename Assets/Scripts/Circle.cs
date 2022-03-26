using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Circle : MonoBehaviour
{
    public GameObject Cube;

    public float Speed = 1.5f;
    public int StartYear = 1749;
    public int EndYear = 1832;

    const int NumberOfFields = 15;
    const float scaleRad = 4.085f; // cubes would spread at 4
    const float NormalSize = 1.2f;
    const float SelectionSize = 1.4f;
    const float Angle = 12.3f;
    const int AdjustAngle = 3;
    const int SelectionNumber = 8;
    const string ElementName = "CircleElement";

    private List<GameObject> fields = new List<GameObject>();
    private List<Color> colors = new List<Color>();
    private List<string> dates = new List<string>();
    private List<YearUpdateListener> listeners = new List<YearUpdateListener>();

    private bool startDragging, endDragging;
    private Vector3 startVec;
    private int index;
    private int step;
    private float offset;

    private string currentYear;
    private float absAngle;
    private int elementAngle, halfAngle;
    private int lowerLimit, upperLimit;

    private GraphicRaycaster raycaster;


    // Start is called before the first frame update
    void Start()
    {
        elementAngle = 360 / NumberOfFields;
        halfAngle = elementAngle / 2;

        float rad = Screen.height / scaleRad;
        colors.Add(HexToColor("#E48F2B"));
        colors.Add(HexToColor("#E35B1F"));
        colors.Add(HexToColor("#C31313"));
        colors.Add(HexToColor("#46506D"));
        colors.Add(HexToColor("#5C93B2"));
        colors.Add(HexToColor("#74896F"));

        for (int i = EndYear; i >= StartYear - 1; i--)
        {
            dates.Add(i.ToString());
        }
        Cube.SetActive(false);
        for (int i = 0; i < NumberOfFields; i++)
        {
            float angle = i * Mathf.PI * 2 / NumberOfFields + AdjustAngle;
            float x = Mathf.Cos(angle) * rad;
            float y = Mathf.Sin(angle) * rad;
            Vector3 pos = transform.position + new Vector3(x, y, 0);
            float angleDegrees = angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angleDegrees + Angle);
            GameObject field = Instantiate(Cube, pos, rot);
            field.name = ElementName + "-" + i;
            field.SetActive(true);
            field.GetComponentInChildren<Text>().text = dates[i % dates.Count];
            field.GetComponent<Image>().color = colors[i % colors.Count];
            field.transform.SetParent(gameObject.transform);

            float scale = i == SelectionNumber ? SelectionSize : NormalSize;
            field.transform.localScale = new Vector3(scale, scale, 1);

            fields.Add(field);
        }
        fields[0].GetComponent<Image>().color = colors[NumberOfFields % colors.Count];
        fields[SelectionNumber].transform.SetAsLastSibling();
        gameObject.transform.RotateAround(gameObject.transform.position, new Vector3(0, 0, 1), -6f);

        endDragging = true;

        upperLimit = SelectionNumber * elementAngle;
        lowerLimit = -(elementAngle * (dates.Count - 2 - SelectionNumber));

        raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();

        listeners.Add(GameObject.Find("Information").GetComponent<Information>());
        listeners.Add(GameObject.Find("Routes").GetComponent<MapControl>());

        #if !UNITY_WEBGL
        Application.targetFrameRate = 30;
        #endif

        UpdateYear();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        EventType type = Event.current.type;

        if (type == EventType.MouseDown && !endDragging)
        {
            if (startDragging = onCircleElement())
            {
                startVec = Input.mousePosition;
                fields[SelectionNumber].transform.localScale = new Vector3(NormalSize, NormalSize, 1);
            }

        }


        if (type == EventType.MouseDrag && startDragging)
        {
            Vector3 diffVec = Input.mousePosition - startVec;
            bool right = Input.mousePosition.x > gameObject.transform.position.x;
            bool top = Input.mousePosition.y > gameObject.transform.position.y;

            bool vertical = Mathf.Abs(diffVec.y) > Mathf.Abs(diffVec.x);
            float movement = vertical ? Mathf.Abs(diffVec.y) : Mathf.Abs(diffVec.x);
            if (vertical)
            {
                step = right ? diffVec.y > 0 ? -1 : 1 : diffVec.y < 0 ? -1 : 1;
            }
            else
            {
                step = top ? diffVec.x > 0 ? 1 : -1 : diffVec.x < 0 ? 1 : -1;
            }

            startVec = Input.mousePosition;

            float sp = Speed;

            if (movement > 100)
            {
                sp *= 4;
            }
            else if (movement > 70)
            {
                sp *= 3;
            }
            else if (movement > 30)
            {
                sp *= 2;
            }

            float rotation = (step * -sp);


            if (absAngle + rotation > upperLimit || absAngle + rotation < lowerLimit)
            {
                return;
            }

            gameObject.transform.RotateAround(gameObject.transform.position, new Vector3(0, 0, 1), rotation);
            offset += rotation;
            absAngle += rotation;

            if (Mathf.Abs(offset) >= elementAngle)
            {
                updateFields(step,false);
                offset %= elementAngle;
            }
        }



        if (type == EventType.MouseUp && !endDragging)
        {
            startDragging = false;
            endDragging = true;

            if (Mathf.Abs(offset) > halfAngle)
            {
                offset = offset > 0 ? -(elementAngle - offset) : (elementAngle + offset);
                if (Mathf.Sign(offset) != Mathf.Sign(step))
                {
                    float fixAngle = -step * elementAngle * 2;
                    gameObject.transform.RotateAround(gameObject.transform.position, new Vector3(0, 0, 1), fixAngle);
                    absAngle += fixAngle;
                    //print("Fehler?");
                }
                updateFields(step, true);
            }

        }

        if (endDragging)
        {
            if (offset != 0)
            {
                float rotation = ((offset > 0 ? -1 : 1) * Speed);
                gameObject.transform.RotateAround(gameObject.transform.position, new Vector3(0, 0, 1), rotation);
                offset += rotation;
                absAngle += rotation;
            }
            else
            {
                GameObject field = fields[SelectionNumber];
                field.transform.SetAsLastSibling();
                field.transform.localScale = new Vector3(SelectionSize, SelectionSize, 1);
                endDragging = false;
            }
        }
    }

    private void updateFields(int direction, bool stop)
    {
        index += direction;

        if (direction > 0)
        {
            fields.Add(fields[0]);
            fields.RemoveAt(0);

        }
        else
        {
            fields.Insert(0, fields[NumberOfFields - 1]);
            fields.RemoveAt(NumberOfFields);
        }

        int i = 0;
        foreach (GameObject field in fields)
        {
            string year = (index + i) >= (dates.Count - 1) || (index + i) < 0 ? "" : dates[index + i];
            Color color = colors[(index + i + 12) % colors.Count];
            if (i == 0 && direction > 0)
            {
                color = colors[(index + NumberOfFields + 12) % colors.Count];
            }
            field.GetComponentInChildren<Text>().text = year;
            field.GetComponent<Image>().color = color;
            i++;
        }
        UpdateYear();
    }


    public void UpdateYear()
    {
        currentYear = fields[SelectionNumber].GetComponentInChildren<Text>().text;
        foreach (YearUpdateListener listener in listeners)
        {
            listener.UpdateYear(currentYear);
        }
    }
    private Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    private bool onCircleElement()
    {
        //Set up the new Pointer Event
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        pointerData.position = Input.mousePosition;
        raycaster.Raycast(pointerData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name.StartsWith(ElementName))
            {
                return true;
            }
        }
        return false;
    }

}
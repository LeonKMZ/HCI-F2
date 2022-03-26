using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;

public class WayPoint : MonoBehaviour
{
    
    public Vector2d Position { get; set;  }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        // print("Clicked on " + Position);
        // Future work: Information on WayPoint
    }


    public void SetColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }


    public void SetScale(float scale)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }


    public void SetPosition(Vector3 pos)
    {
        gameObject.transform.localPosition = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;
using System;

public class MapControl : MonoBehaviour, YearUpdateListener
{
    public GameObject Marker;
    public GameObject MapObject;
    public GameObject WeimarObject;

    public Button ZoomIn;
    public Button ZoomOut;

    private float InitialZoom = 5.5f;
    private float fixTargetZoom;

    private List<WayPoint> waypoints = new List<WayPoint>();

    private LineRenderer lineRenderer;
    private AbstractMap map;

    private float timeElapsed = 0;
    private float lerpDuration = 5;

    private float lerpZoom;
    private float lastZoom;
    private float targetZoom;

    private Vector2d lerpPos;
    private Vector2d lastPos;
    private Vector2d targetPos;

    private WayPoint weimar;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        map = MapObject.GetComponent<AbstractMap>();
        weimar = CreateWayPoint( new Vector2d(50.979492, 11.323544), WeimarObject);

        fixTargetZoom = InitialZoom;
        ZoomIn.onClick.AddListener(() => Zoom(0.1f));
        ZoomOut.onClick.AddListener(() => Zoom(-0.1f));
    }

    private void Zoom(float val)
    {
        val = Mathf.Clamp(targetZoom + val, fixTargetZoom - 2, fixTargetZoom + 2);
        val = Mathf.Clamp(val, 5.0f, 8.0f);
        targetZoom = val;
    }


    // Update is called once per frame
    void Update()
    {

        if (targetZoom == lerpZoom && targetPos.Equals(lerpPos))
        {
            return;
        }

        // Linear Interpolation for zoom and position
        if (timeElapsed < lerpDuration)
        {
            lerpZoom = Mathf.Lerp(lastZoom, targetZoom, timeElapsed / lerpDuration);
            lerpPos =  Vector2d.Lerp(lastPos, targetPos, Mathf.SmoothStep(0, 1, timeElapsed / lerpDuration));
            timeElapsed += Time.deltaTime;
        }
        else
        {
            lerpZoom = targetZoom;
            lerpPos = targetPos;
        }
        map.UpdateMap(lerpPos, lerpZoom);
        UpdateWayPoints();
    }

    public void UpdateYear(string year)
    {
        targetZoom = InitialZoom;
        fixTargetZoom = targetZoom;
        targetPos = weimar.Position;
        timeElapsed = 0;
        lerpDuration = 5;

        double lat = map.CenterLatitudeLongitude.x;
        double lon = map.CenterLatitudeLongitude.y;
        lastZoom = map.Zoom;
        lastPos = new Vector2d(lat, lon);
        
        foreach (WayPoint waypoint in waypoints)
        {
            Destroy(waypoint.gameObject);
        }
        waypoints.Clear();

        List<Vector2d> vecs = new List<Vector2d>();

        double[] doubles = Information.GetData(year).waypoints;
        if(doubles != null) 
        {
            for(int i = 0; i < doubles.Length; i+=2)
            {
                vecs.Add(new Vector2d(doubles[i], doubles[i+1]));
            }
        }
        else
        {
            UpdateWayPoints();
            return;
        }

        double maxX = 0;
        double minX = 1000;
        double maxY = 0;
        double minY = 1000;
      
        for (int i = 0; i < vecs.Count; i++)
        {
            Vector2d vec = vecs[i];
            minY = Math.Min(minY, vec.y);
            maxY = Math.Max(maxY, vec.y);
            minX = Math.Min(minX, vec.x);
            maxX = Math.Max(maxX, vec.x);
            WayPoint wp = CreateWayPoint(vec, Marker);
            if(i == 0 || i == vecs.Count - 1)
            {
                wp.SetScale(2);
            }
            waypoints.Add(wp);

        }

        // Calculate midpoint and route distance for new map section
        Vector2d midPos = new Vector2d((minX + maxX) / 2, (minY + maxY) / 2);
        targetPos = midPos;

        Vector2d minPos = new Vector2d(minX, minY);
        Vector2d maxPos = new Vector2d(maxX, maxY);
        double diff = Vector2d.Distance(minPos, maxPos);

        targetZoom = Mathf.Clamp((float)((InitialZoom + 2) - (diff / 3.5f)), 5.0f, 6.99f);
        fixTargetZoom = targetZoom;
        //print("Target Position: " + targetPos.ToString() + " Target Zoom: " + targetZoom);
        UpdateWayPoints();
    }

    public void UpdateWayPoints()
    {
        Vector3[] positions = new Vector3[waypoints.Count];
        int count = 0;
        foreach (WayPoint waypoint in waypoints)
        {
            Vector2d pos = waypoint.Position;
            Vector3 worldPos = map.GeoToWorldPosition(pos);
            worldPos.y += 0.2f;
            waypoint.SetPosition(worldPos);
            positions[count++] = worldPos;
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

        Vector3 weimarPos = map.GeoToWorldPosition(weimar.Position);
        weimarPos.y += 1f;
        weimar.SetPosition(weimarPos);
    }

    private WayPoint CreateWayPoint(Vector2d pos, GameObject obj)
    {
        GameObject marker = Instantiate(obj);
        marker.transform.SetParent(gameObject.transform);
        marker.SetActive(true);
        WayPoint waypoint = marker.GetComponent<WayPoint>();
        waypoint.Position = pos;
        return waypoint;
    }

}

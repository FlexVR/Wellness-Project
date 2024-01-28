using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;

public class HolographGraphController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineComponent;
    //[SerializeField] private FarmhandManager client;
    [SerializeField] public int numPoints = 5;
    [SerializeField] private float maxHeight;
    [SerializeField] private float maxWidth;
    [SerializeField] private float scalar = 20;
    //This value scales the amount by which differences in values affect height on the graph. Should be set to the count of numbers in the range of values you want to display
    //Defaults to the max height, aka 1 pixel of height per 1 difference in value
    [SerializeField] public float span = 0;
    
    //The minimum value which all values are relative to on the graph.
    //TODO: In the future, we find a way to make this based on the previous values on the graph
    [SerializeField] public float minValue = 0;
    private GraphPoint[] pointPool;
    [SerializeField] private GameObject pointPrefab;
    //[SerializeField] private bool isExtruder;
    private int pointCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        pointPool = new GraphPoint[numPoints];
        //client = FindObjectOfType<FarmhandManager>();
        // if (client == null)
        // {
        //     Debug.LogError("NO FARMHAND CLIENT FOUND!");
        // }
        // client.onPrinterUpdateReceived += onPrinterUpdateReceived;
        Debug.Log("Finished init of graph script!");
        pointCount = 0;
        
    }

    public void alloc_pointpool()
    {
        this.pointPool = new GraphPoint[numPoints];
        if (span == 0)
        {
            span = maxHeight;
        }
    }
    
    public void addPoint(float value)
    {
        Debug.Log("New update received. pointCount = " + pointCount.ToString());
        if (pointCount >= numPoints)
        {
            Debug.Log(pointPool.ToString());
            GraphPoint temp = pointPool[0];
            Array.Copy(pointPool, 1, pointPool, 0, numPoints - 1);
            temp.valueReadout.text = value.ToString();
            temp.transform.localPosition = new Vector3(maxWidth,
                clamp((maxHeight / span) * (value - minValue), 0, maxHeight), 0);
            pointPool[numPoints - 1] = temp;
            for (int i = 0; i < numPoints; i++)
            {
                pointPool[i].transform.localPosition =
                    new Vector3((maxWidth / numPoints) * i, pointPool[i].transform.localPosition.y, 0);
                lineComponent.SetPosition(i, pointPool[i].transform.localPosition);
            }
        }

        else
        {
            //Debug.Log("START");
            pointPool[pointCount] =
                Instantiate(pointPrefab, this.gameObject.GetComponent<RectTransform>().transform)
                .GetComponent<GraphPoint>();
            //Debug.Log("Local pos: " + pointPool[pointCount].transform.localPosition.ToString() + " vs world pos: " +
            //pointPool[pointCount].transform.position.ToString());
            pointPool[pointCount].valueReadout.text = value.ToString();
            pointPool[pointCount].transform.localPosition = new Vector3((maxWidth / numPoints) * pointCount,
                clamp((maxHeight / span) * (value - minValue), 0, maxHeight), 0);
            pointCount++;
            lineComponent.positionCount = pointCount;
            for (int i = 0; i < pointCount; i++)
            {
                lineComponent.SetPosition(i, pointPool[i].transform.localPosition);
            }

            //Debug.Log("STOP");
        }
    }
        // else
        // {
        //     if (pointCount >= numPoints)
        //     {
        //         Debug.Log(pointPool.ToString());
        //         GraphPoint temp = pointPool[0];
        //         Array.Copy(pointPool, 1, pointPool, 0, numPoints-1);
        //         temp.tempReadout.text = client.currentPrinter.current_temp_bed.ToString();
        //         temp.transform.localPosition = new Vector3(maxWidth, (client.currentPrinter.current_temp_bed / maxHeight)*scalar, 0);
        //         pointPool[numPoints - 1] = temp;
        //         for (int i = 0; i < numPoints; i++)
        //         {
        //             pointPool[i].transform.localPosition =
        //                 new Vector3((maxWidth / numPoints) * i, pointPool[i].transform.localPosition.y, 0);
        //             lineComponent.SetPosition(i, pointPool[i].transform.localPosition);
        //         }
        //     }
        //
        //     else
        //     {
        //         Debug.Log("START");
        //         pointPool[pointCount] = Instantiate(pointPrefab, this.gameObject.GetComponent<RectTransform>().transform).GetComponent<GraphPoint>();
        //         Debug.Log("Local pos: " + pointPool[pointCount].transform.localPosition.ToString() + " vs world pos: " + pointPool[pointCount].transform.position.ToString());
        //         pointPool[pointCount].tempReadout.text = client.currentPrinter.current_temp_bed.ToString();
        //         pointPool[pointCount].transform.localPosition = new Vector3((maxWidth/numPoints)*pointCount, (client.currentPrinter.current_temp_bed / maxHeight)*scalar, 0);
        //         pointCount++;
        //         lineComponent.positionCount = pointCount;
        //         for (int i = 0; i < pointCount; i++)
        //         {
        //             lineComponent.SetPosition(i, pointPool[i].transform.localPosition);
        //         }
        //         Debug.Log("STOP");
        //     }
     //   }



   // }

    // Update is called once per frame
    void Update()
    {
        
    }
}

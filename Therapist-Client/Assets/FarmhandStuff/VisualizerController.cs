using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//TODO: Parameterize visualizations so new variables can easily be added
public class VisualizerController : MonoBehaviour
{
    [SerializeField] private HolographGraphController extruderTempGraph;
    [SerializeField] private HolographGraphController bedTempGraph;
    [SerializeField] private HolographGraphController fanPowerGraph;
    [SerializeField] private HolographGraphController velocityGraph;

    [SerializeField] private TextMeshProUGUI graphTitle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onCloseBtn()
    {
        Destroy(this.gameObject);
    }
    
    public void populate(string[] command)
    {
        graphTitle.text = "Visualization\n" + command[1];
        int num_pnts = command.Length - 2;
        extruderTempGraph.numPoints = num_pnts;
        bedTempGraph.numPoints = num_pnts;
        fanPowerGraph.numPoints = num_pnts;
        velocityGraph.numPoints = num_pnts;
        extruderTempGraph.alloc_pointpool();
        bedTempGraph.alloc_pointpool();
        fanPowerGraph.alloc_pointpool();
        velocityGraph.alloc_pointpool();
        for (int i = 2; i < command.Length; i++)
        {
            string[] nums = command[i].Split(",");
            extruderTempGraph.addPoint(Mathf.Round(float.Parse(nums[0])));
            bedTempGraph.addPoint(Mathf.Round(float.Parse(nums[1])));
            fanPowerGraph.addPoint(Mathf.Round(float.Parse(nums[2]) * 100f));
            velocityGraph.addPoint(Mathf.Round(float.Parse(nums[3])));
        }
    }
}

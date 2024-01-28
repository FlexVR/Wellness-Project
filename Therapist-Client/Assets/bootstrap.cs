using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class bootstrap : MonoBehaviour
{

    public GameObject netprefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void StartNetwork() {
        Realtime.Instantiate("NetworkingObj");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

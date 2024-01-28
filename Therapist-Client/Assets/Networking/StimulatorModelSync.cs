using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using OscCore;
//using OSCCore.IntegerInput;

public class StimulatorModelSync : RealtimeComponent<StimulatorModel>
{
    private OscClient oscclient;


    [SerializeField]
    private string NameOfHeartRateObj;

    [SerializeField]
    private Button startbtn;

    [SerializeField]
    private HolographGraphController graph;

    [SerializeField]
    private Button stimbutton;

    [SerializeField]
    private Button resetbutton;

    public int temp = 2;

    [SerializeField]
    private bool IsPatient = false;

    [SerializeField]
    private IntEvent onFreqUpdated;

    [SerializeField]
    private IntEvent onDurationUpdated;

    [SerializeField]
    private IntEvent HeartbeatUpdated;

    [SerializeField]
    private BoolEvent onStimulationSwitched;

    protected override void OnRealtimeModelReplaced(StimulatorModel previousModel, StimulatorModel currentModel) {
        if (previousModel != null) {
            if (IsPatient) {
                currentModel.stimulation_onDidChange -= (model, value) => {onStimulationSwitched?.Invoke(value);};
                currentModel.frequencyDidChange -= (model, value) => {onFreqUpdated?.Invoke(value);};
                currentModel.on_secondsDidChange -= (model, value) => {onDurationUpdated?.Invoke(value);};
            }

            if (!IsPatient) {
                currentModel.heartrate_bpmDidChange -= (model, value) => {HeartbeatUpdated?.Invoke(value);};
            }
            
        }

        if (currentModel != null) {
            if (IsPatient) {
                currentModel.stimulation_onDidChange += (model, value) => {
                    Debug.Log("Stim on: " + value);
                    oscclient.Send("/EMS_CALIB", value);
                    };
                currentModel.frequencyDidChange += (model, value) => {
                    Debug.Log("Freq: " + value);
                    oscclient.Send("/RESTART", value);
                    };
                currentModel.on_secondsDidChange += (model, value) => {
                    Debug.Log("Duration: " + value);
                    onDurationUpdated?.Invoke(value);
                    };
            }

            if (!IsPatient) {
                currentModel.heartrate_bpmDidChange += (model, value) => {
                    Debug.Log("BPM: " + value);
                    HeartbeatUpdated?.Invoke(value);
                    };
                    currentModel.stimulation_onDidChange += (model, value) => {
                    Debug.Log("Stim on: " + value);
                    };
            }
        }
    } 

    public void SetFrequency(int frequency) {
        model.frequency = frequency;
    }
    
    public void SetDuration(int seconds) {
        //TODO: Need to set some timer when stim is started that updates the model to show the stim as off when the duration is up
        model.on_seconds = seconds;

    }



    public void SetStimulationOn(bool isOn) {
        model.stimulation_on = isOn;
    }

    public void UpdateHeartrate(int bpm) {
        model.heartrate_bpm = bpm;
    }

    public int ReadFrequency() {
        return model.frequency;
    }

    public int ReadDuration() {
        return model.on_seconds;
    }

    public bool IsOn() {
        return model.stimulation_on;
    }

    private void HRFromOSC(OscMessageValues values) {
        Debug.Log(values.ReadIntElement(0));
        UpdateHeartrate(values.ReadIntElement(0));
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!this.IsPatient) {
            stimbutton = GameObject.Find("StimButton").GetComponent<Button>();
            stimbutton.onClick.AddListener(this.ToggleStim);

            resetbutton = GameObject.Find("ResetBtn").GetComponent<Button>();
            resetbutton.onClick.AddListener(this.TriggerReset);

            startbtn = GameObject.Find("StartBtn").GetComponent<Button>();
            startbtn.interactable = false;
            //graph = GameObject.Find("HRGraph").GetComponent<HolographGraphController>();
        }

        else {
            OscServer oscHeartRateReceive = GameObject.Find(NameOfHeartRateObj).GetComponent<OscReceiver>().Server;
            oscHeartRateReceive.TryAddMethod("/HEART", HRFromOSC );
            oscclient = new OscClient("127.0.0.1", 2691);
        }
    }

    void OnDestroy() {
        startbtn.interactable = true;
    }
    
    public void LogInt(int val) {
        Debug.Log(val);
    }

    public void ToggleStim() {
        Debug.Log("You clicked the button");
        // if (this.IsOn()) {
        //     SetStimulationOn(false);
        // }

        // else {
        //     SetStimulationOn(true);
        // }
        SetStimulationOn(!IsOn());
    }

    public void TriggerReset() {
        if (temp == 2) {
            temp = 1;
            SetFrequency(temp);
        }
        else {
            temp = 2;
            SetFrequency(temp);
        }
        
    }
    // Update is called once per frame
    void Update()
    {

    }
}

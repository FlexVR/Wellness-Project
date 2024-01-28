using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StimulatorModelSync : RealtimeComponent<StimulatorModel>
{

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
                currentModel.stimulation_onDidChange += (model, value) => {onStimulationSwitched?.Invoke(value);};
                currentModel.frequencyDidChange += (model, value) => {onFreqUpdated?.Invoke(value);};
                currentModel.on_secondsDidChange += (model, value) => {onDurationUpdated?.Invoke(value);};
            }

            if (!IsPatient) {
                currentModel.heartrate_bpmDidChange += (model, value) => {HeartbeatUpdated?.Invoke(value);};
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

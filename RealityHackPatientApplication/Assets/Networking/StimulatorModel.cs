using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RealtimeModel]
public partial class StimulatorModel
{
    [RealtimeProperty(1, true, true)]
    private bool _stimulation_on;
    
    [RealtimeProperty(2, true, true)]
    private int _frequency;

    [RealtimeProperty(3, true, true)]
    private int _on_seconds;

    [RealtimeProperty(4, false, true)]
    private int _heartrate_bpm;

}

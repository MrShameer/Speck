using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    public Slider slider;

    public Text TotalNPC;
    public Text NPCSpawnInterval;
    public Text TrainWaitTime;

    // void Start(){
    //     slider.value = 0;
    //     sliderText.text = slider.value.ToString()+ "(s)";
    // }

    public void NPCSpawnIntervalSlider(float value){
        NPCSpawnInterval.text = value.ToString() + "(s)";
    }

    public void TrainWaitTimeSlider(float value){
        TrainWaitTime.text = value.ToString() + "(s)";
    }

    public void TotalNPCSlider(float value){
        TotalNPC.text = value.ToString();
    }
}

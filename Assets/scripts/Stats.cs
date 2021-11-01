using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour { 
    Slider slider;
    Text score;
    Text playerId;
    Button btn_batteringRamBlock;
    Button btn_timeTravelBlock;

    private void Awake() {
        playerId = transform.Find("PlayerID").gameObject.GetComponent<Text>();
        slider = transform.GetComponentInChildren<Slider>();
        score = transform.Find("score").gameObject.GetComponent<Text>();
        btn_batteringRamBlock = transform.Find("RamBlock").gameObject.GetComponent<Button>();
        btn_timeTravelBlock = transform.Find("TimeBlock").gameObject.GetComponent<Button>();
    }

    public void UpdateBar(float value) {
        slider.value = value;
    }

    public void UpdateScore(int value) {
        score.text = value.ToString();
    }

    public void SetRamBlock(bool value) {
        btn_batteringRamBlock.gameObject.SetActive(value);
    }

    public void SetTimeBlock(bool value) {
        btn_timeTravelBlock.gameObject.SetActive(value);
    }

    public void SetPlayerId(string value) {
        playerId.text = value;
    }
}

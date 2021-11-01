using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBlock : MonoBehaviour
{
    public enum BlockStyle {
        none,
        food,
        enginePower,
        batteringRam,
        timeTravel,
        snakePart,
    };

    public BlockStyle style;

    public void SetStyle(BlockStyle goodsStyle) {
        if (goodsStyle == BlockStyle.food) {
            GetComponent<Renderer>().material.color = Color.blue;
            style = BlockStyle.food;
        }
        else if (goodsStyle == BlockStyle.enginePower) {
            GetComponent<Renderer>().material.color = Color.red;
            style = BlockStyle.enginePower;
        }
        else if (goodsStyle == BlockStyle.batteringRam) {
            GetComponent<Renderer>().material.color = Color.green;
            style = BlockStyle.batteringRam;
        }
        else if (goodsStyle == BlockStyle.timeTravel) {
            GetComponent<Renderer>().material.color = Color.yellow;
            style = BlockStyle.timeTravel;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goods : MonoBehaviour
{
    public enum GoodsStyle {
        foodBlock,
        enginePowerBlock,
        batteringRamBlock,
        timeTravelBlock,
    };

    public GoodsStyle style;

    public void SetStyle(GoodsStyle goodsStyle) {
        if (goodsStyle == GoodsStyle.foodBlock) {
            GetComponent<Renderer>().material.color = Color.blue;
            style = GoodsStyle.foodBlock;
        }
        else if (goodsStyle == GoodsStyle.enginePowerBlock) {
            GetComponent<Renderer>().material.color = Color.red;
            style = GoodsStyle.enginePowerBlock;
        }
        else if (goodsStyle == GoodsStyle.batteringRamBlock) {
            GetComponent<Renderer>().material.color = Color.green;
            style = GoodsStyle.batteringRamBlock;
        }
        else if (goodsStyle == GoodsStyle.timeTravelBlock) {
            GetComponent<Renderer>().material.color = Color.yellow;
            style = GoodsStyle.timeTravelBlock;
        }
    }
}

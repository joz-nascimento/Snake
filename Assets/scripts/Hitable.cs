using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    public enum HitableOptions { 
        none,
        food,
        speedUp,
        ramBlock,
        timeTravel, 
        snake};
    public HitableOptions hitOptions;
}

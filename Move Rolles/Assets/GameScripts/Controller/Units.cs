using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour {

	// Use this for initialization
    public enum moveDirection{
        up,
        down,
        neutral
    }

    public enum unitStat{
        idle,
        moving,
        stop
    }

    public enum unitBoxState{
        offHand,
        inHand
    }

    public enum boxUnitStates{
        idle,
        moving
    }
}

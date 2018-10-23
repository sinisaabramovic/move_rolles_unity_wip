using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControler : MonoBehaviour {

    // Use this for initialization
    public enum unitRotate{
        original,
        nonoriginal
    }
    public unitRotate rotationState;
    public GameObject boxParent;
    public int playerMoveIndex = 3;
    public Vector3 direction;
    public Units.moveDirection moveDirection;
    public Units.unitBoxState boxState;
    public Units.unitStat playerStat = Units.unitStat.idle;
    public GameObject activeBox;
    public float smoothTime = 0.1f;
    public bool canControll = true;
    GameManager gm;

	void Start () {
        direction = transform.position;
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {

        if(playerStat != Units.unitStat.stop){
            if (transform.position.z != direction.z)
            {
                Vector3 m = new Vector3(0, 0, 1.25f);
                float step = smoothTime * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, direction, step);
                playerStat = Units.unitStat.moving;
            }
            else
            {
                playerStat = Units.unitStat.idle;
                //if(playerMoveIndex == gm.tracks.Length - 1 && gm.activePlayer == 0){
                if (gm.activePlayer == 0 && activeBox != null && activeBox.GetComponent<BoxUnit>().canBeDeployed){
                    if(gm.tracksOut[playerMoveIndex].GetComponent<TrackOut>().trackOutType == activeBox.GetComponent<BoxUnit>().packetType) {
                        if (boxState == Units.unitBoxState.inHand)
                        {
                            playerStat = Units.unitStat.stop;
                            StartCoroutine(Turn());
                        }
                    }
                }
            }
        }		
	}

    IEnumerator Turn()
    {
        Debug.Log("TURN");
        this.transform.RotateAround(this.transform.position, Vector3.up, -180);
        this.GetComponent<UnitControler>().rotationState = UnitControler.unitRotate.nonoriginal;
        this.GetComponent<UnitControler>().canControll = false;
        this.GetComponent<UnitControler>().boxState = Units.unitBoxState.offHand;
        yield return new WaitForSeconds(0.3f);
        Debug.Log("TURN END");
        this.activeBox.GetComponent<BoxUnit>().finalEndPoint = gm.tracksOut[playerMoveIndex].transform.GetChild(0).transform.TransformPoint(Vector3.zero);
        this.canControll = true;
        this.playerStat = Units.unitStat.idle;
		this.activeBox.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.trackOut;
        this.activeBox.transform.parent = null;
    }
}

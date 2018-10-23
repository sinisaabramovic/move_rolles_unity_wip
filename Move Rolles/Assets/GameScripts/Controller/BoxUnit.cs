using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxUnit : MonoBehaviour {

    // Use this for initialization

    public enum boxType{
        red,
        green,
        blue,
        yellow,
        cyan,
        magenta,
        white
    }

    public enum boxState{
        trackIn,
        endOfTheTrackIn,
        onTrack,
        endOfTrack,
        playerHand,
        trackOut,
        stoped,
        final
    }

    public enum BoxControl{
        spawn,
        onPointA,
        onPointB,
        onPointC,
        onPointD,
        onPointInstantiate_1,
        inPlayerLeftHand,
        inTrackStartPoint,
        inTrackEndPoint,
        inPlayerRightHand,
        onExitPlatform,
        onEnd,
        onFinalTrackAchive,
        Fail
    }

    public bool canBeDeployed = false;
    public boxType packetType;
    public BoxControl boxControlStates;
    public string whereIAm;
    public int rewardAmount = 1;
    public GameObject[] incomePoints;
    public GameObject orderPlatform;
    public boxState boxStates;
    public float moveSpeed = 0.1f;
    int wpIndex = 0;
    public int boxIndex;
    GameManager gm;
    public TrackController.trackDirection boxMoveDirection;
    public int trackIndex;
    public Vector3 startTrackPoint;
    public Vector3 endTrackPoint;
    public Vector3 finalEndPoint;
    private ScoreManager scm;

	void Start () {
        incomePoints = GameObject.FindGameObjectsWithTag("InPoints");
        gm = GameObject.FindObjectOfType<GameManager>();
        scm = GameObject.FindObjectOfType<ScoreManager>();
        incomePoints = incomePoints.OrderBy(orderPlatform => orderPlatform.transform.position.z).ToArray();
        wpIndex = incomePoints.Length - 1;
        finalEndPoint = Vector3.zero;
        boxControlStates = BoxControl.spawn;
	}

    private void OnDestroy()
    {
        gm.packetCounter--;
        Debug.Log("Destroyed");
    }

    // Update is called once per frame
    void Update () {
        switch(boxStates){
            case boxState.trackIn:                
                if(transform.position != incomePoints[0].transform.position){
                    float step1 = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, incomePoints[wpIndex].transform.position, step1);
                    if(transform.position == incomePoints[wpIndex].transform.position){
						whereIAm = incomePoints[wpIndex].name;
                        wpIndex--;
                        if (wpIndex < 0)
                            wpIndex = 0;
                        switch(wpIndex){
                            case 0:
                                boxControlStates = BoxControl.onPointD;
                                break;
                            case 1:
                                boxControlStates = BoxControl.onPointInstantiate_1;
                                break;
                            case 2:
                                boxControlStates = BoxControl.onPointC;
                                break;
                            case 3:
                                boxControlStates = BoxControl.onPointB;
                                break;
                            case 4:
                                boxControlStates = BoxControl.onPointA;
                                break;
                        }
                        //Debug.Log(wpIndex);
                    }
                }else{
                    boxStates = boxState.endOfTheTrackIn; 
                    if((gm.players[1].GetComponent<UnitControler>().playerMoveIndex == 0 && gm.players[1].GetComponent<UnitControler>().playerStat == Units.unitStat.idle) && gm.players[1].GetComponent<UnitControler>().boxState == Units.unitBoxState.offHand){
                        gm.players[1].transform.RotateAround(gm.players[1].transform.position, Vector3.up, 180);
                        gm.players[1].GetComponent<UnitControler>().rotationState = UnitControler.unitRotate.nonoriginal;
                        this.transform.SetParent(gm.players[1].GetComponent<UnitControler>().boxParent.transform, true);
                        this.gameObject.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.playerHand;
                        gm.players[1].GetComponent<UnitControler>().boxState = Units.unitBoxState.inHand;
                        gm.players[1].GetComponent<UnitControler>().activeBox = this.gameObject;
                        boxControlStates = BoxControl.inPlayerRightHand;
                    }else{
                        boxControlStates = BoxControl.Fail;
                        //Debug.Log("START BUST!");
                        Destroy(this.gameObject);

                    }
                }
                break;
            case boxState.onTrack:
               
                if (transform.position != endTrackPoint){
                    boxControlStates = BoxControl.inTrackStartPoint;
                    float step2 = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, endTrackPoint, step2);
                }
                else{
                    boxStates = boxState.endOfTrack;
                    boxControlStates = BoxControl.inTrackEndPoint;
                    if(boxMoveDirection == TrackController.trackDirection.left){
                        if(gm.players[0].GetComponent<UnitControler>().playerMoveIndex == trackIndex && gm.players[0].GetComponent<UnitControler>().playerStat == Units.unitStat.idle && gm.players[0].GetComponent<UnitControler>().boxState == Units.unitBoxState.offHand){
                            this.transform.SetParent(gm.players[0].GetComponent<UnitControler>().boxParent.transform, true);
                            this.gameObject.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.playerHand;
                            gm.players[0].GetComponent<UnitControler>().boxState = Units.unitBoxState.inHand;
                            gm.players[0].GetComponent<UnitControler>().activeBox = this.gameObject;
                            boxControlStates = BoxControl.inPlayerLeftHand;
                        }else{
                            //Debug.Log("BUST 2 LEFT!");
							boxControlStates = BoxControl.Fail;
                            Destroy(this.gameObject);
                        }
                    }else{
                        if (gm.players[1].GetComponent<UnitControler>().playerMoveIndex == trackIndex && gm.players[1].GetComponent<UnitControler>().playerStat == Units.unitStat.idle && gm.players[1].GetComponent<UnitControler>().boxState == Units.unitBoxState.offHand){
                            this.transform.SetParent(gm.players[1].GetComponent<UnitControler>().boxParent.transform, true);
                            this.gameObject.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.playerHand;
                            gm.players[1].GetComponent<UnitControler>().boxState = Units.unitBoxState.inHand;
                            gm.players[1].GetComponent<UnitControler>().activeBox = this.gameObject;
                            boxControlStates = BoxControl.inPlayerRightHand;
                        }else{
                            //Debug.Log("BUST 2 RIGHT!");
							boxControlStates = BoxControl.Fail;
                            Destroy(this.gameObject);
                        }
                    }
                }
                break;
            case boxState.trackOut:
                this.transform.parent = null;
                //Debug.Log(finalEndPoint);
                if (transform.position != finalEndPoint){
                    boxControlStates = BoxControl.onExitPlatform;
                    float step3 = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, finalEndPoint, step3);
                }else{
                    boxControlStates = BoxControl.onEnd;
                    boxStates = boxState.final;
                    scm.Score = scm.Score + rewardAmount;					
                    Destroy(this.gameObject);
                }
                break;
        }
	}
}

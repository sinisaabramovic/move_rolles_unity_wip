using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

	// Use this for initialization

    public struct GameConditionRythm {
        public float speed;
        public BoxUnit.BoxControl controllState1;
        public BoxUnit.BoxControl controllState2;
        public BoxUnit.BoxControl controllState3;
        public int numOffCycles;
        public int maxNumCycles;
        public BoxUnit.boxType type;
    }

    public GameObject[] tracks;
    public GameObject[] tracksOut;
    public GameObject[] packets;
    public GameObject orderPlatform;
	public GameObject[] players;
	public int playerLMoveIndex = 3;
	public int playerRMoveIndex = 3;
	public float moveTrashold = 1.25f;
	public float moveSpeed = 1.0f;
	public int activePlayer = 0;

    GameObject playerL;
    GameObject playerR;
    Units unitManager;

    public Transform spawnDepolyPoint;
    public GameObject packetPrefab;

    public GameObject[] packetsPrefabs;

    public float deployRate = 1.5F;
    private float nextDepoly = 0.0F;
    public bool canDeployNewPacket = true;
    public int packetCounter = 0;

    public GameConditionRythm[] rythamConditions;
    public int MAXnumOfCycles = 5;
    public int cyclesCounter = 0;

	void Start () {
        playerL = GameObject.FindGameObjectWithTag("PlayerL");
        playerR = GameObject.FindGameObjectWithTag("PlayerR");
        tracks = GameObject.FindGameObjectsWithTag("Track");
        tracksOut = GameObject.FindGameObjectsWithTag("TrackOut");
        tracks = tracks.OrderBy(orderPlatform => orderPlatform.transform.position.z).ToArray();
        tracksOut = tracksOut.OrderBy(orderPlatform => orderPlatform.transform.position.z).ToArray();
        players = new GameObject[2];
        players[0] = playerL;
        players[1] = playerR;

        rythamConditions = new GameConditionRythm[MAXnumOfCycles];

        rythamConditions[0].speed = 0.7f;
        rythamConditions[0].controllState1 = BoxUnit.BoxControl.onPointInstantiate_1;
        rythamConditions[0].controllState2 = BoxUnit.BoxControl.onPointD;
        rythamConditions[0].numOffCycles = 0;
        rythamConditions[0].maxNumCycles = 5;
        rythamConditions[0].type = BoxUnit.boxType.blue;

        rythamConditions[1].speed = 0.7f;
        rythamConditions[1].controllState1 = BoxUnit.BoxControl.onPointInstantiate_1;
        rythamConditions[1].controllState2 = BoxUnit.BoxControl.onPointD;
        rythamConditions[1].numOffCycles = 0;
        rythamConditions[1].maxNumCycles = 2;
        rythamConditions[1].type = BoxUnit.boxType.red;


        rythamConditions[2].speed = 0.8f;
        rythamConditions[2].controllState1 = BoxUnit.BoxControl.onPointInstantiate_1;
        rythamConditions[2].controllState2 = BoxUnit.BoxControl.onPointD;
        rythamConditions[2].numOffCycles = 0;
        rythamConditions[2].maxNumCycles = 3;
        rythamConditions[2].type = BoxUnit.boxType.yellow;


        rythamConditions[3].speed = 1f;
        rythamConditions[3].controllState1 = BoxUnit.BoxControl.onPointInstantiate_1;
        rythamConditions[3].controllState2 = BoxUnit.BoxControl.onPointD;
        rythamConditions[3].numOffCycles = 0;
        rythamConditions[3].maxNumCycles = 7;
        rythamConditions[3].type = BoxUnit.boxType.green;


        rythamConditions[4].speed = 1.5f;
        rythamConditions[4].controllState1 = BoxUnit.BoxControl.onPointInstantiate_1;
        rythamConditions[4].controllState2 = BoxUnit.BoxControl.onPointD;
        rythamConditions[4].numOffCycles = 0;
        rythamConditions[4].maxNumCycles = 15;
        rythamConditions[4].type = BoxUnit.boxType.magenta;
	}

	// Update is called once per frame

    GameObject instatiatePacket(BoxUnit.boxType paramType){
     
        foreach(GameObject go in packetsPrefabs){
            if(go.GetComponent<BoxUnit>().packetType == paramType){
                return go;
                break;
            }
        }

        return null;
    }

	void Update () {

        // Main game manager

        if (cyclesCounter < MAXnumOfCycles)
        {
            if (rythamConditions[cyclesCounter].numOffCycles >= rythamConditions[cyclesCounter].maxNumCycles)
            {
                cyclesCounter++;
            }
        }else {
            cyclesCounter = 0;
        }

        if (Time.time > nextDepoly)
        {
            packets = GameObject.FindGameObjectsWithTag("Packet");
            nextDepoly = Time.time + deployRate;

            if(packets.Length - 1 >= 0 && packets[packets.Length - 1] != null){
                if (packets[packets.Length - 1].GetComponent<BoxUnit>().boxControlStates == rythamConditions[0].controllState1 || packets[packets.Length - 1].GetComponent<BoxUnit>().boxControlStates == rythamConditions[0].controllState2)
                {
                    canDeployNewPacket = true;
                }
                else
                {
                    canDeployNewPacket = false;
                }
            }else{
                packetCounter = 0;
                if (packets.Length == 0)
                {
                    canDeployNewPacket = true;
                }
            }

            if(canDeployNewPacket){
                GameObject packet = Instantiate(instatiatePacket(rythamConditions[Random.Range(0,4)].type), spawnDepolyPoint.position, spawnDepolyPoint.rotation) as GameObject;
				packetCounter++;
                rythamConditions[cyclesCounter].numOffCycles++;
                packet.GetComponent<BoxUnit>().moveSpeed = Random.Range(rythamConditions[0].speed - 0.4f, rythamConditions[0].speed);
                packet.GetComponent<BoxUnit>().boxIndex = packetCounter;
            }

        }

        if(Input.GetMouseButtonDown(0))
        {  
            // Left diretion
            Vector3 mousePos = Input.mousePosition;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			float transformDirection = 0;
            //Debug.Log(tracks[0].transform.TransformPoint(Vector3.zero));
            if (mousePos.x <= Screen.width / 2){
                activePlayer = 0;
            }else{
                activePlayer = 1;
            }

            if (players[activePlayer].GetComponent<UnitControler>().playerStat == Units.unitStat.idle)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    transformDirection = players[activePlayer].transform.position.z - hit.point.z;
                    if(activePlayer == 1){
                        if (((hit.collider.tag == "Untagged" || hit.collider.tag == "PlayerR") || hit.collider.tag == "Track") && players[activePlayer].GetComponent<UnitControler>().boxState == Units.unitBoxState.inHand && players[activePlayer].GetComponent<UnitControler>().rotationState == UnitControler.unitRotate.nonoriginal)
                        {
                            if(players[activePlayer].GetComponent<UnitControler>().rotationState == UnitControler.unitRotate.nonoriginal){
                                players[activePlayer].transform.RotateAround(players[activePlayer].transform.position, Vector3.up, -180);   
                                players[activePlayer].GetComponent<UnitControler>().rotationState = UnitControler.unitRotate.original;
                            }
                        }
                    }

                    if (activePlayer == 0)
                    {
                        if (((hit.collider.tag == "Untagged" || hit.collider.tag == "PlayerL") || hit.collider.tag == "Track") && players[activePlayer].GetComponent<UnitControler>().boxState == Units.unitBoxState.offHand && players[activePlayer].GetComponent<UnitControler>().rotationState == UnitControler.unitRotate.nonoriginal)
                        {
                            if (players[activePlayer].GetComponent<UnitControler>().rotationState == UnitControler.unitRotate.nonoriginal)
                            {
                                //Debug.LogError("ERROR");
                                players[activePlayer].transform.RotateAround(players[activePlayer].transform.position, Vector3.up, 180);
                                players[activePlayer].GetComponent<UnitControler>().rotationState = UnitControler.unitRotate.original;
                            }
                        }
                    }

                    if(players[activePlayer].GetComponent<UnitControler>().playerStat == Units.unitStat.idle && players[activePlayer].GetComponent<UnitControler>().boxState == Units.unitBoxState.inHand){
                        if (hit.collider.tag == "Track" && players[activePlayer].GetComponent<UnitControler>().rotationState == UnitControler.unitRotate.original){                            
                            if(tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().directionMove == TrackController.trackDirection.left && activePlayer == 1){
                                players[activePlayer].GetComponent<UnitControler>().activeBox.transform.parent = null;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().trackIndex = players[activePlayer].GetComponent<UnitControler>().playerMoveIndex;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().boxMoveDirection = TrackController.trackDirection.left;
                                players[activePlayer].GetComponent<UnitControler>().boxState = Units.unitBoxState.offHand;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.transform.position = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointStart.transform.TransformPoint(Vector3.zero);
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().endTrackPoint = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointEnd.transform.TransformPoint(Vector3.zero);
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().startTrackPoint = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointStart.transform.TransformPoint(Vector3.zero);
								players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.onTrack;
                                players[activePlayer].GetComponent<UnitControler>().canControll = false;
                                if(players[activePlayer].GetComponent<UnitControler>().playerMoveIndex == tracks.Length - 1){                                  
                                    players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().canBeDeployed = true;
                                }
                            }else if(tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().directionMove == TrackController.trackDirection.righ && activePlayer == 0){
                                players[activePlayer].GetComponent<UnitControler>().activeBox.transform.parent = null;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().trackIndex = players[activePlayer].GetComponent<UnitControler>().playerMoveIndex;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().boxMoveDirection = TrackController.trackDirection.righ;
                                players[activePlayer].GetComponent<UnitControler>().boxState = Units.unitBoxState.offHand;
                                players[activePlayer].GetComponent<UnitControler>().activeBox.transform.position = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointEnd.transform.TransformPoint(Vector3.zero);
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().endTrackPoint = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointStart.transform.TransformPoint(Vector3.zero);
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().startTrackPoint = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].GetComponent<TrackController>().pointEnd.transform.TransformPoint(Vector3.zero);                                
                                players[activePlayer].GetComponent<UnitControler>().activeBox.GetComponent<BoxUnit>().boxStates = BoxUnit.boxState.onTrack;
                                players[activePlayer].GetComponent<UnitControler>().canControll = false;
                            }
                        }else{
                            
                        }
                    }

                }
                if((hit.collider.tag == "Untagged" || hit.collider.tag == "Track") && players[activePlayer].GetComponent<UnitControler>().canControll == true){
                    if (transformDirection < 0)
                    {
                        // Up
                        players[activePlayer].GetComponent<UnitControler>().playerMoveIndex++;
                        if (players[activePlayer].GetComponent<UnitControler>().playerMoveIndex <= tracks.Length - 1)
                        {
                            Vector3 targetPosition = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].transform.position;
                            targetPosition.x = players[activePlayer].transform.position.x;
                            targetPosition.y = players[activePlayer].transform.position.y;
                            players[activePlayer].GetComponent<UnitControler>().direction = targetPosition;
                            players[activePlayer].GetComponent<UnitControler>().moveDirection = Units.moveDirection.up;
                        }
                        else
                        {
                            players[activePlayer].GetComponent<UnitControler>().playerMoveIndex -= 1;
                        }
                    }
                    else
                    {
                        // Down
                        players[activePlayer].GetComponent<UnitControler>().playerMoveIndex--;
                        if (players[activePlayer].GetComponent<UnitControler>().playerMoveIndex >= 0)
                        {
                            Vector3 targetPosition = tracks[players[activePlayer].GetComponent<UnitControler>().playerMoveIndex].transform.position;
                            targetPosition.x = players[activePlayer].transform.position.x;
                            targetPosition.y = players[activePlayer].transform.position.y;
                            players[activePlayer].GetComponent<UnitControler>().direction = targetPosition;
                            players[activePlayer].GetComponent<UnitControler>().moveDirection = Units.moveDirection.down;
                        }
                        else
                        {
                            players[activePlayer].GetComponent<UnitControler>().playerMoveIndex += 1;
                        }
                    }
                }
            }
        }else{
            if (players[activePlayer].GetComponent<UnitControler>().playerStat != Units.unitStat.stop)
                players[activePlayer].GetComponent<UnitControler>().canControll = true;
        }
		
	}
}

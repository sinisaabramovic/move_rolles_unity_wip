using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackController : MonoBehaviour {

	// Use this for initialization
    public enum trackDirection{
        left,
        righ,
        stay
    }

    public trackDirection directionMove;
    public GameObject leftDirectionPrefab;
    public GameObject rightDirectionPrefab;

    public GameObject pointStart;
    public GameObject pointEnd;
	void Start () {

        leftDirectionPrefab.SetActive(false);
        rightDirectionPrefab.SetActive(false);
		
	}
	
	// Update is called once per frame
	void Update () {

        if(directionMove == trackDirection.left){
            leftDirectionPrefab.SetActive(true);
            rightDirectionPrefab.SetActive(false);
        }else{
            leftDirectionPrefab.SetActive(false);
            rightDirectionPrefab.SetActive(true); 
        }
		
	}
}

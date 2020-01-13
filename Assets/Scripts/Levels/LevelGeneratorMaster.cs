using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorMaster : MonoBehaviour {

	public enum GeneratedLevel{FloorsAndBridges,Maze,Empty}
	[Range(20,100)]
	public int halfWorldSize;
	public GeneratedLevel generatedLevel;

//	public bool autoUpdate;

	public void MasterGenerateLevel(){
		LevelGenerator_01_FloorsAndBridges levelGen01 = GetComponent<LevelGenerator_01_FloorsAndBridges> ();
		LevelGenerator_02_Maze levelGenMaze = GetComponent<LevelGenerator_02_Maze> ();
		if(generatedLevel == GeneratedLevel.FloorsAndBridges){
			levelGen01.GenerateMap ();
			levelGen01.autoUpdate = true;
		}else if(generatedLevel== GeneratedLevel.Maze){
			levelGenMaze.GenerateMap ();
		}

	}
	public void DestroyControlNodes(){
		LevelGenerator_01_FloorsAndBridges levelGen01 = GetComponent<LevelGenerator_01_FloorsAndBridges> ();
		levelGen01.autoUpdate = false;
		string holderName = "Control Nodes";
		if(transform.Find(holderName)){
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
	}
		

//	void GenerateLevel(){
//		if(generatedLevel == GeneratedLevel.RoomsAndBridges){
//			LevelGenerator_01_FloorsAndBridges.Gen
//		}
//	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(LevelGenerator_01_FloorsAndBridges))]
public class LevelGenerator_01Editor : Editor
{



	public override void OnInspectorGUI ()
	{
		LevelGenerator_01_FloorsAndBridges levelGen = (LevelGenerator_01_FloorsAndBridges)target;


		if (DrawDefaultInspector ()) {
			
			if (levelGen.autoUpdate) {
				levelGen.GenerateMap ();
			}
		}
		if (GUILayout.Button ("generate Level")) {
			levelGen.GenerateMap ();
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
		
	}
}

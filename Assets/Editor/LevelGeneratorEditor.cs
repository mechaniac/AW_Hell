using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {

	public override void OnInspectorGUI ()
	{
		LevelGenerator levGen = (LevelGenerator)target;

		if(DrawDefaultInspector()){
			if (levGen.autoUpdate) {
				levGen.GenerateMap ();
			}
		}
		if (GUILayout.Button ("generate Level")) {
			levGen.GenerateMap ();
			}
		}
	}

[CustomEditor(typeof(LevelGenerator_02_Maze))]
public class MazeEditor:Editor{

	public override void OnInspectorGUI ()
	{
		LevelGenerator_02_Maze levGen = (LevelGenerator_02_Maze)target;

		if(DrawDefaultInspector()){
			if (levGen.autoUpdate) {
				levGen.GenerateMap ();
			}
		}
		if (GUILayout.Button ("generate Level")) {
			levGen.GenerateMap ();
		}
	}
}


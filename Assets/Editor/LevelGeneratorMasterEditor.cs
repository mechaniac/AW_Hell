using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(LevelGeneratorMaster))]
public class LevelGeneratorMasterEditor : Editor
{


	public override void OnInspectorGUI ()
	{
		LevelGeneratorMaster levGenMaster = (LevelGeneratorMaster)target;
//		if(levGenMaster.autoUpdate){
//			
//		}
		if (DrawDefaultInspector ()) {
			switch (levGenMaster.generatedLevel) {
			case LevelGeneratorMaster.GeneratedLevel.FloorsAndBridges:
//				levGenMaster.MasterGenerateLevel ();
				break;
			case LevelGeneratorMaster.GeneratedLevel.Maze:
//				levGenMaster.MasterGenerateLevel ();
				break;
			case LevelGeneratorMaster.GeneratedLevel.Empty:
				levGenMaster.DestroyControlNodes ();
				break;
			default:
				break;

			}

			
		}


	}
}

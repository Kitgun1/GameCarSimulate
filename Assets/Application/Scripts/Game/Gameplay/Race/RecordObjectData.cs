using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CarSimulate.Game.Gameplay.Race
{
	[Serializable]
	public class RecordObjectData
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public float Time;
		public RecordSubObjectData[] SubRecords;
	}
	
	[Serializable]
	public class RecordSubObjectData
	{
		public string Name;
		public Vector3 LocalPosition;
		public Quaternion LocalRotation;
	}
}
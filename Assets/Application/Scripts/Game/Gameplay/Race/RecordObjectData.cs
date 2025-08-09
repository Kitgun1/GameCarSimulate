using System;

namespace CarSimulate.Game.Gameplay.Race
{
	[Serializable]
	public class RecordObjectData
	{
		public SaveVector3 Position;
		public SaveQuaternion Rotation;
		public float Time;
		public RecordSubObjectData[] SubRecords;
	}

	[Serializable]
	public class RecordSubObjectData
	{
		public string Name;
		public SaveVector3 LocalPosition;
		public SaveQuaternion LocalRotation;
	}

	[Serializable]
	public struct SaveVector3
	{
		public float X;
		public float Y;
		public float Z;

		public SaveVector3(UnityEngine.Vector3 vec)
		{
			X = vec.x;
			Y = vec.y;
			Z = vec.z;
		}
	}

	[Serializable]
	public struct SaveQuaternion
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public SaveQuaternion(UnityEngine.Quaternion quaternion)
		{
			X = quaternion.x;
			Y = quaternion.y;
			Z = quaternion.z;
			W = quaternion.w;
		}
	}

	public static class SaveExtensions
	{
		public static SaveVector3 ToSaveVector3(this UnityEngine.Vector3 vec) => new SaveVector3(vec);
		public static SaveQuaternion ToSaveQuaternion(this UnityEngine.Quaternion quaternion) => new SaveQuaternion(quaternion);

		public static UnityEngine.Vector3 ToVector3(this SaveVector3 vec) => new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
		public static UnityEngine.Quaternion ToQuaternion(this SaveQuaternion quat) => new UnityEngine.Quaternion(quat.X, quat.Y, quat.Z, quat.W);
	}
}
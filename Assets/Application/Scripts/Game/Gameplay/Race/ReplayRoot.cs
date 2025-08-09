using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CarSimulate.Game.Gameplay.Root;
using Coroutine;
using DI;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CarSimulate.Game.Gameplay.Race
{
	public class ReplayRoot
	{
		private List<RecordObjectData> _lastRaceRecord = new();
		private List<RecordObjectData> _currentRaceRecord = new();

		private IEnumerator _recordCoroutine;
		private IEnumerator _replayCoroutine;

		private ReplayObject _replayObject;

		private readonly ReplayObject _replayPrefab;
		private readonly Coroutines _coroutines;
		private readonly PlayerRoot _playerRoot;

		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		};

		public ReplayRoot(DIContainer container, ReplayObject replayPrefab)
		{
			_replayPrefab = replayPrefab;
			_coroutines = container.Resolve<Coroutines>();
			_playerRoot = container.Resolve<PlayerRoot>();
		}

		public void StartRecord()
		{
			var vehicleTransform = _playerRoot.RecordObject;
			_recordCoroutine = RecordRoutine(vehicleTransform);
			_coroutines.StartCoroutine(_recordCoroutine);
		}

		public void StopRecord()
		{
			_coroutines.StopCoroutine(_recordCoroutine);

			var json = JsonConvert.SerializeObject(_currentRaceRecord, Settings);
			PlayerPrefs.SetString("LastRaceRecord", json);
		}

		public bool TryStartLastReplay()
		{
			var json = PlayerPrefs.GetString("LastRaceRecord", string.Empty);
			if (json == string.Empty) return false;
			_lastRaceRecord = JsonConvert.DeserializeObject<List<RecordObjectData>>(json, Settings);

			_replayObject = Object.Instantiate(_replayPrefab);
			_replayCoroutine = ReplayRoutine();
			_coroutines.StartCoroutine(_replayCoroutine);
			return true;
		}

		public void TryStopReplay()
		{
			if (_lastRaceRecord == null || _lastRaceRecord.Count == 0) return;

			_coroutines.StopCoroutine(_replayCoroutine);
			Object.Destroy(_replayObject.gameObject);
		}

		private IEnumerator ReplayRoutine()
		{
			var startReplayTime = Time.time;
			var endReplayTime = _lastRaceRecord.Max(r => r.Time);
			var currentReplayTime = Time.time - startReplayTime;

			_lastRaceRecord = _lastRaceRecord.OrderBy(r => r.Time).ToList();


			while (currentReplayTime <= endReplayTime)
			{
				currentReplayTime = Time.time - startReplayTime;

				var previousFrame = _lastRaceRecord[0];
				var nextFrame = _lastRaceRecord[^1];
				var framesFound = false;
				for (int i = 0; i < _lastRaceRecord.Count; i++)
				{
					var nextIndex = Mathf.Clamp(i + 1, 0, _lastRaceRecord.Count - 1);
					if (_lastRaceRecord[i].Time <= currentReplayTime && _lastRaceRecord[nextIndex].Time >= currentReplayTime)
					{
						previousFrame = _lastRaceRecord[i];
						nextFrame = _lastRaceRecord[nextIndex];
						framesFound = true;
						break;
					}
				}
				if (!framesFound)
				{
					Debug.LogWarning($"frame is null: {currentReplayTime}.");
					yield return null;
					continue;
				}
				var t = Mathf.InverseLerp(previousFrame.Time, nextFrame.Time, currentReplayTime);
				_replayObject.Transform.SetPositionAndRotation(
					Vector3.Lerp(previousFrame.Position.ToVector3(), nextFrame.Position.ToVector3(), t),
					Quaternion.Slerp(previousFrame.Rotation.ToQuaternion(), nextFrame.Rotation.ToQuaternion(), t)
				);
				(string name, Transform origin)[] wheels = {
					(nameof(_replayObject.RearRightWheel), _replayObject.RearRightWheel), (nameof(_replayObject.FrontRightWheel), _replayObject.FrontRightWheel),
					(nameof(_replayObject.RearLeftWheel), _replayObject.RearLeftWheel), (nameof(_replayObject.FrontLeftWheel), _replayObject.FrontLeftWheel),
					(_replayObject.RearRightWheel.GetChild(0).name, _replayObject.RearRightWheel.GetChild(0)),
					(_replayObject.FrontRightWheel.GetChild(0).name, _replayObject.FrontRightWheel.GetChild(0)),
					(_replayObject.RearLeftWheel.GetChild(0).name, _replayObject.RearLeftWheel.GetChild(0)),
					(_replayObject.FrontLeftWheel.GetChild(0).name, _replayObject.FrontLeftWheel.GetChild(0)),
				};
				foreach (var wheel in wheels)
				{
					var previousSubRecord = previousFrame.SubRecords.First(r => r.Name == wheel.name);
					var nextSubRecord = nextFrame.SubRecords.First(r => r.Name == wheel.name);
					wheel.origin.SetLocalPositionAndRotation(
						Vector3.Lerp(previousSubRecord.LocalPosition.ToVector3(), nextSubRecord.LocalPosition.ToVector3(), t),
						Quaternion.Slerp(previousSubRecord.LocalRotation.ToQuaternion(), nextSubRecord.LocalRotation.ToQuaternion(), t)
					);
				}
				yield return null;
			}
		}

		private IEnumerator RecordRoutine(ReplayObject vehicleObject)
		{
			float startTime = Time.time;
			while (true)
			{
				_currentRaceRecord.Add(new RecordObjectData {
					Position = vehicleObject.Transform.position.ToSaveVector3(),
					Rotation = vehicleObject.Transform.rotation.ToSaveQuaternion(),
					Time = Time.time - startTime,
					SubRecords = new[] {
						new RecordSubObjectData {
							Name = nameof(vehicleObject.FrontLeftWheel),
							LocalPosition = vehicleObject.FrontLeftWheel.localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.FrontLeftWheel.localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = nameof(vehicleObject.FrontRightWheel),
							LocalPosition = vehicleObject.FrontRightWheel.localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.FrontRightWheel.localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = nameof(vehicleObject.RearLeftWheel),
							LocalPosition = vehicleObject.RearLeftWheel.localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.RearLeftWheel.localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = nameof(vehicleObject.RearRightWheel),
							LocalPosition = vehicleObject.RearRightWheel.localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.RearRightWheel.localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = vehicleObject.FrontLeftWheel.GetChild(0).name,
							LocalPosition = vehicleObject.FrontLeftWheel.GetChild(0).localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.FrontLeftWheel.GetChild(0).localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = vehicleObject.FrontRightWheel.GetChild(0).name,
							LocalPosition = vehicleObject.FrontRightWheel.GetChild(0).localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.FrontRightWheel.GetChild(0).localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = vehicleObject.RearLeftWheel.GetChild(0).name,
							LocalPosition = vehicleObject.RearLeftWheel.GetChild(0).localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.RearLeftWheel.GetChild(0).localRotation.ToSaveQuaternion(),
						},
						new RecordSubObjectData {
							Name = vehicleObject.RearRightWheel.GetChild(0).name,
							LocalPosition = vehicleObject.RearRightWheel.GetChild(0).localPosition.ToSaveVector3(),
							LocalRotation = vehicleObject.RearRightWheel.GetChild(0).localRotation.ToSaveQuaternion(),
						},
					}
				});
				yield return new WaitForFixedUpdate();

				if (_recordCoroutine == null)
				{
					yield break;
				}
			}
		}
	}
}
using System;
using Ashsvp;
using CarSimulate.Game.Gameplay.Root;
using CarSimulate.Game.Gameplay.Root.View;
using CarSimulate.Game.GameRoot;
using DI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CarSimulate.Game.Gameplay.Race
{
	public class PlayerRoot : IDisposable
	{
		public readonly ReplayObject RecordObject;
		public readonly SimcadeVehicleController VehicleController;
		private readonly FinishTrigger _finishTrigger;
		private readonly DIContainer _container;
		
		private PlayerCamera _playerCamera;

		// Это можно выделить в отдельное место для хранения информации игрока
		private int _currentRace;

		public PlayerRoot(DIContainer container, SimcadeVehicleController vehicleController, FinishTrigger finishTrigger)
		{
			_container = container;
			VehicleController = vehicleController;
			RecordObject = VehicleController.GetComponent<ReplayObject>();
			_finishTrigger = finishTrigger;
			
			_currentRace = PlayerPrefs.GetInt("Race", 0);
			_container.Resolve<UIGameplayRootView>().SetRaceCount(_currentRace);
			
			_finishTrigger.OnFinished += FinishTriggered;
		}

		public void Dispose()
		{
			_finishTrigger.OnFinished -= FinishTriggered;
		}
		
		public void Setup()
		{
			var playerCameraPrefab = Resources.Load<PlayerCamera>("Prefabs/Player Camera");
			_playerCamera = Object.Instantiate(playerCameraPrefab);
			_playerCamera.Setup(_container);
		}

		private void FinishTriggered()
		{
			PlayerPrefs.SetInt("Race", ++_currentRace);
			_container.Resolve<UIGameplayRootView>().SetRaceCount(_currentRace);
			_container.Resolve<GameplayEntryPoint>().ExitSceneEvent.Invoke(Scenes.GAMEPLAY);
		}
	}
}
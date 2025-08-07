using System;
using Ashsvp;
using CarSimulate.Game.Gameplay.Race;
using CarSimulate.Game.Gameplay.Root.View;
using CarSimulate.Game.GameRoot.View;
using DI;
using UnityCore.Input;
using UnityEngine;
using UnityEngine.Events;

namespace CarSimulate.Game.Gameplay.Root
{
	public class GameplayEntryPoint : MonoBehaviour
	{
		[Header("Triggers")]
		[SerializeField] private FinishTrigger _finishTrigger;
		
		[Header("Points")]
		[SerializeField] private Transform _spawnPoint;

		[Header("Prefabs")]
		[SerializeField] private UIGameplayRootView _sceneRootPrefab;
		[SerializeField] private SimcadeVehicleController _vehicleControllerPrefab;
		[SerializeField] private ReplayObject _replayPrefab;

		private GamplayInputActions _inputAction;
		private DIContainer _sceneContainer;
		private PlayerRoot _playerRoot;
		private ReplayRoot _replayRoot;
		public UnityEvent<string> ExitSceneEvent;

		public UnityEvent<string> Run(DIContainer sceneContainer)
		{
			_sceneContainer = sceneContainer;

			var uiSceneRootView = Instantiate(_sceneRootPrefab);
			_sceneContainer.Resolve<UIRootView>().AttachSceneUI(uiSceneRootView.gameObject);
			_sceneContainer.RegisterInstance(uiSceneRootView);

			_inputAction = new GamplayInputActions();
			_inputAction.Enable();
			_sceneContainer.RegisterInstance(_inputAction);

			var player = Instantiate(_vehicleControllerPrefab, _spawnPoint);
			player.InputActions = _inputAction;

			_playerRoot = new PlayerRoot(sceneContainer, player, _finishTrigger);
			_sceneContainer.RegisterInstance(_playerRoot);
			_playerRoot.Setup();

			_replayRoot = new ReplayRoot(sceneContainer, _replayPrefab);
			_sceneContainer.RegisterInstance(_replayRoot);
			_replayRoot.StartRecord();
			_replayRoot.TryStartLastReplay();

			ExitSceneEvent = new UnityEvent<string>();
			
			ExitSceneEvent.AddListener(targetScene =>
			{
				_replayRoot.StopRecord();
				_replayRoot.TryStopReplay();
			});

			return ExitSceneEvent;
		}

		private void OnDestroy()
		{
			_playerRoot?.Dispose();
		}
	}
}
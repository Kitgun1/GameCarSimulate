using CarSimulate.Game.GameRoot.View;
using DI;
using Unity.Cinemachine;
using UnityCore.Input;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CarSimulate.Game.Gameplay.Race
{
	public class PlayerCamera : MonoBehaviour
	{
		[SerializeField] private CinemachineCamera _cinemachine;
		[SerializeField] private Camera _camera;

		private DIContainer _container;

		public void Setup(DIContainer container)
		{
			_container = container;
			var uiRootCamera = container.Resolve<UIRootView>().UIRootCamera;
			_camera.GetUniversalAdditionalCameraData().cameraStack.Add(uiRootCamera);

			_cinemachine.Target.TrackingTarget = container.Resolve<PlayerRoot>().VehicleController.transform;
		}
	}
}
using System.Collections;
using CarSimulate.Game.Gameplay.Root;
using CarSimulate.Game.GameRoot.View;
using Coroutine;
using DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarSimulate.Game.GameRoot
{
	public class GameEntryPoint
	{
		private static GameEntryPoint _instance;

		private readonly DIContainer _rootContainer;
		private readonly Coroutines _coroutines;
		private readonly UIRootView _uiRoot;

		private DIContainer _cachedSceneContainer;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize()
		{
#if !UNITY_EDITOR // System settings setup
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

			_instance = new GameEntryPoint();
			_instance.RunGame();
		}

		private GameEntryPoint()
		{
			_rootContainer = new DIContainer();

			_coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
			Object.DontDestroyOnLoad(_coroutines.gameObject);
			_rootContainer.RegisterInstance(_coroutines);

			_uiRoot = Object.Instantiate(Resources.Load<UIRootView>("Prefabs/UIRootView"));
			Object.DontDestroyOnLoad(_uiRoot.gameObject);
			_rootContainer.RegisterInstance(_uiRoot);
		}

		private void RunGame()
		{
#if UNITY_EDITOR
			var currentSceneName = SceneManager.GetActiveScene().name;
			switch (currentSceneName)
			{
				case Scenes.GAMEPLAY:
					_coroutines.StartCoroutine(LoadAndStartGameplay());
					break;
			}
			if (currentSceneName != Scenes.BOOT) return;
#endif
			_coroutines.StartCoroutine(LoadAndStartGameplay());
		}

		private IEnumerator LoadAndStartGameplay()
		{
			_uiRoot.ShowLoadingScreen();
			_cachedSceneContainer?.Dispose();
			
			yield return LoadScene(Scenes.BOOT);
			yield return LoadScene(Scenes.GAMEPLAY);
			yield return null;

			var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
			
			var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
			sceneContainer.RegisterInstance(sceneEntryPoint);
			
			sceneEntryPoint.Run(sceneContainer).AddListener(sceneName =>
			{
				switch (sceneName)
				{
					case Scenes.GAMEPLAY:
						_coroutines.StartCoroutine(LoadAndStartGameplay());
						break;
					// Тут добавляем другие сцены на будущее
				}
			}); 
			
			_uiRoot.HideLoadingScreen();
		}

		private IEnumerator LoadScene(string sceneName)
		{
			yield return SceneManager.LoadSceneAsync(sceneName);
		}
	}
}
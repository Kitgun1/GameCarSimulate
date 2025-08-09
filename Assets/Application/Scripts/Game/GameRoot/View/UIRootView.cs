using UnityEngine;
using UnityEngine.UI;

namespace CarSimulate.Game.GameRoot.View
{
	public class UIRootView : MonoBehaviour
	{
		[SerializeField] private GameObject _loadingScreen;
		[SerializeField] private Transform _uiSceneContainer;
		[SerializeField] private Camera _uiRootCamera;
		[SerializeField] private RectTransform _uiCanvas;
		
		public Camera UIRootCamera => _uiRootCamera;

		public void ShowLoadingScreen() => _loadingScreen.SetActive(true);
		public void HideLoadingScreen() => _loadingScreen.SetActive(false);

		public void AttachSceneUI(GameObject sceneUI)
		{
			ClearSceneUI();
			
			sceneUI.transform.SetParent(_uiSceneContainer, false);
		}

		private void ClearSceneUI()
		{
			var childCount = _uiSceneContainer.childCount;
			for (var i = 0; i < childCount; i++)
			{
				Destroy(_uiSceneContainer.GetChild(i).gameObject);
			}
		}
	}
}
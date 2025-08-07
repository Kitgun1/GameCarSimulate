using TMPro;
using UnityEngine;

namespace CarSimulate.Game.Gameplay.Root.View
{
	public class UIGameplayRootView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _raceCounterTMP;

		private static readonly string RaceCountTemplateText = $"Текущий заезд:\n<size=72><color=#0053FF><b>%race_count%</b></color></size>";
		
		public void SetRaceCount(int raceCounter)
		{
			_raceCounterTMP.text = RaceCountTemplateText.Replace("%race_count%", raceCounter.ToString());
		}
	}
}
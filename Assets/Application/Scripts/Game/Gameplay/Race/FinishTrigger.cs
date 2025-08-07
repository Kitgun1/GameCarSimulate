using System;
using UnityEngine;

namespace CarSimulate.Game.Gameplay.Race
{
	[RequireComponent(typeof(Collider))]
	public class FinishTrigger : MonoBehaviour
	{
		// Тут я бы еще R3(реактивность) поставил вместо Action
		public event Action OnFinished;

		private void Awake() => GetComponent<Collider>().isTrigger = true;

		private void OnTriggerEnter(Collider other)
		{
			OnFinished?.Invoke();
		}
	}
}
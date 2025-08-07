using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ashsvp
{
    public class ResetVehicle : MonoBehaviour
    {
        private GamplayInputActions _inputActions;

        private void Start()
        {
            _inputActions = GetComponent<SimcadeVehicleController>().InputActions;
        }

        public void resetVehicle()
        {
            var pos = transform.position;
            pos.y += 1;
            transform.position = pos;
            transform.rotation = Quaternion.identity;
        }
        public void Quit()
        {
            Application.Quit();
        }
        public void ResetScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        private void Update()
        {
            if (_inputActions.Player.Reset.ReadValue<bool>())
            {
                resetVehicle();
            }
        }
    }
}

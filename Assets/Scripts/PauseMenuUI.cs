using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoolDawn
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Canvas pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button optionsButton;
        
        private void Awake() {
            pauseMenu.enabled = false;
            resumeButton.onClick.AddListener(Pause);
            quitButton.onClick.AddListener(() =>{
                Loader.Load(Loader.Scene.MainMenuScene);
            });
            optionsButton.onClick.AddListener(() =>
            {
                // Open options menu
            });
        }

        private void Start()
        {
            InputManager.Instance.Pause += InputManager_Pause;
        }

        private void InputManager_Pause(object sender, System.EventArgs e)
        {
            Pause();
        }
        
        private void Pause()
        {
            pauseMenu.enabled = !pauseMenu.enabled;
            Time.timeScale = pauseMenu.enabled ? 0 : 1;
            resumeButton.Select();
        }
    }
}

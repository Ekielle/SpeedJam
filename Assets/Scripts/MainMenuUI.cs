using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Canvas leaderboard;
    [SerializeField] private Button exitLeaderboardButton;

    private void Awake() {
        leaderboard.enabled = false;
        playButton.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.LVL1);
        });
        quitButton.onClick.AddListener(() =>{
            Application.Quit();
        });
        leaderboardButton.onClick.AddListener(() =>
        {
            leaderboard.enabled = true;
            exitLeaderboardButton.Select();
        });
        exitLeaderboardButton.onClick.AddListener(() =>
        {
            leaderboard.enabled = false;
            playButton.Select();
        });
    } 

}

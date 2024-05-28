using System;
using System.Collections;
using System.Threading.Tasks;
using LootLocker.Requests;
using UnityEngine;

namespace CoolDawn
{
    public enum LeaderboardIds
    {
        Level1 = 22597,
        Level2 = 22599,
        Level3 = 22600,
        Global = 22601
    }
    public class LootLockerManager : MonoBehaviour
    {
        [SerializeField] private Leaderboard leaderboard;
        
        public string playerName;
        public int timer;
        public int levelTimer;
        public static LootLockerManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            StartCoroutine(Clock());
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    Debug.LogError("error starting LootLocker session");

                    return;
                }

                Debug.Log("successfully started LootLocker session");
                leaderboard?.StartGetLeaderboard();
            });
        }

        public void SubmitLvl1Score()
        {
            string playerId = PlayerPrefs.GetString("PlayerId");
            int score = levelTimer;

            LootLockerSDKManager.SubmitScore(playerId, score, LeaderboardIds.Level1.ToString(), playerName, (response) =>
            {
                if (response.statusCode == 200) {
                    Debug.Log("Successful");
                } else {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
        
        public void SubmitLvl2Score()
        {
            string playerId = PlayerPrefs.GetString("PlayerId");
            int score = levelTimer;

            LootLockerSDKManager.SubmitScore(playerId, score, LeaderboardIds.Level2.ToString(), playerName, (response) =>
            {
                if (response.statusCode == 200) {
                    Debug.Log("Successful");
                } else {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
        
        public void SubmitLvl3Score()
        {
            string playerId = PlayerPrefs.GetString("PlayerId");
            int score = levelTimer;

            LootLockerSDKManager.SubmitScore(playerId, score, LeaderboardIds.Level3.ToString(), playerName, (response) =>
            {
                if (response.statusCode == 200) {
                    Debug.Log("Successful");
                } else {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
        
        public void SubmitGlobalScore()
        {
            string playerId = PlayerPrefs.GetString("PlayerId");
            int score = timer;

            LootLockerSDKManager.SubmitScore(playerId, score, LeaderboardIds.Global.ToString(), playerName, (response) =>
            {
                if (response.statusCode == 200) {
                    Debug.Log("Successful");
                } else {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
        
        public async Task<string> GetTopLeaderboard(LeaderboardIds leaderboardId)
        {
            var tcs = new TaskCompletionSource<string>();
            
            LootLockerSDKManager.GetScoreList(((int)leaderboardId).ToString(), 100, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    tcs.SetResult(response.text);
                }
                else
                {
                    tcs.SetException(new Exception("Failed to get leaderboard: " + response.errorData));
                }
            });

            return await tcs.Task;
        }
        
        private IEnumerator Clock()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                levelTimer++;
                timer++;
            }
        }

        public void ResetLevelTimer()
        {
            levelTimer = 0;
        }
        
        public void ResetGlobalTimer()
        {
            timer = 0;
        }
    }
}

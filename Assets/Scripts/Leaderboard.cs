using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CoolDawn
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private Transform recordTemplate;
        
        public void StartGetLeaderboard()
        {
            StartCoroutine(GetTopLeaderboard(LeaderboardIds.Level1));
        }

        private IEnumerator GetTopLeaderboard(LeaderboardIds leaderboardId)
        {
            yield return new WaitForSecondsRealtime(1f);
            var leaderboardDataTask = LootLockerManager.Instance.GetTopLeaderboard(leaderboardId);
            yield return new WaitUntil(() => leaderboardDataTask.IsCompleted);

            if (leaderboardDataTask.IsFaulted)
            {
                Debug.LogError("Failed to get leaderboard data: " + leaderboardDataTask.Exception);
            }
            else
            {
                string leaderboardData = leaderboardDataTask.Result;
                Debug.Log(leaderboardData);
            }
        }
    }
}
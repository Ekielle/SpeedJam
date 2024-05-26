using System;
using UnityEngine;

namespace CoolDawn
{
    public class LevelEndingTrigger : MonoBehaviour
    {
        [SerializeField] private Loader.Scene nextScene;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Loader.Load(nextScene);
        }
    }
}
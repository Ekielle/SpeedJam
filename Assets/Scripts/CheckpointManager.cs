using System;
using UnityEngine;

namespace CoolDawn
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }
        
        [SerializeField] private CheckPoint initialCheckpoint;
        private CheckPoint _currentCheckpoint;

        private void Awake()
        {
            Instance = this;
            _currentCheckpoint = initialCheckpoint;
        }

        public void SetCurrentCheckpoint(CheckPoint checkPoint)
        {
            _currentCheckpoint = checkPoint;
        }
        
        public Vector2 GetCurrentCheckpointPosition()
        {
            return _currentCheckpoint.transform.position;
        }
    }
}
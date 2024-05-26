using UnityEngine;

namespace CoolDawn
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private Trigger trigger;
        [SerializeField] private bool active;
        [SerializeField] private Transform activeVisual;
        [SerializeField] private Transform inactiveVisual;
        
        private void Awake()
        {
            trigger.Triggered += Trigger_Triggered;
            activeVisual.gameObject.SetActive(active);
            inactiveVisual.gameObject.SetActive(!active);
        }
        
        private void Trigger_Triggered(object sender, System.EventArgs e)
        {
            if (active) return;
            Activate();
            CheckpointManager.Instance.SetCurrentCheckpoint(this);
        }
        
        private void Activate()
        {
            activeVisual.gameObject.SetActive(true);
            inactiveVisual.gameObject.SetActive(false);
        }
    }
}
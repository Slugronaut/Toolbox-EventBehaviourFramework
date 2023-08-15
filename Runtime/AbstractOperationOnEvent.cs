using UnityEngine;

namespace Toolbox
{

    /// <summary>
    /// Base class that can be used to implement event triggers.
    /// 
    /// TODO: The OnRelenquish method is no longer used!!
    /// </summary>
    public abstract class AbstractOperationOnEvent : MonoBehaviour
    {
        [Tooltip("How long after the trigger count is reached before performing the set operation.")]
        public float Delay = 0;
        [Tooltip("How many times the event must occur before the operation is performed.")]
        public int TriggerCount = 1;
        [Tooltip("Does the counter reset after the operation is triggered? Once this object has been instantiated, the initial TriggerCount will always be used as the default reset value.")]
        public bool Resets = true;
        [Tooltip("When does the trigger count increment? Note that some events like Awake and Start will only occur once so the TriggerCount *must* be set to 1 for them to work.")]
        public EventAndCollisionTiming TriggerPoint = EventAndCollisionTiming.None;
        [Tooltip("Many events will not be registered if this object is disabled. This will override that setting. This is ignored by the 'Disabled' triaggers point since it must always occur.")]
        public bool AllowIfDisabled = false;

        int CachedCount;

        protected virtual void Awake()
        {
            CachedCount = TriggerCount;
            if (TriggerPoint == EventAndCollisionTiming.Awake)
                TryPerformOp();
        }

        void OnEnable()
        {
            if (TriggerPoint == EventAndCollisionTiming.Enable)
                TryPerformOp();
        }

        void Start()
        {
            if (TriggerPoint == EventAndCollisionTiming.Start)
                TryPerformOp();
        }

        void OnDisable()
        {
            if (TriggerPoint == EventAndCollisionTiming.Disable)
                TryPerformOp();
        }

        void OnDestroy()
        {
            if (TriggerPoint == EventAndCollisionTiming.Destroy)
                TryPerformOp();
        }

        #if TOOLBOX_2DCOLLIDER
        void OnTriggerEnter2D(Collider2D other)
        {
            if (TriggerPoint == EventAndCollisionTiming.Triggered)
                TryPerformOp();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (TriggerPoint == EventAndCollisionTiming.TriggerExit)
                TryPerformOp();
        }
        #else
        void OnTriggerEnter(Collider other)
        {
            if (TriggerPoint == EventAndCollisionTiming.Triggered)
                TryPerformOp();
        }

        void OnTriggerExit(Collider other)
        {
            if (TriggerPoint == EventAndCollisionTiming.TriggerExit)
                TryPerformOp();
        }
        #endif

        void OnRelenquish()
        {
            if (TriggerPoint == EventAndCollisionTiming.Relenquished)
                TryPerformOp();
        }


        protected void TryPerformOp()
        {
            if (!isActiveAndEnabled && TriggerPoint != EventAndCollisionTiming.Disable && !AllowIfDisabled)
                return;
            TriggerCount--;
            if (TriggerCount == 0)
            {
                if (Resets) TriggerCount = CachedCount;
                if (Delay > 0) Invoke("PerformOp", Delay);
                else PerformOp();
            }

        }

        public abstract void PerformOp();
    }

}
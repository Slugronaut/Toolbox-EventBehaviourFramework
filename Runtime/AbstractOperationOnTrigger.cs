using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg
{
    /// <summary>
    /// Base class that can be used to implement event triggers.
    /// </summary>
    public abstract class AbstractOperationOnTrigger : SerializedMonoBehaviour
    {
        [Tooltip("How long after the trigger count is reached before performing the set operation.")]
        public float Delay = 0;
        [Tooltip("How many times the event must occur before the operation is performed.")]
        public int TriggerCount = 1;
        [Tooltip("Does the counter reset after the operation is triggered? Once this object has been instantiated, the initial TriggerCount will always be used as the default reset value.")]
        public bool Resets = true;
        [Tooltip("When does the trigger count increment? Note that some events like Awake and Start will only occur once so the TriggerCount *must* be set to 1 for them to work.")]
        public CollisionTiming TriggerPoint = CollisionTiming.None;

        int CachedCount;
        Collider Cache;

        protected virtual void Awake()
        {
            CachedCount = TriggerCount;
        }

        protected virtual void OnDestroy()
        {
            Cache = null;
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
            if (TriggerPoint == CollisionTiming.Enter)
                TryPerformOp(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (TriggerPoint == CollisionTiming.Exit)
                TryPerformOp(other);
        }
        #endif

        protected void TryPerformOp(Collider other)
        {
            TriggerCount--;
            if (TriggerCount == 0)
            {
                if (Resets) TriggerCount = CachedCount;
                if (Delay > 0)
                {
                    Cache = other;
                    Invoke("DelayedOp", Delay);
                }
                else PerformOp(other);
            }

        }

        void DelayedOp()
        {
            PerformOp(Cache);
            Cache = null;
        }

        public abstract void PerformOp(Collider other);
    }

}
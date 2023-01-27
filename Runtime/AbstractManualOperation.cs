using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Reflection;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace Toolbox
{

    /// <summary>
    /// Base class that can be used to implement event triggers. The derived behaviour will
    /// expose the 'TryPerformOp' method that can be rigged to UnityEvents in the inspector.
    /// </summary>
    public abstract class AbstractManualOperation : SerializedMonoBehaviour
    {
        [Tooltip("How long after the trigger count is reached before performing the set operation.")]
        public float Delay = 0;
        [Tooltip("How many times the event must occur before the operation is performed.")]
        public int TriggerCount = 1;
        [Tooltip("Does the counter reset after the operation is triggered? Once this object has been instantiated, the initial TriggerCount will always be used as the default reset value.")]
        public bool Resets = true;
        [Tooltip("Many events will not be registered if this object is disabled. This will override that setting. This is ignored by the 'Disabled' triaggers point since it must always occur.")]
        public bool AllowIfDisabled = false;

        int CachedCount;

        protected virtual void Awake()
        {
            CachedCount = TriggerCount;
        }
        
        protected void TryPerformOp()
        {
            if (!isActiveAndEnabled && !AllowIfDisabled)
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
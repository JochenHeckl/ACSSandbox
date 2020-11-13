using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
    public interface IContext
    {
        void EnterContext( IContextContainer contextContainer );
        void LeaveContext( IContextContainer contextContainer );
        void ActivateContext( IContextContainer contextContainer );
        void DeactivateContext( IContextContainer contextContainer );

        void Update( IContextContainer contextContainer, float deltaTimeSec );
    }
}
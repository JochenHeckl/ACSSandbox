using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace de.JochenHeckl.Unity.ACSSandbox
{
    public interface IState
    {
        void EnterState( IStateMachine stateMachine );
        void ActivateState( IStateMachine stateMachine );
        void DeactivateState( IStateMachine stateMachine );
        void LeaveState( IStateMachine stateMachine );

        void UpdateState( IStateMachine stateMachine, float deltaTimeSec );
    }
}
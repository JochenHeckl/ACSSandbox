using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
    public interface IContext
    {
        void EnterContext( IContext fromContext );
        void LeaveContext( IContext toContext );

        void Update( float deltaTimeSec );
    }
}
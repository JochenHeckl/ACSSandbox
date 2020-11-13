using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
    public interface IUnitData
    {
        long UnitId { get; }
        UnitTypeId UnityTypeId { get; set; }
        
        string ControllingUserId { get; set; }

        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}
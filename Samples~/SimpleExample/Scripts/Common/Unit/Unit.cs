using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{

    public interface IUnit
    {
        Guid UnitId { get; set; }
        UnitType UnityType { get; set; }

    }
}
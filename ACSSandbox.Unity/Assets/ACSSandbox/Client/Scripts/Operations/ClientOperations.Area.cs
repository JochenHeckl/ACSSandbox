using System;
using System.Linq;
using JH.DataBinding;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Client
{
    public partial class ClientOperations
    {
        private void EnterArea(string areaId)
        {
            // Log.Info("Entering area: {AreaId}.", areaId);

            // var areaToEnter = runtimeData.Resources.Areas.Single(x => x.areaId == areaId);

            // if (areaToEnter == null)
            // {
            //     throw new InvalidOperationException(
            //         $"Failed to enter unknown area ith id: {areaId}"
            //     );
            // }

            // runtimeData.ActiveArea = UnityEngine.Object.Instantiate(
            //     areaToEnter,
            //     runtimeData.WorldRootTransform
            // );
        }
    }
}

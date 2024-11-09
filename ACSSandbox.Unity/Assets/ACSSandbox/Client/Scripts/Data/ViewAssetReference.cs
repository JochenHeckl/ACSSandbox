using System;
using ACSSandbox.Common;
using JH.DataBinding;
using UnityEngine;

namespace ACSSandbox.Client
{
    [Serializable]
    public class ViewAssetReference : ComponentAssetReference<View>
    {
        public ViewAssetReference(string guid)
            : base(guid) { }
    }
}

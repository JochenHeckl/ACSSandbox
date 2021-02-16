using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Common
{
	public static class GameObjectExtensions
	{
		public static void RecursiveMoveToLayer( this GameObject gameObject, int layer )
		{
			gameObject.layer = layer;

			for ( var childIndex = 0; childIndex < gameObject.transform.childCount; childIndex++ )
			{
				RecursiveMoveToLayer( gameObject.transform.GetChild( childIndex ).gameObject, layer );
			}
		}
	}
}

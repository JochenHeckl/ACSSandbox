using Cinemachine;
using UnityEngine;

public class WorldCameraInputAxisProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
	public float GetAxisValue( int axis )
	{
		if ( axis == 0 )
		{
			if ( Input.GetKey( KeyCode.Mouse0 ) )
			{
				return Input.GetAxis("Mouse X");
			}
			return 0f;
		}

		if ( axis == 1 )
		{
			return Input.GetAxis( "Mouse ScrollWheel" );
		}

		return 0f;
	}
}

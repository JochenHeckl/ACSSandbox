using System;

using UnityEngine;

using de.JochenHeckl.Unity.DataBinding;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	[RequireComponent( typeof( CanvasGroup ) )]
	public class ContextUIView : ViewBehaviour
	{
		private enum FadeState
		{
			None = 0,
			FadeIn = 1,
			FadeOut = 2
		};

		[Space( 4 )]
		[Header( "ClientView" )]
		public float fadeInTimeSec;

		private CanvasGroup canvasGroup;
		private FadeState fadeState;

		private float displayAlpha;

		public void Start()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
		}

		public virtual void Update()
		{
			switch ( fadeState )
			{
				case FadeState.None:
					break;

				case FadeState.FadeIn:
				{
					if ( fadeInTimeSec == 0f )
					{
						displayAlpha = 1f;
						fadeState = FadeState.None;
					}
					else
					{
						displayAlpha += Time.deltaTime / fadeInTimeSec;
						displayAlpha = Math.Min( 1f, displayAlpha );
					}

					if ( displayAlpha == 1f )
					{
						fadeState = FadeState.None;
					}

					break;
				}

				case FadeState.FadeOut:
				{
					if ( fadeInTimeSec == 0f )
					{
						displayAlpha = 0f;
						fadeState = FadeState.None;
					}
					else
					{
						displayAlpha -= Time.deltaTime / fadeInTimeSec;
						displayAlpha = Math.Max( 0f, displayAlpha );
					}

					if ( displayAlpha == 0f )
					{
						fadeState = FadeState.None;
					}

					break;
				}

				default:
					throw new InvalidOperationException( $"Unknown fadeState {fadeState} detected." );


			}

			canvasGroup.alpha = displayAlpha;
		}

		public void Show()
		{
			fadeState = FadeState.FadeIn;
		}

		public void Hide()
		{
			fadeState = FadeState.FadeOut;
			Destroy( gameObject, fadeInTimeSec );
		}
	}
}

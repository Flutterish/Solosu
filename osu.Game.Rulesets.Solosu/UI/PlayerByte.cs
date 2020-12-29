using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Utils;
using osu.Game.Rulesets.Solosu.Collections;
using osu.Game.Rulesets.Solosu.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace osu.Game.Rulesets.Solosu.UI {
	public class PlayerByte : CompositeDrawable, IKeyBindingHandler<SolosuAction> {
		List<SolosuAction> moves = new();
		// TimeSeekableList<InputState> allMoves = new(); // we need this because otherwise its impossible to tell what movement was buffered when rewinding ( you cant tell which index a move was removed from )

		[Resolved]
		Dictionary<SolosuLane, Lane> lanes { get; set; }
		public SolosuLane Lane { get; private set; } = SolosuLane.Center;

		PlayerVisual @byte;
		PlayerLine line;
		Dictionary<SolosuLane, BufferIndicator> bufferIndicators = new();
		public PlayerByte () {
			AddInternal( line = new PlayerLine { Origin = Anchor.Centre, Anchor = Anchor.Centre } );
			AddInternal( @byte = new PlayerVisual { Origin = Anchor.Centre, Anchor = Anchor.Centre } );
			Origin = Anchor.Centre;
			AutoSizeAxes = Axes.Y;
			Width = 500;
			Masking = true;
			// allMoves.Add( double.NegativeInfinity, new InputState( moves ) );
		}

		List<SolosuAction> held = new();
		public bool OnPressed ( SolosuAction action ) {
			if ( action.IsMovement() ) {
				//if ( allMoves.AnyAfter( Clock.CurrentTime ) ) {
				//	allMoves.ClearAfter( Clock.CurrentTime );
				//	allMoves.At( Clock.CurrentTime ).Restore( moves );
				//	updatePosition();
				//	FinishTransforms( true );
				//
				//	return false;
				//}

				moves.Add( action );
				//allMoves.Add( Clock.CurrentTime, new InputState( moves ) );
				updatePosition();
			}
			else if ( action.IsAction() ) {
				held.Add( action );
				@byte.ScaleTo( 0.8f, 20 );
			}

			return false;
		}

		public void OnReleased ( SolosuAction action ) {
			if ( action.IsMovement() ) {
				//if ( allMoves.AnyAfter( Clock.CurrentTime ) ) {
				//	allMoves.ClearAfter( Clock.CurrentTime );
				//	allMoves.At( Clock.CurrentTime ).Restore( moves );
				//	updatePosition();
				//	FinishTransforms( true );
				//
				//	return;
				//}

				moves.Remove( action );
				//allMoves.Add( Clock.CurrentTime, new InputState( moves ) );
				updatePosition();
			}
			else if ( action.IsAction() ) {
				held.Remove( action );
				if ( held.IsEmpty() ) @byte.ScaleTo( 1, 50 );
			}
		}

		void updatePosition () {
			SolosuLane lane = SolosuLane.Center;
			if ( moves.Any() ) {
				lane = moves.Last().GetLane();
			}

			if ( lane != this.Lane ) {
				this.Lane = lane;
				@byte.MoveTo( lanes[ lane ] );
			}

			foreach ( var i in bufferIndicators ) {
				i.Value.IsVisible = moves.Contains( i.Key.GetAction() ) && moves.Last() != i.Key.GetAction();
			}
		}

		public void TakeDamage () {
			@byte.FlashColour( Colours.Miss, 500, Easing.Out );
			@byte.RotateTo( RNG.NextSingle( -30, 30 ) ).Then().RotateTo( 0, 500, Easing.In );
		}

		protected override void Update () {
			//if ( allMoves.AnyAfter( Clock.CurrentTime ) ) {
			//	allMoves.ClearAfter( Clock.CurrentTime );
			//	allMoves.At( Clock.CurrentTime ).Restore( moves );
			//	updatePosition();
			//	FinishTransforms( true );
			//}

			line.X = @byte.X / 4;
		}

		[Resolved]
		public SolosuColours Colours { get; private set; }
		[BackgroundDependencyLoader]
		private void load () {
			foreach ( var i in lanes ) {
				var indicator = new BufferIndicator { Size = new Vector2( 12 ), X = i.Value.X, Anchor = Anchor.Centre, Origin = Anchor.Centre, Colour = Colours.Perfect };
				AddInternal( indicator );
				bufferIndicators.Add( i.Key, indicator );
			}
			@byte.Colour = Colours.Perfect;
			line.Colour = Colours.Regular;
		}

		private class BufferIndicator : SpriteIcon {
			public BufferIndicator () {
				Alpha = 0;
				Icon = FontAwesome.Solid.Star;
			}
			bool isVisible = false;
			public bool IsVisible {
				get => isVisible;
				set {
					if ( isVisible == value ) return;
					isVisible = value;

					FinishTransforms();
					if ( IsVisible )
						Alpha = 1;
					else
						this.FadeOut( 100 );
				}
			}
		}

		private class PlayerVisual : CompositeDrawable {
			Box box;
			public PlayerVisual () {
				AddInternal( box = new Box { Size = new Vector2( MathF.Sqrt( volume ) ), Origin = Anchor.Centre, Anchor = Anchor.Centre } );
				Origin = Anchor.Centre;
				Masking = true;
				CornerRadius = 8;
				AutoSizeAxes = Axes.Both;
			}
			private float volume = 32 * 32;
			public void MoveTo ( Lane lane ) {
				FinishTransforms( true );
				this.MoveToX( lane.X, 50, Easing.Out );

				float distance = MathF.Abs( X - lane.X );
				float height = MathF.Sqrt( volume ) - MathF.Sqrt( distance ) / 4;

				box.ResizeTo( new Vector2( volume / height, height ), 40 ).Then().ResizeTo( MathF.Sqrt( volume ), 10 );
			}
		}

		private class PlayerLine : CompositeDrawable {
			public PlayerLine () {
				Origin = Anchor.Centre;
				Anchor = Anchor.Centre;
				AddInternal( new Sprite { Width = 500, Height = 2, Origin = Anchor.Centre, Anchor = Anchor.Centre, Texture = SolosuTextures.WidthFade( 500, 2 ), Alpha = 0.6f } );
				// TODO maybe add the key being held effect from the key overlay when on the side
			}
		}
	}

	public struct InputState {
		public int LeftIndex;
		public int RightIndex;
		public int CenterIndex;

		public InputState ( List<SolosuAction> actions ) {
			LeftIndex = actions.IndexOf( SolosuAction.Left );
			RightIndex = actions.IndexOf( SolosuAction.Right );
			CenterIndex = actions.IndexOf( SolosuAction.Center );
		}

		public void Restore ( List<SolosuAction> actions ) {
			actions.Clear();
			if ( !tryAddIndex( 0, actions ) ) return;
			if ( !tryAddIndex( 1, actions ) ) return;
			if ( !tryAddIndex( 2, actions ) ) return;
		}

		private bool tryAddIndex ( int index, List<SolosuAction> actions ) {
			if ( LeftIndex == index )
				actions.Add( SolosuAction.Left );
			else if ( RightIndex == index )
				actions.Add( SolosuAction.Right );
			else if ( CenterIndex == index )
				actions.Add( SolosuAction.Center );
			else return false;

			return true;
		}
	}
}

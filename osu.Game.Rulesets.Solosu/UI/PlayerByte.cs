using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Solosu.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.UI {
	public class PlayerByte : CompositeDrawable, IKeyBindingHandler<SolosuAction> {
		List<SolosuAction> moves = new();
		[Resolved]
		Dictionary<SolosuLane, Lane> lanes { get; set; }
		public SolosuLane Lane { get; private set; } = SolosuLane.Center;

		PlayerVisual @byte;
		Dictionary<SolosuLane, BufferIndicator> bufferIndicators = new();
		public PlayerByte () {
			AddInternal( @byte = new PlayerVisual { Origin = Anchor.Centre, Anchor = Anchor.Centre } );
			Origin = Anchor.Centre;
			Masking = true;
			CornerRadius = 8;
			AutoSizeAxes = Axes.Both;
		}

		public bool OnPressed ( SolosuAction action ) {
			if ( action.IsMovement() ) {
				moves.Add( action );
				updatePosition();
			}
			else if ( action.IsAction() ) {
				@byte.ScaleTo( 0.8f, 20 ).Then().ScaleTo( 1, 50 );
			}

			return false;
		}

		public void OnReleased ( SolosuAction action ) {
			if ( action.IsMovement() ) {
				moves.Remove( action );
				updatePosition();
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

		[Resolved]
		public SolosuColours Colours { get; private set; }
		[BackgroundDependencyLoader]
		private void load () {
			Colour = Colours.Perfect;
			foreach ( var i in lanes ) {
				var indicator = new BufferIndicator { Size = new Vector2( 10 ), X = i.Value.X, Anchor = Anchor.Centre, Origin = Anchor.Centre };
				AddInternal( indicator );
				bufferIndicators.Add( i.Key, indicator );
			}
		}

		private class BufferIndicator : Circle {
			public BufferIndicator () {
				Alpha = 0;
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
	}
}

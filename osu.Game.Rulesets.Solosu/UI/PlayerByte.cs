using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Solosu.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.UI { // TODO when hit by laser, show an animation
	public class PlayerByte : CompositeDrawable, IKeyBindingHandler<SolosuAction> {
		List<SolosuAction> moves = new();
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
		}

		List<SolosuAction> held = new();
		public bool OnPressed ( SolosuAction action ) {
			if ( action.IsMovement() ) {
				moves.Add( action ); // BUG these get added/removed in the wrong places when rewinding
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
				moves.Remove( action );
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

		protected override void Update () {
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
}

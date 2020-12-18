﻿using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Solosu.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu.Game.Rulesets.Solosu.UI {
	public class PlayerByte : CompositeDrawable, IKeyBindingHandler<SolosuAction> {
		List<SolosuAction> moves = new();
		[Resolved]
		Dictionary<SolosuLane, Lane> lanes { get; set; }
		SolosuLane lane = SolosuLane.Center;

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
			if ( action is SolosuAction.Left or SolosuAction.Right or SolosuAction.Center ) {
				moves.Add( action );
				updatePosition();
			}

			return false;
		}

		public void OnReleased ( SolosuAction action ) {
			if ( action is SolosuAction.Left or SolosuAction.Right or SolosuAction.Center ) {
				moves.Remove( action );
				updatePosition();
			}
		}

		void updatePosition () {
			SolosuLane lane = SolosuLane.Center;
			if ( moves.Any() ) {
				lane = moves.Last().GetLane();
			}

			if ( lane != this.lane ) {
				this.lane = lane;
				@byte.MoveTo( lanes[ lane ] );
			}

			foreach ( var i in bufferIndicators ) {
				i.Value.IsVisible = moves.Contains( i.Key.GetAction() ) && moves.Last() != i.Key.GetAction();
			}
		}

		[Resolved( name: nameof( SolosuPlayfield.AccentColour ) )]
		Bindable<Colour4> accentColour { get; set; }
		[BackgroundDependencyLoader]
		private void load () {
			accentColour.BindValueChanged( v => Colour = v.NewValue, true );
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

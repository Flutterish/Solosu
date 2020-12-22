using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.Solosu.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System;
using System.Linq;
using static osu.Game.Rulesets.Solosu.UI.SolosuPlayfield;

namespace osu.Game.Rulesets.Solosu.UI {
	[Cached]
	public class Lane : Playfield {
		public Lane () {
			AddRangeInternal( new Drawable[] {
				HitObjectContainer,
			} );

			HitObjectContainer.Origin = Anchor.BottomCentre;
			HitObjectContainer.Anchor = Anchor.BottomCentre;

			RegisterPool<Packet, DrawablePacket>( 20 );
			RegisterPool<Stream, DrawableStream>( 5 );
		}


		[Resolved( name: nameof( SolosuPlayfield.ScrollDuration ) )]
		public BindableDouble ScrollDuration { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.HitHeight ) )]
		public BindableDouble HitHeight { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.KiaiBindable ) )]
		public BindableBool KiaiBindable { get; private set; } // BUG when rewinding some transforms this triggers dont revert ( LazerSpeed ). we could check if time is reverted and apply the transforms <duration> before.

		public readonly BindableDouble LazerSpeed = new( 1 );

		[BackgroundDependencyLoader]
		private void load () {
			KiaiBindable.BindValueChanged( v => {
				if ( v.NewValue ) {
					this.TransformBindableTo( LazerSpeed, 2, 400 );
				}
				else {
					this.TransformBindableTo( LazerSpeed, 1, 400 );
				}
			} );

			ScrollDuration.BindValueChanged( v => {
				foreach ( var i in HitObjectContainer.AliveObjects.OfType<DrawableSolosuHitObject>().Where( x => x.UsesPositionalAnimations ) ) {
					i.ReapplyTransforms(); // NOTE might be quite eqpensive. would be nice if we just used a curve that follows y
				}
			} );
		}

		public void EmergeFromTheCube ( DrawableSolosuHitObject ho ) {
			if ( !ho.UsesPositionalAnimations ) throw new InvalidOperationException( "Cannot apply positional animations to non positional hit objects" );
			var timeAtCube = TimeAtHeight( CubeHeight, ho.HitObject.StartTime );

			ho.MoveToX( -X );
			ho.FadeOut();
			ho.RotateTo( MathF.Atan2( -20, -X ) * 180 / MathF.PI + 90 );

			using ( ho.BeginAbsoluteSequence( timeAtCube - 100 ) ) {
				ho.Delay( 100 ).MoveToX( 0, 200, Easing.Out ).RotateTo( 0, 200 );
				ho.FadeInFromZero( 500 );
			}
		}

		public double TimeAtHeight ( double height, double hitTime, double speed = 1 )
			=> hitTime - ( height - HitHeight.Value ) * ScrollDuration.Value / SCROLL_HEIGHT / speed;

		public double HeightAtTime ( double time, double hitTime, double speed = 1 ) {
			var position = ( hitTime - time ) / ScrollDuration.Value;
			var height = speed * SCROLL_HEIGHT * position;
			return height + HitHeight.Value;
		}

		public double CubeHeight => DrawHeight - 150 / 2 - 70;
	}
}

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.Solosu.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System;
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
		}


		[Resolved( name: nameof( SolosuPlayfield.ScrollDuration ) )]
		public BindableDouble ScrollDuration { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.HitHeight ) )]
		public BindableDouble HitHeight { get; private set; }

		public void ApplyInitialTransformsTo ( DrawableSolosuHitObject ho ) {
			ho.MoveToX( -X );
			ho.FadeOut();
			ho.RotateTo( MathF.Atan2( -20, -X ) * 180 / MathF.PI + 90 );

			var timeAtCube = -( ( DrawHeight - HitHeight.Value - 150 / 2 /*cube size*/ - 70 /*cube position*/ ) * ScrollDuration.Value / SCROLL_HEIGHT - ho.HitObject.StartTime );
			using ( ho.BeginAbsoluteSequence( timeAtCube - 100 ) ) {
				ho.Delay( 100 ).MoveToX( 0, 200, Easing.Out ).RotateTo( 0, 200 );
				ho.FadeInFromZero( 500 );
			}
		}

		protected override void UpdateAfterChildren () {
			foreach ( DrawableSolosuHitObject i in HitObjectContainer.AliveObjects ) {
				var position = ( Clock.CurrentTime - i.HitObject.StartTime ) / ScrollDuration.Value + 1;
				var height = (float)( SCROLL_HEIGHT * ( 1 - position ) );
				i.Y = (float)( -height - HitHeight.Value );
			}
		}
	}
}

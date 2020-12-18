using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.Solosu.Objects.Drawables;
using osu.Game.Rulesets.UI;
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

		protected override void UpdateAfterChildren () {
			foreach ( DrawableSolosuHitObject i in HitObjectContainer.AliveObjects ) {
				var position = i.PositionAtTime( ( Clock.CurrentTime - i.HitObject.StartTime ) / ScrollDuration.Value + 1 );
				i.Y = (float)( -SCROLL_HEIGHT * ( 1 - position ) - HitHeight.Value );
			}
		}
	}
}

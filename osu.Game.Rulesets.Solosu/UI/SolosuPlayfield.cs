using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	[Cached]
	public class SolosuPlayfield : Playfield {
		PlayerByte player;

		public SolosuPlayfield () {
			AddInternal( HitObjectContainer );
			HitObjectContainer.Origin = Anchor.BottomCentre;
			HitObjectContainer.Anchor = Anchor.BottomCentre;

			foreach ( var i in Enum<SolosuLane>.Values ) {
				var lane = new Lane {
					RelativeSizeAxes = Axes.Y,
					Width = (float)LANE_WIDTH,
					X = (float)( i.NormalizedOffset() * LANE_WIDTH ),

					Origin = Anchor.BottomCentre,
					Anchor = Anchor.BottomCentre
				};
				Lanes.Add( i, lane );
				AddNested( lane );
				AddInternal( lane );
			}

			AddInternal( player = new PlayerByte { Anchor = Anchor.BottomCentre } );
			HitHeight.BindValueChanged( v => player.Y = -(float)v.NewValue, true );
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject ) => new SolosuLifetimeEntry( hitObject );

		[Cached( name: nameof( ScrollDuration ) )]
		public readonly BindableDouble ScrollDuration = new( 3000 );
		public const double SCROLL_HEIGHT = 900;
		[Cached( name: nameof( HitHeight ) )]
		public readonly BindableDouble HitHeight = new( 200 );
		[Cached]
		public readonly Dictionary<SolosuLane, Lane> Lanes = new();
		public const double LANE_WIDTH = 200;
		[Cached( name: nameof( AccentColour ) )] // NOTE these three arent dynamic anywhere, they only apply when an animation starts
		public readonly Bindable<Colour4> AccentColour = new( Colour4.FromHex( "#ff7be0" ) );
		[Cached( name: nameof( MissColour ) )]
		public readonly Bindable<Colour4> MissColour = new( Colour4.FromHex( "#ed1a48" ) );
		[Cached( name: nameof( RegularColour ) )]
		public readonly Bindable<Colour4> RegularColour = new( Colour4.Cyan );

		public override void Add ( HitObject hitObject ) {
			var h = hitObject as SolosuHitObject;
			Lanes[ h.Lane ].Add( h );
		}
	}
}

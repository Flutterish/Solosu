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
			RegisterPool<Stream, DrawableStream>( 5 );
		}


		[Resolved( name: nameof( SolosuPlayfield.ScrollDuration ) )]
		public BindableDouble ScrollDuration { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.HitHeight ) )]
		public BindableDouble HitHeight { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.KiaiBindable ) )]
		public BindableBool KiaiBindable { get; private set; }

		public readonly BindableDouble LazerSpeed = new( 1 );

		protected override void Update () {
			base.Update();
			if ( Time.Elapsed < 0 ) {
				FinishTransforms();
				ClearTransformsAfter( Time.Current );
			}
		}

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

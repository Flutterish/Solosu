using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawablePacket : DrawableSolosuHitObject<Packet> {
		PacketFill fill;

		public DrawablePacket () {
			AddRangeInternal( new Drawable[] {
				fill = new PacketFill { RelativeSizeAxes = Axes.Both, Size = new Vector2( 0.4f ), Anchor = Anchor.Centre, Origin = Anchor.Centre, CornerRadius = 30 * 0.4f / 2 },
				new Box { AlwaysPresent = true, Alpha = 0, RelativeSizeAxes = Axes.Both }
			} );

			fillProgress.BindValueChanged( v => {
				fill.Size = new Vector2( (float)(0.4 + 0.6 * v.NewValue) );
			} );

			Masking = true;
			BorderThickness = 4f;
			CornerRadius = 5;
		}

		protected override void OnApply () {
			base.OnApply();
			Colour = RegularColour.Value;
			Size = new Vector2( 30 );
			Origin = Anchor.Centre;

			BorderColour = Colour4.White;
			fillProgress.Value = 0;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 )
				ApplyResult( r => r.Type = HitResult.Perfect );
		}

		protected override void UpdateInitialTransforms () {
			this.FadeInFromZero( 500 );
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			const double fillDuration = 150;
			const double delayDuration = 150;
			const double fadeDuration = 150;

			if ( state == ArmedState.Hit ) {
				FillTo( 1, fillDuration ).FadeColour( AccentColour.Value, fillDuration )
					.Then().Delay( delayDuration ).FadeOut( fadeDuration );
			}
			else if ( state == ArmedState.Miss ) {
				FillTo( 1, fillDuration ).FadeColour( MissColour.Value, fillDuration )
					.Then().Delay( delayDuration ).FadeOut( fadeDuration );
			}

			LifetimeEnd = TransformStartTime + fillDuration + delayDuration + fadeDuration;
		}

		private BindableDouble fillProgress = new( 0 );
		private TransformSequence<DrawablePacket> FillTo ( double fill, double duration = 0, Easing easing = Easing.None )
			=> this.TransformBindableTo( fillProgress, fill, duration, easing );

		private class PacketFill : Container {
			public PacketFill () {
				AddInternal( new Box { RelativeSizeAxes = Axes.Both } );
				Masking = true;
			}

			new public float CornerRadius {
				get => base.CornerRadius;
				set => base.CornerRadius = value;
			}
		}
	}
}

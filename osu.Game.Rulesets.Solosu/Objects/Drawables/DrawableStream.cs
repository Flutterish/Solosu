using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.UI;
using osuTK;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableStream : DrawableSolosuHitObject<Stream> {
		StreamVisual main;
		StreamVisual r;
		StreamVisual g;
		StreamVisual b;

		public DrawableStream () {
			AddInternal( r = new StreamVisual { Colour = Colour4.Red, Alpha = 0.4f } );
			AddInternal( g = new StreamVisual { Colour = Colour4.Green, Alpha = 0.4f } );
			AddInternal( b = new StreamVisual { Colour = Colour4.Blue, Alpha = 0.4f } );
			AddInternal( main = new StreamVisual() );
			Origin = Anchor.BottomCentre;
			Anchor = Anchor.BottomCentre;
			Width = 15;
		}

		protected override void OnApply () {
			Alpha = 0;
			main.Colour = Colours.Miss;
		}

		BeatDetector beat;
		[BackgroundDependencyLoader]
		private void load ( BeatDetector beat ) {
			this.beat = beat;
			beat.OnBeat += OnBeat;
		}

		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes ) {
			// thanks, hishi ;))))))
			if ( effectPoint.KiaiMode ) {
				float sickoMode = 30 * amplitudes.Average * beat.RandomFloat( 0 );

				r.MoveToOffset( beat.RandomVector( 0 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
				g.MoveToOffset( beat.RandomVector( 1 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
				b.MoveToOffset( beat.RandomVector( 2 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( Judged ) return;

			if ( timeOffset >= 0 )
				ApplyResult( j => j.Type = HitResult.Perfect );

			if ( Player.Lane == HitObject.Lane && timeOffset + HitObject.Duration >= 0 )
				ApplyResult( j => j.Type = HitResult.Miss );
		}

		protected override void UpdateInitialTransforms () {
			this.FadeIn( 100 );
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			const double fadeDuration = 400;
			this.FadeOut( fadeDuration );

			LifetimeEnd = TransformStartTime + fadeDuration;
		}

		private class StreamVisual : PoolableDrawable {
			public StreamVisual () {
				Origin = Anchor.BottomCentre;
				Anchor = Anchor.BottomCentre;
				RelativeSizeAxes = Axes.Both;

				AddInternal( new Box { RelativeSizeAxes = Axes.Both } );
			}
		}
	}
}

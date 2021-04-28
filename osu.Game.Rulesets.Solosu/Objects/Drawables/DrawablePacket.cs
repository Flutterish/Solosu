using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.UI;
using osuTK;
using System;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawablePacket : DrawableLanedSolosuHitObject<Packet> {
		PacketVisual main;

		public DrawablePacket () {
			AddRangeInternal( new Drawable[] {
				main = new PacketVisual( fillProgress )
			} );
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
				float sickoMode = 10 * amplitudes.Average * beat.RandomFloat( 0 );

				main.ScaleTo( 1 - sickoMode / 28, 50 ).Then().ScaleTo( 1, 100 );
			}
		}

		protected override void Update () {
			base.Update();
			var cubeTime = Lane.TimeAtHeight( Lane.CubeHeight, HitObject.StartTime );

			var timeToApply = Math.Clamp( Clock.CurrentTime, cubeTime - 100, cubeTime + 500 );
			if ( timeToApply != lastAppliedPositionalTransformTime ) {
				lastAppliedPositionalTransformTime = timeToApply;

				var fadeInProgress = Math.Clamp( ( timeToApply - cubeTime + 100 ) / 500, 0, 1 );
				var xAndRotateProgress = Math.Clamp( ( timeToApply - cubeTime ) / ( 200 / SolosuPlayfield.SCROLL_MULTIPLIER ), 0, 1 );

				Alpha = (float)fadeInProgress;
				X = -(float)( Lane.X * ( 1 - xAndRotateProgress ) );
				Rotation = (float)( Math.Atan2( -20, -Lane.X ) * 180 / Math.PI + 90 ).LerpTo( 0, xAndRotateProgress );
			}
		}

		protected double lastAppliedPositionalTransformTime { get; private set; }
		protected override void UpdateInitialTransforms () {
			lastAppliedPositionalTransformTime = TransformStartTime;
		}

		protected override void OnApply () {
			base.OnApply();
			main.Colour = Colours.Regular;
			Size = new Vector2( 30 );
			Origin = Anchor.Centre;

			fillProgress.Value = 0;
			Scale = Vector2.One;
			lastValidOffset = null;
		}

		double? lastValidOffset;
		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( lastValidOffset is null || Math.Abs( timeOffset ) <= Math.Abs( lastValidOffset.Value ) ) { // can get better result
				if ( Player.Lane == HitObject.Lane && HitWindows.ResultFor( timeOffset ) != HitResult.None ) lastValidOffset = timeOffset;
			}
			else {
				var result = HitWindows.ResultFor( lastValidOffset.Value );
				ApplyResult( r => r.Type = result );
			}

			if ( !HitWindows.CanBeHit( timeOffset ) ) {
				ApplyResult( r => r.Type = HitResult.Miss );
			}
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			const double fillDuration = 150;
			const double delayDuration = 150;
			const double fadeDuration = 150;

			if ( state == ArmedState.Hit ) {
				main.FadeColour( Colours.ColourFor( Result.Type ), fillDuration );
				FillTo( 1, fillDuration )
					.Then().Delay( delayDuration )
					.FadeOut( fadeDuration ).ScaleTo( 0.5f, fadeDuration );
			}
			else if ( state == ArmedState.Miss ) {
				main.FadeColour( Colours.ColourFor( Result.Type ), fillDuration );
				FillTo( 1, fillDuration )
					.Then().Delay( delayDuration ).FadeOut( fadeDuration );
				this.RotateTo( (float)HitObject.MissRotation, delayDuration + fillDuration + fadeDuration );
			}

			LifetimeEnd = HitStateUpdateTime + fillDuration + delayDuration + fadeDuration;
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

		private class PacketVisual : CompositeDrawable {
			PacketFill fill;

			public PacketVisual ( BindableDouble fillProgress ) {
				AddRangeInternal( new Drawable[] {
					fill = new PacketFill { RelativeSizeAxes = Axes.Both, Size = new Vector2( 0.4f ), Anchor = Anchor.Centre, Origin = Anchor.Centre, CornerRadius = 30 * 0.4f / 2 },
					new Box { AlwaysPresent = true, Alpha = 0, RelativeSizeAxes = Axes.Both }
				} );

				fillProgress.BindValueChanged( v => {
					fill.Size = new Vector2( (float)( 0.4 + 0.6 * v.NewValue ) );
				} );

				Masking = true;
				BorderThickness = 4f;
				CornerRadius = 5;
				BorderColour = Colour4.White;
				RelativeSizeAxes = Axes.Both;

				Origin = Anchor.Centre;
				Anchor = Anchor.Centre;
			}
		}
	}
}

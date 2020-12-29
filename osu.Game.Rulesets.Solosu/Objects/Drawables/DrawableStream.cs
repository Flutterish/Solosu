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
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableStream : DrawableLanedSolosuHitObject<Stream> {
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
		List<DrawableBonus> bonuses = new();
		protected override void OnFree () {
			base.OnFree();
			foreach ( var i in bonuses ) RemoveInternal( i );
			bonuses.Clear();
		}
		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			bonuses.Add( hitObject as DrawableBonus );
			AddInternal( hitObject );
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

		protected override void Update () {
			base.Update();
			var a = Lane.HeightAtTime( Clock.CurrentTime, HitObject.StartTime, Lane.LazerSpeed.Value );
			var b = Lane.HeightAtTime( Clock.CurrentTime, HitObject.EndTime, Lane.LazerSpeed.Value );

			Y = -(float)a;
			Height = MathF.Max( (float)( b - a ), 10 ); // imagine putting a 0 length hold in your map lmao couldnt be me
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset + HitObject.Duration >= 0 ) {
				if ( timeOffset >= 0 )
					ApplyResult( j => j.Type = HitResult.Perfect );
				else if ( Player.Lane == HitObject.Lane )
					ApplyResult( j => j.Type = HitResult.Miss );
			}
		}

		protected override void UpdateInitialTransforms () {
			this.FadeIn( 100 );
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			const double fadeDuration = 400;
			this.FadeOut( fadeDuration );

			LifetimeEnd = HitStateUpdateTime + fadeDuration;
		}

		private class StreamVisual : PoolableDrawable {
			public StreamVisual () {
				Origin = Anchor.BottomCentre;
				Anchor = Anchor.BottomCentre;
				RelativeSizeAxes = Axes.Both;

				AddInternal( new Circle { RelativeSizeAxes = Axes.Both } );
			}
		}
	}
}

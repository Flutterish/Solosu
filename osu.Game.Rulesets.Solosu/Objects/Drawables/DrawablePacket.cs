using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.UI;
using osuTK;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawablePacket : DrawableSolosuHitObject<Packet>, IKeyBindingHandler<SolosuAction> {
		PacketVisual main;
		PacketVisual r;
		PacketVisual g;
		PacketVisual b;

		public DrawablePacket () {
			AddRangeInternal( new Drawable[] {
				r = new PacketVisual( fillProgress ) { Colour = Colour4.Red, Alpha = 0.4f },
				g = new PacketVisual( fillProgress ) { Colour = Colour4.Green, Alpha = 0.4f },
				b = new PacketVisual( fillProgress ) { Colour = Colour4.Blue, Alpha = 0.4f },
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
				float sickoMode = 10 * amplitudes.Average;

				main.ScaleTo( 1 - sickoMode / 28, 50 ).Then().ScaleTo( 1, 100 );
				r.MoveToOffset( beat.RandomVector( 0 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0 ), 100 );
				g.MoveToOffset( beat.RandomVector( 1 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0 ), 100 );
				b.MoveToOffset( beat.RandomVector( 2 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0 ), 100 );
			}
		}

		protected override void OnApply () {
			base.OnApply();
			main.Colour = RegularColour.Value;
			Size = new Vector2( 30 );
			Origin = Anchor.Centre;

			fillProgress.Value = 0;
			Scale = Vector2.One;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( HitWindows.CanBeHit( timeOffset ) ) {
				var result = HitWindows.ResultFor( timeOffset );
				if ( userTriggered && result != HitResult.None ) {
					ApplyResult( r => r.Type = result );
				}
			}
			else {
				ApplyResult( r => r.Type = HitResult.Miss );
			}
		}
		public bool OnPressed ( SolosuAction action ) {
			if ( !action.IsAction() ) return false;

			if ( Judged ) return false;
			if ( Player.Lane == HitObject.Lane ) {
				UpdateResult( true );
				return true;
			}
			return false;
		}
		public void OnReleased ( SolosuAction action ) { }

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			const double fillDuration = 150;
			const double delayDuration = 150;
			const double fadeDuration = 150;

			if ( state == ArmedState.Hit ) {
				main.FadeColour( ColourFor( Result.Type ), fillDuration );
				FillTo( 1, fillDuration )
					.Then().Delay( delayDuration )
					.FadeOut( fadeDuration ).ScaleTo( 0.5f, fadeDuration );
			}
			else if ( state == ArmedState.Miss ) {
				main.FadeColour( ColourFor( Result.Type ), fillDuration );
				FillTo( 1, fillDuration )
					.Then().Delay( delayDuration ).FadeOut( fadeDuration );
				this.RotateTo( (float)HitObject.MissRotation, delayDuration + fillDuration + fadeDuration );
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

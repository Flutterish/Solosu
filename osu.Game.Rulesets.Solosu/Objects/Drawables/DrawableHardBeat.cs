using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using System;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableHardBeat : DrawableSolosuHitObject<HardBeat> {
		Circle child;
		public DrawableHardBeat () {
			AddInternal( child = new Circle {
				Anchor = Anchor.BottomCentre,
				Origin = Anchor.BottomCentre,
				Width = 500,
				Height = 10
			} );
			Origin = Anchor.BottomCentre;
			AutoSizeAxes = Axes.Y;
		}

		protected override void Update () {
			base.Update();

			Y = -(float)( Lanes[ SolosuLane.Center ].HeightAtTime( Math.Min( Clock.CurrentTime, HitObject.GetEndTime() ), HitObject.StartTime, 1.4 ) );
		}

		protected override void OnApply () {
			base.OnApply();
			Colour = Colours.Regular;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset > 0 ) {
				ApplyResult( j => j.Type = HitResult.IgnoreHit );
			}
		}

		protected override void UpdateInitialTransforms () {
			this.FadeIn( 500 );
		}
		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			if ( state == ArmedState.Hit ) {
				this.FadeOut( 150 ).FadeColour( Colours.ColourFor( Result.Type ), 100 );
				child.ResizeHeightTo( 100, 150, Easing.Out );
			}
			else if ( state == ArmedState.Miss ) {
				this.FadeOut( 500 ).FadeColour( Colours.Miss, 200 );
			}

			LifetimeEnd = HitStateUpdateTime + 500;
		}
	}
}

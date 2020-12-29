using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.UI;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableMultiStream : DrawableSolosuHitObject<MultiLaneStream> {
		public DrawableMultiStream () {
			Origin = Anchor.BottomCentre;
			RelativeSizeAxes = Axes.Y;
		}

		DrawablePool<StraightMultiStreamVisual> straightPool = new( 10 );
		DrawablePool<MultiStreamVisualConnector> connectorPool = new( 9 );
		List<PoolableDrawable> parts = new();

		protected override void OnApply () {
			base.OnApply();
			Colour = Colours.Miss;
			StraightMultiStreamVisual prev = null;
			foreach ( var i in HitObject.Lanes.Ranges( HitObject.EndTime ) ) {
				var straight = straightPool.Get( x => { x.Lane = Lanes[ i.value ]; x.LaneEnum = i.value; x.StartTime = i.startTime; x.EndTime = i.endTime; } );
				parts.Add( straight );
				AddInternal( straight );

				if ( prev is not null ) {
					var connector = connectorPool.Get( x => { x.From = prev; x.To = straight; } );
					parts.Add( connector );
					AddInternal( connector );
				}

				prev = straight;
			}
		}
		protected override void OnFree () {
			base.OnFree();
			foreach ( var i in parts ) {
				RemoveInternal( i );
			}
			parts.Clear();
		}

		protected override void UpdateInitialTransforms () {
			this.FadeInFromZero( 100 );
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset + HitObject.Duration >= 0 ) {
				if ( timeOffset >= 0 )
					ApplyResult( j => j.Type = HitResult.Perfect );
				else if ( Player.Lane == HitObject.Lanes.At( HitObject.EndTime + timeOffset ) )
					ApplyResult( j => j.Type = HitResult.Miss );
			}
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			if ( state == ArmedState.Hit ) {
				this.FadeOutFromOne( 400 );
			}
			else if ( state == ArmedState.Miss ) {
				this.FadeOutFromOne( 400 );
			}
		}

		private class StraightMultiStreamVisual : PoolableDrawable {
			public SolosuLane LaneEnum;
			public Lane Lane;
			public double StartTime;
			public double EndTime;

			Drawable box;
			public StraightMultiStreamVisual () {
				AddInternal( box = new Box { RelativeSizeAxes = Axes.Both, Origin = Anchor.BottomCentre, Anchor = Anchor.BottomCentre } );
				Anchor = Anchor.BottomCentre;
				Origin = Anchor.BottomCentre;
				Width = 15;
			}

			protected override void Update () {
				base.Update();
				Height = MathF.Max( (float)( TopHeight - BottomHeight ), 10 );
				Y = -(float)BottomHeight;
				X = Lane.X;
			}

			public double BottomHeight => Lane.HeightAtTime( Clock.CurrentTime, StartTime, Lane.LazerSpeed.Value );
			public double TopHeight => Lane.HeightAtTime( Clock.CurrentTime, EndTime, Lane.LazerSpeed.Value );
		}

		private class MultiStreamVisualConnector : PoolableDrawable {
			public StraightMultiStreamVisual From;
			public StraightMultiStreamVisual To;

			Drawable boxA;
			Drawable boxB;
			public MultiStreamVisualConnector () {
				Anchor = Anchor.BottomCentre;
				Origin = Anchor.BottomCentre;

				AddInternal( boxA = new Box { Origin = Anchor.CentreLeft, Anchor = Anchor.Centre, Width = 40, Height = 15 } );
				AddInternal( boxB = new Box { Origin = Anchor.CentreLeft, Anchor = Anchor.Centre, Width = 40, Height = 15 } );
			}

			protected override void Update () {
				base.Update();

				boxA.Y = -(float)From.TopHeight + 15f / 2;
				boxB.Y = -(float)To.BottomHeight - 15f / 2;

				boxA.X = From.Lane.X;
				boxB.X = To.Lane.X;

				boxA.Rotation = (float)( Math.Atan2( boxB.Y - boxA.Y, boxB.X - boxA.X ) / Math.PI * 180 );
				boxB.Rotation = (float)( Math.Atan2( boxA.Y - boxB.Y, boxA.X - boxB.X ) / Math.PI * 180 );
			}
		}
	}
}

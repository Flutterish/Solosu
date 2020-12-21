using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.UI;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	[Cached]
	public class SolosuPlayfield : Playfield {
		[Cached]
		private PlayerByte player = new PlayerByte { Anchor = Anchor.BottomCentre, Depth = -9999 };
		public readonly Container CubeContainer;
		private Wireframe cube;
		private Wireframe cubeR;
		private Wireframe cubeG;
		private Wireframe cubeB;

		public SolosuPlayfield () {
			AddInternal( solosuColours = new() );
			AddInternal( player );
			AddInternal( beat );
			AddInternal( CubeContainer = new Container {
				Origin = Anchor.TopCentre,
				Anchor = Anchor.TopCentre,
				Y = 70,
				Height = 150,
				Children = new[] {
					cubeR = new Cube { Size = new Vector2( 150 ), Anchor = Anchor.Centre, Origin = Anchor.Centre, Colour = Colour4.Red, Alpha = 0.4f },
					cubeG = new Cube { Size = new Vector2( 150 ), Anchor = Anchor.Centre, Origin = Anchor.Centre, Colour = Colour4.Green, Alpha = 0.4f },
					cubeB = new Cube { Size = new Vector2( 150 ), Anchor = Anchor.Centre, Origin = Anchor.Centre, Colour = Colour4.Blue, Alpha = 0.4f },
					cube = new Cube { Size = new Vector2( 150 ), Anchor = Anchor.Centre, Origin = Anchor.Centre }
				}
			} );
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

			HitHeight.BindValueChanged( v => player.Y = -(float)v.NewValue, true );
			beat.OnBeat += OnBeat;

			NewResult += OnNewResult;
			KiaiBindable.BindValueChanged( v => {
				if ( v.NewValue ) {
					this.TransformBindableTo( ScrollDuration, 2000, 400 );
				}
				else {
					this.TransformBindableTo( ScrollDuration, 3000, 400 );
				}
			} );
		}

		protected override void Update () {
			cube.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeR.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeG.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeB.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
		}

		[Cached( name: nameof( KiaiBindable ) )]
		public readonly BindableBool KiaiBindable = new( false );
		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes ) {
			// thanks, hishi ;))))))
			if ( effectPoint.KiaiMode ) {
				float sickoMode = 30 * amplitudes.Average * beat.RandomFloat( 0 );

				cubeR.MoveToOffset( beat.RandomVector( 0 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
				cubeG.MoveToOffset( beat.RandomVector( 1 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
				cubeB.MoveToOffset( beat.RandomVector( 2 ) * sickoMode, 50 ).Then().MoveTo( Vector2.Zero, 100 );
			}

			KiaiBindable.Value = effectPoint.KiaiMode;
		}

		private void OnNewResult ( DrawableHitObject dho, Judgements.JudgementResult j ) {
			if ( j.Type == Rulesets.Scoring.HitResult.Miss ) {

			}
			else {
				CubeContainer.ScaleTo( MathF.Max( CubeContainer.Scale.X * 0.92f, 0.5f ), 50 ).Then().ScaleTo( 1, 100 );
			}
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject ) => new SolosuLifetimeEntry( hitObject, ScrollDuration );
		[Cached]
		BeatDetector beat = new BeatDetector();
		[Cached( name: nameof( ScrollDuration ) )]
		public readonly BindableDouble ScrollDuration = new( 3000 );
		public const double SCROLL_HEIGHT = 900;
		[Cached( name: nameof( HitHeight ) )]
		public readonly BindableDouble HitHeight = new( 200 );
		[Cached]
		public readonly Dictionary<SolosuLane, Lane> Lanes = new();
		public const double LANE_WIDTH = 200;
		[Cached]
		public readonly SolosuColours solosuColours;

		public override void Add ( HitObject hitObject ) {
			var h = hitObject as SolosuHitObject;
			Lanes[ h.Lane ].Add( h );
		}
	}
}

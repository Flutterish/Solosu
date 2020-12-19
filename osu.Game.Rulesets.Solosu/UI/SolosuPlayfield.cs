using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.UI;
using osuTK;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	[Cached]
	public class SolosuPlayfield : Playfield {
		[Cached]
		private PlayerByte player = new PlayerByte { Anchor = Anchor.BottomCentre, Depth = -9999 };
		private Wireframe cube;
		private Wireframe cubeR;
		private Wireframe cubeG;
		private Wireframe cubeB;

		public SolosuPlayfield () {
			AddInternal( player ); // player goes before hitobjects bc input blocking
			AddInternal( beat );
			AddInternal( cubeR = new Cube { Size = new Vector2( 150 ), Y = 70, Anchor = Anchor.TopCentre, Origin = Anchor.TopCentre, Colour = Colour4.Red, Alpha = 0.4f } );
			AddInternal( cubeG = new Cube { Size = new Vector2( 150 ), Y = 70, Anchor = Anchor.TopCentre, Origin = Anchor.TopCentre, Colour = Colour4.Green, Alpha = 0.4f } );
			AddInternal( cubeB = new Cube { Size = new Vector2( 150 ), Y = 70, Anchor = Anchor.TopCentre, Origin = Anchor.TopCentre, Colour = Colour4.Blue, Alpha = 0.4f } );
			AddInternal( cube = new Cube { Size = new Vector2( 150 ), Y = 70, Anchor = Anchor.TopCentre, Origin = Anchor.TopCentre } );
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
		}

		protected override void Update () {
			cube.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeR.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeG.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
			cubeB.Rotation3D = Quaternion.FromAxisAngle( new Vector3( 0.3f, 1, 0 ), (float)Time.Current / 1000 );
		}

		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes ) {
			// thanks, hishi ;))))))
			if ( effectPoint.KiaiMode ) {
				float sickoMode = 30 * amplitudes.Average;

				cube.ScaleTo( 1 - sickoMode / 200, 50 ).Then().ScaleTo( 1, 100 );
				cubeR.MoveToOffset( beat.RandomVector( 0 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0, 70 ), 100 );
				cubeG.MoveToOffset( beat.RandomVector( 1 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0, 70 ), 100 );
				cubeB.MoveToOffset( beat.RandomVector( 2 ) * sickoMode, 50 ).Then().MoveTo( new Vector2( 0, 70 ), 100 );
			}
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject ) => new SolosuLifetimeEntry( hitObject );
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
		[Cached( name: nameof( PerfectColour ) )] // NOTE these three arent dynamic anywhere, they only apply when an animation starts
		public readonly Bindable<Colour4> PerfectColour = new( Colour4.FromHex( "#ff7be0" ) );
		[Cached( name: nameof( GreatColour ) )]
		public readonly Bindable<Colour4> GreatColour = new( Colour4.FromHex( "#f95bae" ) );
		[Cached( name: nameof( MehColour ) )]
		public readonly Bindable<Colour4> MehColour = new( Colour4.FromHex( "#f33b7c" ) );
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

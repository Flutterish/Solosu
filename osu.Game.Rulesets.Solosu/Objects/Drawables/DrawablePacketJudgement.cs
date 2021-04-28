using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Solosu.UI;
using osuTK;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawablePacketJudgement : PoolableDrawable {
		public override bool RemoveCompletedTransforms => false;
		[Resolved]
		SolosuColours colours { get; set; }

		DrawableSolosuHitObject dho;
		JudgementResult judgement;

		[Resolved]
		BeatDetector beat { get; set; }

		public DrawablePacketJudgement () {
			AddInternal( new Circle { Anchor = Anchor.Centre, Origin = Anchor.Centre, Size = new Vector2( 8 ) } );
		}

		[BackgroundDependencyLoader]
		private void load () {
			beat.OnBeat += OnBeat;
		}

		bool isSent;
		int beatsToGo;
		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes ) {
			if ( !IsInUse || isSent ) return;
			beatsToGo--;
			if ( beatsToGo > 0 ) return;
			isSent = true;
			this.MoveTo( new Vector2( 0, 150 / 2 + 70 ), 200, Easing.In ).Delay( 100 ).FadeOut( 100 ).Then().Expire();
		}

		public void Apply ( DrawableSolosuHitObject dho, JudgementResult judgement ) {
			this.dho = dho;
			this.judgement = judgement;
		}

		protected override void PrepareForUse () {
			ClearTransforms( true );
			Anchor = Anchor.TopCentre;
			Origin = Anchor.Centre;
			isSent = false;
			beatsToGo = 2;

			applyHitAnimations();
		}

		Vector2 offset;
		protected override void Update () {
			if ( !IsInUse || isSent ) return;
			if ( LatestTransformEndTime <= Clock.CurrentTime ) {
				var shake = 25;
				this.MoveTo( offset + new Vector2( RNG.NextSingle( -shake, shake ), RNG.NextSingle( -shake, shake ) ), 200, Easing.InOutCubic );
			}
		}

		private void applyHitAnimations () {
			Colour = colours.ColourFor( judgement.Type );
			Position = dho.ToSpaceOfOtherDrawable( Vector2.Zero, Parent ) - new Vector2( Parent.DrawWidth / 2, 0 );
			Alpha = 0;

			offset = new Vector2( ( dho.HitObject.PieceDirection == LeftRight.Right ? 350 : -350 ), Position.Y - 140 );

			this.FadeTo( 0.6f, 60 ).MoveTo( offset, 200, Easing.Out );
		}
	}
}

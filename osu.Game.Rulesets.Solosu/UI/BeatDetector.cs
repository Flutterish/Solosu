using osu.Framework.Audio.Track;
using osu.Framework.Utils;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osuTK;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	public class BeatDetector : BeatSyncedContainer {
		public delegate void BeatEvent ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes );
		public event BeatEvent OnBeat;
		protected override void OnNewBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes ) {
			vectors.Clear();
			floats.Clear();

			base.OnNewBeat( beatIndex, timingPoint, effectPoint, amplitudes );
			OnBeat?.Invoke( beatIndex, timingPoint, effectPoint, amplitudes );
		}

		private List<Vector2> vectors = new();
		public Vector2 RandomVector ( int index ) {
			while ( index >= vectors.Count ) {
				vectors.Add( new Vector2( RNG.NextSingle( -1, 1 ), RNG.NextSingle( -1, 1 ) ).Normalized() );
			}

			return vectors[ index ];
		}
		private List<float> floats = new();
		public float RandomFloat ( int index ) {
			while ( index >= floats.Count ) {
				floats.Add( RNG.NextSingle() );
			}

			return floats[ index ];
		}
	}
}

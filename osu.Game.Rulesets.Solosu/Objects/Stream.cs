using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Solosu.Replays;
using System;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Stream : LanedSolosuHitObject, IHasDuration, IFlowObject {
		public double EndTime { get; set; }

		public double Duration {
			get => EndTime - StartTime;
			set => EndTime = StartTime + value;
		}

		public IEnumerable<FlowObject> CreateFlowObjects () {
			yield return new FlowObject( StartTime, EndTime, FlowObjectType.Dangerous, Lane ) { Source = this };
		}

		public double BonusInterval { get; private set; }
		protected override void ApplyDefaultsToSelf ( ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty ) {
			base.ApplyDefaultsToSelf( controlPointInfo, difficulty );
			BonusInterval = controlPointInfo.TimingPointAt( StartTime ).BeatLength;
		}

		protected override void CreateNestedHitObjects ( CancellationToken cancellationToken ) {
			cancellationToken.ThrowIfCancellationRequested();

			int bonusCount = (int)Math.Floor( Duration / BonusInterval );
			double paddingTime = ( Duration - ( bonusCount - 1 ) * BonusInterval ) / 2;

			for ( int i = 0; i < bonusCount; i++ ) {
				AddNested( new Bonus { StartTime = StartTime + paddingTime + i * BonusInterval, Lane = Lane } );
			}
		}
	}
}

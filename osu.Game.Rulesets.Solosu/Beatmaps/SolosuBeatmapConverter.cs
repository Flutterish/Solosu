using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Solosu.Beatmaps {
	public class SolosuBeatmapConverter : BeatmapConverter<SolosuHitObject> {
		Random random;
		public SolosuBeatmapConverter ( IBeatmap beatmap, Ruleset ruleset ) : base( beatmap, ruleset ) {
			random = new Random( beatmap.BeatmapInfo.Hash.GetHashCode() );
		}

		public override bool CanConvert () => true;

		protected override IEnumerable<SolosuHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap, CancellationToken cancellationToken ) {
			cancellationToken.ThrowIfCancellationRequested();
			yield return new Packet {
				Samples = original.Samples,
				StartTime = original.StartTime,
				Lane = random.FromEnum<SolosuLane>()
			};
		}
	}
}

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
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
			if ( original is IHasDuration dur ) {
				yield return new Stream {
					Samples = original.Samples,
					StartTime = original.StartTime,
					EndTime = dur.EndTime,
					Lane = random.FromEnum<SolosuLane>()
				};
				//var lane = random.FromEnum<SolosuLane>();
				//yield return new Packet {
				//	Samples = original.Samples,
				//	StartTime = original.StartTime,
				//	Lane = lane
				//};
				//yield return new Packet {
				//	Samples = original.Samples,
				//	StartTime = dur.EndTime,
				//	Lane = lane
				//};
			}
			else {
				yield return new Packet {
					Samples = original.Samples,
					StartTime = original.StartTime,
					Lane = random.FromEnum<SolosuLane>()
				};
			}
		}

		protected override Beatmap<SolosuHitObject> CreateBeatmap ()
			=> new SolosuBeatmap();
	}
}

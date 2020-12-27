﻿using osu.Game.Beatmaps;
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
			random = beatmap.Random();
		}

		public override bool CanConvert () => true;

		protected override IEnumerable<SolosuHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap, CancellationToken cancellationToken ) {
			cancellationToken.ThrowIfCancellationRequested();
			if ( original is IHasDuration dur ) {
				yield return new Stream { // TODO twin streams
					Samples = original.Samples,
					StartTime = original.StartTime,
					EndTime = dur.EndTime,
					Lane = random.FromEnum<SolosuLane>()
				};
				if ( original.Kiai ) {
					yield return new Stream { // TODO twin streams
						StartTime = original.StartTime,
						EndTime = dur.EndTime,
						Lane = random.FromEnum<SolosuLane>()
					};
				}
			}
			else {
				yield return new Packet {
					Samples = original.Samples,
					StartTime = original.StartTime,
					Lane = random.FromEnum<SolosuLane>()
				};
			} // TODO bonus. my idea is something like the hold notes in tau
		}

		protected override Beatmap<SolosuHitObject> CreateBeatmap ()
			=> new SolosuBeatmap();
	}
}

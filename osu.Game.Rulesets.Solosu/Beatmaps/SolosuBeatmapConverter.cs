using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Solosu.Beatmaps {
	public class SolosuBeatmapConverter : BeatmapConverter<SolosuHitObject> {
		Random laneRandom;
		Random bonusRandom;
		public SolosuBeatmapConverter ( IBeatmap beatmap, Ruleset ruleset ) : base( beatmap, ruleset ) {
			laneRandom = beatmap.Random();
			bonusRandom = beatmap.Random();
		}

		public override bool CanConvert () => true;

		protected override IEnumerable<SolosuHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap, CancellationToken cancellationToken ) { // currently lazer lets you only convert std beatmaps
			cancellationToken.ThrowIfCancellationRequested(); // TODO aspire maps overlay packets on top of streams. do something to make that playable
			if ( original is IHasDuration bonus and not ( IHasPath or IHasPathWithRepeats ) ) {
				yield return new MultiLaneStream {
					Samples = original.Samples,
					StartTime = original.StartTime,
					EndTime = bonus.EndTime
				}.Randomize( bonusRandom, Beatmap.ControlPointInfo.TimingPointAt( original.StartTime ).BPM / 60 / 1000 );
			}
			else if ( original is IHasDuration dur ) {
				if ( original.Kiai ) {
					bool setSamples = true;
					foreach ( var i in laneRandom.FromEnum<SolosuLane>( 2 ) ) {
						var str = new Stream {
							StartTime = original.StartTime,
							EndTime = dur.EndTime,
							Lane = i
						};
						if ( setSamples ) {
							setSamples = false;
							str.Samples = original.Samples;
						}
						yield return str;
					}
				}
				else {
					yield return new Stream {
						Samples = original.Samples,
						StartTime = original.StartTime,
						EndTime = dur.EndTime,
						Lane = laneRandom.FromEnum<SolosuLane>()
					};
				}
			}
			else {
				yield return new Packet {
					Samples = original.Samples,
					StartTime = original.StartTime,
					Lane = laneRandom.FromEnum<SolosuLane>()
				};
			}
		}

		protected override Beatmap<SolosuHitObject> CreateBeatmap ()
			=> new SolosuBeatmap();
	}
}

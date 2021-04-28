using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
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
		bool isKiai = false;
		SolosuLane[] possibleLanes = new[] { SolosuLane.Center };

		protected override IEnumerable<SolosuHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap, CancellationToken cancellationToken ) { // currently lazer lets you only convert std beatmaps
			cancellationToken.ThrowIfCancellationRequested(); // TODO aspire maps overlay packets on top of streams. do something to make that playable

			// this way of converting maximizes movement to the beat for lazers and generates random patterns for packets
			if ( original is IHasDuration bonus and not ( IHasPath or IHasPathWithRepeats ) ) {
				MultiLaneStream next;
				yield return next = new MultiLaneStream {
					Samples = original.Samples,
					StartTime = original.StartTime,
					EndTime = bonus.EndTime
				}.Randomize( bonusRandom, Beatmap.ControlPointInfo.TimingPointAt( original.StartTime ).BPM / 60 / 1000 );
				possibleLanes = Enum<SolosuLane>.Values.Except( next.Lanes.Last().value.Yield() ).ToArray();
			}
			else if ( original is IHasDuration dur ) {
				if ( possibleLanes.Length > 1 || laneRandom.Chance( 0.4 ) ) {
					bool setSamples = true;
					IEnumerable<SolosuLane> danger;
					if ( possibleLanes.Length == 1 ) {
						danger = new[] { possibleLanes[ 0 ], laneRandom.From( Enum<SolosuLane>.Values.Except( possibleLanes ) ) };
					}
					else if ( possibleLanes.Length == 2 ) {
						danger = possibleLanes;
					}
					else {
						throw new InvalidOperationException( "How?" );
					}

					foreach ( var i in danger ) {
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
					possibleLanes = Enum<SolosuLane>.Values.Except( danger ).ToArray();
				}
				else { // there is only 1 free lane
					yield return new Stream {
						Samples = original.Samples,
						StartTime = original.StartTime,
						EndTime = original.StartTime + dur.Duration * 0.9,
						Lane = possibleLanes[ 0 ]
					};

					possibleLanes = Enum<SolosuLane>.Values.Except( possibleLanes[ 0 ].Yield() ).ToArray();
				}
			}
			else {
				Packet next;
				yield return next = new Packet {
					Samples = original.Samples,
					StartTime = original.StartTime,
					Lane = laneRandom.FromEnum<SolosuLane>()
				};

				possibleLanes = new[] { next.Lane };
			}

			if ( isKiai != original.Kiai ) {
				isKiai = !isKiai;
				yield return new HardBeat {
					// TODO samples
					StartTime = original.StartTime
				};
			}
		}

		protected override Beatmap<SolosuHitObject> CreateBeatmap ()
			=> new SolosuBeatmap();
	}
}

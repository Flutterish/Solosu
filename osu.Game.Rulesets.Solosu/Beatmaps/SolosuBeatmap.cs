using osu.Game.Beatmaps;
using osu.Game.Rulesets.Solosu.Objects;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Beatmaps {
	public class SolosuBeatmap : Beatmap<SolosuHitObject> {
		public override IEnumerable<BeatmapStatistic> GetStatistics () {
			var packets = HitObjects.OfType<Packet>();
			if ( packets.Any() ) yield return new BeatmapStatistic {
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.Circles ),
				Name = "Packets",
				Content = packets.Count().ToString()
			};

			var streams = HitObjects.OfType<Stream>();
			if ( streams.Any() ) yield return new BeatmapStatistic {
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.Sliders ),
				Name = "Streams",
				Content = streams.Count().ToString()
			};

			var multistreams = HitObjects.OfType<MultiLaneStream>();
			if ( multistreams.Any() ) yield return new BeatmapStatistic {
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.Spinners ),
				Name = "Multi-Streams",
				Content = multistreams.Count().ToString()
			};

			var hardBeats = HitObjects.OfType<HardBeat>();
			if ( hardBeats.Any() ) yield return new BeatmapStatistic {
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.Circles ),
				Name = "Hard Beats",
				Content = hardBeats.Count().ToString()
			};
		}
	}
}

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Beatmaps {
	public class SolosuBeatmapProcessor : BeatmapProcessor {
		public SolosuBeatmapProcessor ( IBeatmap beatmap ) : base( beatmap ) { }

		public override void PreProcess () {
			base.PreProcess();

			var random = new Random( Beatmap.BeatmapInfo.Hash.GetHashCode() );
			foreach ( var i in Beatmap.HitObjects.OfType<SolosuHitObject>() ) {
				i.ApplyVisualRandom( random );
			}
		}
	}
}

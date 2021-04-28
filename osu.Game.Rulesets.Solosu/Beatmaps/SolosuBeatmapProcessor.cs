using osu.Game.Beatmaps;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Beatmaps {
	public class SolosuBeatmapProcessor : BeatmapProcessor {
		public SolosuBeatmapProcessor ( IBeatmap beatmap ) : base( beatmap ) { }

		public override void PostProcess () {
			base.PostProcess();
			var random = Beatmap.Random();

			foreach ( var i in Beatmap.HitObjects.OfType<SolosuHitObject>() ) {
				applyRandom( i, random );
			}
		}

		void applyRandom ( SolosuHitObject h, Random random ) {
			h.ApplyVisualRandom( random );
			foreach ( var k in h.NestedHitObjects.OfType<SolosuHitObject>() ) {
				applyRandom( k, random );
			}
		}
	}
}

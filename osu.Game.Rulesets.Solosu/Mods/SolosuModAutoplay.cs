using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.Solosu.Replays;
using osu.Game.Scoring;
using osu.Game.Users;
using System;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModAutoplay : ModAutoplay<SolosuHitObject> {
		public override Score CreateReplayScore ( IBeatmap beatmap ) => new Score {
			ScoreInfo = new ScoreInfo {
				User = new User { Username = "Autosu" },
			},
			Replay = new SolosuAutoGenerator( beatmap ).Generate(),
		};

		public override string Description => "Let the cute bot do... wait, where did he go?";
		public override Type[] IncompatibleMods => new[] { typeof( ModRelax ), typeof( SolosuModAutopilot ) };
	}
}

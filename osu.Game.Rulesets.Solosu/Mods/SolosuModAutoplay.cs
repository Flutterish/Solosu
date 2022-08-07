using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Solosu.Replays;
using osu.Game.Scoring;
using osu.Game.Users;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModAutoplay : ModAutoplay {
		public override Score CreateReplayScore ( IBeatmap beatmap, IReadOnlyList<Mod> mods ) => new Score {
			ScoreInfo = new ScoreInfo {
				User = new APIUser { Username = "Autosu" },
			},
			Replay = new SolosuAutoGenerator( beatmap ).Generate(),
		};

		public override string Description => SolosuRuleset.GetLocalisedHack( Localisation.Mod.Strings.AutoplayDescription );
	}
}

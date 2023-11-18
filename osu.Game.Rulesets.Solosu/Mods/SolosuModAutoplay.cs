using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Solosu.Replays;
using osu.Game.Scoring;
using osu.Game.Users;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModAutoplay : ModAutoplay {
		public override ModReplayData CreateReplayData ( IBeatmap beatmap, IReadOnlyList<Mod> mods ) => new( new SolosuAutoGenerator( beatmap ).Generate(), new() {
			Username = "Autosu"
		} );

		public override LocalisableString Description => Localisation.ModStrings.AutoplayDescription;
	}
}

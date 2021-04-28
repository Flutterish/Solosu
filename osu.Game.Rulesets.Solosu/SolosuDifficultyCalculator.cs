﻿using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuDifficultyCalculator : DifficultyCalculator {
		// TODO SolosuDifficultyCalculator
		public SolosuDifficultyCalculator ( Ruleset ruleset, WorkingBeatmap beatmap )
			: base( ruleset, beatmap ) {
		}

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			return new DifficultyAttributes( mods, skills, beatmap.BeatmapInfo.StarDifficulty );
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) => Enumerable.Empty<DifficultyHitObject>();

		protected override Skill[] CreateSkills ( IBeatmap beatmap, Mod[] mods )
			=> new Skill[ 0 ];
	}
}

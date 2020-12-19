using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.Beatmaps;
using osu.Game.Rulesets.Solosu.Mods;
using osu.Game.Rulesets.Solosu.UI;
using osu.Game.Rulesets.UI;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuRuleset : Ruleset {
		public override string ShortName => "solosu";
		public override string Description => ShortName;
		public override string PlayingVerb => "glitching out";

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) => new DrawableSolosuRuleset( this, beatmap, mods );

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap ) => new SolosuBeatmapConverter( beatmap, this );
		public override IBeatmapProcessor CreateBeatmapProcessor ( IBeatmap beatmap ) => new SolosuBeatmapProcessor( beatmap );

		public override DifficultyCalculator CreateDifficultyCalculator ( WorkingBeatmap beatmap ) => new SolosuDifficultyCalculator( this, beatmap );

		public override IEnumerable<Mod> GetModsFor ( ModType type ) {
			switch ( type ) {
				case ModType.Automation:
					return new[] { new SolosuModAutoplay() };

				case ModType.DifficultyReduction:
					return new[] { new SolosuModNoFail() };

				default:
					return Array.Empty<Mod>();
			}
		}


		public override IEnumerable<KeyBinding> GetDefaultKeyBindings ( int variant = 0 ) => new[] {
			new KeyBinding(InputKey.Z, SolosuAction.Button1),
			new KeyBinding(InputKey.X, SolosuAction.Button2),
			new KeyBinding(InputKey.Left, SolosuAction.Left),
			new KeyBinding(InputKey.Up, SolosuAction.Center),
			new KeyBinding(InputKey.Right, SolosuAction.Right)
		};

		public override Drawable CreateIcon () => new SpriteText {
			Anchor = Anchor.Centre,
			Origin = Anchor.Centre,
			Text = ShortName[ 0 ].ToString(),
			Font = OsuFont.Default.With( size: 18 )
		};

		private Dictionary<HitResult, string> results = new Dictionary<HitResult, string> {
			[ HitResult.Perfect ] = "Perfect",
			[ HitResult.Great ] = "Nice",
			[ HitResult.Meh ] = "OK",
			[ HitResult.Miss ] = "Corrupt"
		};

		protected override IEnumerable<HitResult> GetValidHitResults ()
			=> results.Keys;

		public override string GetDisplayNameForHitResult ( HitResult result )
			=> results[ result ];
	}
}

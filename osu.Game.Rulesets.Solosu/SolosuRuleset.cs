using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.Beatmaps;
using osu.Game.Rulesets.Solosu.Mods;
using osu.Game.Rulesets.Solosu.Replays;
using osu.Game.Rulesets.Solosu.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuRuleset : Ruleset {
		public override string ShortName => "solosu";
		public override string Description => ShortName;
		public override string PlayingVerb => "glitching out";

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) => new DrawableSolosuRuleset( this, beatmap, mods );

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap ) => new SolosuBeatmapConverter( beatmap, this );
		public override IBeatmapProcessor CreateBeatmapProcessor ( IBeatmap beatmap ) => new SolosuBeatmapProcessor( beatmap );

		public override DifficultyCalculator CreateDifficultyCalculator ( IWorkingBeatmap beatmap ) => new SolosuDifficultyCalculator( RulesetInfo, beatmap );
		public override IConvertibleReplayFrame CreateConvertibleReplayFrame ()
			=> new SolosuReplayFrame();

		public override IEnumerable<Mod> GetModsFor ( ModType type ) { // TODO autoclick and automovement mod
			switch ( type ) {
				case ModType.Automation:
					return new Mod[] { new SolosuModAutoplay() };

				case ModType.DifficultyReduction:
					return new[] { new SolosuModNoFail() };

				default:
					return Array.Empty<Mod>();
			}
		}


		public override IEnumerable<KeyBinding> GetDefaultKeyBindings ( int variant = 0 ) => new[] {
			new KeyBinding(InputKey.Left, SolosuAction.Left),
			new KeyBinding(InputKey.Up, SolosuAction.Center),
			new KeyBinding(InputKey.Right, SolosuAction.Right)
		};

		public override Drawable CreateIcon () => new SolosuIcon( this );

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

		public override StatisticRow[] CreateStatisticsForScore ( ScoreInfo score, IBeatmap playableBeatmap ) => new StatisticRow[]
		{

		};

		private class SolosuIcon : Sprite {
			private readonly Ruleset ruleset;
			public SolosuIcon ( Ruleset ruleset ) {
				this.ruleset = ruleset;
				Size = new osuTK.Vector2( 40 );
				FillMode = FillMode.Fit;
				Origin = Anchor.Centre;
				Anchor = Anchor.Centre;
			}

			[BackgroundDependencyLoader]
			private void load ( TextureStore textures, GameHost host ) {
				if ( !textures.GetAvailableResources().Contains( "Textures/Icon.png" ) )
					textures.AddStore( host.CreateTextureLoaderStore( ruleset.CreateResourceStore() ) );

				Texture = textures.Get( "Textures/SolosuIcon" );
			}
		}
	}
}

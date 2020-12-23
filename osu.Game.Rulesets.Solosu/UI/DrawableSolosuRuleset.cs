using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Solosu.Objects;
using osu.Game.Rulesets.Solosu.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	[Cached]
	public class DrawableSolosuRuleset : DrawableRuleset<SolosuHitObject> {
		public DrawableSolosuRuleset ( SolosuRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) : base( ruleset, beatmap, mods ) { }

		protected override Playfield CreatePlayfield () => new SolosuPlayfield();
		protected override ReplayInputHandler CreateReplayInputHandler ( Replay replay ) => new SolosuFramedReplayInputHandler( replay );
		public override DrawableHitObject<SolosuHitObject> CreateDrawableRepresentation ( SolosuHitObject h ) => h.AsDrawable();
		protected override PassThroughInputManager CreateInputManager () => new SolosuInputManager( Ruleset?.RulesetInfo );
		protected override ReplayRecorder CreateReplayRecorder ( Score score )
			=> new SolosuReplayRecorder( score );
	}
}

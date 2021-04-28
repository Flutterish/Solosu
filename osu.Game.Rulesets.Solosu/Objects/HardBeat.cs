using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class HardBeat : SolosuHitObject {
		public override Judgement CreateJudgement () {
			return new IgnoreJudgement();
		}
	}
}

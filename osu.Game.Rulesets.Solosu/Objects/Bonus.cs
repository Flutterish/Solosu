using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Bonus : LanedSolosuHitObject {
		public override Judgement CreateJudgement ()
			=> new BonusJudgement();
	}

	public class BonusJudgement : Judgement {
		public override HitResult MaxResult => HitResult.SmallBonus;
	}
}

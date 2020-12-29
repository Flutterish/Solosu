using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableBonus : DrawableLanedSolosuHitObject<Bonus> {
		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( ParentHitObject?.Result.Type == HitResult.Miss )
				ApplyResult( j => j.Type = HitResult.IgnoreMiss );
			else if ( timeOffset >= 0 )
				ApplyResult( j => j.Type = HitResult.SmallBonus );
		}
	}
}

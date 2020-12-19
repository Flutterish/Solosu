using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Solosu.Scoring {
	public class SolosuHitWindows : HitWindows {
		public override bool IsHitResultAllowed ( HitResult result )
			=> result is HitResult.Perfect or HitResult.Great or HitResult.Meh or HitResult.Miss;
		protected override DifficultyRange[] GetRanges () => new DifficultyRange[] {
			new DifficultyRange( HitResult.Perfect, 40, 35, 30 ),
			new DifficultyRange( HitResult.Great, 80, 70, 60 ),
			new DifficultyRange( HitResult.Meh, 200, 180, 150 ),
			new DifficultyRange( HitResult.Miss, 400, 450, 500 )
		};
	}
}

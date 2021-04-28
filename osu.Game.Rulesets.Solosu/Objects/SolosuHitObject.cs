using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.Objects.Drawables;
using osu.Game.Rulesets.Solosu.Scoring;
using System;

namespace osu.Game.Rulesets.Solosu.Objects {
	public abstract class SolosuHitObject : HitObject {
		public virtual DrawableSolosuHitObject AsDrawable () => null;
		public LeftRight PieceDirection;

		public virtual void ApplyVisualRandom ( Random random ) {
			PieceDirection = random.Chance( 0.5 ) ? LeftRight.Left : LeftRight.Right;
		}
		protected override HitWindows CreateHitWindows () => new SolosuHitWindows();
	}
}

using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.Objects.Drawables;
using osu.Game.Rulesets.Solosu.Scoring;
using System;

namespace osu.Game.Rulesets.Solosu.Objects {
	public abstract class SolosuHitObject : HitObject {
		public virtual DrawableSolosuHitObject AsDrawable () => null;
		public virtual void ApplyVisualRandom ( Random random ) { }
		protected override HitWindows CreateHitWindows () => new SolosuHitWindows();
		public SolosuLane Lane;
	}

	public enum SolosuLane {
		Left,
		Center,
		Right
	}
}

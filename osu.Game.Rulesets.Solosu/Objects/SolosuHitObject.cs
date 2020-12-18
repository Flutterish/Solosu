using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Solosu.Objects.Drawables;

namespace osu.Game.Rulesets.Solosu.Objects {
	public abstract class SolosuHitObject : HitObject {
		public virtual DrawableSolosuHitObject AsDrawable () => null;
		public SolosuLane Lane;
	}

	public enum SolosuLane {
		Left,
		Center,
		Right
	}
}

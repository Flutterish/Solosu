namespace osu.Game.Rulesets.Solosu.Objects {
	public class LanedSolosuHitObject : SolosuHitObject {
		public SolosuLane Lane;
	}

	public enum SolosuLane {
		Left,
		Center,
		Right
	}
}

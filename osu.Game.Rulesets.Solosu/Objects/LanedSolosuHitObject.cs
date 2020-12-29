namespace osu.Game.Rulesets.Solosu.Objects {
	public class LanedSolosuHitObject : SolosuHitObject, IHasLane {
		public SolosuLane Lane { get; set; }
	}

	public enum SolosuLane {
		Left,
		Center,
		Right
	}
}

using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class SolosuLifetimeEntry : HitObjectLifetimeEntry {
		public SolosuLifetimeEntry ( HitObject hitObject ) : base( hitObject ) { }
		protected override double InitialLifetimeOffset => 6000;
	}
}

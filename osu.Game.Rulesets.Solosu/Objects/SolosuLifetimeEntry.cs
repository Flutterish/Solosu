using osu.Framework.Bindables;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class SolosuLifetimeEntry : HitObjectLifetimeEntry {
		BindableDouble scrollDuration;
		public SolosuLifetimeEntry ( HitObject hitObject, BindableDouble scrollDuration ) : base( hitObject ) {
			this.scrollDuration = scrollDuration;
		}
		protected override double InitialLifetimeOffset => scrollDuration?.Value ?? 3000; // NOTE this doesnt take into account scroll speeds
	}
}

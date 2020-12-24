using osu.Framework.Allocation;
using osu.Game.Rulesets.Solosu.UI;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableLanedSolosuHitObject<T> : DrawableSolosuHitObject<T> where T : SolosuHitObject {
		[Resolved]
		public Lane Lane { get; private set; }

		protected override void Update () {
			Y = -(float)Lane.HeightAtTime( Clock.CurrentTime, HitObject.StartTime );
		}
	}
}

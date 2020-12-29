using osu.Game.Rulesets.Solosu.UI;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public class DrawableLanedSolosuHitObject<T> : DrawableSolosuHitObject<T> where T : LanedSolosuHitObject {
		public Lane Lane => Lanes[ HitObject.Lane ];

		protected override void Update () {
			base.Update();
			Y = -(float)Lane.HeightAtTime( Clock.CurrentTime, HitObject.StartTime );
		}
	}
}

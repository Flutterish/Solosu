using osuTK;
using System;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Packet : SolosuHitObject {
		public Vector2 Offset;
		public double MissRotation;

		public override void ApplyVisualRandom ( Random random ) {
			MissRotation = random.Chance( 0.5 ) ? random.Range( 50, 80 ) : random.Range( -50, -80 );
		}
	}
}

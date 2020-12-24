using osuTK;
using System;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Packet : LanedSolosuHitObject {
		public Vector2 Offset;
		public double MissRotation;
		public LeftRight PieceDirection;

		public override void ApplyVisualRandom ( Random random ) {
			MissRotation = random.Chance( 0.5 ) ? random.Range( 50, 80 ) : random.Range( -50, -80 );
			PieceDirection = random.Chance( 0.5 ) ? LeftRight.Left : LeftRight.Right;
		}
	}

	public enum LeftRight {
		Left,
		Right
	}
}

using osu.Game.Rulesets.Solosu.Replays;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Packet : LanedSolosuHitObject, IFlowObject {
		public Vector2 Offset;
		public double MissRotation;
		public LeftRight PieceDirection;

		public override void ApplyVisualRandom ( Random random ) {
			MissRotation = random.Chance( 0.5 ) ? random.Range( 50, 80 ) : random.Range( -50, -80 );
			PieceDirection = random.Chance( 0.5 ) ? LeftRight.Left : LeftRight.Right;
		}

		public IEnumerable<FlowObject> CreateFlowObjects () {
			yield return new FlowObject( StartTime, StartTime, FlowObjectType.Intercept, Lane ) { Source = this };
		}
	}

	public enum LeftRight {
		Left,
		Right
	}
}

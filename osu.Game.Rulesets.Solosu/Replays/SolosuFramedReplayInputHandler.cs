using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuFramedReplayInputHandler : FramedReplayInputHandler<SolosuReplayFrame> {
		public SolosuFramedReplayInputHandler ( Replay replay ) : base( replay ) { }

		protected override bool IsImportant ( SolosuReplayFrame frame ) => true;
		protected override void CollectReplayInputs ( List<IInput> inputs ) {
			inputs.Add( new ReplayState<SolosuAction> {
				PressedActions = CurrentFrame?.Actions ?? new List<SolosuAction>()
			} );
		}
	}
}

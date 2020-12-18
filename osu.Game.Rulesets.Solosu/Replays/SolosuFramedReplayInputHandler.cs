using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuFramedReplayInputHandler : FramedReplayInputHandler<SolosuReplayFrame> {
		// TODO SolosuFramedReplayInputHandler
		public SolosuFramedReplayInputHandler ( Replay replay )
			: base( replay ) {
		}

		protected override bool IsImportant ( SolosuReplayFrame frame ) => frame.Actions.Any();
		public override void CollectPendingInputs ( List<IInput> inputs ) {
			inputs.Clear();
			inputs.Add( new ReplayState<SolosuAction> {
				PressedActions = CurrentFrame?.Actions ?? new List<SolosuAction>(),
			} );
		}
	}
}

using osu.Game.Rulesets.Replays;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuReplayFrame : ReplayFrame {
		public List<SolosuAction> Actions = new List<SolosuAction>();

		public SolosuReplayFrame ( params SolosuAction[] buttons ) : this( buttons.AsEnumerable() ) { }
		public SolosuReplayFrame ( IEnumerable<SolosuAction> buttons ) {
			Actions.AddRange( buttons.Distinct() );
		}
	}
}

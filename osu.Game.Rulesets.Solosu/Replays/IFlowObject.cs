using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Replays {
	public interface IFlowObject {
		IEnumerable<FlowObject> CreateFlowObjects ();
	}
}

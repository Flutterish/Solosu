using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Solosu.Replays;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Stream : LanedSolosuHitObject, IHasDuration, IFlowObject {
		public double EndTime { get; set; }

		public double Duration {
			get => EndTime - StartTime;
			set => EndTime = StartTime + value;
		}

		public IEnumerable<FlowObject> CreateFlowObjects () {
			yield return new FlowObject( StartTime, EndTime, FlowObjectType.Dangerous, Lane ) { Source = this };
		}
	}
}

using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuReplayRecorder : ReplayRecorder<SolosuAction> {
		public SolosuReplayRecorder ( Score target ) : base( target ) { }

		protected override ReplayFrame HandleFrame ( Vector2 mousePosition, List<SolosuAction> actions, ReplayFrame previousFrame ) {
			SolosuReplayFrame previous = previousFrame as SolosuReplayFrame;
			if ( previous is null ) {
				return new SolosuReplayFrame( actions ) { Time = Time.Current };
			}
			else if ( ( actions.Count() != previous.Actions.Count ) || actions.Except( previous.Actions ).Any() ) {
				return new SolosuReplayFrame( actions ) { Time = Time.Current };
			}
			return null;
		}
	}
}

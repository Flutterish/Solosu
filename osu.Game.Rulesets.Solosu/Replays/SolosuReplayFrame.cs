using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuReplayFrame : ReplayFrame, IConvertibleReplayFrame {
		public List<SolosuAction> Actions = new List<SolosuAction>();

		public SolosuReplayFrame ( params SolosuAction[] buttons ) : this( buttons.AsEnumerable() ) { }
		public SolosuReplayFrame ( IEnumerable<SolosuAction> buttons ) {
			Actions.AddRange( buttons.Distinct() );
		}

		public void FromLegacy ( LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null ) {
			Time = currentFrame.Time;
			if ( currentFrame.MouseLeft1 ) Actions.Add( SolosuAction.HardBeat );
			if ( currentFrame.MouseRight1 ) Actions.Add( SolosuAction.Left );
			if ( currentFrame.MouseRight2 ) Actions.Add( SolosuAction.Right );
			if ( ( currentFrame.ButtonState & ReplayButtonState.Smoke ) != 0 ) Actions.Add( SolosuAction.Center );
		}
		public LegacyReplayFrame ToLegacy ( IBeatmap beatmap ) {
			ReplayButtonState s = ReplayButtonState.None;
			if ( Actions.Contains( SolosuAction.HardBeat ) ) s |= ReplayButtonState.Left1;
			if ( Actions.Contains( SolosuAction.Left ) ) s |= ReplayButtonState.Right1;
			if ( Actions.Contains( SolosuAction.Right ) ) s |= ReplayButtonState.Right2;
			if ( Actions.Contains( SolosuAction.Center ) ) s |= ReplayButtonState.Smoke;
			return new LegacyReplayFrame( Time, null, null, s );
		}
	}
}

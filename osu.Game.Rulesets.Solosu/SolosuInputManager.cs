using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;
using System.ComponentModel;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuInputManager : RulesetInputManager<SolosuAction> {
		public SolosuInputManager ( RulesetInfo ruleset ) : base( ruleset, 0, SimultaneousBindingMode.Unique ) { }
	}

	public enum SolosuAction {
		[Description( "Hard Beat" )]
		HardBeat,

		[Description( "Left" )]
		Left,

		[Description( "Center" )] // this is for buffered input
		Center,

		[Description( "Right" )]
		Right
	}
}

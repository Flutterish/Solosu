using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;
using System.ComponentModel;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuInputManager : RulesetInputManager<SolosuAction> {
		public SolosuInputManager ( RulesetInfo ruleset )
			: base( ruleset, 0, SimultaneousBindingMode.Unique ) {
		}
	}

	public enum SolosuAction {
		[Description( "Button 1" )]
		Button1,

		[Description( "Button 2" )]
		Button2,

		[Description( "Left" )]
		Left,

		[Description( "Right" )]
		Right,

		[Description( "Center" )] // this is for elastic input
		Center
	}
}

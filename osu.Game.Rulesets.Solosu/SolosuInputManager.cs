using osu.Framework.Input.Bindings;
using osu.Framework.Localisation;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuInputManager : RulesetInputManager<SolosuAction> {
		public SolosuInputManager ( RulesetInfo ruleset ) : base( ruleset, 0, SimultaneousBindingMode.Unique ) { }
	}

	public enum SolosuAction {
		[LocalisableDescription( typeof( Localisation.Action.Strings ), nameof( Localisation.Action.Strings.Left ) )]
		Left,

		// this is for buffered input
		[LocalisableDescription( typeof( Localisation.Action.Strings ), nameof( Localisation.Action.Strings.Center ) )]
		Center,

		[LocalisableDescription( typeof( Localisation.Action.Strings ), nameof( Localisation.Action.Strings.Right ) )]
		Right
	}
}

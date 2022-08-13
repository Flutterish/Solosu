using osu.Framework.Input.Bindings;
using osu.Framework.Localisation;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Solosu {
	public class SolosuInputManager : RulesetInputManager<SolosuAction> {
		public SolosuInputManager ( RulesetInfo ruleset ) : base( ruleset, 0, SimultaneousBindingMode.Unique ) { }
	}

	public enum SolosuAction {
		[LocalisableDescription( typeof( Localisation.ActionStrings ), nameof( Localisation.ActionStrings.Left ) )]
		Left,

		// this is for buffered input
		[LocalisableDescription( typeof( Localisation.ActionStrings ), nameof( Localisation.ActionStrings.Center ) )]
		Center,

		[LocalisableDescription( typeof( Localisation.ActionStrings ), nameof( Localisation.ActionStrings.Right ) )]
		Right
	}
}

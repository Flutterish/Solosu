using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModNoFail : ModNoFail {
		public override LocalisableString Description => Localisation.ModStrings.NoFailDescription;
	}
}

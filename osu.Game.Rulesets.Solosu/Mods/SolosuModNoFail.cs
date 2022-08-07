using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModNoFail : ModNoFail {
		public override string Description => SolosuRuleset.GetLocalisedHack( Localisation.Mod.Strings.NoFailDescription );
	}
}

using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Solosu.Tests {
	[TestFixture]
	public class TestSceneSolosuPlayer : PlayerTestScene {
		protected override Ruleset CreatePlayerRuleset () => new SolosuRuleset();
		protected override bool Autoplay => true;
	}
}

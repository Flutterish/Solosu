using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModAutopilot : Mod {
		public override IconUsage? Icon => OsuIcon.ModAutopilot;
		public override ModType Type => ModType.Automation;
		public override LocalisableString Description => "No need to move, just tap to the rhythm";
		public override string Name => "Autopilot";
		public override string Acronym => "AP";
		public override double ScoreMultiplier => 1;
		public override Type[] IncompatibleMods => new[] { typeof( ModRelax ), typeof( ModAutoplay ) };
		public override bool HasImplementation => true;
	}
}

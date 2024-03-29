﻿using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Solosu.Mods {
	public class SolosuModRelax : ModRelax {
		public override LocalisableString Description => "No need to click, just follow the path";
		public override Type[] IncompatibleMods => new[] { typeof( SolosuModAutopilot ), typeof( ModAutoplay ) };
		public override bool HasImplementation => true;
	}
}

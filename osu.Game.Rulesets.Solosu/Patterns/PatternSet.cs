using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.Patterns {
	public class PatternSet {
		public List<Pattern> Patterns = new List<Pattern> {
			"ABBA",
			"A(AA)!",
			"(ABAC)+", // stairs
			"(AB)+",   // 2 lane castle
			"(ABCB)+"  // 3 lane castle
		};
	}
}

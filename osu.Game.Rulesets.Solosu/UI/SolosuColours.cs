using osu.Framework.Graphics;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Solosu.UI {
	public class SolosuColours : Drawable {
		public readonly Colour4 Perfect = Colour4.FromHex( "#ff7be0" );
		public readonly Colour4 Great = Colour4.FromHex( "#f95bae" );
		public readonly Colour4 Meh = Colour4.FromHex( "#f33b7c" );
		public readonly Colour4 Miss = Colour4.FromHex( "#ed1a48" );
		public readonly Colour4 Regular = Colour4.Cyan;

		public Colour4 ColourFor ( HitResult result ) {
			if ( result == HitResult.IgnoreHit ) return Perfect;
			if ( result == HitResult.Perfect ) return Perfect;
			if ( result == HitResult.Great ) return Great;
			if ( result == HitResult.Meh ) return Meh;
			else return Miss;
		}
	}
}

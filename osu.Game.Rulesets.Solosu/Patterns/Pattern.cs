namespace osu.Game.Rulesets.Solosu.Patterns {
	public class Pattern {
		public readonly string Source;
		/// <summary>
		/// Patterns are noted as follows:
		/// `A`, `B`, `C` are any distinct directions.
		/// `L`, `R`, `C` are left, right center respectively.
		/// `(ABC)` is a group. It does nothing by itself, but allows more complex behaviours.
		/// `[ABC]` is an intercept group. It places an intecept at that position. This is the default group.
		/// `{ABC}` is a danger group. It places a danger at that position.
		/// `&lt;ABC&gt;` is a move group. It forces the player to move there.
		/// `(ABC)?` makes a group optinal.
		/// `(ABC)xN` makes a group repeat N times.
		/// `(ABC)xN-K` makes a group repeat between N and K times.
		/// `(ABC)+` makes a group repeat one or more times.
		/// `(ABC)?+` makes a group repeat zero or more times.
		/// `(ABC)!` makes a group faster. Stackable.
		/// `(ABC)#` makes a group slower. Stackable.
		/// It is not possible to place a `[]`, `{}` or `&lt;&gt;` group around any notes with already defined behaviours.
		/// `!` and `#` are exclusive to each other and affect speed as follows:
		/// `sqrt(2)^(!)` is the minimum note speed, `sqrt(2)^(!+1)` is the maximum
		/// `sqrt(2)^(-#)` is the maximum note speed, `sqrt(2)^(1-#)` is the minimum
		/// </summary>
		public Pattern ( string source ) {
			Source = source;
		}

		public override string ToString ()
			=> Source;

		public static implicit operator Pattern ( string source )
			=> new Pattern( source );
	}
}

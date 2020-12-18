using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Solosu.UI;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public abstract class DrawableSolosuHitObject : DrawableHitObject<SolosuHitObject> {
		public DrawableSolosuHitObject () : this( null ) { }
		public DrawableSolosuHitObject ( SolosuHitObject hitObject ) : base( hitObject ) {
			Anchor = Anchor.BottomCentre;
		}
		private SolosuInputManager inputManager;
		protected SolosuInputManager InputManager => inputManager ??= ( GetContainingInputManager() as SolosuInputManager );
		protected override double InitialLifetimeOffset => 6000;
		public virtual double PositionAtTime ( double normalizedTime ) => normalizedTime;

		[Resolved( name: nameof( SolosuPlayfield.AccentColour ) )]
		new public Bindable<Colour4> AccentColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.MissColour ) )]
		public Bindable<Colour4> MissColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.RegularColour ) )]
		public Bindable<Colour4> RegularColour { get; private set; }
	}

	public abstract class DrawableSolosuHitObject<T> : DrawableSolosuHitObject where T : SolosuHitObject {
		new public T HitObject => base.HitObject as T;
	}
}

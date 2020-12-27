using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Solosu.UI;

namespace osu.Game.Rulesets.Solosu.Objects.Drawables {
	public abstract class DrawableSolosuHitObject : DrawableHitObject<SolosuHitObject> {
		public DrawableSolosuHitObject () : this( null ) { }
		public DrawableSolosuHitObject ( SolosuHitObject hitObject ) : base( hitObject ) {
			Anchor = Anchor.BottomCentre;
			AlwaysPresent = true;
		}
		private SolosuInputManager inputManager;
		protected SolosuInputManager InputManager => inputManager ??= ( GetContainingInputManager() as SolosuInputManager );
		protected override double InitialLifetimeOffset => 3000; // TODO this might need to be set on test scenes

		[Resolved]
		public SolosuColours Colours { get; private set; }
		[Resolved]
		public PlayerByte Player { get; private set; }
		public HitWindows HitWindows => HitObject.HitWindows;
	}

	public abstract class DrawableSolosuHitObject<T> : DrawableSolosuHitObject where T : SolosuHitObject {
		new public T HitObject => base.HitObject as T;
	}
}

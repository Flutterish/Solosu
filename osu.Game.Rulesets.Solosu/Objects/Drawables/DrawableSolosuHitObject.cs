using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
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

		[Resolved( name: nameof( SolosuPlayfield.PerfectColour ) )]
		public Bindable<Colour4> PerfectColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.GreatColour ) )]
		public Bindable<Colour4> GreatColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.MehColour ) )]
		public Bindable<Colour4> MehColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.MissColour ) )]
		public Bindable<Colour4> MissColour { get; private set; }
		[Resolved( name: nameof( SolosuPlayfield.RegularColour ) )]
		public Bindable<Colour4> RegularColour { get; private set; }
		[Resolved]
		public PlayerByte Player { get; private set; }
		public HitWindows HitWindows => HitObject.HitWindows;
		[Resolved]
		public Lane Lane { get; private set; }

		protected virtual Colour4 ColourFor ( HitResult result ) {
			if ( result == HitResult.Perfect ) return PerfectColour.Value;
			if ( result == HitResult.Great ) return GreatColour.Value;
			if ( result == HitResult.Meh ) return MehColour.Value;
			else return MissColour.Value;
		}

		protected override void UpdateInitialTransforms () {
			Lane.ApplyInitialTransformsTo( this );
		}
	}

	public abstract class DrawableSolosuHitObject<T> : DrawableSolosuHitObject where T : SolosuHitObject {
		new public T HitObject => base.HitObject as T;
	}
}

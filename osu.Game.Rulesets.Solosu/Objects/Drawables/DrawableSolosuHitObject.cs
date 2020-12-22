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
		}
		private SolosuInputManager inputManager;
		protected SolosuInputManager InputManager => inputManager ??= ( GetContainingInputManager() as SolosuInputManager );
		protected override double InitialLifetimeOffset => 3000; // TODO this might need to be set on test scenes

		[Resolved]
		public SolosuColours Colours { get; private set; }
		[Resolved]
		public PlayerByte Player { get; private set; }
		public HitWindows HitWindows => HitObject.HitWindows;
		[Resolved]
		public Lane Lane { get; private set; }

		protected override void Update () {
			Y = -(float)Lane.HeightAtTime( Clock.CurrentTime, HitObject.StartTime );
		}

		protected virtual Colour4 ColourFor ( HitResult result ) {
			if ( result == HitResult.Perfect ) return Colours.Perfect;
			if ( result == HitResult.Great ) return Colours.Great;
			if ( result == HitResult.Meh ) return Colours.Meh;
			else return Colours.Miss;
		}

		public void ReapplyTransforms () {
			ClearTransforms( true );
			using ( BeginAbsoluteSequence( LifetimeStart ) ) {
				UpdateInitialTransforms();
			}
			using ( BeginAbsoluteSequence( HitObject.StartTime ) ) {
				UpdateStartTimeStateTransforms();
			}
			using ( BeginAbsoluteSequence( HitStateUpdateTime ) ) {
				UpdateHitStateTransforms( State.Value );
			}
		}
	}

	public abstract class DrawableSolosuHitObject<T> : DrawableSolosuHitObject where T : SolosuHitObject {
		new public T HitObject => base.HitObject as T;
	}
}

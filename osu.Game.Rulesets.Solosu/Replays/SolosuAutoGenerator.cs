using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuAutoGenerator : AutoGenerator {
		protected Replay Replay;
		protected List<ReplayFrame> Frames => Replay.Frames;

		public new Beatmap<SolosuHitObject> Beatmap => (Beatmap<SolosuHitObject>)base.Beatmap;

		public SolosuAutoGenerator ( IBeatmap beatmap ) : base( beatmap ) {
			Replay = new Replay();
		}

		public override Replay Generate () {
			DifficultyFlowPlayfield flow = new();
			foreach ( var i in Beatmap.HitObjects.OfType<IFlowObject>() ) {
				flow.Add( i );
			}

			SolosuAction? buffered = null;
			SolosuAction held = SolosuAction.Center;

			List<(double time, IEnumerable<SolosuAction> actions)> movement = new();
			List<(double time, IEnumerable<SolosuAction> actions)> presses = new();

			void addMovementFrame ( double time, params SolosuAction[] buttons ) { // we dont want auto to hold the center
				if ( buttons.Length > 0 && buttons[ 0 ] == SolosuAction.Center ) {
					movement.Add( (time, buttons.Skip( 1 )) );
				}
				else {
					movement.Add( (time, buttons) );
				}
			}

			void moveTo ( SolosuAction action, double time ) { // the AI autually uses the flexible inputs!
				if ( held != action ) {
					if ( buffered is null || buffered == SolosuAction.Center ) {
						if ( action == SolosuAction.Center ) {
							buffered = null;
							held = action;
							addMovementFrame( time );
						}
						else {
							buffered = held;
							held = action;
							addMovementFrame( time, buffered.Value, held );
						}
					}
					else {
						if ( buffered == action ) {
							held = action;
							buffered = null;
							addMovementFrame( time, action );
						}
						else {
							held = action;
							buffered = null;
							addMovementFrame( time, action );
						}
					}
				}
			}

			SolosuAction lastButton = SolosuAction.Button2;
			double lastPressTime = 0;
			void press ( double time ) {
				lastButton = lastButton == SolosuAction.Button2 ? SolosuAction.Button1 : SolosuAction.Button2;

				if ( time - lastPressTime >= KEY_UP_DELAY ) {
					presses.Add( (lastPressTime + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );
				}
				presses.Add( (time, lastButton.Yield()) );
				lastPressTime = time;
			}

			var rng = Beatmap.Random();
			double time = 0;

			while ( flow.AnyConcernsLeft( time, held.GetLane() ) ) {
				var (type, list) = flow.NextConcern( time, held.GetLane() );
				if ( type == FlowObjectType.Dangerous ) {
					var safeLanes = Enum<SolosuLane>.Values.Except( list.Select( x => ( x.Source as LanedSolosuHitObject ).Lane ) );

					if ( safeLanes.Any() ) {
						var latestPossibleTime = flow.LastSafeTimeAfter( time, held.GetLane() );
						var lane = flow.GetChillestLane( latestPossibleTime );
						var earliestPossibleTime = Math.Max( time, flow.FirstSafeTimeBefore( latestPossibleTime, lane ) );

						time = ( earliestPossibleTime + latestPossibleTime ) / 2;
						moveTo( lane.GetAction(), time );
					}
					else {
						time = list.Where( x => ( x.Source as LanedSolosuHitObject ).Lane == held.GetLane() ).First().StartTime;
					}
				}
				else if ( type == FlowObjectType.Intercept ) {
					if ( list.Count() == 1 ) {
						var intercept = list.Single().Source as LanedSolosuHitObject;
						var safeTime = Math.Max( time, flow.FirstSafeTimeBefore( intercept.StartTime, intercept.Lane ) );
						moveTo( intercept.Lane.GetAction(), ( safeTime + intercept.StartTime ) / 2 );
						press( intercept.StartTime );
						time = intercept.StartTime;
					}
					else {
						var first = list.First().Source as LanedSolosuHitObject;
						var safeTime = Math.Max( time, flow.FirstSafeTimeBefore( first.StartTime, first.Lane ) );
						time = ( safeTime + list.First().StartTime ) / 2;
						double offset = 0;
						foreach ( var i in list ) {
							moveTo( ( i.Source as LanedSolosuHitObject ).Lane.GetAction(), time );
							press( time );
							time = i.StartTime + ++offset;
						}
						time = first.StartTime;
					}
				}
			}

			presses.Add( (lastPressTime + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );
			if ( !flow.AnyDangerAfterOrAt( time, SolosuLane.Center ) ) movement.Add( (time + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );

			movement.Sort( ( x, y ) => x.time > y.time ? 1 : -1 );
			presses.Sort( ( x, y ) => x.time > y.time ? 1 : -1 );

			int movementIndex = 0;
			int pressIndex = 0;
			time = -9999999;

			Frames.Add( new SolosuReplayFrame() { Time = 0 } );

			IEnumerable<SolosuAction> currentMovement = Array.Empty<SolosuAction>();
			IEnumerable<SolosuAction> currentPresses = Array.Empty<SolosuAction>();

			while ( movementIndex < movement.Count || pressIndex < presses.Count ) {
				(double time, IEnumerable<SolosuAction> actions) chosen;
				if ( movementIndex < movement.Count && pressIndex < presses.Count ) {
					var a = movement[ movementIndex ];
					var b = presses[ pressIndex ];

					if ( a.time < b.time ) {
						chosen = a;
						movementIndex++;
						currentMovement = chosen.actions;
					}
					else {
						chosen = b;
						pressIndex++;
						currentPresses = chosen.actions;
					}
				}
				else if ( movementIndex < movement.Count ) {
					chosen = movement[ movementIndex++ ];
					currentMovement = chosen.actions;
				}
				else { // pressIndex < presses.Count
					chosen = presses[ pressIndex++ ];
					currentPresses = chosen.actions;
				}

				time = chosen.time;
				Frames.Add( new SolosuReplayFrame( currentMovement.Concat( currentPresses ) ) { Time = time } );
			}

			return Replay;
		}
	}
}

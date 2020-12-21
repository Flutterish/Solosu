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
			void press ( double time, double duration = 0 ) {
				lastButton = lastButton == SolosuAction.Button2 ? SolosuAction.Button1 : SolosuAction.Button2; // NOTE you could make these 2 buttons independant

				if ( time - lastPressTime >= KEY_UP_DELAY ) {
					presses.Add( (lastPressTime + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );
				}
				presses.Add( (time, lastButton.Yield()) );
				lastPressTime = time + duration;
			}

			double previousTime = 0;
			SolosuAction dodgeDirection = SolosuAction.Right;
			foreach ( SolosuHitObject hitObject in Beatmap.HitObjects ) {
				SolosuAction action = hitObject.Lane.GetAction();

				if ( hitObject is Packet ) {
					moveTo( action, ( previousTime + hitObject.StartTime ) / 2 );
					press( hitObject.StartTime );

					previousTime = hitObject.StartTime;
				}
				else if ( hitObject is Stream s ) {
					if ( held.GetLane() == s.Lane ) {
						if ( action == SolosuAction.Center ) {
							action = dodgeDirection;
							dodgeDirection = dodgeDirection == SolosuAction.Right ? SolosuAction.Left : SolosuAction.Right;
						}
						else
							action = SolosuAction.Center;

						moveTo( action, ( previousTime + hitObject.StartTime ) / 2 );
					}
					previousTime = s.EndTime;
				}
			}

			presses.Add( (lastPressTime + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );
			movement.Add( (previousTime + KEY_UP_DELAY, Array.Empty<SolosuAction>()) );

			movement.Sort( ( x, y ) => x.time > y.time ? 1 : -1 );
			presses.Sort( ( x, y ) => x.time > y.time ? 1 : -1 );

			int movementIndex = 0;
			int pressIndex = 0;
			double time = -9999999;

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

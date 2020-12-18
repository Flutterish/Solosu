using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Solosu.Objects;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class SolosuAutoGenerator : AutoGenerator {
		// TODO SolosuAutoGenerator
		protected Replay Replay;
		protected List<ReplayFrame> Frames => Replay.Frames;

		public new Beatmap<SolosuHitObject> Beatmap => (Beatmap<SolosuHitObject>)base.Beatmap;

		public SolosuAutoGenerator ( IBeatmap beatmap ) : base( beatmap ) {
			Replay = new Replay();
		}

		public override Replay Generate () {
			SolosuAction? buffered = null;
			SolosuAction held = SolosuAction.Center;

			void addFrame ( double time, params SolosuAction[] buttons ) { // we dont want auto to hold the center
				if ( buttons.Length > 0 && buttons[ 0 ] == SolosuAction.Center ) {
					Frames.Add( new SolosuReplayFrame ( buttons.Skip( 1 ).ToArray() ) { Time = time } );
				}
				else {
					Frames.Add( new SolosuReplayFrame ( buttons ) { Time = time } );
				}
			}

			void hold ( SolosuAction action, double time ) { // the AI autually uses the flexible inputs!
				if ( held != action ) {
					if ( buffered is null || buffered == SolosuAction.Center ) {
						if ( action == SolosuAction.Center ) {
							buffered = null;
							held = action;
							addFrame( time );
						}
						else {
							buffered = held;
							held = action;
							addFrame( time, buffered.Value, held );
						}
					}
					else {
						if ( buffered == action ) {
							held = action;
							buffered = null;
							addFrame( time, action );
						}
						else {
							held = action;
							buffered = null;
							addFrame( time, action );
						}
					}
				}
			}

			double previousTime = 0;
			foreach ( SolosuHitObject hitObject in Beatmap.HitObjects ) {
				SolosuAction action = SolosuAction.Center;
				if ( hitObject.Lane == SolosuLane.Center ) action = SolosuAction.Center;
				else if ( hitObject.Lane == SolosuLane.Left ) action = SolosuAction.Left;
				else if ( hitObject.Lane == SolosuLane.Right ) action = SolosuAction.Right;

				hold( action, ( previousTime + hitObject.StartTime ) / 2 );
				previousTime = hitObject.StartTime;
			}

			return Replay;
		}
	}
}

using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Solosu.Collections;
using osu.Game.Rulesets.Solosu.Replays;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class MultiLaneStream : SolosuHitObject, IHasDuration, IFlowObject { // NOTE perhaps this should be in between 2 lanes and block movement between them so its unique from regular streams
		public double EndTime { get; set; }

		public double Duration {
			get => EndTime - StartTime;
			set => EndTime = StartTime + value;
		}

		public TimeSeekableList<SolosuLane> Lanes = new();

		public MultiLaneStream Randomize ( Random random, double frequency ) {
			double time = StartTime;
			double timePerSwap = 1 / frequency;

			void Next () {
				Lanes.Add( time, random.FromEnum<SolosuLane>() );
				time += timePerSwap;
			}

			do {
				Next();
			} while ( time < EndTime - timePerSwap );

			return this;
		}

		public IEnumerable<FlowObject> CreateFlowObjects () {
			return Lanes.Ranges( EndTime ).Select( x => new FlowObject( x.startTime, x.endTime, FlowObjectType.Dangerous, x.value ) { Source = new Stream { StartTime = x.startTime, EndTime = x.endTime, Lane = x.value } } );
		}
	}
}

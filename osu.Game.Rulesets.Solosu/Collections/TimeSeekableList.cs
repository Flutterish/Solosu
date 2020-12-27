using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Collections {
	public class TimeSeekableList<T> {
		private struct TimeEntry {
			public double Time;
			public T Value;
		}

		private List<TimeEntry> entries = new();

		public void Add ( double time, T value ) {
			entries.Add( new TimeEntry { Time = time, Value = value } );
			entries.Sort( ( a, b ) => Math.Sign( a.Time - b.Time ) );
		}

		public void ClearAfter ( double time ) {
			entries.RemoveAll( x => x.Time > time );
		}

		public T At ( double time )
			=> entries.Last( x => x.Time <= time ).Value;

		public bool AnyAfter ( double time )
			=> entries.Any() && entries.Last().Time > time;
	}
}

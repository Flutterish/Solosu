using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Collections {
	public class TimeSeekableList<T> : IEnumerable<(double time, T value)> {
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

		public IEnumerator<(double time, T value)> GetEnumerator ()
			=> entries.Select( x => (x.Time, x.Value) ).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator();

		public IEnumerable<(double startTime, double endTime, T value)> Ranges ( double endTime = double.PositiveInfinity ) {
			var prev = entries.First();
			foreach ( var i in entries.Skip( 1 ) ) {
				yield return (prev.Time, i.Time, prev.Value);
				prev = i;
			}
			yield return (prev.Time, endTime, prev.Value);
		}
	}
}

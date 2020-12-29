using osu.Game.Rulesets.Solosu.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class DifficultyFlowLane {
		List<FlowObject> flowObjects = new();

		public void Clear ()
			=> flowObjects.Clear();

		public void Add ( FlowObject fo ) {
			flowObjects.Add( fo );

			flowObjects.Sort( ( a, b ) => Math.Sign( a.StartTime - b.StartTime ) );
		}

		public void AddRange ( IEnumerable<FlowObject> fo ) {
			flowObjects.AddRange( fo );

			flowObjects.Sort( ( a, b ) => Math.Sign( a.StartTime - b.StartTime ) );
		}

		public bool AnyDangersAfter ( double time )
			=> flowObjects.Any( x => x.Type == FlowObjectType.Dangerous && x.StartTime > time );
		public bool AnyInterceptsAfter ( double time )
			=> flowObjects.Any( x => x.Type == FlowObjectType.Intercept && x.StartTime > time );
		public FlowObject FirstDangerAfter ( double time )
			=> flowObjects.First( x => x.Type == FlowObjectType.Dangerous && x.StartTime > time );
		public FlowObject FirstInterceptAfter ( double time )
			=> flowObjects.First( x => x.Type == FlowObjectType.Intercept && x.StartTime > time );
		public IEnumerable<FlowObject> GetInterceptsAt ( double time )
			=> flowObjects.Where( x => x.Type == FlowObjectType.Intercept ).Where( x => x.StartTime == time );
		public IEnumerable<FlowObject> GetDangersAt ( double time )
			=> flowObjects.Where( x => x.Type == FlowObjectType.Dangerous ).Where( x => x.StartTime <= time && x.EndTime >= time );
		public bool AnyDangersBeforeOrAt ( double time )
			=> flowObjects.Any( x => x.Type == FlowObjectType.Dangerous && ( ( x.StartTime <= time && x.EndTime >= time ) || ( x.EndTime < time ) ) );
		public FlowObject LastDangerBeforeOrAt ( double time )
			=> flowObjects.Last( x => x.Type == FlowObjectType.Dangerous && ( ( x.StartTime <= time && x.EndTime >= time ) || ( x.EndTime < time ) ) );
		public bool AnyDangersAfterOrAt ( double time )
			=> flowObjects.Any( x => x.Type == FlowObjectType.Dangerous && ( ( x.StartTime <= time && x.EndTime >= time ) || ( x.StartTime > time ) ) );
		public FlowObject FirstDangerAfterOrAt ( double time )
			=> flowObjects.First( x => x.Type == FlowObjectType.Dangerous && ( ( x.StartTime <= time && x.EndTime >= time ) || ( x.StartTime > time ) ) );
		public bool HasInterceptBeforeDangerAfter ( double time ) {
			if ( AnyInterceptsAfter( time ) ) {
				if ( AnyDangersAfterOrAt( time ) ) {
					return FirstDangerAfterOrAt( time ).StartTime > FirstInterceptAfter( time ).StartTime;
				}
				else return true;
			}
			else return false;
		}
	}

	public struct FlowObject : IHasLane {
		public double StartTime;
		public double EndTime;
		public FlowObjectType Type;
		public SolosuHitObject Source;

		public FlowObject ( double startTime, double endTime, FlowObjectType type, SolosuLane lane ) {
			StartTime = startTime;
			EndTime = endTime;
			Type = type;
			Source = null;
			Lane = lane;
		}

		public SolosuLane Lane { get; set; }
	}

	public enum FlowObjectType {
		Dangerous,
		Intercept
	}

	public class DifficultyFlowPlayfield {
		Dictionary<SolosuLane, DifficultyFlowLane> lanes = new();
		public DifficultyFlowPlayfield () {
			foreach ( var i in Enum<SolosuLane>.Values ) {
				lanes.Add( i, new() );
			}
		}

		public void Clear () {
			foreach ( var i in lanes ) i.Value.Clear();
		}

		public void Add ( IFlowObject lho ) {
			if ( lho is IHasLane laned ) {
				lanes[ laned.Lane ].AddRange( lho.CreateFlowObjects().Mutate( ( ref FlowObject x ) => x.Lane = laned.Lane ) );
			}
			else {
				foreach ( var i in lho.CreateFlowObjects() ) {
					lanes[ i.Lane ].Add( i );
				}
			}
		}

		public bool AnyConcernsLeft ( double time, SolosuLane currentLane )
			=> lanes[ currentLane ].AnyDangersAfter( time ) || lanes.Any( x => x.Value.AnyInterceptsAfter( time ) );
		public (FlowObjectType type, IEnumerable<FlowObject> list) NextConcern ( double time, SolosuLane currentLane ) {
			if ( lanes[ currentLane ].AnyDangersAfter( time ) ) {
				var dangerTime = Math.Max( time, lanes[ currentLane ].FirstDangerAfter( time ).StartTime );
				if ( lanes.Any( x => x.Value.AnyInterceptsAfter( time ) ) ) {
					var intercepts = FirstInterceptsAfter( time );
					var interceptTime = intercepts.First().StartTime;

					return ( dangerTime < interceptTime ) ? (FlowObjectType.Dangerous, lanes.SelectMany( x => x.Value.GetDangersAt( dangerTime ) )) : (FlowObjectType.Intercept, intercepts);
				}
				else {
					return (FlowObjectType.Dangerous, lanes.SelectMany( x => x.Value.GetDangersAt( dangerTime ) ));
				}
			}
			else { // must be intercept
				return (FlowObjectType.Intercept, FirstInterceptsAfter( time ));
			}
		}
		public SolosuLane GetChillestLane ( double time ) {
			Dictionary<SolosuLane, double> alignedIntercepts = new();

			foreach ( var lane in lanes ) {
				if ( lane.Value.HasInterceptBeforeDangerAfter( time ) ) {
					alignedIntercepts.Add( lane.Key, lane.Value.FirstInterceptAfter( time ).StartTime );
				}
			}
			if ( alignedIntercepts.Any() ) {
				double earliest = alignedIntercepts.Min( x => x.Value );
				return alignedIntercepts.First( x => x.Value == earliest ).Key;
			}

			foreach ( var lane in lanes ) {
				if ( !lane.Value.AnyDangersAfterOrAt( time ) ) return lane.Key;
				alignedIntercepts.Add( lane.Key, lane.Value.FirstDangerAfterOrAt( time ).StartTime );
			}

			double latest = alignedIntercepts.Max( x => x.Value );
			return alignedIntercepts.First( x => x.Value == latest ).Key;
		}
		public IEnumerable<FlowObject> FirstInterceptsAfter ( double time ) {
			var firsties = lanes.Where( x => x.Value.AnyInterceptsAfter( time ) ).Select( x => x.Value.FirstInterceptAfter( time ) );
			var earliestTime = firsties.Min( x => x.StartTime );
			return lanes.SelectMany( x => x.Value.GetInterceptsAt( earliestTime ) );
		}
		public double FirstSafeTimeBefore ( double time, SolosuLane lane )
			=> lanes[ lane ].AnyDangersBeforeOrAt( time ) ? Math.Min( lanes[ lane ].LastDangerBeforeOrAt( time ).EndTime, time ) : 0;
		public double LastSafeTimeAfter ( double time, SolosuLane lane )
			=> lanes[ lane ].AnyDangersAfter( time ) ? Math.Max( lanes[ lane ].FirstDangerAfter( time ).StartTime, time ) : double.PositiveInfinity;
		public bool AnyDangerAfterOrAt ( double time, SolosuLane lane )
			=> lanes[ lane ].AnyDangersAfterOrAt( time );
	}
}

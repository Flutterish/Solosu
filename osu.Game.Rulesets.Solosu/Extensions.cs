using osu.Game.Beatmaps;
using osu.Game.Rulesets.Solosu.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Solosu {
	public static class Extensions {
		public static bool Chance ( this Random random, double chance ) => random.NextDouble() < chance;
		public static T From<T> ( this Random random, IEnumerable<T> collection ) {
			var array = collection.ToArray();
			return array[ random.Next( 0, array.Length ) ];
		}
		public static T[] From<T> ( this Random random, IEnumerable<T> collection, int count ) {
			var array = collection.ToList();
			T[] result = new T[ count ];
			for ( ; count > 0; count-- ) {
				int index = random.Next( array.Count );
				result[ count - 1 ] = array[ index ];
				array.RemoveAt( index );
			}
			return result;
		}
		public static T FromEnum<T> ( this Random random ) where T : Enum
			=> random.From( Enum<T>.Values );
		public static T[] FromEnum<T> ( this Random random, int count ) where T : Enum
			=> random.From( Enum<T>.Values, count );

		public static double Range ( this Random random, double min, double max )
			=> min + ( max - min ) * random.NextDouble();

		public static double NormalizedOffset ( this SolosuLane lane ) {
			if ( lane == SolosuLane.Center ) return 0;
			if ( lane == SolosuLane.Left ) return -1;
			if ( lane == SolosuLane.Right ) return 1;
			return 0;
		}

		public static SolosuLane GetLane ( this SolosuAction action ) {
			if ( action == SolosuAction.Left ) return SolosuLane.Left;
			else if ( action == SolosuAction.Right ) return SolosuLane.Right;
			else if ( action == SolosuAction.Center ) return SolosuLane.Center;
			else throw new InvalidOperationException( $"{action} is not {SolosuAction.Left}, {SolosuAction.Center} nor {SolosuAction.Right}" );
		}

		public static SolosuAction GetAction ( this SolosuLane lane ) {
			if ( lane == SolosuLane.Left ) return SolosuAction.Left;
			else if ( lane == SolosuLane.Right ) return SolosuAction.Right;
			else if ( lane == SolosuLane.Center ) return SolosuAction.Center;
			else throw new InvalidOperationException( $"{lane} is not {SolosuLane.Left}, {SolosuLane.Center} nor {SolosuLane.Right}" );
		}

		public static bool IsMovement ( this SolosuAction action )
			=> action is SolosuAction.Left or SolosuAction.Right or SolosuAction.Center;
		public static bool IsAction ( this SolosuAction action )
			=> action is SolosuAction.HardBeat;

		public static bool IsEmpty<T> ( this IEnumerable<T> self )
			=> !self.Any();

		public static double LerpTo ( this double from, double to, double progress )
			=> from + ( to - from ) * progress;
		public static Vector2 LerpTo ( this Vector2 from, Vector2 to, double progress )
			=> from + ( to - from ) * (float)progress;

		public static Random Random ( this IBeatmap beatmap )
			=> new Random( (int)( beatmap.BeatmapInfo.Length + beatmap.BeatmapInfo.ID + beatmap.BeatmapInfo.AudioLeadIn * beatmap.BeatmapInfo.BeatDivisor / beatmap.BeatmapInfo.BPM ) );

		public delegate void Mutator<T> ( ref T value );
		public static IEnumerable<T> Mutate<T> ( this IEnumerable<T> self, Mutator<T> mutator ) {
			return self.Select( x => { mutator( ref x ); return x; } );
		}
	}

	public static class Enum<T> where T : Enum {
		public static IEnumerable<T> Values => Enum.GetValues( typeof( T ) ).OfType<T>();
	}
}

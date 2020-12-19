using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	public class Wireframe : CompositeDrawable {
		public Quaternion Rotation3D;
		public Vector3 Offset;
		public Vector2 FOV = new Vector2( 120 );
		public List<Line> Lines = new();

		protected override void Update () {
			while ( InternalChildren.Count < Lines.Count ) {
				AddInternal( new Box { Anchor = Anchor.Centre, Origin = Anchor.Centre } );
			}
			foreach ( var i in InternalChildren ) {
				i.Alpha = 0;
			}
			for ( int i = 0; i < Lines.Count; i++ ) {
				var line = Lines[ i ];
				var child = InternalChildren[ i ];

				var a = Project( line.From.Position );
				var b = Project( line.To.Position );

				if ( Math.Abs( a.X ) > Size.X ) continue;
				if ( Math.Abs( b.X ) > Size.X ) continue;
				if ( Math.Abs( a.Y ) > Size.Y ) continue;
				if ( Math.Abs( b.Y ) > Size.Y ) continue;

				var length = ( a - b ).Length;

				child.Height = length + 2;
				child.Width = 2;

				child.Position = ( a + b ) / 2;
				child.Rotation = MathF.Atan2( ( a - b ).Y, ( a - b ).X ) * 180 / MathF.PI + 90;
				child.Alpha = 1;
			}
		}

		public void AddLine ( Vertice from, Vertice to, int segments ) {
			for ( int i = 0; i < segments; i++ ) {
				float t1 = (float)i / segments;
				float t2 = (float)( i + 1 ) / segments;

				Lines.Add( new Line( from.Position + ( to.Position - from.Position ) * t1, from.Position + ( to.Position - from.Position ) * t2 ) );
			}
		}

		public Vector2 Project ( Vector3 pos ) {
			pos = ( Rotation3D * new Vector4( pos ) ).Xyz - Offset;

			return new Vector2(
				Size.X * MathF.Atan2( pos.X, pos.Z ) / ( FOV.X / 180 * MathF.PI ),
				Size.Y * MathF.Atan2( pos.Y, pos.Z ) / ( FOV.X / 180 * MathF.PI )
			);
		}
	}

	public record Vertice {
		public readonly Vector3 Position;
		public Vertice ( Vector3 position ) {
			Position = position;
		}

		public static implicit operator Vertice ( Vector3 v )
			=> new Vertice( v );
	}
	public record Line {
		public readonly Vertice From;
		public readonly Vertice To;
		public Line ( Vertice from, Vertice to ) {
			From = from;
			To = to;
		}

		public static implicit operator Line ( (Vertice a, Vertice b) v )
			=> new Line( v.a, v.b );
	}

	public class Cube : Wireframe {
		public Cube () {
			Offset = new Vector3( 0, 0, -2 );

			Vertice a = new Vector3( 1, 1, 1 );
			Vertice b = new Vector3( -1, 1, 1 );
			Vertice c = new Vector3( 1, -1, 1 );
			Vertice d = new Vector3( 1, 1, -1 );
			Vertice e = new Vector3( -1, -1, 1 );
			Vertice f = new Vector3( -1, 1, -1 );
			Vertice g = new Vector3( 1, -1, -1 );
			Vertice h = new Vector3( -1, -1, -1 );

			int seg = 20;

			AddLine( a, b, seg );
			AddLine( a, c, seg );
			AddLine( a, d, seg );

			AddLine( b, e, seg );
			AddLine( b, f, seg );

			AddLine( c, e, seg );
			AddLine( c, g, seg );

			AddLine( d, f, seg );
			AddLine( d, g, seg );

			AddLine( e, h, seg );
			AddLine( f, h, seg );
			AddLine( g, h, seg );
		}
	}
}

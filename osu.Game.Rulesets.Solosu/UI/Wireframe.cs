using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Layout;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Solosu.UI {
	public class Wireframe : Drawable {
		public Quaternion Rotation3D;
		public Vector3 Offset;
		public Vector2 FOV = new Vector2( 120 );
		public readonly List<Line> Lines = new();

		protected override void Update () {
			base.Update();

			Invalidate( Invalidation.MiscGeometry, InvalidationSource.Self );
		}

		public Vector2 ToLocalSpace ( Vector3 pos ) {
			pos = ( Rotation3D * new Vector4( pos ) ).Xyz - Offset;

			return new Vector2(
				Size.X * MathF.Atan2( pos.X, pos.Z ) / ( FOV.X / 180 * MathF.PI ),
				Size.Y * MathF.Atan2( pos.Y, pos.Z ) / ( FOV.X / 180 * MathF.PI )
			);
		}

		public void AddLine ( Vertice from, Vertice to, int segments ) {
			for ( int i = 0; i < segments; i++ ) {
				float t1 = (float)i / segments;
				float t2 = (float)( i + 1 ) / segments;

				Lines.Add( new Line( from.Position + ( to.Position - from.Position ) * t1, from.Position + ( to.Position - from.Position ) * t2 ) );
			}
		}

		[BackgroundDependencyLoader]
		private void load ( ShaderManager shaders ) {
			TextureShader = shaders.Load( VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE );
			RoundedTextureShader = shaders.Load( VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE_ROUNDED );
		}

		public IShader TextureShader { get; protected set; }
		public IShader RoundedTextureShader { get; protected set; }

		protected override DrawNode CreateDrawNode ()
			=> new WireframeDrawNode( this );

		private class WireframeDrawNode : DrawNode {
			private Wireframe wf;
			private Texture texture;
			readonly List<Line> lines;
			Vector2 size;
			IShader shader;
			float width = 3;
			public WireframeDrawNode ( Wireframe source ) : base( source ) {
				texture = Texture.WhitePixel;
				wf = source;
				lines = wf.Lines;
			}

			public override void ApplyState () {
				base.ApplyState();
				size = wf.Size;
				shader = wf.TextureShader;
			}

			public override void Draw ( Action<TexturedVertex2D> vertexAction ) {
				base.Draw( vertexAction );

				shader.Bind();

				for ( int i = 0; i < lines.Count; i++ ) {
					var line = lines[ i ];

					var from = Project( line.From.Position );
					var to = Project( line.To.Position );
					var dif = ( from - to ).Normalized(); // make them a bit longer so they overlap
					from += dif;
					to -= dif;
					var perp = dif.PerpendicularLeft * width / 2;

					DrawQuad( texture, new Quad( from + perp, from - perp, to + perp, to - perp ), DrawColourInfo.Colour );
				}

				shader.Unbind();
			}
			protected override bool CanDrawOpaqueInterior => false;

			Vector2 Project ( Vector3 pos ) {
				pos = ( wf.Rotation3D * new Vector4( pos ) ).Xyz - wf.Offset;

				return ToScreen( new Vector2(
					size.X * ( 0.5f + MathF.Atan2( pos.X, pos.Z ) / ( wf.FOV.X / 180 * MathF.PI ) ),
					size.Y * ( 0.5f + MathF.Atan2( pos.Y, pos.Z ) / ( wf.FOV.Y / 180 * MathF.PI ) )
				) );
			}

			Vector2 ToScreen ( Vector2 pos )
				=> Vector2Extensions.Transform( pos, DrawInfo.Matrix );
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

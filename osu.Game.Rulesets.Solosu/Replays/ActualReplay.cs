using osu.Game.Replays;
using System;

namespace osu.Game.Rulesets.Solosu.Replays {
	public class ActualReplay {
		public Replay replay;

		private int frameIndex = 0;
		int nextFrameIndex => Math.Clamp( frameIndex + 1, 0, replay.Frames.Count - 1 );
		int previousFrameIndex => Math.Clamp( frameIndex - 1, 0, replay.Frames.Count - 1 );
		SolosuReplayFrame currentFrame => replay.Frames[ frameIndex ] as SolosuReplayFrame;
		SolosuReplayFrame nextFrame => replay.Frames[ nextFrameIndex ] as SolosuReplayFrame;
		SolosuReplayFrame previousFrame => replay.Frames[ previousFrameIndex ] as SolosuReplayFrame;

		SolosuReplayFrame lastFrame;

		/// <summary>
		/// Next frame towards that time, or null if no frame change occured.
		/// </summary>
		public SolosuReplayFrame FrameAt ( double time ) {
			if ( time > nextFrame.Time ) {
				frameIndex = nextFrameIndex;
			}
			else if ( time < currentFrame.Time ) {
				frameIndex = previousFrameIndex;
			}

			if ( currentFrame == lastFrame ) return null;
			return lastFrame = currentFrame;
		}
	}
}

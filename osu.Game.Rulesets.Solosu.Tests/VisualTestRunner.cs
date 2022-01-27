﻿using osu.Framework;
using osu.Framework.Platform;
using osu.Game.Tests;
using System;

namespace osu.Game.Rulesets.Solosu.Tests {
	public static class VisualTestRunner {
		[STAThread]
		public static int Main ( string[] args ) {
			using ( DesktopGameHost host = Host.GetSuitableDesktopHost( @"osu", new() { BindIPC = true } ) ) {
				host.Run( new OsuTestBrowser() );
				return 0;
			}
		}
	}
}

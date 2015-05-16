# Source Tree Layout #

  * **BambooServices, CruiseControlNetServices, HudsonServices, TeamCityServices, TfsServices, BuildBotServices, TravisCiServices:** All of these produce dll’s that go into the \Plugins directory and are dynamically loaded at runtime via the Managed Extensibility Framework (MEF).
  * **MockCiServerServices:** This is similar to the above plugins, but allows you to simulate a CI server via a dialog box that pops up.  This is not included in the installation project.
  * **SimpleSirenOfShameDeviceExample:** This is a sample project for use if you want to build your own app for a Siren of Shame device (see [Introduction to the Siren of Shame Device API](http://sirenofshame.blogspot.com/2011/09/introduction-to-siren-of-shame-device.html))
  * **SirenOfShame:** The main Windows Forms UI
  * **SirenOfShame.HardwareTestGui:** Mainly for our internal use only for loading firmware and testing Siren of Shame devices.
  * **SirenOfShame.Lib:** The library where most of the real work happens.  The most prominent class is RulesEngine.cs.
  * **SirenOfShame.Test.Unit:** Our unit test project.
  * **SoxLib:** A library for converting audio formats.  At the moment this is exclusively for premium units.
  * **SoxLib.Test.Unit:** Integration style test for the SoxLib.
  * **TeensyHidBootloaderLib:** Used by SirenOfShame.Lib to perform firmware upgrades to siren of shame devices.
  * **TestPages:** Legacy, needs to be deleted
  * **UsbLib:** All of the logic for communicating with Siren of Shame devices.
  * **WallOfShame:** Legacy, needs to be deleted
  * **WavToC:** A helper command line utility for converting .wav files into bytes that can put in firmware code that can then play to the Siren of Shame speakers.
  * **SirenOfShame.WixSetup:** The installation project for the windows forms application.
# Getting Started
## Nexus.Client and Nexus.Client.Unity
**Nexus.Client**: .NET API for interacting with Nexus's service.
**Nexus.Client.Unity**: Wrapper integrating Nexus.Client with Unity.

Open `Nexus.Client.sln` in Visual Studio. Edit and run tests.

## Nexus.Client.Unity.Sample
Demonstration application for Nexus service. Open the project in Unity and hit play.

  1) Download Unity Hub: https://unity.com/download
  2) Install Unity via the Hub -- 2020.3.25f1 is known to work
  3) Add a project to the Hub, click "Add", then navigate to `Nexus.Client.Unity.Sample` and click "Select Folder"
  4) Wait for the project to open then press the play button in the top center
  
## Updating the Library
Making changes to `Nexus.Client` will not automatically be picked up by `Nexus.Client.Unity`. After building the client the DLL must be copied to `Nexus.Client.Unity/Assets/nexus.client.unity/Runtime/Nexus.Client.dll`. Changes to the unity client should automatically appear in the sample appliation.
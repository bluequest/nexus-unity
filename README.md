# Getting Started
## Nexus.Client
.NET API for interacting with Nexus's service.

Open `Nexus.Client.sln` in Visual Studio. Edit and run tests.

  1) Download Visual Studio: https://visualstudio.microsoft.com/
  2) Install the Unity and Desktop workloads
  3) Open solution, then navigate to `Nexus.Client.sln` and select it
  4) In the toolbar, select Build > Build Solution to build the three projects
  5) Right-click on the test project and select "Run Tests" in order to execute the unit tests

 > Note: If you receive a warning about not having .NET 4.7.2 installed you can install the Developer Pack here: https://dotnet.microsoft.com/en-us/download/visual-studio-sdks

## Nexus.Client.Unity
Wrapper integrating Nexus.Client with Unity.

  1) Download Unity Hub: https://unity.com/download
  2) Install Unity via the Hub -- 2020.3.25f1 is known to work
  3) Add a project to the Hub, click "Add", then navigate to `Nexus.Client.Unity` and click "Select Folder"
  4) Wait for the project to open then, in the Project window, double click on a script under `nexus.client.unity/Runtime` to open Visual Studio
  
 > Note: the last step above triggers Unity to generate a Visual Studio csproj file for the associated scripts. This has been referenced in the solution we opened in the previous step, so you can now use that solution for the client and Unity client projects.

## Nexus.Client.Unity.Sample
Demonstration application for Nexus service. Open the project in Unity and hit play.

  1) Download Unity Hub: https://unity.com/download
  2) Install Unity via the Hub -- 2020.3.25f1 is known to work
  3) Add a project to the Hub, click "Add", then navigate to `Nexus.Client.Unity.Sample` and click "Select Folder"
  4) Wait for the project to open then press the play button in the top center
  
## Updating the Library
Making changes to `Nexus.Client` will not automatically be picked up by `Nexus.Client.Unity`. After building the client the DLL must be copied to `Nexus.Client.Unity/Assets/nexus.client.unity/Runtime/Nexus.Client.dll`. Changes to the unity client should automatically appear in the sample appliation.
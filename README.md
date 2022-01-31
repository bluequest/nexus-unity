# Getting Started

Hey ya'll, here is a first-draft of our attribution API and an example Unity project showing how a purchase and attribution flow might be put together!

Please remember that this API is in early beta and subject to change quite a bit over the coming days.

If there are any questions please shoot me a note, [I'd love to chat!](mailto:dusty@nexus.gg)

## Sample API Documentation
- https://app.swaggerhub.com/apis-docs/nexus-gg/Attribution/1.0.1-oas3#/

# Nexus - Integration Overview

![Nexus - Integration Overview](https://user-images.githubusercontent.com/389949/151759774-72a2343a-d5c1-42ff-a80a-41a6e4cb002a.png)

## Nexus.Client
.NET API for interacting with Nexus's service. Edit and run tests.

Open `Nexus.Client.sln` in Visual Studio.

  1) Download Visual Studio: https://visualstudio.microsoft.com/
  2) Install the Unity and Desktop workloads
  3) Open solution, then navigate to `Nexus.Client.sln` and select it
  4) In the toolbar, select Build > Build Solution to build the three projects
  5) Right-click on the test project and select "Run Tests" in order to execute the unit tests

 > Note: You may receive a warning and see that the Nexus.Client.Unity project is not found. This is expected until you complete the next step.
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

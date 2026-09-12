#!/usr/bin/dotnet run

#:package Bullseye
#:package SimpleExec

#:include targets/build.cs

Target("start", () => Console.WriteLine(DateTime.Now.ToString("O")));

var build = Target<BuildTarget>(["start"]);

Target("default", dependsOn: [build], () => Console.WriteLine(DateTime.Now.ToString("O")));

await RunTargetsAndExitAsync([.. args, "--parallel",], ex => ex is SimpleExec.ExitCodeException);

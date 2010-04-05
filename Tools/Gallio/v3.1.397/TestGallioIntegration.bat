@echo off
set buildDir=..\..\..\Build\Debug

echo *** Executing Gallio runner tests. ***
Gallio.Echo %buildDir%\Tests\Gallio\Machine.Specifications.TestGallioAdapter.3.1.Tests.dll /plugin-directory:%buildDir% /r:Local

echo *** Executing MSpec examples with the Gallio runnner. IT IS OK FOR THIS TO FAIL. ***
Gallio.Echo %buildDir%\Tests\Gallio\Machine.Specifications.TestGallioAdapter.3.1.TestResources.dll /plugin-directory:%buildDir% /r:Local
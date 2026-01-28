The Chromeleon SDK checks for the correct architecture before setting up the scope:

``` 
assembly.ManifestModule.GetPEKind(out var peKind, out var _);
                              if ((peKind & PortableExecutableKinds.Required32Bit) != PortableExecutableKinds.Required32Bit)
                              {
                                            throw new SdkException(Resources.WrongArchitecture);
                              }

In a test project, the `assembly` is TestHost.x86.exe, which DOES NOT have the Required32Bit flag the SDK is checking against.

In order to have a "test library" bypassing this, the IFPEN.AllotropeConverters.Chromeleon.IntegrationTests project has been created, and set up as an executable project. 
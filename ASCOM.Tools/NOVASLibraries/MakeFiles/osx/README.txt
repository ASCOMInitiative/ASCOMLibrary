This make file creates a "universal"" binary containing both Intel x64 and Apple Silicon Arm binaries. 

The make file creates a universal dylib (OSX dynamic library) file that must be copied to both the osx-x64 and the osx-arm64 executable folders.
The libraries in these folders are compiled native libraries, which means they must be prepared on their target systems and copied here.
The SOFA component uses the library name "libsofa", which means that all native libraries must be named "libsofa.dylib" (MacOS), "libsofa.so" (Linux variants) or "libsofa.dll" (Windows).

It is best to build the library and test application using ths as supplied make file:
	make
	make test

This builds a static library file and runs the test program using it.

We need shared rather than static libraries so these can be build after the SOFA routine object files have been created above, using the command
gcc *.o -pedantic -Wall -shared -o libsofa.so (or libsofa.dylib for MacOS)
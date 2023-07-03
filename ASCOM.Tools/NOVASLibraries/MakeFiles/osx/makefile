CC=gcc

main: nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c
	$(CC) nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -shared -fPIC -arch x86_64 -o libnovas_intel.dylib
	$(CC) checkout-stars-full.c nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -arch x86_64 -o checkout-stars-full_intel
	$(CC) nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -shared -fPIC -arch arm64 -o libnovas_arm.dylib
	$(CC) checkout-stars-full.c nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -arch arm64 -o checkout-stars-full_arm
	$(CC) cio_file.c -lm -arch arm64 -o createciofile
	codesign  --options runtime --timestamp --sign "Developer ID Application: Peter Simpson (D8V242YV2M)" --force --deep --digest-algorithm=sha1,sha256 --entitlements ./entitlements.plist libnovas_intel.dylib
	@otool -hv libnovas_intel.dylib
	@echo "Signed Intel"
	@codesign  --options runtime --timestamp --sign "Developer ID Application: Peter Simpson (D8V242YV2M)" --force --deep --digest-algorithm=sha1,sha256 --entitlements ./entitlements.plist libnovas_arm.dylib
	@otool -hv libnovas_arm.dylib
	@echo "Signed Arm"
	lipo -create -output libnovas.dylib libnovas_intel.dylib libnovas_arm.dylib
	file libnovas.dylib
	chmod +x libnovas.dylib
	codesign  --options runtime --timestamp --sign "Developer ID Application: Peter Simpson (D8V242YV2M)" --force --deep --digest-algorithm=sha1,sha256 --entitlements ./entitlements.plist libnovas.dylib
	otool -hf libnovas.dylib
	codesign -dv --verbose=4 libnovas.dylib
	@echo "Universal NOVAS shared library created and signed."
	codesign  --options runtime --timestamp --sign "Developer ID Application: Peter Simpson (D8V242YV2M)" --force --deep --digest-algorithm=sha1,sha256 --entitlements ./entitlements.plist createciofile
	./createciofile
	@rm -f *.o libnovas_intel.dylib checkout-stars-full_intel libnovas_arm.dylib checkout-stars-full_arm libnovas.dylib

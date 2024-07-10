CC=gcc

main: nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c
	$(CC) nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -shared -fPIC -o libnovas.so
	$(CC) checkout-stars-full.c nutation.c novascon.c novas.c solsys1.c readeph0.c eph_manager.c -lm -o checkout-stars-full

clean:
	@rm -f *.o libnovas.so checkout-stars-full

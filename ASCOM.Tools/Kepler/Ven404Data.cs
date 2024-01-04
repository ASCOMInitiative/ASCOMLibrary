﻿namespace ASCOM.Tools.Kepler
{
    static class Ven404Data
    {
        // /*
        // First date in file = 1228000.50
        // Number of records = 397276.0
        // Days per record = 4.0
        // Julian Years      Lon    Lat    Rad
        // -1349.9 to  -1000.0:   0.23   0.15   0.10
        // -1000.0 to   -500.0:   0.25   0.15   0.10
        // -500.0 to      0.0:   0.20   0.13   0.09
        // 0.0 to    500.0:   0.16   0.11   0.08
        // 500.0 to   1000.0:   0.19   0.09   0.08
        // 1000.0 to   1500.0:   0.16   0.09   0.08
        // 1500.0 to   2000.0:   0.21   0.12   0.08
        // 2000.0 to   2500.0:   0.28   0.14   0.09
        // 2500.0 to   3000.0:   0.30   0.15   0.10
        // 3000.0 to   3000.8:  0.116  0.062  0.058
        // */

        internal static double[] tabl = new double[] { 9.08078d, 55.42416d, 21066413644.989109d, 655127.20186d, 0.00329d, 0.10408d, 0.00268d, -0.01908d, 0.00653d, 0.00183d, 0.15083d, -0.21997d, 6.08596d, 2.34841d, 3.70668d, -0.2274d, -2.29376d, -1.46741d, -0.0384d, 0.01242d, 0.00176d, 0.00913d, 0.00121d, -0.01222d, -1.22624d, 0.65264d, -1.15974d, -1.28172d, 1.00656d, -0.66266d, 0.0156d, -0.00654d, 0.00896d, 0.00069d, 0.21649d, -0.01786d, 0.01239d, 0.00255d, 0.00084d, -0.06086d, -0.00041d, 0.00887d, 0.13453d, -0.20013d, 0.08234d, 0.01575d, 0.00658d, -0.00214d, 0.00254d, 0.00857d, -0.01047d, -0.00519d, 0.63215d, -0.40914d, 0.34271d, -1.53258d, 0.00038d, -0.01437d, -0.02599d, -2.27805d, -0.36873d, -1.01799d, -0.36798d, 1.41356d, -0.08167d, 0.01368d, 0.20676d, 0.06807d, 0.02282d, -0.04691d, 0.30308d, -0.20218d, 0.24785d, 0.27522d, 0.00197d, -0.00499d, 1.43909d, -0.46154d, 0.93459d, 2.99583d, -3.43274d, 0.05672d, -0.06586d, 0.12467d, 0.02505d, -0.08433d, 0.00743d, 0.00174d, -0.04013d, 0.17715d, -0.00603d, -0.01024d, 0.01542d, -0.02378d, 0.00676d, 0.00002d, -0.00168d, -4.89487d, 0.02393d, -0.03064d, 0.0009d, 0.00977d, 0.01223d, 0.00381d, 0.28135d, -0.09158d, 0.1855d, 0.58372d, -0.67437d, 0.01409d, -0.25404d, -0.06863d, 0.06763d, -0.02939d, -0.00009d, -0.04888d, 0.01718d, -0.00978d, -0.01945d, 0.08847d, -0.00135d, -11.2992d, 0.01689d, -0.04756d, 0.02075d, -0.01667d, 0.01397d, 0.00443d, -0.28437d, 0.076d, 0.17996d, -0.44326d, 0.29356d, 1.41869d, -1.58617d, 0.03206d, 0.00229d, -0.00753d, -0.03076d, -2.96766d, 0.00245d, 0.00697d, 0.01063d, -0.02468d, -0.00351d, -0.18179d, -0.01088d, 0.0038d, 0.00496d, 0.02072d, -0.1289d, 0.16719d, -0.0682d, -0.03234d, -60.36135d, -11.74485d, -11.03752d, -3.80145d, -21.33955d, -284.54495d, -763.43839d, 248.50823d, 1493.02775d, 1288.79621d, -2091.10921d, -1851.1542d, -0.00922d, 0.06233d, 0.00004d, 0.00785d, 0.10363d, -0.1677d, 0.45497d, 0.24051d, -0.28057d, 0.61126d, -0.02057d, 0.0001d, 0.00561d, 0.01994d, 0.01416d, -0.00442d, 0.03073d, -0.14961d, -0.06272d, 0.08301d, 0.0204d, 7.12824d, -0.00453d, -0.01815d, 0.00004d, -0.00013d, -0.03593d, -0.18147d, 0.20353d, -0.00683d, 0.00003d, 0.06226d, -0.00443d, 0.00257d, 0.03194d, 0.03254d, 0.00282d, -0.01401d, 0.00422d, 1.03169d, -0.00169d, -0.00591d, -0.00307d, 0.0054d, 0.05511d, 0.00347d, 0.07896d, 0.06583d, 0.00783d, 0.01926d, 0.03109d, 0.15967d, 0.00343d, 0.88734d, 0.01047d, 0.32054d, 0.00814d, 0.00051d, 0.02474d, 0.00047d, 0.00052d, 0.03763d, -57.06618d, 20.34614d, -45.06541d, -115.20465d, 136.46887d, -84.67046d, 92.93308d, 160.44644d, -0.0002d, -0.00082d, 0.02496d, 0.00279d, 0.00849d, 0.00195d, -0.05013d, -0.04331d, -0.00136d, 0.14491d, -0.00183d, -0.00406d, 0.01163d, 0.00093d, -0.00604d, -0.0068d, -0.00036d, 0.06861d, -0.0045d, -0.00969d, 0.00171d, 0.00979d, -0.00152d, 0.03929d, 0.00631d, 0.00048d, -0.00709d, -0.00864d, 1.51002d, -0.24657d, 1.27338d, 2.64699d, -2.4099d, -0.57413d, -0.00023d, 0.03528d, 0.00268d, 0.00522d, -0.0001d, 0.01933d, -0.00006d, 0.011d, 0.06313d, -0.09939d, 0.08571d, 0.03206d, -0.00004d, 0.00645d };
        internal static double[] tabb = new double[] { -23.91858d, 31.44154d, 25.93273d, -67.68643d, -0.00171d, 0.00123d, 0.00001d, -0.00018d, -0.00005d, 0.00018d, -0.00001d, 0.00019d, 0.00733d, 0.0003d, -0.00038d, 0.00011d, 0.00181d, 0.0012d, 0.0001d, 0.00002d, -0.00012d, 0.00002d, 0.00021d, 0.00004d, -0.00403d, 0.00101d, 0.00342d, -0.00328d, 0.01564d, 0.01212d, 0.00011d, 0.0001d, -0.00002d, -0.00004d, -0.00524d, 0.00079d, 0.00011d, 0.00002d, -0.00001d, 0.00003d, 0.00001d, 0.0d, 0.00108d, 0.00035d, 0.00003d, 0.00064d, -0.0d, -0.00002d, -0.00069d, 0.00031d, 0.0002d, 0.00003d, 0.00768d, 0.03697d, -0.07906d, 0.01673d, -0.00003d, -0.00001d, -0.00198d, -0.01045d, 0.01761d, -0.00803d, -0.00751d, 0.04199d, 0.0028d, -0.00213d, -0.00482d, -0.00209d, -0.01077d, 0.00715d, 0.00048d, -0.00004d, 0.00199d, 0.00237d, 0.00017d, -0.00032d, -0.07513d, -0.00658d, -0.04213d, 0.16065d, 0.27661d, 0.06515d, 0.02156d, -0.08144d, -0.23994d, -0.05674d, 0.00167d, 0.00069d, 0.00244d, -0.01247d, -0.001d, 0.00036d, 0.0024d, 0.00012d, 0.0001d, 0.00018d, 0.00208d, -0.00098d, -0.00217d, 0.00707d, -0.00338d, 0.0126d, -0.00127d, -0.00039d, -0.03516d, -0.00544d, -0.01746d, 0.08258d, 0.10633d, 0.02523d, 0.00077d, -0.00214d, -0.02335d, 0.00976d, -0.00019d, 0.00003d, 0.00041d, 0.00039d, 0.00199d, -0.01098d, 0.00813d, -0.00853d, 0.0223d, 0.00349d, -0.0225d, 0.08119d, -0.00214d, -0.00052d, -0.0022d, 0.15216d, 0.17152d, 0.08051d, -0.01561d, 0.27727d, 0.25837d, 0.07021d, -0.00005d, -0.0d, -0.02692d, -0.00047d, -0.00007d, -0.00016d, 0.01072d, 0.01418d, -0.00076d, 0.00379d, -0.00807d, 0.03463d, -0.05199d, 0.0668d, -0.00622d, 0.00787d, 0.00672d, 0.00453d, -10.69951d, -67.43445d, -183.55956d, -37.87932d, -102.30497d, -780.40465d, 2572.2199d, -446.97798d, 1665.42632d, 5698.61327d, -11889.66501d, 2814.93799d, 0.03204d, -0.09479d, 0.00014d, -0.00001d, -0.04118d, -0.04562d, 0.03435d, -0.05878d, 0.017d, 0.02566d, -0.00121d, 0.0017d, 0.0239d, 0.00403d, 0.04629d, 0.01896d, -0.00521d, 0.03215d, -0.01051d, 0.00696d, -0.01332d, -0.08937d, -0.00469d, -0.00751d, 0.00016d, -0.00035d, 0.00492d, -0.0393d, -0.04742d, -0.01013d, 0.00065d, 0.00021d, -0.00006d, 0.00017d, 0.06768d, -0.01558d, -0.00055d, 0.00322d, -0.00287d, -0.01656d, 0.00061d, -0.00041d, 0.0003d, 0.00047d, -0.01436d, -0.00148d, 0.30302d, -0.05511d, -0.0002d, -0.00005d, 0.00042d, -0.00025d, 0.0127d, 0.00458d, -0.00593d, -0.0448d, 0.00005d, -0.00008d, 0.08457d, -0.01569d, 0.00062d, 0.00018d, 9.79942d, -2.48836d, 4.17423d, 6.72044d, -63.33456d, 34.63597d, 39.11878d, -72.89581d, -0.00066d, 0.00036d, -0.00045d, -0.00062d, -0.00287d, -0.00118d, -0.21879d, 0.03947d, 0.00086d, 0.00671d, -0.00113d, 0.00122d, -0.00193d, -0.00029d, -0.03612d, 0.00635d, 0.00024d, 0.00207d, -0.00273d, 0.00443d, -0.00055d, 0.0003d, -0.00451d, 0.00175d, -0.0011d, -0.00015d, -0.02608d, 0.0048d, 2.16555d, -0.70419d, 1.74648d, 0.97514d, -1.1536d, 1.73688d, 0.00004d, 0.00105d, 0.00187d, -0.00311d, 0.00005d, 0.00055d, 0.00004d, 0.00032d, -0.04629d, 0.02292d, -0.00363d, -0.03807d, 0.00002d, 0.0002d };
        internal static double[] tabr = new double[] { -0.24459d, 3.72698d, -6.67281d, 5.24378d, 0.0003d, 0.00003d, -0.00002d, -0.0d, -0.0d, 0.00001d, 0.00032d, 0.00021d, -0.00326d, 0.01002d, 0.00067d, 0.00653d, 0.00243d, -0.00417d, -0.00004d, -0.0001d, -0.00002d, -0.00001d, 0.00004d, -0.00002d, -0.00638d, -0.01453d, 0.01458d, -0.01235d, 0.00755d, 0.0103d, 0.00006d, 0.00014d, 0.0d, 0.00009d, 0.00063d, 0.00176d, 0.00003d, -0.00022d, 0.00112d, 0.00001d, -0.00014d, -0.00001d, 0.00485d, 0.00322d, -0.00035d, 0.00198d, 0.00004d, 0.00013d, -0.00015d, -0.00003d, 0.00011d, -0.00025d, 0.00634d, 0.02207d, 0.0462d, 0.0016d, 0.00045d, 0.00001d, -0.11563d, 0.00643d, -0.05947d, 0.02018d, 0.07704d, 0.01574d, -0.0009d, -0.00471d, -0.00322d, 0.01104d, 0.00265d, -0.00038d, 0.01395d, 0.02165d, -0.01948d, 0.01713d, -0.00057d, -0.00019d, 0.04889d, 0.13403d, -0.28327d, 0.10597d, -0.02325d, -0.35829d, 0.01171d, -0.00904d, 0.00747d, 0.02546d, 0.00029d, -0.0019d, -0.03408d, -0.00703d, 0.00176d, -0.00109d, 0.00463d, 0.00293d, 0.0d, 0.00148d, 1.06691d, -0.00054d, -0.00935d, -0.0079d, 0.00552d, -0.00084d, -0.001d, 0.00336d, 0.02874d, 0.08604d, -0.17876d, 0.05973d, -0.0072d, -0.21195d, 0.02134d, -0.0798d, 0.015d, 0.01398d, 0.01758d, -0.00004d, 0.00371d, 0.0065d, -0.03375d, -0.00723d, 4.65465d, -0.0004d, 0.0204d, 0.00707d, -0.00727d, -0.01144d, -0.00196d, 0.0062d, -0.03396d, -0.12904d, 0.2016d, 0.08092d, -0.67045d, 0.14014d, -0.01571d, -0.75141d, 0.00361d, 0.0011d, 1.42165d, -0.01499d, -0.00334d, 0.00117d, 0.01187d, 0.00507d, 0.08935d, -0.00174d, -0.00211d, -0.00525d, 0.01035d, -0.00252d, -0.08355d, -0.06442d, 0.01616d, -0.03409d, 5.55241d, -30.62428d, 2.03824d, -6.26978d, 143.07279d, -10.24734d, -125.25411d, -380.8536d, -644.78411d, 745.02852d, 926.7d, -1045.0982d, -0.03124d, -0.00465d, -0.00396d, 0.00002d, 0.08518d, 0.05248d, -0.12178d, 0.23023d, -0.30943d, -0.14208d, -0.00005d, -0.01054d, -0.00894d, 0.00233d, -0.00173d, -0.00768d, 0.07881d, 0.01633d, -0.04463d, -0.03347d, -3.92991d, 0.00945d, 0.01524d, -0.00422d, -0.00011d, -0.00005d, 0.10842d, -0.02126d, 0.00349d, 0.12097d, -0.03752d, 0.00001d, -0.00156d, -0.0027d, -0.0152d, 0.01349d, 0.00895d, 0.00186d, -0.67751d, 0.0018d, 0.00516d, -0.00151d, -0.00365d, -0.0021d, -0.00276d, 0.03793d, -0.02637d, 0.03235d, -0.01343d, 0.00541d, -0.1127d, 0.02169d, -0.63365d, 0.00122d, -0.24329d, 0.00428d, -0.0004d, 0.00586d, 0.00581d, 0.01112d, -0.02731d, 0.00008d, -2.69091d, 0.42729d, 2.78805d, 3.43849d, -0.87998d, -6.62373d, 0.56882d, 4.6937d, 0.00005d, -0.00008d, -0.00181d, 0.01767d, -0.00168d, 0.0066d, 0.01802d, -0.01836d, -0.11245d, -0.00061d, 0.00199d, -0.0007d, -0.00076d, 0.00919d, 0.00311d, -0.00165d, -0.0565d, -0.00018d, 0.00121d, -0.00069d, -0.00803d, 0.00146d, -0.0326d, -0.00072d, -0.00042d, 0.00524d, 0.00464d, -0.00339d, -0.06203d, -0.00278d, 0.04145d, 0.02871d, -0.01962d, -0.01362d, -0.0304d, -0.0001d, 0.00085d, -0.00001d, -0.01712d, -0.00006d, -0.00996d, -0.00003d, -0.00029d, 0.00026d, 0.00016d, -0.00005d, -0.00594d, -0.00003d };
        internal static int[] args = new int[] { 0, 3, 2, 2, 5, -5, 6, 0, 3, 2, 2, 1, 3, -8, 4, 0, 3, 5, 1, -14, 2, 2, 3, 0, 3, 3, 2, -7, 3, 4, 4, 0, 2, 8, 2, -13, 3, 2, 3, 6, 2, -10, 3, 3, 5, 0, 1, 1, 7, 0, 2, 1, 5, -2, 6, 0, 2, 1, 2, -3, 4, 2, 2, 2, 5, -4, 6, 1, 1, 1, 6, 0, 3, 3, 2, -5, 3, 1, 5, 0, 3, 3, 2, -5, 3, 2, 5, 0, 2, 1, 5, -1, 6, 0, 2, 2, 2, -6, 4, 1, 2, 2, 5, -3, 6, 0, 1, 2, 6, 0, 2, 3, 5, -5, 6, 0, 1, 1, 5, 1, 2, 2, 5, -2, 6, 0, 2, 3, 2, -5, 3, 2, 2, 5, 2, -8, 3, 1, 1, 2, 5, 0, 2, 2, 1, -5, 2, 1, 2, 6, 2, -10, 3, 0, 2, 2, 2, -3, 3, 2, 2, 1, 2, -2, 3, 1, 2, 4, 2, -7, 3, 0, 2, 4, 2, -6, 3, 0, 1, 1, 4, 0, 2, 1, 2, -2, 4, 0, 2, 2, 2, -5, 4, 0, 2, 1, 2, -1, 3, 0, 2, 1, 1, -3, 2, 0, 2, 2, 2, -4, 3, 0, 2, 6, 2, -9, 3, 0, 2, 3, 2, -4, 3, 2, 2, 1, 1, -2, 2, 0, 1, 1, 3, 0, 2, 1, 2, -1, 4, 0, 2, 2, 2, -4, 4, 0, 2, 5, 2, -7, 3, 0, 2, 2, 2, -2, 3, 0, 2, 1, 2, -3, 5, 0, 2, 1, 2, -3, 3, 0, 2, 7, 2, -10, 3, 0, 2, 1, 2, -2, 5, 1, 2, 4, 2, -5, 3, 1, 3, 1, 2, 1, 5, -5, 6, 0, 2, 1, 2, -1, 5, 0, 3, 1, 2, -3, 5, 5, 6, 0, 2, 1, 2, -2, 6, 0, 2, 1, 2, -1, 6, 0, 1, 3, 4, 0, 2, 7, 2, -13, 3, 0, 3, 1, 2, 2, 5, -5, 6, 1, 1, 1, 2, 5, 2, 9, 2, -13, 3, 0, 3, 1, 2, 1, 5, -2, 6, 0, 2, 2, 2, -3, 4, 2, 2, 3, 2, -6, 4, 0, 2, 1, 2, 1, 5, 0, 2, 2, 2, -5, 3, 0, 2, 6, 2, -8, 3, 0, 2, 2, 1, -4, 2, 0, 2, 3, 2, -3, 3, 0, 1, 2, 3, 0, 2, 3, 2, -7, 3, 0, 2, 5, 2, -6, 3, 1, 2, 2, 2, -2, 4, 0, 2, 3, 2, -5, 4, 0, 2, 2, 2, -1, 3, 0, 2, 7, 2, -9, 3, 0, 2, 4, 2, -4, 3, 0, 2, 1, 2, 1, 3, 0, 2, 3, 2, -4, 4, 0, 2, 6, 2, -7, 3, 0, 2, 3, 2, -2, 3, 0, 2, 2, 2, -4, 5, 0, 2, 2, 2, -3, 5, 0, 2, 2, 2, -2, 5, 0, 2, 5, 2, -5, 3, 0, 2, 2, 2, -3, 6, 0, 2, 2, 2, -1, 5, 0, 2, 2, 2, -2, 6, 0, 1, 2, 2, 3, 2, 2, 2, 1, 5, 0, 2, 7, 2, -8, 3, 0, 2, 2, 1, -3, 2, 0, 2, 4, 2, -3, 3, 0, 2, 6, 2, -6, 3, 0, 2, 3, 2, -1, 3, 0, 2, 8, 2, -9, 3, 0, 2, 5, 2, -4, 3, 0, 2, 7, 2, -7, 3, 0, 2, 4, 2, -2, 3, 0, 2, 3, 2, -4, 5, 0, 2, 3, 2, -3, 5, 0, 2, 9, 2, -10, 3, 0, 2, 3, 2, -2, 5, 0, 1, 3, 2, 2, 2, 8, 2, -8, 3, 0, 2, 5, 2, -3, 3, 0, 2, 9, 2, -9, 3, 0, 2, 10, 2, -10, 3, 0, 1, 4, 2, 1, 2, 11, 2, -11, 3, 0, -1 };

        // /* Total terms = 108, small = 107 */
        internal static PlanetTable ven404 = new PlanetTable(9, new int[19] { 5, 14, 13, 8, 4, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 5, args, tabl, tabb, tabr, 0.72332982d, 3652500.0d, 1.0d);

    }
}
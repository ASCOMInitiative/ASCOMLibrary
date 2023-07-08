﻿
namespace ASCOM.Tools.Kepler
{
    static class Mar404Data
    {
        // /*
        // First date in file = 1228000.50
        // Number of records = 397276.0
        // Days per record = 4.0
        // Julian Years      Lon    Lat    Rad
        // -1349.9 to  -1000.0:   0.42   0.18   0.25
        // -1000.0 to   -500.0:   0.45   0.14   0.21
        // -500.0 to      0.0:   0.37   0.10   0.20
        // 0.0 to    500.0:   0.33   0.09   0.22
        // 500.0 to   1000.0:   0.48   0.07   0.22
        // 1000.0 to   1500.0:   0.40   0.07   0.19
        // 1500.0 to   2000.0:   0.36   0.11   0.19
        // 2000.0 to   2500.0:   0.38   0.14   0.20
        // 2500.0 to   3000.0:   0.45   0.15   0.24
        // 3000.0 to   3000.8:  0.182  0.125  0.087
        // */

        internal static double[] tabl = new double[] { 43471.6614d, 21291.11063d, 2033.37848d, 6890507597.78366d, 1279543.73631d, 317.74183d, 730.69258d, -15.26502d, 277.5696d, -62.96711d, 20.96285d, 1.01857d, -2.19395d, 3.75708d, 3.65854d, 0.01049d, 1.09183d, -0.00605d, -0.04769d, 0.41839d, 0.10091d, 0.03887d, 0.11666d, -0.03301d, 0.02664d, 0.38777d, -0.56974d, 0.02974d, -0.15041d, 0.02179d, -0.00808d, 0.08594d, 0.09773d, -0.00902d, -0.04597d, 0.00762d, -0.03858d, -0.00139d, 0.01562d, 0.02019d, 0.01878d, -0.01244d, 0.00795d, 0.00815d, 0.03501d, -0.00335d, -0.0297d, -0.00518d, -0.01763d, 0.17257d, 0.14698d, -0.14417d, 0.26028d, 0.00062d, -0.0018d, 13.35262d, 39.38771d, -15.49558d, 22.0015d, -7.71321d, -4.20035d, 0.62074d, -1.42376d, 0.07043d, -0.0667d, 0.1696d, -0.06859d, 0.07787d, 0.01845d, -0.01608d, -0.00914d, 5.60438d, -3.44436d, 5.88876d, 6.77238d, -5.29704d, 3.48944d, 0.01291d, 0.0128d, -0.53532d, 0.86584d, 0.79604d, 0.31635d, -3.92977d, -0.94829d, -0.74254d, -1.37947d, 0.17871d, -0.12477d, 0.00171d, 0.11537d, 0.02281d, -0.03922d, -0.00165d, 0.02965d, 1.59773d, 1.24565d, -0.35802d, 1.37272d, -0.44811d, -0.08611d, 3.04184d, -3.39729d, 8.8627d, 6.65967d, -9.1058d, 10.66103d, 0.02015d, -0.00902d, -0.01166d, -0.23957d, -0.12128d, -0.0464d, -0.07114d, 0.14053d, -0.04966d, -0.01665d, 0.28411d, -0.37754d, -1.26265d, 1.01377d, 3.70433d, -0.21025d, -0.00972d, 0.0035d, 0.00997d, 0.0045d, -2.15305d, 3.18147d, -1.81957d, -0.02321d, -0.0256d, -0.35188d, 0.00003d, -0.0111d, 0.00244d, -0.05083d, -0.00216d, -0.02026d, 0.05179d, 0.04188d, 5.92031d, -1.61316d, 3.72001d, 6.98783d, -4.1769d, 2.6125d, 0.04157d, 2.76453d, -1.34043d, 0.74586d, -0.20258d, -0.30467d, 0.00733d, 0.00376d, 1.728d, 0.76593d, 1.26577d, -2.02682d, -1.14637d, -0.91894d, -0.00002d, 0.00036d, 2.54213d, 0.89533d, -0.04166d, 2.36838d, -0.97069d, 0.05486d, 0.46927d, 0.045d, 0.23388d, 0.35005d, 1.61402d, 2.30209d, -0.99859d, 1.63349d, -0.5149d, -0.26112d, 0.27848d, -0.261d, -0.07645d, -0.22001d, 0.92901d, 1.12627d, -0.39829d, 0.7712d, -0.23716d, -0.11245d, -0.02387d, 0.0396d, -0.00802d, 0.02179d, 2.86448d, 1.00246d, -0.14647d, 2.80278d, -1.14143d, 0.05177d, 1.68671d, -1.23451d, 3.16285d, 0.7007d, 0.25817d, 3.17416d, 0.07447d, -0.08116d, -0.03029d, -0.02795d, 0.00816d, 0.01023d, 0.00685d, -0.01075d, -0.34268d, 0.0368d, -0.05488d, -0.0743d, -0.00041d, -0.02968d, 3.13228d, -0.83209d, 1.95765d, 3.78394d, -2.26196d, 1.3852d, -0.00401d, -0.01397d, 1.01604d, -0.99485d, 0.62465d, 0.22431d, -0.05076d, 0.12025d, 4.35229d, -5.04483d, 14.87533d, 9.00826d, -10.37595d, 19.26596d, 0.40352d, 0.19895d, 0.09463d, -0.10774d, -0.17809d, -0.08979d, -0.00796d, -0.04313d, 0.0152d, -0.03538d, 1.53301d, -1.75553d, 4.87236d, 3.23662d, -3.62305d, 6.42351d, -0.00439d, -0.01305d, 0.17194d, -0.64003d, 0.26609d, 0.066d, 0.01767d, -0.00251d, -0.08871d, -0.15523d, 0.01201d, -0.03408d, -0.29126d, -0.07093d, -0.00998d, -0.07876d, 1.05932d, -25.3865d, -0.29354d, 0.04179d, -0.01726d, 0.07473d, -0.07607d, -0.08859d, 0.00842d, -0.02359d, 0.47858d, -0.39809d, 1.25061d, 0.87017d, -0.82453d, 1.56864d, -0.00463d, 0.02385d, -0.2907d, 8.56535d, -0.12495d, 0.0658d, -0.03395d, -0.02465d, -1.06759d, 0.47004d, -0.40281d, -0.23957d, 0.03572d, -0.07012d, 0.00571d, -0.00731d, 0.18601d, -1.34068d, 0.03798d, -0.00532d, 0.00448d, -0.01147d, 1.41208d, -0.00668d, 0.25883d, 1.23788d, -0.57774d, 0.09166d, -2.49664d, -0.25235d, -0.53582d, -0.80126d, 0.10827d, -0.08861d, -0.03577d, 0.06825d, -0.00143d, 0.04633d, 0.01586d, -0.01056d, -0.02106d, 0.03804d, -0.00088d, -0.03458d, -0.00033d, -0.01079d, 0.05821d, -0.02445d, 0.00602d, 0.00721d, -0.00315d, -0.01021d, -0.65454d, 1.08478d, -0.44593d, -0.21492d, -1.35004d, 4.47299d, -4.1917d, 3.51236d, 1946.04629d, 13960.88247d, 576.24572d, 8023.81797d, 2402.48512d, -753.87007d, -6376.99217d, -10278.88014d, -25743.89874d, 15506.87748d, 15609.59853d, 35173.63133d, -3.7037d, 6.29538d, -4.84183d, -0.76942d, -0.02465d, -0.0384d, 0.00565d, -0.06071d, 0.01174d, 0.00253d, -0.0023d, 0.05252d, -0.02813d, 0.01359d, 0.23208d, 0.03393d, 0.01734d, 0.04838d, -0.4634d, -0.18941d, 0.25428d, -0.56925d, 0.05213d, 0.24704d, 0.12922d, -0.01531d, 0.06885d, -0.0851d, 0.01853d, -0.0039d, 0.01196d, -0.3053d, 0.13117d, -0.03533d, 1.79597d, -0.42743d, 0.98545d, 2.13503d, -1.32942d, 0.68005d, -0.01226d, 0.00571d, 0.31081d, 0.34932d, 0.34531d, -0.32947d, -0.00548d, 0.00186d, -0.00157d, -0.00065d, 0.30877d, -0.03864d, 0.04921d, 0.06693d, 0.01761d, -0.04119d, 1.28318d, 0.38546d, 0.06462d, 1.18337d, -0.48698d, 0.07086d, 0.26031d, -0.22813d, 0.10272d, 0.04737d, -0.04506d, -0.38581d, -0.16624d, -0.04588d, 0.00992d, 0.00722d, -0.21041d, 0.2056d, -0.09267d, -0.03438d, 0.32264d, -0.07383d, 0.09553d, -0.3873d, 0.17109d, -0.01342d, -0.02336d, -0.01286d, 0.0023d, 0.04626d, 0.01176d, 0.01868d, -0.15411d, -0.32799d, 0.22083d, -0.14077d, 1.98392d, 1.68058d, -0.02526d, -0.13164d, -0.04447d, -0.00153d, 0.01277d, 0.00553d, -0.26035d, -0.11362d, 0.14672d, -0.32242d, 0.16686d, -0.69957d, 0.40091d, -0.06721d, 0.00837d, 0.09635d, -0.08545d, 0.25178d, -0.22486d, 16.03256d, 0.3413d, -0.06313d, 0.01469d, -0.09012d, -0.00744d, -0.0251d, -0.08492d, -0.13733d, -0.0762d, -0.15329d, 0.13716d, -0.03769d, 2.01176d, -1.35991d, -1.04319d, -2.97226d, -0.01433d, 0.61219d, -0.55522d, 0.38579d, 0.31831d, 0.81843d, -0.04583d, -0.14585d, -0.10218d, 0.16039d, -0.06552d, -0.01802d, 0.0648d, -0.06641d, 0.01672d, -0.00287d, 0.00308d, 0.09982d, -0.05679d, -0.00249d, -0.36034d, 0.52385d, -0.29759d, 0.59539d, -3.59641d, -1.02499d, -547.53774d, 734.1147d, 441.8676d, -626.68255d, -2255.81376d, -1309.01028d, -2025.6959d, 2774.69901d, 1711.21478d, 1509.99797d, -0.99274d, 0.61858d, -0.47634d, -0.33034d, 0.00261d, 0.01183d, -0.00038d, 0.11687d, 0.00994d, -0.01122d, 0.03482d, -0.01942d, -0.11557d, 0.38237d, -0.17826d, 0.0083d, 0.01193d, -0.05469d, 0.01557d, 0.01747d, 0.0273d, -0.01182d, -0.11284d, 0.12939d, -0.05621d, -0.01615d, 0.04258d, 0.01058d, -0.01723d, 0.00963d, 0.20666d, 0.11742d, 0.0783d, -0.02922d, -0.10659d, -0.05407d, 0.07254d, -0.13005d, -0.02365d, 0.24583d, 0.31915d, 1.2706d, 0.00009d, -0.21541d, -0.55324d, -0.45999d, -1.45885d, 0.8653d, 0.85932d, 1.92999d, -0.00755d, -0.00715d, -0.02004d, -0.00788d, 0.01539d, 0.00837d, 0.27652d, -0.50297d, -0.26703d, -0.28159d, 0.0395d, 0.07182d, -0.07177d, 0.1414d, 0.07693d, 0.07564d, -0.01316d, -0.01259d, 0.01529d, 0.07773d, -90.74225d, -378.15784d, -510.3019d, -52.35396d, -89.15267d, 415.56828d, 181.52119d, 54.0157d, -0.01093d, -0.05931d, -0.01344d, -0.0239d, 0.01432d, -0.0247d, -0.01509d, -0.01346d, 0.03352d, 0.02248d, 0.02588d, -0.00948d, 0.0361d, 0.17238d, 0.02909d, -0.04065d, 0.00155d, -0.07025d, -0.09508d, 0.14487d, 0.12441d, 0.16451d, 0.00001d, -0.00005d, -0.00982d, -0.01895d, -0.16968d, 0.36565d, 0.20234d, 0.17789d, -0.04519d, -0.00588d, 0.01268d, 0.00107d, -56.32137d, -58.22145d, -80.5527d, 28.14532d, 11.43301d, 52.05752d, 17.7948d, -2.61997d, -0.00005d, -0.02629d, 0.0108d, -0.0039d, 0.00744d, 0.03132d, 0.01156d, -0.01621d, 0.02162d, 0.02552d, 0.00075d, -0.02497d, 0.02495d, 0.0083d, 0.0323d, 0.00103d, -14.84965d, -4.502d, -9.73043d, 9.40426d, 4.08054d, 5.38571d, 1.53731d, -1.01288d, 0.21076d, 1.74227d, 0.7976d, 0.39583d, 0.09879d, -0.16736d, -0.00723d, -0.01536d };















































































































































































































































        internal static double[] tabb = new double[] { -364.4938d, -47.17612d, -554.97858d, -430.63121d, 596.44312d, -3.94434d, -7.43169d, -0.06665d, -2.23987d, 0.10366d, -0.05567d, -0.01463d, 0.01908d, -0.02611d, -0.0035d, -0.01057d, -0.0061d, -0.00015d, 0.00002d, 0.0001d, 0.00033d, 0.00007d, -0.0d, -0.0001d, -0.00004d, 0.00012d, 0.00002d, -0.00014d, -0.00048d, -0.00003d, -0.00007d, 0.00008d, -0.00005d, -0.00043d, -0.00003d, -0.0001d, -0.00004d, 0.00001d, 0.00001d, -0.00003d, -0.00003d, 0.00004d, 0.00007d, -0.00041d, 0.00031d, 0.00076d, 0.00062d, 0.00001d, -0.00002d, 0.00035d, 0.00053d, 0.00026d, 0.00019d, 0.0002d, 0.0001d, 0.02936d, 0.09624d, -0.01153d, 0.01386d, 0.00551d, -0.0069d, 0.00196d, 0.00148d, -0.00408d, -0.00673d, -0.00067d, -0.00152d, -0.00014d, -0.00005d, 0.0d, 0.00005d, -0.00116d, 0.00276d, -0.00391d, 0.00983d, -0.01327d, -0.01986d, -0.00003d, 0.00001d, 0.01104d, 0.00631d, -0.01364d, 0.01152d, -0.00439d, 0.01103d, -0.00546d, 0.00181d, -0.00039d, -0.00083d, 0.00007d, 0.00002d, -0.0001d, -0.00008d, 0.00005d, 0.00002d, -0.00584d, 0.00512d, -0.00722d, -0.00174d, 0.00101d, -0.00316d, -0.02229d, -0.02797d, -0.10718d, 0.05741d, 0.11403d, 0.10033d, 0.00036d, -0.00022d, 0.00787d, 0.01191d, 0.01756d, -0.02121d, -0.00169d, -0.00364d, 0.0007d, -0.00051d, 0.0185d, -0.06836d, 0.21471d, 0.00162d, -0.29165d, 0.16799d, -0.00002d, 0.00011d, -0.00075d, -0.00077d, -0.00675d, -0.00814d, 0.00029d, -0.00599d, 0.00107d, 0.00013d, 0.0001d, -0.00002d, 0.00005d, 0.0002d, 0.00355d, 0.00306d, -0.00013d, -0.00061d, -0.0295d, -0.00847d, 0.01037d, -0.04783d, 0.04237d, 0.11662d, -0.00331d, 0.00207d, -0.00107d, -0.00264d, 0.00072d, -0.00023d, -0.00151d, 0.00146d, -0.12847d, 0.02294d, 0.03611d, 0.19705d, 0.16855d, -0.28279d, -0.0d, -0.00002d, -0.00525d, -0.03619d, 0.05048d, -0.00481d, -0.00745d, 0.04618d, 0.00286d, 0.00443d, 0.00521d, -0.00351d, 0.002d, 0.00474d, -0.00149d, 0.00031d, -0.00003d, 0.00029d, 0.00686d, 0.02467d, 0.04275d, -0.02223d, 0.02282d, -0.04228d, 0.03312d, 0.01847d, -0.01253d, 0.01601d, 0.00076d, 0.00091d, 0.00045d, 0.00035d, 0.00658d, 0.01586d, -0.0031d, 0.00628d, -0.00045d, 0.00316d, -0.01602d, -0.0034d, -0.01744d, 0.04907d, 0.06426d, 0.02275d, -0.00217d, -0.00377d, -0.00091d, 0.00037d, 0.0004d, -0.00003d, -0.00017d, -0.00027d, 0.00366d, 0.02693d, -0.00934d, 0.00386d, 0.00616d, -0.00037d, 0.02028d, 0.0212d, -0.01768d, 0.02421d, 0.00102d, 0.00877d, 0.00012d, 0.0003d, -0.00019d, -0.02165d, 0.01245d, -0.00742d, 0.00172d, 0.0032d, -0.17117d, -0.12908d, -0.43134d, 0.15617d, 0.21216d, 0.56432d, 0.01139d, -0.00937d, -0.00058d, -0.00337d, -0.00999d, 0.01862d, -0.00621d, -0.0008d, -0.00025d, -0.0014d, 0.0925d, 0.01173d, -0.03549d, 0.14651d, -0.01784d, 0.00945d, 0.0d, -0.00006d, -0.005d, 0.00086d, 0.01079d, -0.00002d, -0.00012d, -0.00029d, -0.02661d, 0.0014d, -0.00524d, -0.0046d, -0.00352d, -0.00563d, -0.00277d, -0.00052d, -0.10171d, -0.02001d, 0.00045d, 0.00265d, -0.00082d, 0.0016d, -0.00302d, -0.00434d, -0.00022d, -0.00134d, 0.03285d, 0.02964d, -0.05612d, -0.00668d, -0.01821d, 0.0659d, 0.00039d, 0.00061d, -0.13531d, -0.03831d, 0.02553d, 0.0213d, -0.00336d, 0.00468d, -0.04522d, -0.0554d, 0.00129d, -0.01767d, 0.00181d, 0.00031d, -0.00011d, -0.00034d, -0.00146d, 0.01101d, -0.0003d, 0.0024d, -0.00039d, 0.00072d, -0.01954d, -0.03822d, 0.09682d, -0.04541d, -0.01567d, 0.09617d, -0.03371d, 0.33028d, -0.12102d, 0.05874d, -0.0099d, -0.02236d, 0.00109d, 0.00158d, -0.00482d, 0.00019d, -0.00036d, 0.00004d, 0.00024d, 0.00201d, 0.00017d, 0.00011d, -0.00012d, 0.00002d, -0.00323d, -0.01062d, -0.0013d, 0.00091d, 0.00056d, -0.00017d, 0.00774d, 0.00601d, 0.0255d, 0.017d, -0.84327d, 0.77533d, -0.71414d, -0.50643d, -473.30877d, -1504.79179d, -458.52274d, -865.82237d, -417.34994d, -681.03976d, 765.50697d, -1653.67165d, 4427.33176d, 710.53895d, -5016.39367d, 4280.60361d, 0.33957d, 0.3839d, -0.38631d, 0.81193d, 0.00154d, -0.00043d, 0.01103d, -0.00017d, -0.00046d, 0.00221d, 0.00059d, 0.00014d, 0.0016d, 0.00475d, 0.06191d, -0.13289d, 0.02884d, -0.00566d, -0.01572d, 0.2378d, -0.0514d, -0.03228d, -0.00716d, -0.00978d, -0.01048d, 0.01317d, -0.01267d, -0.01198d, 0.00037d, -0.0033d, -0.02305d, 0.00355d, -0.00121d, -0.00496d, -0.04369d, -0.01343d, 0.05347d, -0.12433d, 0.0209d, 0.17683d, 0.00028d, -0.0049d, -0.02778d, -0.05587d, -0.01658d, 0.05655d, 0.00204d, -0.00092d, 0.0002d, 0.00014d, -0.00603d, -0.03829d, 0.00778d, -0.00588d, -0.00266d, 0.00097d, -0.02158d, -0.07742d, 0.09306d, -0.01827d, -0.01048d, 0.07885d, -0.02485d, -0.02505d, 0.00471d, -0.01026d, 0.06663d, 0.0111d, 0.00469d, -0.05347d, -0.00016d, -0.00013d, 0.02622d, 0.02273d, -0.01009d, 0.01391d, -0.01042d, -0.00444d, -0.04293d, -0.00767d, -0.00154d, -0.01739d, 0.00353d, -0.00763d, -0.0006d, 0.0001d, -0.00053d, -0.00146d, -0.05317d, 0.0576d, -0.01801d, -0.02099d, -0.02611d, -0.01836d, -0.00256d, 0.00812d, -0.00145d, 0.00054d, -0.00008d, 0.00015d, -0.04087d, 0.0886d, -0.05385d, -0.02134d, 0.02771d, 0.02441d, -0.00234d, 0.01571d, -0.0026d, 0.00097d, 0.10151d, 0.49378d, -0.28555d, 0.11428d, -0.00286d, 0.01224d, 0.0016d, 0.00069d, 0.0d, -0.0004d, -0.13286d, 0.00448d, 0.01225d, -0.00568d, 0.00341d, 0.00224d, -0.23483d, -0.07859d, 0.30733d, -0.21548d, -0.02608d, 0.00756d, 0.09789d, 0.02878d, -0.11968d, 0.08981d, 0.02046d, -0.00888d, 0.02955d, 0.01486d, -0.00981d, 0.01542d, -0.01674d, -0.0154d, 0.00019d, -0.00449d, -0.0214d, 0.00638d, 0.00112d, -0.0073d, -0.08571d, 0.13811d, -0.16951d, -0.02917d, -0.03931d, -0.32643d, -68.64541d, -81.00521d, -47.97737d, 15.7529d, 181.76392d, -36.00647d, -48.32098d, -259.02226d, -265.57466d, 554.05904d, 0.09017d, 0.18803d, -0.12459d, 0.10852d, 0.00211d, 0.00002d, 0.00304d, -0.0037d, 0.00174d, 0.00279d, 0.00139d, 0.00095d, 0.04881d, 0.00262d, -0.0102d, 0.03762d, 0.00987d, 0.00612d, 0.00054d, -0.00036d, 0.00009d, -0.00094d, 0.02279d, 0.01785d, -0.00778d, 0.01263d, 0.0004d, -0.00112d, -0.00452d, -0.00662d, 0.00483d, -0.0003d, -0.00054d, -0.00205d, -0.00052d, -0.00362d, -0.00215d, -0.00247d, 0.02893d, -0.01965d, -0.00004d, 0.04114d, -0.00284d, -0.00103d, 0.01827d, -0.07822d, 0.1801d, 0.04805d, -0.21702d, 0.18808d, 0.00095d, -0.00132d, -0.01488d, 0.00746d, 0.00198d, 0.0019d, 0.01032d, 0.03392d, 0.04318d, -0.07332d, -0.01004d, 0.00787d, -0.00308d, -0.01177d, -0.01431d, 0.02659d, 0.00273d, -0.00374d, -0.02545d, 0.00644d, 28.68376d, 13.74978d, 29.60401d, -47.98255d, -65.91944d, -18.48404d, -1.7358d, 64.67487d, -0.02492d, 0.00104d, -0.00829d, -0.00134d, 0.00077d, 0.00005d, -0.00513d, 0.00403d, 0.00071d, -0.00047d, -0.00023d, -0.00063d, 0.0012d, 0.0037d, -0.00038d, -0.00037d, 0.0008d, -0.00018d, 0.00866d, 0.00156d, -0.01064d, 0.02131d, 0.0d, -0.00001d, 0.00038d, -0.00068d, -0.00909d, -0.02187d, -0.02599d, 0.05507d, -0.00022d, -0.01468d, 0.00032d, 0.005d, 9.86233d, -2.85314d, -2.25791d, -13.83444d, -12.38794d, 3.79861d, 2.76343d, 6.63505d, 0.00066d, 0.00007d, -0.00016d, -0.00039d, 0.00014d, 0.00059d, -0.00031d, -0.00024d, -0.00168d, 0.00259d, 0.00007d, -0.00005d, -0.00052d, 0.00558d, 0.0011d, 0.01037d, 1.59224d, -2.37284d, -2.00023d, -2.2828d, -1.49571d, 1.48293d, 0.60041d, 0.56376d, -0.54386d, 0.03568d, -0.10392d, 0.31005d, 0.09104d, 0.03015d, 0.00826d, -0.00524d };















































































































































































































































        internal static double[] tabr = new double[] { -816.07287d, -381.41365d, -33.69436d, 177.22955d, 0.1863d, -8.29605d, -11.15519d, -0.57407d, -3.53642d, 0.16663d, -0.06334d, -0.03056d, 0.02767d, -0.04161d, 0.03917d, -0.02425d, 0.00204d, -0.00034d, 0.00023d, 0.00058d, -0.00111d, 0.00039d, -0.00015d, 0.00006d, -0.00023d, 0.00237d, 0.00191d, 0.00154d, -0.00029d, 0.00009d, 0.00011d, -0.00041d, 0.00037d, -0.0001d, -0.00064d, 0.00015d, -0.00005d, 0.00012d, -0.00003d, -0.00034d, 0.00026d, 0.00011d, -0.00007d, -0.00158d, 0.00087d, 0.00278d, 0.00137d, 0.00024d, -0.0002d, 0.0053d, -0.00448d, 0.0078d, 0.00408d, 0.00062d, 0.00035d, -1.35261d, 0.79891d, -0.81597d, -0.43774d, 0.14713d, -0.27415d, 0.05298d, 0.0223d, -0.02089d, -0.0107d, -0.00374d, 0.00342d, -0.00142d, 0.0027d, -0.00039d, 0.00063d, 0.16024d, 0.27088d, -0.32127d, 0.27467d, -0.16615d, -0.2446d, -0.00073d, 0.00032d, -0.0571d, -0.05265d, -0.06025d, 0.0512d, -0.05295d, 0.23477d, -0.08211d, 0.04575d, -0.00769d, -0.01067d, -0.0057d, 0.00015d, -0.00251d, -0.0014d, -0.00131d, -0.00018d, -0.12246d, 0.15836d, -0.13065d, -0.03222d, 0.00795d, -0.04232d, -0.36585d, -0.31154d, 0.68504d, -0.96006d, 1.19304d, 0.88631d, 0.00132d, 0.00046d, 0.13105d, 0.04252d, 0.05164d, -0.06837d, -0.01351d, -0.01458d, 0.00376d, -0.00557d, 0.28532d, -0.1729d, -0.53946d, -0.79365d, -0.95246d, 0.74984d, 0.00019d, 0.00132d, -0.00163d, -0.00295d, -0.40106d, -0.26573d, -0.00155d, -0.22655d, 0.04349d, -0.00376d, 0.00149d, -0.00001d, 0.00523d, 0.00078d, 0.01203d, 0.00558d, -0.00708d, 0.0052d, -0.36428d, -1.28827d, 1.50845d, -0.83063d, 0.58802d, 0.89998d, -0.55256d, 0.01255d, -0.15169d, -0.26715d, 0.06061d, -0.04122d, -0.00397d, 0.00534d, -0.52576d, 1.22031d, 1.44098d, 0.92406d, 0.67214d, -0.85486d, -0.0001d, 0.00001d, 0.2882d, -0.84198d, 0.78291d, 0.00251d, 0.02398d, 0.32093d, -0.02331d, 0.10109d, -0.07555d, 0.03557d, -0.6158d, 0.43399d, -0.43779d, -0.2639d, 0.06885d, -0.13803d, 0.17694d, 0.19245d, 0.15119d, -0.051d, 0.49469d, -0.45028d, 0.3359d, 0.15677d, -0.04702d, 0.10265d, -0.00942d, -0.0058d, -0.00555d, -0.00252d, -0.32933d, 0.92539d, -0.91004d, -0.0449d, -0.01812d, -0.37121d, 0.34695d, 0.50855d, -0.24721d, 0.86063d, -0.84747d, 0.01983d, 0.01948d, 0.02039d, 0.00748d, -0.00727d, -0.00271d, 0.0022d, 0.00309d, 0.00196d, 0.0203d, 0.17201d, -0.03716d, 0.02801d, 0.01871d, 0.00002d, 0.31736d, 1.17319d, -1.42245d, 0.73416d, -0.52302d, -0.85056d, 0.00522d, -0.00126d, 0.33571d, 0.34594d, -0.07709d, 0.21114d, -0.04066d, -0.01742d, 1.72228d, 1.46934d, -3.06437d, 5.06723d, -6.538d, -3.55839d, -0.06933d, 0.13815d, 0.03684d, 0.03284d, -0.04841d, 0.09571d, -0.0235d, 0.00418d, 0.01302d, 0.00579d, 0.73408d, 0.64718d, -1.37437d, 2.04816d, -2.70756d, -1.52808d, 0.00523d, -0.00166d, 0.25915d, 0.069d, -0.02758d, 0.10707d, 0.00062d, 0.00744d, -0.08117d, 0.0484d, -0.01806d, -0.00637d, 0.03034d, -0.12414d, 0.03419d, -0.00388d, 10.92603d, 0.48169d, -0.01753d, -0.12853d, -0.03207d, -0.00801d, 0.03904d, -0.03326d, 0.01033d, 0.00366d, 0.17249d, 0.20846d, -0.38157d, 0.54639d, -0.68518d, -0.36121d, -0.01043d, -0.00186d, -3.33843d, -0.16353d, 0.03462d, 0.06669d, -0.01305d, 0.01803d, -0.22703d, -0.52219d, 0.11709d, -0.19628d, 0.0341d, 0.01741d, 0.00338d, 0.00265d, 0.63213d, 0.08944d, 0.00236d, 0.01829d, 0.00546d, 0.00218d, 0.00073d, -0.7257d, 0.63698d, -0.1334d, 0.04698d, 0.29716d, -0.13126d, 1.27705d, -0.4098d, 0.274d, -0.04525d, -0.05529d, -0.03249d, -0.01696d, -0.02314d, -0.00076d, 0.0051d, 0.00764d, -0.01847d, -0.01021d, 0.01688d, -0.00044d, 0.00531d, -0.00016d, -0.01219d, -0.02903d, -0.00361d, 0.00299d, 0.00504d, -0.00153d, -0.53625d, -0.3246d, 0.10642d, -0.2207d, -2.21651d, -0.66036d, -1.74652d, -2.08198d, -6810.78679d, 967.02869d, -3915.9714d, 291.65905d, 372.99563d, 1196.01966d, 5108.01033d, -3172.64698d, -7685.78246d, -12789.43898d, -17474.50562d, 7757.84703d, 3.13224d, 1.84743d, -0.38257d, 2.4059d, 0.0186d, -0.01217d, 0.03004d, 0.00278d, -0.00125d, 0.00579d, -0.02673d, -0.00112d, 0.00662d, 0.01374d, -0.02729d, 0.13109d, -0.02836d, 0.00877d, 0.12171d, -0.27475d, 0.34765d, 0.15882d, -0.12548d, 0.02603d, 0.0071d, 0.06538d, -0.04039d, -0.03257d, -0.00186d, -0.0088d, 0.16643d, 0.00707d, 0.01918d, 0.07156d, -0.20459d, -0.85107d, 1.01832d, -0.47158d, 0.32582d, 0.63002d, -0.00282d, -0.00711d, -0.19695d, 0.15053d, 0.15676d, 0.17847d, 0.00071d, 0.00286d, -0.00039d, 0.00083d, 0.02009d, 0.17859d, -0.03894d, 0.02805d, 0.02379d, 0.00752d, 0.17529d, -0.57783d, 0.53257d, -0.02829d, 0.03211d, 0.21777d, 0.13813d, 0.16305d, -0.02996d, 0.06303d, 0.21058d, -0.02659d, 0.02596d, -0.08808d, -0.00389d, 0.00586d, 0.08986d, 0.09204d, -0.0148d, 0.04031d, 0.06115d, 0.18366d, 0.25636d, 0.06905d, 0.00719d, 0.11391d, 0.00636d, -0.01113d, -0.02808d, 0.0015d, -0.01219d, 0.00832d, 0.28626d, -0.09573d, 0.10481d, 0.16559d, -0.94578d, 1.26394d, 0.08846d, -0.01623d, 0.00082d, -0.0264d, -0.00347d, 0.00798d, 0.12873d, -0.21248d, 0.27999d, 0.14348d, 0.44082d, 0.10453d, 0.04362d, 0.25332d, -0.06077d, 0.00555d, -0.06947d, -0.05511d, -10.08703d, -0.10614d, 0.04059d, 0.21355d, 0.05632d, 0.00871d, 0.01599d, -0.00531d, 0.36835d, -0.0353d, 0.09519d, -0.04961d, 0.02568d, 0.08613d, 0.57033d, 0.84599d, 1.27123d, -0.41266d, -0.36937d, -0.00655d, -0.16547d, -0.24d, -0.35213d, 0.13345d, 0.0587d, -0.01524d, 0.06419d, 0.04136d, -0.00681d, 0.02606d, -0.02519d, -0.02732d, -0.00105d, -0.00677d, -0.03891d, 0.00106d, 0.00087d, -0.02256d, -0.20834d, -0.14624d, -0.23178d, -0.11786d, 0.32479d, -1.41222d, -303.74549d, -202.79324d, 260.2029d, 184.8432d, 536.68016d, -881.56427d, -1125.64824d, -791.09928d, -596.61162d, 659.35664d, 0.24561d, 0.39519d, -0.12601d, 0.18709d, -0.007d, 0.00136d, 0.3075d, 0.00009d, 0.00443d, 0.00384d, 0.0117d, 0.02078d, 0.15043d, 0.04802d, 0.00386d, 0.06942d, 0.02107d, 0.00495d, -0.01067d, 0.00951d, 0.00937d, 0.01996d, 0.04922d, 0.04337d, -0.00583d, 0.0211d, -0.00691d, 0.02793d, -0.00364d, -0.00682d, -0.09143d, 0.15369d, 0.02043d, 0.05451d, 0.04053d, -0.08179d, 0.09645d, 0.0533d, -0.10149d, -0.01594d, -0.96773d, 0.1366d, 0.17326d, 0.00013d, 0.2099d, -0.23184d, -0.38407d, -0.64733d, -0.84754d, 0.38889d, 0.0031d, -0.0034d, 0.0097d, -0.00788d, -0.01111d, 0.00677d, 0.18147d, 0.09968d, 0.1017d, -0.09233d, -0.03165d, 0.0179d, -0.04727d, -0.02364d, -0.02546d, 0.02451d, 0.00442d, -0.00426d, -0.0254d, 0.00471d, 130.42585d, -31.30051d, 17.99957d, -174.75585d, -142.96798d, -27.89752d, -19.42122d, 59.14872d, -0.01899d, 0.00388d, -0.01265d, 0.00694d, 0.01966d, 0.0114d, -0.00439d, 0.00503d, -0.01867d, 0.02826d, 0.00752d, 0.02012d, -0.14734d, 0.01909d, 0.03312d, 0.02327d, 0.05843d, 0.00061d, -0.06958d, -0.05798d, -0.09174d, 0.06242d, 0.00003d, 0.00001d, 0.0067d, -0.00305d, -0.13637d, -0.06058d, -0.06372d, 0.07257d, 0.00209d, -0.01369d, -0.00044d, 0.00355d, 17.90079d, -17.4827d, -8.77915d, -24.54483d, -15.67123d, 3.62668d, 0.52038d, 5.1322d, 0.02574d, 0.00003d, 0.00339d, 0.00919d, -0.02778d, 0.00464d, 0.01429d, 0.01003d, -0.01661d, 0.01327d, 0.02216d, 0.00034d, -0.00389d, 0.01076d, -0.00035d, 0.00983d, 1.23731d, -4.18017d, -2.61932d, -2.66346d, -1.4554d, 1.1031d, 0.23322d, 0.40775d, -0.43623d, 0.06212d, -0.099d, 0.19456d, 0.03639d, 0.02566d, 0.00309d, -0.00116d };















































































































































































































































        internal static int[] args = new int[] { 0, 4, 3, 4, 3, -8, 4, 3, 5, 2, 3, 5, 2, -6, 3, -4, 4, 0, 2, 2, 5, -5, 6, 1, 3, 12, 3, -24, 4, 9, 5, 0, 3, 2, 2, 1, 3, -8, 4, 1, 3, 11, 3, -21, 4, 2, 5, 0, 3, 3, 2, -7, 3, 4, 4, 0, 3, 7, 3, -13, 4, -1, 5, 1, 3, 1, 3, -2, 4, 2, 6, 0, 3, 1, 2, -8, 3, 12, 4, 1, 3, 1, 4, -8, 5, 4, 6, 0, 3, 1, 4, -7, 5, 2, 6, 0, 3, 1, 4, -9, 5, 7, 6, 0, 1, 1, 7, 0, 2, 1, 5, -2, 6, 0, 3, 1, 3, -2, 4, 1, 5, 0, 3, 3, 3, -6, 4, 2, 5, 1, 3, 12, 3, -23, 4, 3, 5, 0, 2, 8, 3, -15, 4, 3, 2, 1, 4, -6, 5, 2, 3, 2, 2, -7, 3, 7, 4, 0, 2, 1, 2, -3, 4, 2, 2, 2, 5, -4, 6, 0, 1, 1, 6, 1, 2, 9, 3, -17, 4, 2, 3, 2, 3, -4, 4, 2, 5, 0, 3, 2, 3, -4, 4, 1, 5, 0, 2, 1, 5, -1, 6, 0, 2, 2, 2, -6, 4, 2, 2, 1, 3, -2, 4, 2, 2, 2, 5, -3, 6, 0, 1, 2, 6, 1, 2, 3, 5, -5, 6, 1, 1, 1, 5, 2, 3, 4, 3, -8, 4, 2, 5, 0, 2, 1, 5, -5, 6, 0, 2, 7, 3, -13, 4, 2, 2, 3, 2, -9, 4, 0, 2, 2, 5, -2, 6, 0, 1, 3, 6, 0, 2, 1, 4, -5, 5, 0, 2, 2, 3, -4, 4, 2, 2, 6, 3, -11, 4, 2, 2, 4, 5, -5, 6, 0, 1, 2, 5, 2, 3, 1, 4, -3, 5, -3, 6, 0, 2, 3, 3, -6, 4, 2, 2, 1, 4, -4, 5, 1, 2, 5, 3, -9, 4, 2, 1, 3, 5, 1, 2, 4, 3, -8, 4, 2, 3, 1, 4, -4, 5, 2, 6, 0, 3, 1, 4, -1, 5, -5, 6, 0, 2, 4, 3, -7, 4, 2, 2, 1, 4, -3, 5, 2, 3, 1, 4, -5, 5, 5, 6, 1, 3, 1, 4, -4, 5, 3, 6, 0, 3, 1, 4, -3, 5, 1, 6, 0, 2, 5, 3, -10, 4, 1, 1, 4, 5, 0, 2, 3, 3, -5, 4, 2, 3, 1, 4, -3, 5, 2, 6, 0, 2, 1, 4, -5, 6, 2, 2, 1, 4, -2, 5, 2, 3, 1, 4, -4, 5, 5, 6, 1, 2, 6, 3, -12, 4, 1, 2, 1, 4, -4, 6, 0, 2, 2, 3, -3, 4, 2, 2, 10, 3, -18, 4, 0, 2, 1, 4, -3, 6, 1, 3, 1, 4, -2, 5, 2, 6, 0, 2, 7, 3, -14, 4, 1, 3, 1, 4, 1, 5, -5, 6, 1, 2, 1, 4, -1, 5, 0, 3, 1, 4, -3, 5, 5, 6, 1, 3, 1, 4, 2, 5, -7, 6, 1, 2, 1, 4, -2, 6, 2, 3, 1, 4, -2, 5, 3, 6, 0, 2, 1, 3, -1, 4, 0, 2, 2, 2, -7, 4, 1, 2, 9, 3, -16, 4, 2, 2, 1, 4, -3, 7, 0, 2, 1, 4, -1, 6, 0, 3, 1, 4, -2, 5, 4, 6, 1, 2, 1, 2, -4, 4, 2, 2, 8, 3, -16, 4, 2, 2, 1, 4, -2, 7, 0, 3, 3, 3, -5, 4, 2, 5, 0, 3, 1, 4, 1, 5, -3, 6, 0, 2, 1, 4, -2, 8, 0, 2, 1, 4, -1, 7, 0, 2, 1, 4, -1, 8, 0, 3, 3, 2, -7, 3, 3, 4, 0, 3, 2, 2, 1, 3, -7, 4, 0, 3, 1, 4, 1, 6, -3, 7, 0, 3, 1, 4, 2, 5, -5, 6, 1, 3, 4, 3, -7, 4, 3, 5, 1, 1, 1, 4, 5, 3, 4, 3, -9, 4, 3, 5, 1, 3, 1, 4, -2, 5, 5, 6, 0, 3, 3, 2, -7, 3, 5, 4, 0, 3, 1, 3, -1, 4, 2, 6, 0, 3, 1, 4, 1, 5, -2, 6, 0, 3, 3, 3, -7, 4, 2, 5, 0, 2, 8, 3, -14, 4, 1, 2, 1, 2, -2, 4, 1, 2, 1, 4, 1, 6, 1, 2, 9, 3, -18, 4, 1, 2, 2, 2, -5, 4, 1, 2, 1, 3, -3, 4, 2, 2, 1, 4, 2, 6, 0, 2, 1, 4, 1, 5, 1, 3, 4, 3, -9, 4, 2, 5, 1, 2, 7, 3, -12, 4, 1, 2, 2, 4, -5, 5, 0, 2, 2, 3, -5, 4, 2, 2, 6, 3, -10, 4, 1, 2, 1, 4, 2, 5, 1, 3, 2, 4, -5, 5, 2, 6, 0, 2, 3, 3, -7, 4, 1, 2, 2, 4, -4, 5, 0, 2, 5, 3, -8, 4, 1, 2, 1, 4, 3, 5, 0, 3, 2, 4, -4, 5, 2, 6, 0, 3, 2, 4, -1, 5, -5, 6, 0, 2, 4, 3, -6, 4, 1, 2, 2, 4, -3, 5, 0, 3, 2, 4, -5, 5, 5, 6, 1, 3, 2, 4, -4, 5, 3, 6, 0, 2, 3, 3, -4, 4, 1, 2, 2, 4, -5, 6, 2, 2, 2, 4, -2, 5, 1, 3, 2, 4, -4, 5, 5, 6, 1, 2, 2, 4, -4, 6, 0, 2, 2, 3, -2, 4, 0, 2, 2, 4, -3, 6, 1, 2, 2, 4, -1, 5, 1, 2, 2, 4, -2, 6, 0, 1, 1, 3, 1, 2, 2, 4, -1, 6, 0, 2, 1, 2, -5, 4, 1, 2, 8, 3, -17, 4, 1, 3, 2, 4, 2, 5, -5, 6, 1, 3, 4, 3, -6, 4, 3, 5, 1, 3, 10, 3, -17, 4, 3, 6, 0, 1, 2, 4, 4, 3, 4, 3, -10, 4, 3, 5, 1, 2, 8, 3, -13, 4, 0, 2, 1, 2, -1, 4, 0, 2, 2, 4, 1, 6, 0, 2, 2, 2, -4, 4, 0, 2, 1, 3, -4, 4, 1, 2, 2, 4, 1, 5, 0, 2, 7, 3, -11, 4, 0, 2, 3, 4, -5, 5, 0, 2, 2, 3, -6, 4, 1, 2, 6, 3, -9, 4, 0, 2, 2, 4, 2, 5, 0, 2, 3, 4, -4, 5, 0, 2, 5, 3, -7, 4, 0, 2, 4, 3, -5, 4, 1, 2, 3, 4, -3, 5, 1, 2, 3, 3, -3, 4, 0, 2, 3, 4, -2, 5, 2, 3, 3, 4, -4, 5, 5, 6, 0, 2, 2, 3, -1, 4, 0, 2, 3, 4, -3, 6, 0, 2, 3, 4, -1, 5, 1, 2, 3, 4, -2, 6, 0, 2, 1, 3, 1, 4, 1, 2, 3, 4, -1, 6, 0, 3, 4, 3, -5, 4, 3, 5, 0, 1, 3, 4, 3, 3, 4, 3, -11, 4, 3, 5, 0, 1, 1, 2, 0, 2, 2, 2, -3, 4, 0, 2, 1, 3, -5, 4, 0, 2, 4, 4, -5, 5, 0, 2, 6, 3, -8, 4, 0, 2, 4, 4, -4, 5, 0, 2, 5, 3, -6, 4, 0, 2, 4, 3, -4, 4, 0, 2, 4, 4, -3, 5, 1, 3, 6, 3, -8, 4, 2, 5, 0, 2, 3, 3, -2, 4, 0, 2, 4, 4, -2, 5, 1, 2, 4, 4, -1, 5, 0, 2, 1, 3, 2, 4, 0, 1, 4, 4, 3, 2, 2, 2, -2, 4, 0, 2, 7, 3, -9, 4, 0, 2, 5, 4, -5, 5, 0, 2, 6, 3, -7, 4, 0, 2, 5, 4, -4, 5, 0, 2, 5, 3, -5, 4, 0, 2, 5, 4, -3, 5, 0, 2, 5, 4, -2, 5, 0, 1, 5, 4, 3, 1, 6, 4, 2, 1, 7, 4, 0, -1 };










































































































































































































        // /* Total terms = 201, small = 199 */
        internal static KeplerGlobalCode.plantbl mar404 = new KeplerGlobalCode.plantbl(9, new int[19] { 0, 5, 12, 24, 9, 7, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 5, args, tabl, tabb, tabr, 1.53033488271d, 3652500.0d, 1.0d);










    }
}
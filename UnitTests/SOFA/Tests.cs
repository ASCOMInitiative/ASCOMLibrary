using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static ASCOM.Tools.Sofa;

namespace SOFA
{
    public class SofaTests
    {
        DateTime sofaDatTestDate = new DateTime(2024, 1, 1, 0, 0, 0); // This must be the first day of the year following the SOFA release

        private readonly ITestOutputHelper logger;

        public SofaTests(ITestOutputHelper testOutputHelper)
        {
            logger = testOutputHelper;
        }

        #region ASCOM Additional members

        [Fact]
        public void IssueDate()
        {
            Assert.Equal("2023-10-11", Sofa.SofaIssueDate());
        }

        [Fact]
        public void ReleaseNumber()
        {
            Assert.Equal("19", Sofa.SofaReleaseNumber().ToString());
        }

        [Fact]
        public void RevisionDate()
        {
            Assert.Equal("2023-10-11", Sofa.SofaRevisionDate());
        }
        [Fact]
        public void RevisionNumber()
        {
            Assert.Equal("0", Sofa.SofaRevisionNumber().ToString());
        }

        #endregion

        #region SOFA Standard members

        [Fact]
        public void A2af()
        {
            byte sign = (byte)'X';
            int[] idmsf = new int[4];
            Sofa.A2af(4, 2.345, out sign, idmsf);
            char signChar = (char)sign;
            Assert.Equal('+', signChar);
            Assert.Equal(134, idmsf[0]);
            Assert.Equal(21, idmsf[1]);
            Assert.Equal(30, idmsf[2]);
            Assert.Equal(9706, idmsf[3]);
        }

        [Fact]
        public void A2tf()
        {
            byte sign = (byte)'X';
            int[] ihmsf = new int[4];
            Sofa.A2tf(4, -3.01234, out sign, ihmsf);
            char signChar = (char)sign;
            Assert.Equal('-', signChar);
            Assert.Equal(11, ihmsf[0]);
            Assert.Equal(30, ihmsf[1]);
            Assert.Equal(22, ihmsf[2]);
            Assert.Equal(6484, ihmsf[3]);
        }
        [Fact]
        public void Ab()
        {
            double[] pnat = new double[3];
            double[] v = new double[3];
            double s, bm1;
            double[] ppr = new double[3];
            pnat[0] = -0.76321968546737951;
            pnat[1] = -0.60869453983060384;
            pnat[2] = -0.21676408580639883;
            v[0] = 2.1044018893653786e-5;
            v[1] = -8.9108923304429319e-5;
            v[2] = -3.8633714797716569e-5;
            s = 0.99980921395708788;
            bm1 = 0.99999999506209258;
            Sofa.Ab(pnat, v, s, bm1, ppr);
            Assert.Equal(-0.7631631094219556269, ppr[0], 12);
            Assert.Equal(-0.6087553082505590832, ppr[1], 12);
            Assert.Equal(-0.2167926269368471279, ppr[2], 12);
        }

        [Fact]
        public void Ae2hd()
        {
            double a, e, p, h = 0, d = 0;
            a = 5.5;
            e = 1.1;
            p = 0.7;

            Sofa.Ae2hd(a, e, p, ref h, ref d);
            Assert.Equal(0.5933291115507309663, h, 14);
            Assert.Equal(0.9613934761647817620, d, 14);
        }

        [Fact]
        public void Af2a()
        {
            double a = 0;
            int j = Sofa.Af2a('-', 45, 13, 27.2, ref a);
            Assert.Equal(0, j);
            Assert.Equal(-0.7893115794313644842, a, 12);
        }

        [Fact]
        public void Anp()
        {
            double r = Sofa.Anp(-0.1);
            Assert.Equal(6.183185307179586477, r, 12);
        }

        [Fact]
        public void Anpm()
        {
            double r = Sofa.Anpm(-4.0);
            Assert.Equal(2.283185307179586477, r, 12);
        }

        [Fact]
        public void Apcg()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[] ebpv = new double[6]; // flattened 2x3
            double[] ehp = new double[3];
            // ebpv[0][0]..ebpv[1][2]
            ebpv[0] = 0.901310875;
            ebpv[1] = -0.417402664;
            ebpv[2] = -0.180982288;
            ebpv[3] = 0.00742727954;
            ebpv[4] = 0.0140507459;
            ebpv[5] = 0.00609045792;
            ehp[0] = 0.903358544;
            ehp[1] = -0.415395237;
            ehp[2] = -0.180084014;
            // astrom is returned via ref/out
            var astrom = new Sofa.Astrom();
            Sofa.Apcg(date1, date2, ebpv, ehp, ref astrom);
            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.901310875, astrom.eb[0], 12);
            Assert.Equal(-0.417402664, astrom.eb[1], 12);
            Assert.Equal(-0.180982288, astrom.eb[2], 12);
            Assert.Equal(0.8940025429324143045, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268679817955, astrom.eh[1], 12);
            Assert.Equal(-0.1782189004872870264, astrom.eh[2], 12);
            Assert.Equal(1.010465295811013146, astrom.em, 12);
            Assert.Equal(0.4289638913597693554e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034051581320575e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555136380563427e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686012981, astrom.bm1, 12);
            Assert.Equal(1.0, astrom.bpn[0], 0);
            Assert.Equal(0.0, astrom.bpn[3], 0);
            Assert.Equal(0.0, astrom.bpn[6], 0);
            Assert.Equal(0.0, astrom.bpn[1], 0);
            Assert.Equal(1.0, astrom.bpn[4], 0);
            Assert.Equal(0.0, astrom.bpn[7], 0);
            Assert.Equal(0.0, astrom.bpn[2], 0);
            Assert.Equal(0.0, astrom.bpn[5], 0);
            Assert.Equal(1.0, astrom.bpn[8], 0);
        }

        [Fact]
        public void Apcg13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            Sofa.Apcg13(date1, date2, ref astrom);
            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.9013108747340644755, astrom.eb[0], 12);
            Assert.Equal(-0.4174026640406119957, astrom.eb[1], 12);
            Assert.Equal(-0.1809822877867817771, astrom.eb[2], 12);
            Assert.Equal(0.8940025429255499549, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268331896318, astrom.eh[1], 12);
            Assert.Equal(-0.1782189006019749850, astrom.eh[2], 12);
            Assert.Equal(1.010465295964664178, astrom.em, 12);
            Assert.Equal(0.4289638912941341125e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034032405042132e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555135536470279e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686013142, astrom.bm1, 12);

            Assert.Equal(1.0, astrom.bpn[0], 15);
            Assert.Equal(0.0, astrom.bpn[3], 15);
            Assert.Equal(0.0, astrom.bpn[6], 15);
            Assert.Equal(0.0, astrom.bpn[1], 15);
            Assert.Equal(1.0, astrom.bpn[4], 15);
            Assert.Equal(0.0, astrom.bpn[7], 15);
            Assert.Equal(0.0, astrom.bpn[2], 15);
            Assert.Equal(0.0, astrom.bpn[5], 15);
            Assert.Equal(1.0, astrom.bpn[8], 15);
        }

        [Fact]
        public void Apci()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[] ebpv = new double[6];
            double[] ehp = new double[3];
            double x = 0.0013122272;
            double y = -2.92808623e-5;
            double s = 3.05749468e-8;
            var astrom = new Sofa.Astrom();
            ebpv[0] = 0.901310875;
            ebpv[1] = -0.417402664;
            ebpv[2] = -0.180982288;
            ebpv[3] = 0.00742727954;
            ebpv[4] = 0.0140507459;
            ebpv[5] = 0.00609045792;
            ehp[0] = 0.903358544;
            ehp[1] = -0.415395237;
            ehp[2] = -0.180084014;
            Sofa.Apci(date1, date2, ebpv, ehp, x, y, s, ref astrom);
            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.901310875, astrom.eb[0], 12);
            Assert.Equal(-0.417402664, astrom.eb[1], 12);
            Assert.Equal(-0.180982288, astrom.eb[2], 12);
            Assert.Equal(0.8940025429324143045, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268679817955, astrom.eh[1], 12);
            Assert.Equal(-0.1782189004872870264, astrom.eh[2], 12);
            Assert.Equal(1.010465295811013146, astrom.em, 12);
            Assert.Equal(0.4289638913597693554e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034051581320575e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555136380563427e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686012981, astrom.bm1, 12);
            Assert.Equal(0.9999991390295159156, astrom.bpn[0], 12);
            Assert.Equal(0.4978650072505016932e-7, astrom.bpn[3], 12);
            Assert.Equal(0.1312227200000000000e-2, astrom.bpn[6], 12);
            Assert.Equal(-0.1136336653771609630e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999995713154868, astrom.bpn[4], 12);
            Assert.Equal(-0.2928086230000000000e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.1312227200895260194e-2, astrom.bpn[2], 12);
            Assert.Equal(0.2928082217872315680e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999991386008323373, astrom.bpn[8], 12);
        }

        [Fact]
        public void Apci13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.9013108747340644755, astrom.eb[0], 12);
            Assert.Equal(-0.4174026640406119957, astrom.eb[1], 12);
            Assert.Equal(-0.1809822877867817771, astrom.eb[2], 12);
            Assert.Equal(0.8940025429255499549, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268331896318, astrom.eh[1], 12);
            Assert.Equal(-0.1782189006019749850, astrom.eh[2], 12);
            Assert.Equal(1.010465295964664178, astrom.em, 12);
            Assert.Equal(0.4289638912941341125e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034032405042132e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555135536470279e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686013142, astrom.bm1, 12);
            Assert.Equal(0.9999992060376761710, astrom.bpn[0], 12);
            Assert.Equal(0.4124244860106037157e-7, astrom.bpn[3], 12);
            Assert.Equal(0.1260128571051709670e-2, astrom.bpn[6], 12);
            Assert.Equal(-0.1282291987222130690e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999997456835325, astrom.bpn[4], 12);
            Assert.Equal(-0.2255288829420524935e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.1260128571661374559e-2, astrom.bpn[2], 12);
            Assert.Equal(0.2255285422953395494e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999992057833604343, astrom.bpn[8], 12);
            Assert.Equal(-0.2900618712657375647e-2, eo, 12);
        }

        [Fact]
        public void Apco()
        {
            double date1 = 2456384.5;
            double date2 = 0.970031644;
            double[] ebpv = new double[6];
            double[] ehp = new double[3];
            double x = 0.0013122272;
            double y = -2.92808623e-5;
            double s = 3.05749468e-8;
            double theta = 3.14540971;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double sp = -3.01974337e-11;
            double refa = 0.000201418779;
            double refb = -2.36140831e-7;
            var astrom = new Sofa.Astrom();
            ebpv[0] = -0.974170438;
            ebpv[1] = -0.211520082;
            ebpv[2] = -0.0917583024;
            ebpv[3] = 0.00364365824;
            ebpv[4] = -0.0154287319;
            ebpv[5] = -0.00668922024;
            ehp[0] = -0.973458265;
            ehp[1] = -0.209215307;
            ehp[2] = -0.0906996477;
            Sofa.Apco(date1, date2, ebpv, ehp, x, y, s, theta, elong, phi, hm, xp, yp, sp, refa, refb, ref astrom);
            Assert.Equal(13.25248468622587269, astrom.pmt, 11);
            Assert.Equal(-0.9741827110630322720, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190135344832, astrom.eb[1], 12);
            Assert.Equal(-0.09179840186949532298, astrom.eb[2], 12);
            Assert.Equal(-0.9736425571689739035, astrom.eh[0], 12);
            Assert.Equal(-0.2092452125849330936, astrom.eh[1], 12);
            Assert.Equal(-0.09075578152243272599, astrom.eh[2], 12);
            Assert.Equal(0.9998233241709957653, astrom.em, 12);
            Assert.Equal(0.2078704992916728762e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360107151952319e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338994288951082e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561236, astrom.bm1, 12);
            Assert.Equal(0.9999991390295159156, astrom.bpn[0], 12);
            Assert.Equal(0.4978650072505016932e-7, astrom.bpn[3], 12);
            Assert.Equal(0.1312227200000000000e-2, astrom.bpn[6], 12);
            Assert.Equal(-0.1136336653771609630e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999995713154868, astrom.bpn[4], 12);
            Assert.Equal(-0.2928086230000000000e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.1312227200895260194e-2, astrom.bpn[2], 12);
            Assert.Equal(0.2928082217872315680e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999991386008323373, astrom.bpn[8], 12);
            Assert.Equal(-0.5278008060295995734, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0, astrom.diurab, 0);
            Assert.Equal(2.617608903970400427, astrom.eral, 12);
            Assert.Equal(0.2014187790000000000e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408310000000000e-6, astrom.refb, 15);
        }

        [Fact]
        public void Apco13()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            int j = Sofa.Apco13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom, ref eo);
            Assert.Equal(13.25248468622475727, astrom.pmt, 11);
            Assert.Equal(-0.9741827107320875162, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190489716682, astrom.eb[1], 12);
            Assert.Equal(-0.09179840189496755339, astrom.eb[2], 12);
            Assert.Equal(-0.9736425572586935247, astrom.eh[0], 12);
            Assert.Equal(-0.2092452121603336166, astrom.eh[1], 12);
            Assert.Equal(-0.09075578153885665295, astrom.eh[2], 12);
            Assert.Equal(0.9998233240913898141, astrom.em, 12);
            Assert.Equal(0.2078704994520489246e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360133238868938e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338993055887398e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561004, astrom.bm1, 12);
            Assert.Equal(0.9999991390295147999, astrom.bpn[0], 12);
            Assert.Equal(0.4978650075315529277e-7, astrom.bpn[3], 12);
            Assert.Equal(0.001312227200850293372, astrom.bpn[6], 12);
            Assert.Equal(-0.1136336652812486604e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999995713154865, astrom.bpn[4], 12);
            Assert.Equal(-0.2928086230975367296e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.001312227201745553566, astrom.bpn[2], 12);
            Assert.Equal(0.2928082218847679162e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999991386008312212, astrom.bpn[8], 12);
            Assert.Equal(-0.003020548354802412839, eo, 14);
            Assert.Equal(0, j);
        }

        [Fact]
        public void Apcs()
        {
            double date1 = 2456384.5;
            double date2 = 0.970031644;
            double[] pv = new double[6];
            double[] ebpv = new double[6];
            double[] ehp = new double[3];
            var astrom = new Sofa.Astrom();
            pv[0] = -1836024.09;
            pv[1] = 1056607.72;
            pv[2] = -5998795.26;
            pv[3] = -77.0361767;
            pv[4] = -133.310856;
            pv[5] = 0.0971855934;
            ebpv[0] = -0.974170438;
            ebpv[1] = -0.211520082;
            ebpv[2] = -0.0917583024;
            ebpv[3] = 0.00364365824;
            ebpv[4] = -0.0154287319;
            ebpv[5] = -0.00668922024;
            ehp[0] = -0.973458265;
            ehp[1] = -0.209215307;
            ehp[2] = -0.0906996477;
            Sofa.Apcs(date1, date2, pv, ebpv, ehp, ref astrom);
            Assert.Equal(13.25248468622587269, astrom.pmt, 11);
            Assert.Equal(-0.9741827110629881886, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190136415986, astrom.eb[1], 12);
            Assert.Equal(-0.09179840186954412099, astrom.eb[2], 12);
            Assert.Equal(-0.9736425571689454706, astrom.eh[0], 12);
            Assert.Equal(-0.2092452125850435930, astrom.eh[1], 12);
            Assert.Equal(-0.09075578152248299218, astrom.eh[2], 12);
            Assert.Equal(0.9998233241709796859, astrom.em, 12);
            Assert.Equal(0.2078704993282685510e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360106989405683e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338994289409097e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561237, astrom.bm1, 12);
            Assert.Equal(1, astrom.bpn[0], 0);
            Assert.Equal(0, astrom.bpn[3], 0);
            Assert.Equal(0, astrom.bpn[6], 0);
            Assert.Equal(0, astrom.bpn[1], 0);
            Assert.Equal(1, astrom.bpn[4], 0);
            Assert.Equal(0, astrom.bpn[7], 0);
            Assert.Equal(0, astrom.bpn[2], 0);
            Assert.Equal(0, astrom.bpn[5], 0);
            Assert.Equal(1, astrom.bpn[8], 0);
        }

        [Fact]
        public void Apcs13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[] pv = new double[6];
            var astrom = new Sofa.Astrom();
            pv[0] = -6241497.16;
            pv[1] = 401346.896;
            pv[2] = -1251136.04;
            pv[3] = -29.264597;
            pv[4] = -455.021831;
            pv[5] = 0.0266151194;
            Sofa.Apcs13(date1, date2, pv, ref astrom);
            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.9012691529025250644, astrom.eb[0], 12);
            Assert.Equal(-0.4173999812023194317, astrom.eb[1], 12);
            Assert.Equal(-0.1809906511146429670, astrom.eb[2], 12);
            Assert.Equal(0.8939939101760130792, astrom.eh[0], 12);
            Assert.Equal(-0.4111053891734021478, astrom.eh[1], 12);
            Assert.Equal(-0.1782336880636997374, astrom.eh[2], 12);
            Assert.Equal(1.010428384373491095, astrom.em, 12);
            Assert.Equal(0.4279877294121697570e-4, astrom.v[0], 15);
            Assert.Equal(0.7963255087052120678e-4, astrom.v[1], 15);
            Assert.Equal(0.3517564013384691531e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999952947980978, astrom.bm1, 12);
            Assert.Equal(1, astrom.bpn[0], 0);
            Assert.Equal(0, astrom.bpn[3], 0);
            Assert.Equal(0, astrom.bpn[6], 0);
            Assert.Equal(0, astrom.bpn[1], 0);
            Assert.Equal(1, astrom.bpn[4], 0);
            Assert.Equal(0, astrom.bpn[7], 0);
            Assert.Equal(0, astrom.bpn[2], 0);
            Assert.Equal(0, astrom.bpn[5], 0);
            Assert.Equal(1, astrom.bpn[8], 0);
        }

        [Fact]
        public void Aper()
        {
            var astrom = new Sofa.Astrom();
            astrom.along = 1.234;
            double theta = 5.678;
            Sofa.Aper(theta, ref astrom);
            Assert.Equal(6.912000000000000000, astrom.eral, 12);
        }

        [Fact]
        public void Aper13()
        {
            var astrom = new Sofa.Astrom();
            astrom.along = 1.234;
            double ut11 = 2456165.5;
            double ut12 = 0.401182685;
            Sofa.Aper13(ut11, ut12, ref astrom);
            Assert.Equal(3.316236661789694933, astrom.eral, 12);
        }

        [Fact]
        public void Apio()
        {
            double sp = -3.01974337e-11;
            double theta = 3.14540971;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double refa = 0.000201418779;
            double refb = -2.36140831e-7;
            var astrom = new Sofa.Astrom();
            Sofa.Apio(sp, theta, elong, phi, hm, xp, yp, refa, refb, ref astrom);
            Assert.Equal(-0.5278008060295995734, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0.5135843661699913529e-6, astrom.diurab, 12);
            Assert.Equal(2.617608903970400427, astrom.eral, 12);
            Assert.Equal(0.2014187790000000000e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408310000000000e-6, astrom.refb, 15);
        }

        [Fact]
        public void Apio13()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            int j;
            var astrom = new Sofa.Astrom();
            j = Sofa.Apio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);


            Assert.Equal(-0.5278008060295995733, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0.5135843661699913529e-6, astrom.diurab, 12);
            Assert.Equal(2.617608909189664000, astrom.eral, 12);
            Assert.Equal(0.2014187785940396921e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408314943696227e-6, astrom.refb, 15);
            Assert.Equal(0, j);
        }

        [Fact]
        public void Atcc13()
        {
            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double ra = 0, da = 0;
            Sofa.Atcc13(rc, dc, pr, pd, px, rv, date1, date2, ref ra, ref da);
            Assert.Equal(2.710126504531372384, ra, 12);
            Assert.Equal(0.1740632537628350152, da, 12);

        }
        [Fact]
        public void Atccq()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            // get astrom from Apci13 as in C
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            double ra = 0, da = 0;
            Sofa.Atccq(rc, dc, pr, pd, px, rv, ref astrom, ref ra, ref da);
            Assert.Equal(2.710126504531372384, ra, 12);
            Assert.Equal(0.1740632537628350152, da, 12);
        }

        [Fact]
        public void Atci13()
        {
            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double ri = 0, di = 0, eo = 0;
            Sofa.Atci13(rc, dc, pr, pd, px, rv, date1, date2, ref ri, ref di, ref eo);
            Assert.Equal(2.710121572968696744, ri, 12);
            Assert.Equal(0.1729371367219539137, di, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }

        [Fact]
        public void Atciq()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double rc = 0, dc = 0;
            Sofa.Aticq(ri, di, ref astrom, ref rc, ref dc); // mapping name: Aticq/Atciq may vary; using Aticq as in C naming similarity
            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
        }

        [Fact]
        public void Atciqn()
        {
            try
            {
                double date1 = 2456165.5;
                double date2 = 0.401182685;
#if NET8_0_OR_GREATER
                var astrom = new Sofa.Astrom();
#else
                var astrom = Sofa.CreateAstrom();
#endif
                double eo = 0;
                Sofa.Apci13(date1, date2, ref astrom, ref eo);
                double ri = 2.709994899247599271;
                double di = 0.1728740720983623469;

                // prepare b array of LdBody
                var b = new LdBody[3];
#if NET8_0_OR_GREATER
                // .NET 8 and later automatically initialise structs
                for (int i = 0; i < 3; i++) b[i] = new Sofa.LdBody();
#else
                // .NET Framework does not initialise structs so we have to do it here!
                b[0] = Sofa.CreateLdBody();
                b[1] = Sofa.CreateLdBody();
                b[2] = Sofa.CreateLdBody();
#endif

                double rc = 2.71;
                double dc = 0.174;
                double pr = 1e-5;
                double pd = 5e-6;
                double px = 0.1;
                double rv = 55.0;
                b[0].bm = 0.00028574;
                b[0].dl = 3e-10;
                b[0].pv[0] = -7.81014427;
                b[0].pv[1] = -5.60956681;
                b[0].pv[2] = -1.98079819;
                b[0].pv[3] = 0.0030723249;
                b[0].pv[4] = -0.00406995477;
                b[0].pv[5] = -0.00181335842;
                b[1].bm = 0.00095435;
                b[1].dl = 3e-9;
                b[1].pv[0] = 0.738098796;
                b[1].pv[1] = 4.63658692;
                b[1].pv[2] = 1.9693136;
                b[1].pv[3] = -0.00755816922;
                b[1].pv[4] = 0.00126913722;
                b[1].pv[5] = 0.000727999001;
                b[2].bm = 1.0;
                b[2].dl = 6e-6;
                b[2].pv[0] = -0.000712174377;
                b[2].pv[1] = -0.00230478303;
                b[2].pv[2] = -0.00105865966;
                b[2].pv[3] = 6.29235213e-6;
                b[2].pv[4] = -3.30888387e-7;
                b[2].pv[5] = -2.96486623e-7;

                Sofa.Atciqn(rc, dc, pr, pd, px, rv, ref astrom, 3, b, ref ri, ref di);

                Assert.Equal(2.710122008104983335, ri, 12);
                Assert.Equal(0.1729371916492767821, di, 12);
            }
            catch (Exception ex)
            {
                logger.WriteLine($"Exception - {ex.Message}\r\n{ex}");
                Assert.Fail("Exception thrown");
            }
        }

        [Fact]
        public void Atciqz()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            double rc = 2.71;
            double dc = 0.174;
            double ri = 0.0;
            double di = 0.0;
            Sofa.Atciqz(rc, dc, ref astrom, ref ri, ref di);
            Assert.Equal(2.709994899247256984, ri, 12);
            Assert.Equal(0.1728740720984931891, di, 12);
        }

        [Fact]
        public void Atco13()
        {
            // Atco13 tests
            double rc = 2.71;
            double dc = 0.174;
            double pr = 0.00001;
            double pd = 0.000005;
            double px = 0.1;
            double rv = 55.0;
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 0.000000247230737;
            double yp = 0.00000182640464;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            double aob = 0;
            double zob = 0;
            double hob = 0;
            double dob = 0;
            double rob = 0;
            double eo = 0;

            short j = Sofa.Atco13(rc, dc, pr, pd, px, rv, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);

            Assert.Equal(0, j);
            Assert.Equal(0.9251774485485515207e-1, aob, 12);
            Assert.Equal(1.407661405256499357, zob, 12);
            Assert.Equal(-0.9265154431529724692e-1, hob, 12);
            Assert.Equal(0.1716626560072526200, dob, 12);
            Assert.Equal(2.710260453504961012, rob, 12);
            Assert.Equal(-0.003020548354802412839, eo, 14);
        }

        [Fact]
        public void Atic13()
        {
            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double rc = 0, dc = 0, eo = 0;
            Sofa.Atic13(ri, di, date1, date2, ref rc, ref dc, ref eo);
            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }

        [Fact]
        public void Aticq()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double rc = 0, dc = 0;
            Sofa.Aticq(ri, di, ref astrom, ref rc, ref dc);
            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
        }

        [Fact]
        public void Aticqn()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);
            double ri = 2.709994899247599271;
            double di = 0.1728740720983623469;
            var b = new Sofa.LdBody[3];
            for (int i = 0; i < 3; i++) b[i] = new Sofa.LdBody { pv = new double[6] };
            b[0].bm = 0.00028574;
            b[0].dl = 3e-10;
            b[0].pv[0] = -7.81014427;
            b[0].pv[1] = -5.60956681;
            b[0].pv[2] = -1.98079819;
            b[0].pv[3] = 0.0030723249;
            b[0].pv[4] = -0.00406995477;
            b[0].pv[5] = -0.00181335842;
            b[1].bm = 0.00095435;
            b[1].dl = 3e-9;
            b[1].pv[0] = 0.738098796;
            b[1].pv[1] = 4.63658692;
            b[1].pv[2] = 1.9693136;
            b[1].pv[3] = -0.00755816922;
            b[1].pv[4] = 0.00126913722;
            b[1].pv[5] = 0.000727999001;
            b[2].bm = 1.0;
            b[2].dl = 6e-6;
            b[2].pv[0] = -0.000712174377;
            b[2].pv[1] = -0.00230478303;
            b[2].pv[2] = -0.00105865966;
            b[2].pv[3] = 6.29235213e-6;
            b[2].pv[4] = -3.30888387e-7;
            b[2].pv[5] = -2.96486623e-7;
            double rc = 0, dc = 0;
            Sofa.Aticqn(ri, di, ref astrom, 3, b, ref rc, ref dc);
            Assert.Equal(2.709999575033027333, rc, 12);
            Assert.Equal(0.1739999656316469990, dc, 12);
        }

        [Fact]
        public void Atio13()
        {
            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            double aob = 0, zob = 0, hob = 0, dob = 0, rob = 0;

            int j = Sofa.Atio13(ri, di, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob);
            Assert.Equal(0, j); // sanity check: ensure call succeeded or returns known code
            Assert.Equal(0.9233952224895122499e-1, aob, 12);
            Assert.Equal(1.407758704513549991, zob, 12);
            Assert.Equal(-0.9247619879881698140e-1, hob, 12);
            Assert.Equal(0.1717653435756234676, dob, 12);
            Assert.Equal(2.710085107988480746, rob, 12);
        }

        [Fact]
        public void Atioq()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;

            var astrom = new Sofa.Astrom();
            Sofa.Apio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);

            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double aob = 0, zob = 0, hob = 0, dob = 0, rob = 0;

            Sofa.Atioq(ri, di, ref astrom, ref aob, ref zob, ref hob, ref dob, ref rob);

            Assert.Equal(0.9233952224895122499e-1, aob, 12);
            Assert.Equal(1.407758704513549991, zob, 12);
            Assert.Equal(-0.9247619879881698140e-1, hob, 12);
            Assert.Equal(0.1717653435756234676, dob, 12);
            Assert.Equal(2.710085107988480746, rob, 12);
        }

        [Fact]
        public void Atoc13()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            double ob1 = 2.710085107986886201;
            double ob2 = 0.1717653435758265198;
            double rc = 0, dc = 0;

            int j = Sofa.Atoc13("R", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
            Assert.Equal(0, j);
            Assert.Equal(2.709956744659136129, rc, 12);
            Assert.Equal(0.1741696500898471362, dc, 12);

            ob1 = -0.09247619879782006106;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoc13("H", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
            Assert.Equal(0, j);
            Assert.Equal(2.709956744659734086, rc, 12);
            Assert.Equal(0.1741696500898471362, dc, 12);

            ob1 = 0.09233952224794989993;
            ob2 = 1.407758704513722461;
            j = Sofa.Atoc13("A", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
            Assert.Equal(0, j);
            Assert.Equal(2.709956744659734086, rc, 12);
            Assert.Equal(0.1741696500898471366, dc, 12);
        }

        [Fact]
        public void Atoi13()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            double ob1 = 2.710085107986886201;
            double ob2 = 0.1717653435758265198;
            double ri = 0, di = 0;

            int j = Sofa.Atoi13("R", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574447540810, ri, 12);
            Assert.Equal(0.1729371839116608778, di, 12);

            ob1 = -0.09247619879782006106;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoi13("H", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574448138676, ri, 12);
            Assert.Equal(0.1729371839116608778, di, 12);

            ob1 = 0.09233952224794989993;
            ob2 = 1.407758704513722461;
            j = Sofa.Atoi13("A", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574448138676, ri, 12);
            Assert.Equal(0.1729371839116608781, di, 12);
        }

        [Fact]
        public void Atoiq()
        {
            Sofa.Astrom astrom = Sofa.CreateAstrom();
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;
            Sofa.Apio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);
            double ob1 = 2.710085107986886201;
            double ob2 = 0.1717653435758265198;
            double ri = 0;
            double di = 0;
            Sofa.Atoiq("R", ob1, ob2, ref astrom, ref ri, ref di);
            Assert.Equal(2.710121574447540810, ri, 1e-12);
            Assert.Equal(0.17293718391166087785, di, 1e-12);

            ob1 = -0.09247619879782006106;
            ob2 = 0.1717653435758265198;
            Sofa.Atoiq("H", ob1, ob2, ref astrom, ref ri, ref di);
            Assert.Equal(2.710121574448138676, ri, 1e-12);
            Assert.Equal(0.1729371839116608778, di, 1e-12);

            ob1 = 0.09233952224794989993;
            ob2 = 1.407758704513722461;
            Sofa.Atoiq("A", ob1, ob2, ref astrom, ref ri, ref di);
            Assert.Equal(2.710121574448138676, ri, 1e-12);
            Assert.Equal(0.1729371839116608781, di, 1e-12);
        }
        [Fact]
        public void Bi00()
        {
            double dpsibi = 0, depsbi = 0, dra = 0;

            // ToDo: Implement Sofa.Bi00 and remove the following line
            Sofa.Bi00(ref dpsibi, ref depsbi, ref dra);
            Assert.Equal(-0.2025309152835086613e-6, dpsibi, 12);
            Assert.Equal(-0.3306041454222147847e-7, depsbi, 12);
            Assert.Equal(-0.7078279744199225506e-7, dra, 12);
        }
        [Fact]
        public void Bp00()
        {
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];

            Sofa.Bp00(2400000.5, 50123.9999, rb, rp, rbp);

            Assert.Equal(0.9999999999999942498, rb[0], 12);
            Assert.Equal(-0.7078279744199196626e-7, rb[1], 14);
            Assert.Equal(0.8056217146976134152e-7, rb[2], 14);
            Assert.Equal(0.7078279477857337206e-7, rb[3], 14);
            Assert.Equal(0.9999999999999969484, rb[4], 12);
            Assert.Equal(0.3306041454222136517e-7, rb[5], 14);
            Assert.Equal(-0.8056217380986972157e-7, rb[6], 14);
            Assert.Equal(-0.3306040883980552500e-7, rb[7], 14);
            Assert.Equal(0.9999999999999962084, rb[8], 12);

            Assert.Equal(0.9999995504864048241, rp[0], 12);
            Assert.Equal(0.8696113836207084411e-3, rp[1], 14);
            Assert.Equal(0.3778928813389333402e-3, rp[2], 14);
            Assert.Equal(-0.8696113818227265968e-3, rp[3], 14);
            Assert.Equal(0.9999996218879365258, rp[4], 12);
            Assert.Equal(-0.1690679263009242066e-6, rp[5], 14);
            Assert.Equal(-0.3778928854764695214e-3, rp[6], 14);
            Assert.Equal(-0.1595521004195286491e-6, rp[7], 14);
            Assert.Equal(0.9999999285984682756, rp[8], 12);

            Assert.Equal(0.9999995505175087260, rbp[0], 12);
            Assert.Equal(0.8695405883617884705e-3, rbp[1], 14);
            Assert.Equal(0.3779734722239007105e-3, rbp[2], 14);
            Assert.Equal(-0.8695405990410863719e-3, rbp[3], 14);
            Assert.Equal(0.9999996219494925900, rbp[4], 12);
            Assert.Equal(-0.1360775820404982209e-6, rbp[5], 14);
            Assert.Equal(-0.3779734476558184991e-3, rbp[6], 14);
            Assert.Equal(-0.1925857585832024058e-6, rbp[7], 14);
            Assert.Equal(0.9999999285680153377, rbp[8], 12);
        }
        [Fact]
        public void Bp06()
        {
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            Sofa.Bp06(2400000.5, 50123.9999, rb, rp, rbp);
            Assert.Equal(0.9999999999999942497, rb[0], 12);
            Assert.Equal(-0.7078368960971557145e-7, rb[1], 14);
            Assert.Equal(0.8056213977613185606e-7, rb[2], 14);
            Assert.Equal(0.7078368694637674333e-7, rb[3], 14);
            Assert.Equal(0.9999999999999969484, rb[4], 12);
            Assert.Equal(0.3305943742989134124e-7, rb[5], 14);
            Assert.Equal(-0.8056214211620056792e-7, rb[6], 14);
            Assert.Equal(-0.3305943172740586950e-7, rb[7], 14);
            Assert.Equal(0.9999999999999962084, rb[8], 12);

            Assert.Equal(0.9999995504864960278, rp[0], 12);
            Assert.Equal(0.8696112578855404832e-3, rp[1], 14);
            Assert.Equal(0.3778929293341390127e-3, rp[2], 14);
            Assert.Equal(-0.8696112560510186244e-3, rp[3], 14);
            Assert.Equal(0.9999996218880458820, rp[4], 12);
            Assert.Equal(-0.1691646168941896285e-6, rp[5], 14);
            Assert.Equal(-0.3778929335557603418e-3, rp[6], 14);
            Assert.Equal(-0.1594554040786495076e-6, rp[7], 14);
            Assert.Equal(0.9999999285984501222, rp[8], 12);

            Assert.Equal(0.9999995505176007047, rbp[0], 12);
            Assert.Equal(0.8695404617348208406e-3, rbp[1], 14);
            Assert.Equal(0.3779735201865589104e-3, rbp[2], 14);
            Assert.Equal(-0.8695404723772031414e-3, rbp[3], 14);
            Assert.Equal(0.9999996219496027161, rbp[4], 12);
            Assert.Equal(-0.1361752497080270143e-6, rbp[5], 14);
            Assert.Equal(-0.3779734957034089490e-3, rbp[6], 14);
            Assert.Equal(-0.1924880847894457113e-6, rbp[7], 14);
            Assert.Equal(0.9999999285679971958, rbp[8], 12);
        }
        [Fact]
        public void Bpn2xy()
        {
            double[] rbpn = new double[9];
            rbpn[0] = 9.999962358680738e-1;
            rbpn[1] = -2.516417057665452e-3;
            rbpn[2] = -1.093569785342370e-3;
            rbpn[3] = 2.516462370370876e-3;
            rbpn[4] = 9.999968329010883e-1;
            rbpn[5] = 4.006159587358310e-5;
            rbpn[6] = 1.093465510215479e-3;
            rbpn[7] = -4.281337229063151e-5;
            rbpn[8] = 9.999994012499173e-1;
            double x = 0, y = 0;

            Sofa.Bpn2xy(rbpn, ref x, ref y);
            Assert.Equal(1.093465510215479e-3, x, 12);
            Assert.Equal(-4.281337229063151e-5, y, 12);
        }

        [Fact]
        public void C2i00a()
        {
            double[] rc2i = new double[9];
            Sofa.C2i00a(2400000.5, 53736.0, rc2i);
            Assert.Equal(0.9999998323037165557, rc2i[0], 12);
            Assert.Equal(0.5581526348992140183e-9, rc2i[1], 12);
            Assert.Equal(-0.5791308477073443415e-3, rc2i[2], 12);
            Assert.Equal(-0.2384266227870752452e-7, rc2i[3], 12);
            Assert.Equal(0.9999999991917405258, rc2i[4], 12);
            Assert.Equal(-0.4020594955028209745e-4, rc2i[5], 12);
            Assert.Equal(0.5791308472168152904e-3, rc2i[6], 12);
            Assert.Equal(0.4020595661591500259e-4, rc2i[7], 12);
            Assert.Equal(0.9999998314954572304, rc2i[8], 12);
        }

        [Fact]
        public void C2i00b()
        {
            double[] rc2i = new double[9];

            Sofa.C2i00b(2400000.5, 53736.0, rc2i);
            Assert.Equal(0.9999998323040954356, rc2i[0], 12);
            Assert.Equal(0.5581526349131823372e-9, rc2i[1], 12);
            Assert.Equal(-0.5791301934855394005e-3, rc2i[2], 12);
            Assert.Equal(-0.2384239285499175543e-7, rc2i[3], 12);
            Assert.Equal(0.9999999991917574043, rc2i[4], 12);
            Assert.Equal(-0.4020552974819030066e-4, rc2i[5], 12);
            Assert.Equal(0.5791301929950208873e-3, rc2i[6], 12);
            Assert.Equal(0.4020553681373720832e-4, rc2i[7], 12);
            Assert.Equal(0.9999998314958529887, rc2i[8], 12);
        }

        [Fact]
        public void C2i06a()
        {
            double[] rc2i = new double[9];
            Sofa.C2i06a(2400000.5, 53736.0, rc2i);

            Assert.Equal(0.9999998323037159379, rc2i[0], 12);
            Assert.Equal(0.5581121329587613787e-9, rc2i[1], 12);
            Assert.Equal(-0.5791308487740529749e-3, rc2i[2], 12);
            Assert.Equal(-0.2384253169452306581e-7, rc2i[3], 12);
            Assert.Equal(0.9999999991917467827, rc2i[4], 12);
            Assert.Equal(-0.4020579392895682558e-4, rc2i[5], 12);
            Assert.Equal(0.5791308482835292617e-3, rc2i[6], 12);
            Assert.Equal(0.4020580099454020310e-4, rc2i[7], 12);
            Assert.Equal(0.9999998314954628695, rc2i[8], 12);
        }

        [Fact]
        public void C2ibpn()
        {
            const double TOLERANCE = 1e-12;

            // Arrange
            double[] rbpn = new double[9]
                {
                9.999962358680738e-1,
                -2.516417057665452e-3,
                -1.093569785342370e-3,
                2.516462370370876e-3,
                9.999968329010883e-1,
                4.006159587358310e-5,
                1.093465510215479e-3,
                -4.281337229063151e-5,
                9.999994012499173e-1
                };
            double date1 = 2400000.5;
            double date2 = 50123.9999;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ibpn(date1, date2, rbpn, rc2i);

            // Assert
            Assert.Equal(0.9999994021664089977, rc2i[0], TOLERANCE);
            Assert.Equal(-0.3869195948017503664e-8, rc2i[1], TOLERANCE);
            Assert.Equal(-0.1093465511383285076e-2, rc2i[2], TOLERANCE);
            Assert.Equal(0.5068413965715446111e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999990835075686, rc2i[4], TOLERANCE);
            Assert.Equal(0.4281334246452708915e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.1093465510215479000e-2, rc2i[6], TOLERANCE);
            Assert.Equal(-0.4281337229063151000e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999994012499173103, rc2i[8], TOLERANCE);
        }

        [Fact]
        public void C2ixy()
        {
            const double TOLERANCE = 1e-12;

            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double date1 = 2400000.5;
            double date2 = 53736;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ixy(date1, date2, x, y, rc2i);

            // Assert
            Assert.Equal(0.9999998323037157138, rc2i[0], TOLERANCE);
            Assert.Equal(0.5581526349032241205e-9, rc2i[1], TOLERANCE);
            Assert.Equal(-0.5791308491611263745e-3, rc2i[2], TOLERANCE);
            Assert.Equal(-0.2384257057469842953e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999991917468964, rc2i[4], TOLERANCE);
            Assert.Equal(-0.4020579110172324363e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.5791308486706011000e-3, rc2i[6], TOLERANCE);
            Assert.Equal(0.4020579816732961219e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999998314954627590, rc2i[8], TOLERANCE);
        }


        [Fact]
        public void C2ixys()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double s = -0.1220040848472271978e-7;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ixys(x, y, s, rc2i);

            // Assert
            Assert.Equal(0.9999998323037157138, rc2i[0], TOLERANCE);
            Assert.Equal(0.5581984869168499149e-9, rc2i[1], TOLERANCE);
            Assert.Equal(-0.5791308491611282180e-3, rc2i[2], TOLERANCE);
            Assert.Equal(-0.2384261642670440317e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999991917468964, rc2i[4], TOLERANCE);
            Assert.Equal(-0.4020579110169668931e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.5791308486706011000e-3, rc2i[6], TOLERANCE);
            Assert.Equal(0.4020579816732961219e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999998314954627590, rc2i[8], TOLERANCE);
        }


        [Fact]
        public void C2s()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double[] p = new double[3] { 100.0, -50.0, 25.0 };

            // Act
            double theta = 0;
            double phi = 0;
            Sofa.C2s(p, ref theta, ref phi);

            // Assert
            Assert.Equal(-0.4636476090008061162, theta, TOLERANCE);
            Assert.Equal(0.2199879773954594463, phi, TOLERANCE);
        }


        [Fact]
        public void C2t00a()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t00a(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128307182668, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938457836, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535638688341725e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134135984552, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649520727, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116141056317e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081406921e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391770163647e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501692289, rc2t[8], TOLERANCE);
        }


        [Fact]
        public void C2t00b()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t00b(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128439678965, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806913872359, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555565082458415611e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134115435923, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203784001946, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749793922030017230e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773467471863534901e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961790411549945020e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325505635738, rc2t[8], TOLERANCE);
        }


        [Fact]
        public void C2t06a()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t06a(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128305897282, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938592296, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555550962998436505e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134136214897, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649130832, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749800844905594110e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474024748545878e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961816829632690581e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501747785, rc2t[8], TOLERANCE);
        }


        [Fact]
        public void C2tcio()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] rc2i = new double[9]
            {
                0.9999998323037164738,
                0.5581526271714303683e-9,
                -0.5791308477073443903e-3,
                -0.2384266227524722273e-7,
                0.9999999991917404296,
                -0.4020594955030704125e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };
            double era = 1.75283325530307;
            double[] rpom = new double[9]
            {
                0.9999999999999674705,
                -0.1367174580728847031e-10,
                0.2550602379999972723e-6,
                0.1414624947957029721e-10,
                0.9999999999982694954,
                -0.1860359246998866338e-5,
                -0.2550602379741215275e-6,
                0.1860359247002413923e-5,
                0.9999999999982369658
            };

            // Act
            double[] rc2t = new double[9];
            Sofa.C2tcio(rc2i, era, rpom, rc2t);

            // Assert
            Assert.Equal(-0.1810332128307110439, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938470149, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535638685466874e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134135996657, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649448367, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116141106528e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081407076e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391772658944e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501691969, rc2t[8], TOLERANCE);
        }


        [Fact]
        public void C2teqx()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] rbpn = new double[9]
            {
                0.9999989440476103608,
                -0.1332881761240011518e-2,
                -0.5790767434730085097e-3,
                0.1332858254308954453e-2,
                0.9999991109044505944,
                -0.4097782710401555759e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };
            double gst = 1.754166138040730516;
            double[] rpom = new double[9]
            {
                0.9999999999999674705,
                -0.1367174580728847031e-10,
                0.2550602379999972723e-6,
                0.1414624947957029721e-10,
                0.9999999999982694954,
                -0.1860359246998866338e-5,
                -0.2550602379741215275e-6,
                0.1860359247002413923e-5,
                0.9999999999982369658
            };

            // Act
            double[] rc2t = new double[9];
            Sofa.C2teqx(rbpn, gst, rpom, rc2t);

            // Assert
            Assert.Equal(-0.1810332128528685730, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806897685071, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535639982634449e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134095211257, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203871023800, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116126438962e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081539467e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391768640871e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501691969, rc2t[8], TOLERANCE);
        }

        [Fact]
        public void C2tpe()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double dpsi = -0.9630909107115582393e-5;
            double deps = 0.4090789763356509900;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2tpe(tta, ttb, uta, utb, dpsi, deps, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1813677995763029394, rc2t[0], TOLERANCE);
            Assert.Equal(0.9023482206891683275, rc2t[1], TOLERANCE);
            Assert.Equal(-0.3909902938641085751, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834147641476804807, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1659883635434995121, rc2t[4], TOLERANCE);
            Assert.Equal(0.7309763898042819705e-1, rc2t[5], TOLERANCE);
            Assert.Equal(0.1059685430673215247e-2, rc2t[6], TOLERANCE);
            Assert.Equal(0.3977631855605078674, rc2t[7], TOLERANCE);
            Assert.Equal(0.9174875068792735362, rc2t[8], TOLERANCE);
        }

        [Fact]
        public void C2txy()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2txy(tta, ttb, uta, utb, x, y, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128306279253, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938520084, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555551248057665829e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134136142314, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649529312, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749800843594139912e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474028619264494e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961816546911624260e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501746670, rc2t[8], TOLERANCE);
        }

        [Fact]
        public void Cal2jd()
        {
            // Arrange
            int iy = 2003;
            int im = 6;
            int id = 1;

            // Act
            double djm0 = 0;
            double djm = 0;
            int j = Sofa.Cal2jd(iy, im, id, ref djm0, ref djm);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(2400000.5, djm0);
            Assert.Equal(52791.0, djm);
        }

        [Fact]
        public void Cp()
        {
            // Arrange
            double[] p = new double[3] { 0.3, 1.2, -2.5 };

            // Act
            double[] c = new double[3];
            Sofa.Cp(p, c);

            // Assert
            Assert.Equal(0.3, c[0]);
            Assert.Equal(1.2, c[1]);
            Assert.Equal(-2.5, c[2]);
        }

        [Fact]
        public void Cpv()
        {
            // Arrange
            double[] pv = new double[6] { 0.3, 1.2, -2.5, -0.5, 3.1, 0.9 };

            // Act
            double[] c = new double[6];
            Sofa.Cpv(pv, c);

            // Assert
            Assert.Equal(0.3, c[0]);
            Assert.Equal(1.2, c[1]);
            Assert.Equal(-2.5, c[2]);
            Assert.Equal(-0.5, c[3]);
            Assert.Equal(3.1, c[4]);
            Assert.Equal(0.9, c[5]);
        }

        [Fact]
        public void Cr()
        {
            // Arrange
            double[] r = new double[9]
            {
                2.0, 3.0, 2.0,
                3.0, 2.0, 3.0,
                3.0, 4.0, 5.0
            };

            // Act
            double[] c = new double[9];
            Sofa.Cr(r, c);

            // Assert
            for (int i = 0; i < 9; i++)
            {
                Assert.Equal(r[i], c[i]);
            }
        }

        [Fact]
        public void D2dtf()
        {
            // Arrange
            string scale = "UTC";
            int ndp = 5;
            double d1 = 2400000.5;
            double d2 = 49533.99999;

            // Act
            int iy = 0;
            int im = 0;
            int id = 0;
            int[] ihmsf = new int[4];
            int j = Sofa.D2dtf(scale, ndp, d1, d2, ref iy, ref im, ref id, ihmsf);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1994, iy);
            Assert.Equal(6, im);
            Assert.Equal(30, id);
            Assert.Equal(23, ihmsf[0]);
            Assert.Equal(59, ihmsf[1]);
            Assert.Equal(60, ihmsf[2]);
            Assert.Equal(13599, ihmsf[3]);
        }

        [Fact]
        public void D2tf()
        {
            // Arrange
            int ndp = 4;
            double days = -0.987654321;

            // Act
            byte sign = (byte)'X';
            int[] ihmsf = new int[4];
            Sofa.D2tf(ndp, days, out sign, ihmsf);
            char signChar = (char)sign;
            // Assert
            Assert.Equal('-', signChar);
            Assert.Equal(23, ihmsf[0]);
            Assert.Equal(42, ihmsf[1]);
            Assert.Equal(13, ihmsf[2]);
            Assert.Equal(3333, ihmsf[3]);
        }

        [Fact]
        public void Dat()
        {
            // Arrange
            int year = 2017;
            int month = 9;
            int day = 1;
            double frac = 0.0;

            // Act
            double leapSeconds = 0.0;
            short j = Sofa.Dat(year, month, day, frac, ref leapSeconds);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(37.0, leapSeconds, 6);
        }

        [Fact]
        public void Dat_2()
        {
            // Dat tests

            DateTime testDate = sofaDatTestDate;
            double leapSeconds = 0.0;
            short j = Sofa.Dat(testDate.Year, testDate.Month, testDate.Day, testDate.TimeOfDay.TotalHours / 24.0, ref leapSeconds);

            Assert.Equal(0, j); // Return code is 0 when called for dates that are less than 5 years after the SOFA release year
            Assert.Equal(37.0, leapSeconds, 6);
        }


        [Fact]
        public void Dat_3()
        {
            // Arrange
            int year = 2008;
            int month = 1;
            int day = 17;
            double frac = 0.0;

            // Act
            double deltat = 0;
            short j = Sofa.Dat(year, month, day, frac, ref deltat);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(33.0, deltat);
        }

        [Fact]
        public void DatPlusFiveYears()
        {
            // Dat tests

            DateTime testDate = sofaDatTestDate.AddYears(5);
            double leapSeconds = 0.0;
            short j = Sofa.Dat(testDate.Year, testDate.Month, testDate.Day, testDate.TimeOfDay.TotalHours / 24.0, ref leapSeconds);

            Assert.Equal(1, j); // The return code is 1 when called for dates that are 5 years or more after the SOFA release year
            Assert.Equal(37.0, leapSeconds, 6);
        }

        [Fact]
        public void Dtdb()
        {
            const double TOLERANCE = 1e-15;
            // Arrange
            double date1 = 2448939.5;
            double date2 = 0.123;
            double ut = 0.76543;
            double elong = 5.0123;
            double u = 5525.242;
            double v = 3190.0;

            // Act
            double dtdb = Sofa.Dtdb(date1, date2, ut, elong, u, v);

            // Assert
            Assert.Equal(-0.1280368005936998991e-2, dtdb, TOLERANCE);
        }

        [Fact]
        public void Dtf2d()
        {
            double u1 = 0;
            double u2 = 0;
            short j = Sofa.Dtf2d("UTC", 1994, 6, 30, 23, 59, 60.13599, ref u1, ref u2);
            Assert.Equal(0, j);
            Assert.Equal(2449534.49999, u1 + u2, 6);
        }

        [Fact]
        public void Eceq06()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double dl = 5.1;
            double db = -0.9;

            // Act
            double dr = 0;
            double dd = 0;
            Sofa.Eceq06(date1, date2, dl, db, ref dr, ref dd);

            // Assert
            Assert.Equal(5.533459733613627767, dr, TOLERANCE);
            Assert.Equal(-1.246542932554480576, dd, TOLERANCE);
        }

        [Fact]
        public void Ecm06()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double date1 = 2456165.5;
            double date2 = 0.401182685;

            // Act
            double[] rm = new double[9];
            Sofa.Ecm06(date1, date2, rm);

            // Assert
            Assert.Equal(0.9999952427708701137, rm[0], TOLERANCE);
            Assert.Equal(-0.2829062057663042347e-2, rm[1], TOLERANCE);
            Assert.Equal(-0.1229163741100017629e-2, rm[2], TOLERANCE);
            Assert.Equal(0.3084546876908653562e-2, rm[3], TOLERANCE);
            Assert.Equal(0.9174891871550392514, rm[4], TOLERANCE);
            Assert.Equal(0.3977487611849338124, rm[5], TOLERANCE);
            Assert.Equal(0.2488512951527405928e-5, rm[6], TOLERANCE);
            Assert.Equal(-0.3977506604161195467, rm[7], TOLERANCE);
            Assert.Equal(0.9174935488232863071, rm[8], TOLERANCE);
        }

        [Fact]
        public void Ee00()
        {
            const double TOLERANCE = 1e-18;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;
            double epsa = 0.4090789763356509900;
            double dpsi = -0.9630909107115582393e-5;

            // Act
            double ee = Sofa.Ee00(date1, date2, epsa, dpsi);

            // Assert
            Assert.Equal(-0.8834193235367965479e-5, ee, TOLERANCE);
        }

        [Fact]
        public void Ee00a()
        {
            const double TOLERANCE = 1e-18;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee00a(date1, date2);

            // Assert
            Assert.Equal(-0.8834192459222588227e-5, ee, TOLERANCE);
        }

        [Fact]
        public void Ee00b()
        {
            const double TOLERANCE = 1e-18;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee00b(date1, date2);

            // Assert
            Assert.Equal(-0.8835700060003032831e-5, ee, TOLERANCE);
        }

        [Fact]
        public void Ee06a()
        {
            const double TOLERANCE = 1e-15;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee06a(date1, date2);

            // Assert
            Assert.Equal(-0.8834195072043790156e-5, ee, TOLERANCE);
        }

        [Fact]
        public void Eect00()
        {
            const double TOLERANCE = 1e-20;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double eect = Sofa.Eect00(date1, date2);

            // Assert
            Assert.Equal(0.2046085004885125264e-8, eect, TOLERANCE);
        }

        [Fact]
        public void Eform_Bad1()
        {
            // Arrange
            SofaReferenceEllipsoids id = 0;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(-1, j);
        }

        [Fact]
        public void Eform_Bad2()
        {
            // Arrange
            int id = 4;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform((SofaReferenceEllipsoids)id, ref a, ref f);

            // Assert
            Assert.Equal(-1, j);
        }

        [Fact]
        public void Eform_GRS80()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.GRS80;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378137.0, a, 1e-10);
            Assert.Equal(0.3352810681182318935e-2, f, 1e-18);
        }

        [Fact]
        public void Eform_WGS72()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.WGS72;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378135.0, a, 1e-10);
            Assert.Equal(0.3352779454167504862e-2, f, 1e-18);
        }

        [Fact]
        public void Eform_WGS84()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.WGS84;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378137.0, a, 1e-10);
            Assert.Equal(0.3352810664747480720e-2, f, 1e-18);
        }

        [Fact]
        public void Eo06a()
        {
            double eo;
            // Eo06a tests
            eo = Sofa.Eo06a(2400000.5, 53736.0);
            Assert.Equal(-0.1332882371941833644e-2, eo, 15);
        }

        [Fact]
        public void Eors()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double[] rnpb = new double[9]
            {
                0.9999989440476103608,
                -0.1332881761240011518e-2,
                -0.5790767434730085097e-3,
                0.1332858254308954453e-2,
                0.9999991109044505944,
                -0.4097782710401555759e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };
            double s = -0.1220040848472271978e-7;

            // Act
            double eo = Sofa.Eors(rnpb, s);

            // Assert
            Assert.Equal(-0.1332882715130744606e-2, eo, TOLERANCE);
        }

        [Fact]
        public void Epb()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double dj1 = 2415019.8135;
            double dj2 = 30103.18648;

            // Act
            double epb = Sofa.Epb(dj1, dj2);

            // Assert
            Assert.Equal(1982.418424159278580, epb, TOLERANCE);
        }

        [Fact]
        public void Epb2jd()
        {
            const double TOLERANCE = 1e-9;
            // Arrange
            double epb = 1957.3;

            // Act
            double djm0 = 0;
            double djm = 0;
            Sofa.Epb2jd(epb, ref djm0, ref djm);

            // Assert
            Assert.Equal(2400000.5, djm0, TOLERANCE);
            Assert.Equal(35948.1915101513, djm, TOLERANCE);
        }

        [Fact]
        public void Epj()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double dj1 = 2451545;
            double dj2 = -7392.5;

            // Act
            double epj = Sofa.Epj(dj1, dj2);

            // Assert
            Assert.Equal(1979.760438056125941, epj, TOLERANCE);
        }

        [Fact]
        public void Epj2jd()
        {
            const double TOLERANCE = 1e-9;
            // Arrange
            double epj = 1996.8;

            // Act
            double djm0 = 0;
            double djm = 0;
            Sofa.Epj2jd(epj, ref djm0, ref djm);

            // Assert
            Assert.Equal(2400000.5, djm0, TOLERANCE);
            Assert.Equal(50375.7, djm, TOLERANCE);
        }

        [Fact]
        public void Epv00()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53411.52501161;

            // Act
            double[] pvh = new double[6];
            double[] pvb = new double[6];
            int j = Sofa.Epv00(date1, date2, pvh, pvb);

            // Assert
            Assert.Equal(0, j);

            // Heliocentric
            Assert.Equal(-0.7757238809297706813, pvh[0], TOLERANCE);
            Assert.Equal(0.5598052241363340596, pvh[1], TOLERANCE);
            Assert.Equal(0.2426998466481686993, pvh[2], TOLERANCE);
            Assert.Equal(-0.1091891824147313846e-1, pvh[3], 1e-15);
            Assert.Equal(-0.1247187268440845008e-1, pvh[4], 1e-15);
            Assert.Equal(-0.5407569418065039061e-2, pvh[5], 1e-15);

            // Barycentric
            Assert.Equal(-0.7714104440491111971, pvb[0], TOLERANCE);
            Assert.Equal(0.5598412061824171323, pvb[1], TOLERANCE);
            Assert.Equal(0.2425996277722452400, pvb[2], TOLERANCE);
            Assert.Equal(-0.1091874268116823295e-1, pvb[3], 1e-15);
            Assert.Equal(-0.1246525461732861538e-1, pvb[4], 1e-15);
            Assert.Equal(-0.5404773180966231279e-2, pvb[5], 1e-15);
        }

        [Fact]
        public void Eqec06()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double date1 = 1234.5;
            double date2 = 2440000.5;
            double dr = 1.234;
            double dd = 0.987;

            // Act
            double dl = 0;
            double db = 0;
            Sofa.Eqec06(date1, date2, dr, dd, ref dl, ref db);

            // Assert
            Assert.Equal(1.342509918994654619, dl, TOLERANCE);
            Assert.Equal(0.5926215259704608132, db, TOLERANCE);
        }

        [Fact]
        public void Eqeq94()
        {
            const double TOLERANCE = 1e-17;
            // Arrange
            double date1 = 2400000.5;
            double date2 = 41234.0;

            // Act
            double eqeq = Sofa.Eqeq94(date1, date2);

            // Assert
            Assert.Equal(0.5357758254609256894e-4, eqeq, TOLERANCE);
        }

        [Fact]
        public void Era00()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 54388.0;

            // Act
            double era = Sofa.Era00(dj1, dj2);

            // Assert
            Assert.Equal(0.4022837240028158102, era, TOLERANCE);
        }

        [Fact]
        public void Fad03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fad03(0.80);

            // Assert
            Assert.Equal(1.946709205396925672, result, TOLERANCE);
        }

        [Fact]
        public void Fae03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fae03(0.80);

            // Assert
            Assert.Equal(1.744713738913081846, result, TOLERANCE);
        }

        [Fact]
        public void Faf03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Faf03(0.80);

            // Assert
            Assert.Equal(0.2597711366745499518, result, TOLERANCE);
        }

        [Fact]
        public void Faju03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Faju03(0.80);

            // Assert
            Assert.Equal(5.275711665202481138, result, TOLERANCE);
        }

        [Fact]
        public void Fal03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fal03(0.80);

            // Assert
            Assert.Equal(5.132369751108684150, result, TOLERANCE);
        }

        [Fact]
        public void Falp03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Falp03(0.80);

            // Assert
            Assert.Equal(6.226797973505507345, result, TOLERANCE);
        }

        [Fact]
        public void Fama03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fama03(0.80);

            // Assert
            Assert.Equal(3.275506840277781492, result, TOLERANCE);
        }

        [Fact]
        public void Fame03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fame03(0.80);

            // Assert
            Assert.Equal(5.417338184297289661, result, TOLERANCE);
        }

        [Fact]
        public void Fane03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fane03(0.80);

            // Assert
            Assert.Equal(2.079343830860413523, result, TOLERANCE);
        }

        [Fact]
        public void Faom03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Faom03(0.80);

            // Assert
            Assert.Equal(-5.973618440951302183, result, TOLERANCE);
        }

        [Fact]
        public void Fapa03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fapa03(0.80);

            // Assert
            Assert.Equal(0.1950884762240000000e-1, result, TOLERANCE);
        }

        [Fact]
        public void Fasa03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fasa03(0.80);

            // Assert
            Assert.Equal(5.371574539440827046, result, TOLERANCE);
        }

        [Fact]
        public void Faur03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Faur03(0.80);

            // Assert
            Assert.Equal(5.180636450180413523, result, TOLERANCE);
        }

        [Fact]
        public void Fave03()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double result = Sofa.Fave03(0.80);

            // Assert
            Assert.Equal(3.424900460533758000, result, TOLERANCE);
        }
        [Fact]
        public void Fk425()
        {
            // Arrange
            const double TOLERANCE = 1e-14;
            double r1950 = 0.07626899753879587532;
            double d1950 = -1.137405378399605780;
            double dr1950 = 0.1973749217849087460e-4;
            double dd1950 = 0.5659714913272723189e-5;
            double p1950 = 0.134;
            double v1950 = 8.7;
            // Act
            double r2000 = 0, d2000 = 0, dr2000 = 0, dd2000 = 0, p2000 = 0, v2000 = 0;
            Sofa.Fk425(r1950, d1950, dr1950, dd1950, p1950, v1950,
                ref r2000, ref d2000, ref dr2000, ref dd2000, ref p2000, ref v2000);
            // Assert
            Assert.Equal(0.08757989933556446040, r2000, TOLERANCE);
            Assert.Equal(-1.132279113042091895, d2000, 1e-12);
            Assert.Equal(0.1953670614474396139e-4, dr2000, 1e-17);
            Assert.Equal(0.5637686678659640164e-5, dd2000, 1e-18);
            Assert.Equal(0.1339919950582767871, p2000, 1e-13);
            Assert.Equal(8.736999669183529069, v2000, 1e-12);
        }

        [Fact]
        public void Fk45z()
        {
            // Arrange
            const double TOLERANCE = 1e-15;
            double r1950 = 0.01602284975382960982;
            double d1950 = -0.1164347929099906024;
            double bepoch = 1954.677617625256806;
            // Act
            double r2000 = 0, d2000 = 0;
            Sofa.Fk45z(r1950, d1950, bepoch, ref r2000, ref d2000);
            // Assert
            Assert.Equal(0.02719295911606862303, r2000, TOLERANCE);
            Assert.Equal(-0.1115766001565926892, d2000, 1e-13);
        }

        [Fact]
        public void Fk524()
        {
            // Arrange
            const double TOLERANCE = 1e-13;
            double r2000 = 0.8723503576487275595;
            double d2000 = -0.7517076365138887672;
            double dr2000 = 0.2019447755430472323e-4;
            double dd2000 = 0.3541563940505160433e-5;
            double p2000 = 0.1559;
            double v2000 = 86.87;
            // Act
            double r1950 = 0, d1950 = 0, dr1950 = 0, dd1950 = 0, p1950 = 0, v1950 = 0;
            Sofa.Fk524(r2000, d2000, dr2000, dd2000, p2000, v2000,
                ref r1950, ref d1950, ref dr1950, ref dd1950, ref p1950, ref v1950);
            // Assert
            Assert.Equal(0.8636359659799603487, r1950, TOLERANCE);
            Assert.Equal(-0.7550281733160843059, d1950, TOLERANCE);
            Assert.Equal(0.2023628192747172486e-4, dr1950, 1e-17);
            Assert.Equal(0.3624459754935334718e-5, dd1950, 1e-18);
            Assert.Equal(0.1560079963299390241, p1950, 1e-13);
            Assert.Equal(86.79606353469163751, v1950, 1e-11);
        }

        [Fact]
        public void Fk52h()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double r5 = 1.76779433;
            double d5 = -0.2917517103;
            double dr5 = -1.91851572e-7;
            double dd5 = -5.8468475e-6;
            double px5 = 0.379210;
            double rv5 = -7.6;
            // Act
            double rh = 0, dh = 0, drh = 0, ddh = 0, pxh = 0, rvh = 0;
            Sofa.Fk52h(r5, d5, dr5, dd5, px5, rv5, ref rh, ref dh, ref drh, ref ddh, ref pxh, ref rvh);
            // Assert
            Assert.Equal(1.767794226299947632, rh, TOLERANCE);
            Assert.Equal(-0.2917516070530391757, dh, TOLERANCE);
            Assert.Equal(-0.1961874125605721270e-6, drh, 1e-19);
            Assert.Equal(-0.58459905176693911e-5, ddh, 1e-19);
            Assert.Equal(0.37921, pxh, TOLERANCE);
            Assert.Equal(-7.6000000940000254, rvh, 1e-11);
        }

        [Fact]
        public void Fk54z()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double r2000 = 0.02719026625066316119;
            double d2000 = -0.1115815170738754813;
            double bepoch = 1954.677308160316374;
            // Act
            double r1950 = 0, d1950 = 0, dr1950 = 0, dd1950 = 0;
            Sofa.Fk54z(r2000, d2000, bepoch, ref r1950, ref d1950, ref dr1950, ref dd1950);
            // Assert
            Assert.Equal(0.01602015588390065476, r1950, TOLERANCE);
            Assert.Equal(-0.1164397101110765346, d1950, 1e-13);
            Assert.Equal(-0.1175712648471090704e-7, dr1950, 1e-20);
            Assert.Equal(0.2108109051316431056e-7, dd1950, 1e-20);
        }

        [Fact]
        public void Fk5hip()
        {
            const double TOLERANCE = 1e-14;
            // Act
            double[] r5h = new double[9];
            double[] s5h = new double[3];
            Sofa.Fk5hip(r5h, s5h);
            // Assert
            Assert.Equal(0.9999999999999928638, r5h[0], TOLERANCE);
            Assert.Equal(0.1110223351022919694e-6, r5h[1], TOLERANCE);
            Assert.Equal(0.4411803962536558154e-7, r5h[2], TOLERANCE);
            Assert.Equal(-0.1110223308458746430e-6, r5h[3], TOLERANCE);
            Assert.Equal(0.9999999999999891830, r5h[4], TOLERANCE);
            Assert.Equal(-0.9647792498984142358e-7, r5h[5], TOLERANCE);
            Assert.Equal(-0.4411805033656962252e-7, r5h[6], TOLERANCE);
            Assert.Equal(0.9647792009175314354e-7, r5h[7], TOLERANCE);
            Assert.Equal(0.9999999999999943728, r5h[8], TOLERANCE);

            Assert.Equal(-0.1454441043328607981e-8, s5h[0], 1e-17);
            Assert.Equal(0.2908882086657215962e-8, s5h[1], 1e-17);
            Assert.Equal(0.3393695767766751955e-8, s5h[2], 1e-17);
        }

        [Fact]
        public void Fk5hz()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double r5 = 1.76779433;
            double d5 = -0.2917517103;
            // Act
            double rh = 0, dh = 0;
            Sofa.Fk5hz(r5, d5, 2400000.5, 54479.0, ref rh, ref dh);
            // Assert
            Assert.Equal(1.767794191464423978, rh, TOLERANCE);
            Assert.Equal(-0.2917516001679884419, dh, TOLERANCE);
        }

        [Fact]
        public void Fw2m()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double gamb = -0.2243387670997992368e-5;
            double phib = 0.4091014602391312982;
            double psi = -0.9501954178013015092e-3;
            double eps = 0.4091014316587367472;
            // Act
            double[] r = new double[9];
            Sofa.Fw2m(gamb, phib, psi, eps, r);
            // Assert
            Assert.Equal(0.9999995505176007047, r[0], TOLERANCE);
            Assert.Equal(0.8695404617348192957e-3, r[1], TOLERANCE);
            Assert.Equal(0.3779735201865582571e-3, r[2], TOLERANCE);
            Assert.Equal(-0.8695404723772016038e-3, r[3], TOLERANCE);
            Assert.Equal(0.9999996219496027161, r[4], TOLERANCE);
            Assert.Equal(-0.1361752496887100026e-6, r[5], TOLERANCE);
            Assert.Equal(-0.3779734957034082790e-3, r[6], TOLERANCE);
            Assert.Equal(-0.1924880848087615651e-6, r[7], TOLERANCE);
            Assert.Equal(0.9999999285679971958, r[8], TOLERANCE);
        }

        [Fact]
        public void Fw2xy()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double gamb = -0.2243387670997992368e-5;
            double phib = 0.4091014602391312982;
            double psi = -0.9501954178013015092e-3;
            double eps = 0.4091014316587367472;
            // Act
            double x = 0, y = 0;
            Sofa.Fw2xy(gamb, phib, psi, eps, ref x, ref y);
            // Assert
            Assert.Equal(-0.3779734957034082790e-3, x, TOLERANCE);
            Assert.Equal(-0.1924880848087615651e-6, y, TOLERANCE);
        }

        [Fact]
        public void G2icrs()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double dl = 5.5850536063818546461558105;
            double db = -0.7853981633974483096156608;
            // Act
            double dr = 0, dd = 0;
            Sofa.G2icrs(dl, db, ref dr, ref dd);
            // Assert
            Assert.Equal(5.9338074302227188048671, dr, TOLERANCE);
            Assert.Equal(-1.1784870613579944551541, dd, TOLERANCE);
        }

        [Fact]
        public void Gc2gd()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double[] xyz = { 2e6, 3e6, 5.244e6 };
            // Act
            double e = 0, p = 0, h = 0;
            int j = Sofa.Gc2gd(SofaReferenceEllipsoids.WGS84, xyz, ref e, ref p, ref h);
            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.9827937232473290680, e, TOLERANCE);
            Assert.Equal(0.97160184819075459, p, TOLERANCE);
            Assert.Equal(331.4172461426059892, h, 1e-8);

            j = Sofa.Gc2gd(SofaReferenceEllipsoids.GRS80, xyz, ref e, ref p, ref h);
            Assert.Equal(0, j);
            Assert.Equal(0.9827937232473290680, e, TOLERANCE);
            Assert.Equal(0.97160184820607853, p, TOLERANCE);
            Assert.Equal(331.41731754844348, h, 1e-8);

            j = Sofa.Gc2gd(SofaReferenceEllipsoids.WGS72, xyz, ref e, ref p, ref h);
            Assert.Equal(0, j);
            Assert.Equal(0.9827937232473290680, e, TOLERANCE);
            Assert.Equal(0.9716018181101511937, p, TOLERANCE);
            Assert.Equal(333.2770726130318123, h, 1e-8);

            j = Sofa.Gc2gd((SofaReferenceEllipsoids)4, xyz, ref e, ref p, ref h);
            Assert.Equal(-1, j);
        }

        [Fact]
        public void Gc2gde()
        {
            const double TOL = 1e-14;
            double a = 6378136.0;
            double f = 0.0033528;
            double[] xyz = { 2e6, 3e6, 5.244e6 };
            double e = 0, p = 0, h = 0;

            int j = Sofa.Gc2gde(a, f, xyz, ref e, ref p, ref h);

            Assert.Equal(0, j);
            Assert.Equal(0.9827937232473290680, e, TOL);
            Assert.Equal(0.9716018377570411532, p, TOL);
            Assert.Equal(332.36862495764397, h, 1e-8);
        }

        [Fact]
        public void Gd2gc()
        {
            const double TOLERANCE = 1e-7;
            // Arrange
            double e = 3.1;
            double p = -0.5;
            double h = 2500.0;
            // Act
            double[] xyz = new double[3];
            int j = Sofa.Gd2gc(SofaReferenceEllipsoids.WGS84, e, p, h, xyz);
            // Assert
            Assert.Equal(0, j);
            Assert.Equal(-5599000.5577049947, xyz[0], TOLERANCE);
            Assert.Equal(233011.67223479203, xyz[1], TOLERANCE);
            Assert.Equal(-3040909.4706983363, xyz[2], TOLERANCE);

            j = Sofa.Gd2gc(SofaReferenceEllipsoids.GRS80, e, p, h, xyz);
            Assert.Equal(0, j);
            Assert.Equal(-5599000.5577260984, xyz[0], TOLERANCE);
            Assert.Equal(233011.6722356702949, xyz[1], TOLERANCE);
            Assert.Equal(-3040909.4706095476, xyz[2], TOLERANCE);

            j = Sofa.Gd2gc(SofaReferenceEllipsoids.WGS72, e, p, h, xyz);
            Assert.Equal(0, j);
            Assert.Equal(-5598998.7626301490, xyz[0], TOLERANCE);
            Assert.Equal(233011.5975297822211, xyz[1], TOLERANCE);
            Assert.Equal(-3040908.6861467111, xyz[2], TOLERANCE);

            j = Sofa.Gd2gc((SofaReferenceEllipsoids)4, e, p, h, xyz);
            Assert.Equal(-1, j);
        }

        [Fact]
        public void Gd2gce()
        {
            const double TOL = 1e-7;
            double a = 6378136.0;
            double f = 0.0033528;
            double e = 3.1;
            double p = -0.5;
            double h = 2500.0;
            double[] xyz = new double[3];

            int j = Sofa.Gd2gce(a, f, e, p, h, xyz);

            Assert.Equal(0, j);
            Assert.Equal(-5598999.6665116328, xyz[0], TOL);
            Assert.Equal(233011.6351463057189, xyz[1], TOL);
            Assert.Equal(-3040909.0517314132, xyz[2], TOL);
        }

        [Fact]
        public void Gmst00()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gmst00(2400000.5, 53736.0, 2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754174972210740592, theta, TOLERANCE);
        }

        [Fact]
        public void Gmst06()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gmst06(2400000.5, 53736.0, 2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754174971870091203, theta, TOLERANCE);
        }

        [Fact]
        public void Gmst82()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gmst82(2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754174981860675096, theta, TOLERANCE);
        }

        [Fact]
        public void Gst00a()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gst00a(2400000.5, 53736.0, 2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754166138018281369, theta, TOLERANCE);
        }

        [Fact]
        public void Gst00b()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gst00b(2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754166136510680589, theta, TOLERANCE);
        }

        [Fact]
        public void Gst06()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] rnpb = new double[9]
            {
                0.9999989440476103608,
                -0.1332881761240011518e-2,
                -0.5790767434730085097e-3,
                0.1332858254308954453e-2,
                0.9999991109044505944,
                -0.4097782710401555759e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };
            // Act
            double theta = Sofa.Gst06(2400000.5, 53736.0, 2400000.5, 53736.0, rnpb);
            // Assert
            Assert.Equal(1.754166138018167568, theta, TOLERANCE);
        }

        [Fact]
        public void Gst06a()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gst06a(2400000.5, 53736.0, 2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754166137675019159, theta, TOLERANCE);
        }

        [Fact]
        public void Gst94()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double theta = Sofa.Gst94(2400000.5, 53736.0);
            // Assert
            Assert.Equal(1.754166136020645203, theta, TOLERANCE);
        }

        [Fact]
        public void H2fk5()
        {
            const double TOLERANCE = 1e-13;
            // Arrange
            double rh = 1.767794352;
            double dh = -0.2917512594;
            double drh = -2.76413026e-6;
            double ddh = -5.92994449e-6;
            double pxh = 0.379210;
            double rvh = -7.6;
            // Act
            double r5 = 0, d5 = 0, dr5 = 0, dd5 = 0, px5 = 0, rv5 = 0;
            Sofa.H2fk5(rh, dh, drh, ddh, pxh, rvh, ref r5, ref d5, ref dr5, ref dd5, ref px5, ref rv5);
            // Assert
            Assert.Equal(1.767794455700065506, r5, TOLERANCE);
            Assert.Equal(-0.2917513626469638890, d5, TOLERANCE);
            Assert.Equal(-0.27597945024511204e-5, dr5, 1e-18);
            Assert.Equal(-0.59308014093262838e-5, dd5, 1e-18);
            Assert.Equal(0.37921, px5, TOLERANCE);
            Assert.Equal(-7.6000001309071126, rv5, 1e-11);
        }

        [Fact]
        public void Hd2ae()
        {
            const double TOLERANCE = 1e-13;
            // Arrange
            double h = 1.1;
            double d = 1.2;
            double p = 0.3;
            // Act
            double a = 0, e = 0;
            Sofa.Hd2ae(h, d, p, ref a, ref e);
            // Assert
            Assert.Equal(5.916889243730066194, a, TOLERANCE);
            Assert.Equal(0.4472186304990486228, e, 1e-14);
        }

        [Fact]
        public void Hd2pa()
        {
            const double TOLERANCE = 1e-13;
            // Arrange
            double h = 1.1;
            double d = 1.2;
            double p = 0.3;
            // Act
            double q = Sofa.Hd2pa(h, d, p);
            // Assert
            Assert.Equal(1.906227428001995580, q, TOLERANCE);
        }

        [Fact]
        public void Hfk5z()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double rh = 1.767794352;
            double dh = -0.2917512594;
            // Act
            double r5 = 0, d5 = 0, dr5 = 0, dd5 = 0;
            Sofa.Hfk5z(rh, dh, 2400000.5, 54479.0, ref r5, ref d5, ref dr5, ref dd5);
            // Assert
            Assert.Equal(1.767794490535581026, r5, 1e-13);
            Assert.Equal(-0.2917513695320114258, d5, TOLERANCE);
            Assert.Equal(0.4335890983539243029e-8, dr5, 1e-22);
            Assert.Equal(-0.8569648841237745902e-9, dd5, 1e-23);
        }

        [Fact]
        public void Icrs2g()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double dr = 5.9338074302227188048671087;
            double dd = -1.1784870613579944551540570;
            // Act
            double dl = 0, db = 0;
            Sofa.Icrs2g(dr, dd, ref dl, ref db);
            // Assert
            Assert.Equal(5.5850536063818546461558, dl, TOLERANCE);
            Assert.Equal(-0.7853981633974483096157, db, TOLERANCE);
        }

        [Fact]
        public void Ir()
        {
            // Arrange
            double[] r = new double[9]
            {
                2.0, 3.0, 2.0,
                3.0, 2.0, 3.0,
                3.0, 4.0, 5.0
            };
            // Act
            Sofa.Ir(r);
            // Assert
            Assert.Equal(1.0, r[0]);
            Assert.Equal(0.0, r[1]);
            Assert.Equal(0.0, r[2]);
            Assert.Equal(0.0, r[3]);
            Assert.Equal(1.0, r[4]);
            Assert.Equal(0.0, r[5]);
            Assert.Equal(0.0, r[6]);
            Assert.Equal(0.0, r[7]);
            Assert.Equal(1.0, r[8]);
        }

        [Fact]
        public void Jd2cal()
        {
            const double TOLERANCE = 1e-7;
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 50123.9999;
            // Act
            int iy = 0, im = 0, id = 0;
            double fd = 0;
            int j = Sofa.Jd2cal(dj1, dj2, ref iy, ref im, ref id, ref fd);
            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1996, iy);
            Assert.Equal(2, im);
            Assert.Equal(10, id);
            Assert.Equal(0.9999, fd, TOLERANCE);
        }

        [Fact]
        public void Jdcalf()
        {
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 50123.9999;
            // Act
            int[] iydmf = new int[4];
            int j = Sofa.Jdcalf(4, dj1, dj2, iydmf);
            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1996, iydmf[0]);
            Assert.Equal(2, iydmf[1]);
            Assert.Equal(10, iydmf[2]);
            Assert.Equal(9999, iydmf[3]);
        }

        [Fact]
        public void Ld()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double bm = 0.00028574;
            double[] p = { -0.763276255, -0.608633767, -0.216735543 };
            double[] q = { -0.763276255, -0.608633767, -0.216735543 };
            double[] e = { 0.76700421, 0.605629598, 0.211937094 };
            double em = 8.91276983;
            double dlim = 3e-10;
            // Act
            double[] p1 = new double[3];
            Sofa.Ld(bm, p, q, e, em, dlim, p1);
            // Assert
            Assert.Equal(-0.7632762548968159627, p1[0], TOLERANCE);
            Assert.Equal(-0.6086337670823762701, p1[1], TOLERANCE);
            Assert.Equal(-0.2167355431320546947, p1[2], TOLERANCE);
        }

        [Fact]
        public void Ldn()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            int n = 3;
            var b = new Sofa.LdBody[3];
            b[0].bm = 0.00028574;
            b[0].dl = 3e-10;
            b[0].pv = new double[6] { -7.81014427, -5.60956681, -1.98079819, 0.0030723249, -0.00406995477, -0.00181335842 };

            b[1].bm = 0.00095435;
            b[1].dl = 3e-9;
            b[1].pv = new double[6] { 0.738098796, 4.63658692, 1.9693136, -0.00755816922, 0.00126913722, 0.000727999001 };

            b[2].bm = 1.0;
            b[2].dl = 6e-6;
            b[2].pv = new double[6] { -0.000712174377, -0.00230478303, -0.00105865966, 6.29235213e-6, -3.30888387e-7, -2.96486623e-7 };

            // Additional body data...

            double[] ob = { -0.974170437, -0.2115201, -0.0917583114 };
            double[] sc = { -0.763276255, -0.608633767, -0.216735543 };
            // Act
            double[] sn = new double[3];
            Sofa.Ldn(n, b, ob, sc, sn);
            // Assert
            Assert.Equal(-0.7632762579693333866, sn[0], TOLERANCE);
            Assert.Equal(-0.6086337636093002660, sn[1], TOLERANCE);
            Assert.Equal(-0.2167355420646328159, sn[2], TOLERANCE);
        }

        [Fact]
        public void Ldsun()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] p = { -0.763276255, -0.608633767, -0.216735543 };
            double[] e = { -0.973644023, -0.20925523, -0.0907169552 };
            double em = 0.999809214;
            // Act
            double[] p1 = new double[3];
            Sofa.Ldsun(p, e, em, p1);
            // Assert
            Assert.Equal(-0.7632762580731413169, p1[0], TOLERANCE);
            Assert.Equal(-0.6086337635262647900, p1[1], TOLERANCE);
            Assert.Equal(-0.2167355419322321302, p1[2], TOLERANCE);
        }

        [Fact]
        public void Lteceq()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = 2500.0;
            double dl = 1.5;
            double db = 0.6;
            // Act
            double dr = 0, dd = 0;
            Sofa.Lteceq(epj, dl, db, ref dr, ref dd);
            // Assert
            Assert.Equal(1.275156021861921167, dr, TOLERANCE);
            Assert.Equal(0.9966573543519204791, dd, TOLERANCE);
        }

        [Fact]
        public void Ltecm()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = -3000.0;
            // Act
            double[] rm = new double[9];
            Sofa.Ltecm(epj, rm);
            // Assert
            Assert.Equal(0.3564105644859788825, rm[0], TOLERANCE);
            Assert.Equal(0.8530575738617682284, rm[1], TOLERANCE);
            Assert.Equal(0.3811355207795060435, rm[2], TOLERANCE);
            Assert.Equal(-0.9343283469640709942, rm[3], TOLERANCE);
            Assert.Equal(0.3247830597681745976, rm[4], TOLERANCE);
            Assert.Equal(0.1467872751535940865, rm[5], TOLERANCE);
            Assert.Equal(0.1431636191201167793e-2, rm[6], TOLERANCE);
            Assert.Equal(-0.4084222566960599342, rm[7], TOLERANCE);
            Assert.Equal(0.9127919865189030899, rm[8], TOLERANCE);
        }

        [Fact]
        public void Lteqec()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = -1500.0;
            double dr = 1.234;
            double dd = 0.987;
            // Act
            double dl = 0, db = 0;
            Sofa.Lteqec(epj, dr, dd, ref dl, ref db);
            // Assert
            Assert.Equal(0.5039483649047114859, dl, TOLERANCE);
            Assert.Equal(0.5848534459726224882, db, TOLERANCE);
        }

        [Fact]
        public void Ltp()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = 1666.666;
            // Act
            double[] rp = new double[9];
            Sofa.Ltp(epj, rp);
            // Assert
            Assert.Equal(0.9967044141159213819, rp[0], TOLERANCE);
            Assert.Equal(0.7437801893193210840e-1, rp[1], TOLERANCE);
            Assert.Equal(0.3237624409345603401e-1, rp[2], TOLERANCE);
            Assert.Equal(-0.7437802731819618167e-1, rp[3], TOLERANCE);
            Assert.Equal(0.9972293894454533070, rp[4], TOLERANCE);
            Assert.Equal(-0.1205768842723593346e-2, rp[5], TOLERANCE);
            Assert.Equal(-0.3237622482766575399e-1, rp[6], TOLERANCE);
            Assert.Equal(-0.1206286039697609008e-2, rp[7], TOLERANCE);
            Assert.Equal(0.9994750246704010914, rp[8], TOLERANCE);
        }

        [Fact]
        public void Ltpb()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = 1666.666;
            // Act
            double[] rpb = new double[9];
            Sofa.Ltpb(epj, rpb);
            // Assert
            Assert.Equal(0.9967044167723271851, rpb[0], TOLERANCE);
            Assert.Equal(0.7437794731203340345e-1, rpb[1], TOLERANCE);
            Assert.Equal(0.3237632684841625547e-1, rpb[2], TOLERANCE);
            Assert.Equal(-0.7437795663437177152e-1, rpb[3], TOLERANCE);
            Assert.Equal(0.9972293947500013666, rpb[4], TOLERANCE);
            Assert.Equal(-0.1205741865911243235e-2, rpb[5], TOLERANCE);
            Assert.Equal(-0.3237630543224664992e-1, rpb[6], TOLERANCE);
            Assert.Equal(-0.1206316791076485295e-2, rpb[7], TOLERANCE);
            Assert.Equal(0.9994750220222438819, rpb[8], TOLERANCE);
        }

        [Fact]
        public void Ltpecl()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = -1500.0;
            // Act
            double[] vec = new double[3];
            Sofa.Ltpecl(epj, vec);
            // Assert
            Assert.Equal(0.4768625676477096525e-3, vec[0], TOLERANCE);
            Assert.Equal(-0.4052259533091875112, vec[1], TOLERANCE);
            Assert.Equal(0.9142164401096448012, vec[2], TOLERANCE);
        }

        [Fact]
        public void Ltpequ()
        {
            const double TOLERANCE = 1e-14;
            // Arrange
            double epj = -2500.0;
            // Act
            double[] veq = new double[3];
            Sofa.Ltpequ(epj, veq);
            // Assert
            Assert.Equal(-0.3586652560237326659, veq[0], TOLERANCE);
            Assert.Equal(-0.1996978910771128475, veq[1], TOLERANCE);
            Assert.Equal(0.9118552442250819624, veq[2], TOLERANCE);
        }

        [Fact]
        public void Moon98()
        {
            const double TOLERANCE = 1e-11;
            // Act
            double[] pv = new double[6];
            Sofa.Moon98(2400000.5, 43999.9, pv);
            // Assert
            Assert.Equal(-0.2601295959971044180e-2, pv[0], TOLERANCE);
            Assert.Equal(0.6139750944302742189e-3, pv[1], TOLERANCE);
            Assert.Equal(0.2640794528229828909e-3, pv[2], TOLERANCE);
            Assert.Equal(-0.1244321506649895021e-3, pv[3], TOLERANCE);
            Assert.Equal(-0.5219076942678119398e-3, pv[4], TOLERANCE);
            Assert.Equal(-0.1716132214378462047e-3, pv[5], TOLERANCE);
        }

        [Fact]
        public void Num00a()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double[] rmatn = new double[9];
            Sofa.Num00a(2400000.5, 53736.0, rmatn);
            // Assert
            Assert.Equal(0.9999999999536227949, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836238544090873336e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830835237722400669e-5, rmatn[2], TOLERANCE);
            Assert.Equal(-0.8836082880798569274e-5, rmatn[3], TOLERANCE);
            Assert.Equal(0.9999999991354655028, rmatn[4], TOLERANCE);
            Assert.Equal(-0.4063240865362499850e-4, rmatn[5], TOLERANCE);
            Assert.Equal(-0.3831194272065995866e-5, rmatn[6], TOLERANCE);
            Assert.Equal(0.4063237480216291775e-4, rmatn[7], TOLERANCE);
            Assert.Equal(0.9999999991671660338, rmatn[8], TOLERANCE);
        }

        [Fact]
        public void Num00b()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double[] rmatn = new double[9];
            Sofa.Num00b(2400000.5, 53736, rmatn);
            // Assert
            Assert.Equal(0.9999999999536069682, rmatn[0], TOLERANCE);
            Assert.Equal(0.8837746144871248011e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3831488838252202945e-5, rmatn[2], TOLERANCE);
            Assert.Equal(-0.8837590456632304720e-5, rmatn[3], TOLERANCE);
            Assert.Equal(0.9999999991354692733, rmatn[4], TOLERANCE);
            Assert.Equal(-0.4063198798559591654e-4, rmatn[5], TOLERANCE);
            Assert.Equal(-0.3831847930134941271e-5, rmatn[6], TOLERANCE);
            Assert.Equal(0.4063195412258168380e-4, rmatn[7], TOLERANCE);
            Assert.Equal(0.9999999991671806225, rmatn[8], TOLERANCE);
        }

        [Fact]
        public void Num06a()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double[] rmatn = new double[9];
            Sofa.Num06a(2400000.5, 53736, rmatn);
            // Assert
            Assert.Equal(0.9999999999536227668, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836241998111535233e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830834608415287707e-5, rmatn[2], TOLERANCE);
            Assert.Equal(-0.8836086334870740138e-5, rmatn[3], TOLERANCE);
            Assert.Equal(0.9999999991354657474, rmatn[4], TOLERANCE);
            Assert.Equal(-0.4063240188248455065e-4, rmatn[5], TOLERANCE);
            Assert.Equal(-0.3831193642839398128e-5, rmatn[6], TOLERANCE);
            Assert.Equal(0.4063236803101479770e-4, rmatn[7], TOLERANCE);
            Assert.Equal(0.9999999991671663114, rmatn[8], TOLERANCE);
        }

        [Fact]
        public void Numat()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double epsa = 0.4090789763356509900;
            double dpsi = -0.9630909107115582393e-5;
            double deps = 0.4063239174001678826e-4;
            // Act
            double[] rmatn = new double[9];
            Sofa.Numat(epsa, dpsi, deps, rmatn);
            // Assert
            Assert.Equal(0.9999999999536227949, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836239320236250577e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830833447458251908e-5, rmatn[2], TOLERANCE);
            Assert.Equal(-0.8836083657016688588e-5, rmatn[3], TOLERANCE);
            Assert.Equal(0.9999999991354654959, rmatn[4], TOLERANCE);
            Assert.Equal(-0.4063240865361857698e-4, rmatn[5], TOLERANCE);
            Assert.Equal(-0.3831192481833385226e-5, rmatn[6], TOLERANCE);
            Assert.Equal(0.4063237480216934159e-4, rmatn[7], TOLERANCE);
            Assert.Equal(0.9999999991671660407, rmatn[8], TOLERANCE);
        }
        [Fact]
        public void Nut00a()
        {
            const double TOLERANCE = 1e-13;
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut00a(2400000.5, 53736.0, ref dpsi, ref deps);
            // Assert
            Assert.Equal(-0.9630909107115518431e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063239174001678710e-4, deps, TOLERANCE);
        }

        [Fact]
        public void Nut00b()
        {
            const double TOLERANCE = 1e-13;
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut00b(2400000.5, 53736.0, ref dpsi, ref deps);
            // Assert
            Assert.Equal(-0.9632552291148362783e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063197106621159367e-4, deps, TOLERANCE);
        }

        [Fact]
        public void Nut06a()
        {
            const double TOLERANCE = 1e-13;
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut06a(2400000.5, 53736.0, ref dpsi, ref deps);
            // Assert
            Assert.Equal(-0.9630912025820308797e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063238496887249798e-4, deps, TOLERANCE);
        }

        [Fact]
        public void Nut80()
        {
            const double TOLERANCE = 1e-13;
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut80(2400000.5, 53736.0, ref dpsi, ref deps);
            // Assert
            Assert.Equal(-0.9643658353226563966e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4060051006879713322e-4, deps, TOLERANCE);
        }

        [Fact]
        public void Nutm80()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double[] rmatn = new double[9];
            Sofa.Nutm80(2400000.5, 53736.0, rmatn);
            // Assert
            Assert.Equal(0.9999999999534999268, rmatn[0], TOLERANCE);
            Assert.Equal(0.8847935789636432161e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3835906502164019142e-5, rmatn[2], TOLERANCE);
            Assert.Equal(-0.8847780042583435924e-5, rmatn[3], TOLERANCE);
            Assert.Equal(0.9999999991366569963, rmatn[4], TOLERANCE);
            Assert.Equal(-0.4060052702727130809e-4, rmatn[5], TOLERANCE);
            Assert.Equal(-0.3836265729708478796e-5, rmatn[6], TOLERANCE);
            Assert.Equal(0.4060049308612638555e-4, rmatn[7], TOLERANCE);
            Assert.Equal(0.9999999991684415129, rmatn[8], TOLERANCE);
        }

        [Fact]
        public void Obl06()
        {
            const double TOLERANCE = 1e-14;
            // Act
            double obl = Sofa.Obl06(2400000.5, 54388.0);
            // Assert
            Assert.Equal(0.4090749229387258204, obl, TOLERANCE);
        }

        [Fact]
        public void Obl80()
        {
            const double TOLERANCE = 1e-14;
            // Act
            double eps0 = Sofa.Obl80(2400000.5, 54388.0);
            // Assert
            Assert.Equal(0.4090751347643816218, eps0, TOLERANCE);
        }

        [Fact]
        public void P06e()
        {
            const double TOLERANCE = 1e-14;
            // Act
            double eps0 = 0, psia = 0, oma = 0, bpa = 0, bqa = 0, pia = 0, bpia = 0,
                   epsa = 0, chia = 0, za = 0, zetaa = 0, thetaa = 0, pa = 0, gam = 0, phi = 0, psi = 0;
            Sofa.P06e(2400000.5, 52541.0, ref eps0, ref psia, ref oma, ref bpa,
                      ref bqa, ref pia, ref bpia, ref epsa, ref chia, ref za,
                      ref zetaa, ref thetaa, ref pa, ref gam, ref phi, ref psi);
            // Assert
            Assert.Equal(0.4090926006005828715, eps0, TOLERANCE);
            Assert.Equal(0.6664369630191613431e-3, psia, TOLERANCE);
            Assert.Equal(0.4090925973783255982, oma, TOLERANCE);
            Assert.Equal(0.5561149371265209445e-6, bpa, TOLERANCE);
            Assert.Equal(-0.6191517193290621270e-5, bqa, TOLERANCE);
            Assert.Equal(0.6216441751884382923e-5, pia, TOLERANCE);
            Assert.Equal(3.052014180023779882, bpia, TOLERANCE);
            Assert.Equal(0.4090864054922431688, epsa, TOLERANCE);
            Assert.Equal(0.1387703379530915364e-5, chia, TOLERANCE);
            Assert.Equal(0.2921789846651790546e-3, za, TOLERANCE);
            Assert.Equal(0.3178773290332009310e-3, zetaa, TOLERANCE);
            Assert.Equal(0.2650932701657497181e-3, thetaa, TOLERANCE);
            Assert.Equal(0.6651637681381016288e-3, pa, TOLERANCE);
            Assert.Equal(0.1398077115963754987e-5, gam, TOLERANCE);
            Assert.Equal(0.4090864090837462602, phi, TOLERANCE);
            Assert.Equal(0.6664464807480920325e-3, psi, TOLERANCE);
        }

        [Fact]
        public void P2pv()
        {
            // Arrange
            double[] p = { 0.25, 1.2, 3.0 };
            double[] pv = new double[6]
            {
                0.3, 1.2, -2.5,
                -0.5, 3.1, 0.9
            };
            // Act
            Sofa.P2pv(p, pv);
            // Assert
            Assert.Equal(0.25, pv[0]);
            Assert.Equal(1.2, pv[1]);
            Assert.Equal(3.0, pv[2]);
            Assert.Equal(0.0, pv[3]);
            Assert.Equal(0.0, pv[4]);
            Assert.Equal(0.0, pv[5]);
        }

        [Fact]
        public void P2s()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] p = { 100.0, -50.0, 25.0 };
            // Act
            double theta = 0, phi = 0, r = 0;
            Sofa.P2s(p, ref theta, ref phi, ref r);
            // Assert
            Assert.Equal(-0.4636476090008061162, theta, TOLERANCE);
            Assert.Equal(0.2199879773954594463, phi, TOLERANCE);
            Assert.Equal(114.5643923738960002, r, 1e-9);
        }

        [Fact]
        public void Pap()
        {
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };

            double theta = Sofa.Pap(a, b);

            Assert.Equal(0.3671514267841113674, theta, 12);
        }

        [Fact]
        public void Pap2()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };
            // Act
            double theta = Sofa.Pap(a, b);
            // Assert
            Assert.Equal(0.3671514267841113674, theta, TOLERANCE);
        }

        [Fact]
        public void Pas()
        {
            double al = 1.0;
            double ap = 0.1;
            double bl = 0.2;
            double bp = -1.0;

            double theta = Sofa.Pas(al, ap, bl, bp);

            Assert.Equal(-2.724544922932270424, theta, 12);
        }

        [Fact]
        public void Pb06()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double bzeta = 0, bz = 0, btheta = 0;
            Sofa.Pb06(2400000.5, 50123.9999, ref bzeta, ref bz, ref btheta);
            // Assert
            Assert.Equal(-0.5092634016326478238e-3, bzeta, TOLERANCE);
            Assert.Equal(-0.3602772060566044413e-3, bz, TOLERANCE);
            Assert.Equal(-0.3779735537167811177e-3, btheta, TOLERANCE);
        }

        [Fact]
        public void Pdp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };

            double adb = Sofa.Pdp(a, b);

            Assert.Equal(20, adb, 12);
        }

        [Fact]
        public void Pfw06()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double gamb = 0, phib = 0, psib = 0, epsa = 0;
            Sofa.Pfw06(2400000.5, 50123.9999, ref gamb, ref phib, ref psib, ref epsa);
            // Assert
            Assert.Equal(-0.2243387670997995690e-5, gamb, 1e-16);
            Assert.Equal(0.4091014602391312808, phib, TOLERANCE);
            Assert.Equal(-0.9501954178013015895e-3, psib, 1e-14);
            Assert.Equal(0.4091014316587367491, epsa, TOLERANCE);
        }

        [Fact]
        public void Plan94()
        {
            const double TOLERANCE = 1e-11;
            // Act
            double[] pv = new double[6];
            int j;

            // Test 1: Invalid planet number (0)
            j = Sofa.Plan94(2400000.5, 1e6, 0, pv);
            Assert.Equal(0.0, pv[0], 0);
            Assert.Equal(0.0, pv[1], 0);
            Assert.Equal(0.0, pv[2], 0);
            Assert.Equal(0.0, pv[3], 0);
            Assert.Equal(0.0, pv[4], 0);
            Assert.Equal(0.0, pv[5], 0);
            Assert.Equal(-1, j);

            // Test 2: Invalid planet number (10)
            j = Sofa.Plan94(2400000.5, 1e6, 10, pv);
            Assert.Equal(-1, j);

            // Test 3: Date outside valid range
            j = Sofa.Plan94(2400000.5, -320000, 3, pv);
            Assert.Equal(0.9308038666832975759, pv[0], TOLERANCE);
            Assert.Equal(0.3258319040261346000, pv[1], TOLERANCE);
            Assert.Equal(0.1422794544481140560, pv[2], TOLERANCE);
            Assert.Equal(-0.6429458958255170006e-2, pv[3], TOLERANCE);
            Assert.Equal(0.1468570657704237764e-1, pv[4], TOLERANCE);
            Assert.Equal(0.6406996426270981189e-2, pv[5], TOLERANCE);
            Assert.Equal(1, j);

            // Test 4: Mercury, valid date
            j = Sofa.Plan94(2400000.5, 43999.9, 1, pv);
            Assert.Equal(0.2945293959257430832, pv[0], TOLERANCE);
            Assert.Equal(-0.2452204176601049596, pv[1], TOLERANCE);
            Assert.Equal(-0.1615427700571978153, pv[2], TOLERANCE);
            Assert.Equal(0.1413867871404614441e-1, pv[3], TOLERANCE);
            Assert.Equal(0.1946548301104706582e-1, pv[4], TOLERANCE);
            Assert.Equal(0.8929809783898904786e-2, pv[5], TOLERANCE);
            Assert.Equal(0, j);
        }
        [Fact]
        public void Pm()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] p = { 0.3, 1.2, -2.5 };
            // Act
            double r = Sofa.Pm(p);
            // Assert
            Assert.Equal(2.789265136196270604, r, TOLERANCE);
        }

        [Fact]
        public void Pmat00()
        {
            const double TOL = 1e-12;
            double[] rbp = new double[9];

            Sofa.Pmat00(2400000.5, 50123.9999, rbp);

            Assert.Equal(0.9999995505175087260, rbp[0], TOL);
            Assert.Equal(0.8695405883617884705e-3, rbp[1], 1e-14);
            Assert.Equal(0.3779734722239007105e-3, rbp[2], 1e-14);
            Assert.Equal(-0.8695405990410863719e-3, rbp[3], 1e-14);
            Assert.Equal(0.9999996219494925900, rbp[4], TOL);
            Assert.Equal(-0.1360775820404982209e-6, rbp[5], 1e-14);
            Assert.Equal(-0.3779734476558184991e-3, rbp[6], 1e-14);
            Assert.Equal(-0.1925857585832024058e-6, rbp[7], 1e-14);
            Assert.Equal(0.9999999285680153377, rbp[8], TOL);
        }

        [Fact]
        public void Pmat06()
        {
            const double TOL = 1e-12;
            double[] rbp = new double[9];

            Sofa.Pmat06(2400000.5, 50123.9999, rbp);

            Assert.Equal(0.9999995505176007047, rbp[0], TOL);
            Assert.Equal(0.8695404617348208406e-3, rbp[1], 1e-14);
            Assert.Equal(0.3779735201865589104e-3, rbp[2], 1e-14);
            Assert.Equal(-0.8695404723772031414e-3, rbp[3], 1e-14);
            Assert.Equal(0.9999996219496027161, rbp[4], TOL);
            Assert.Equal(-0.1361752497080270143e-6, rbp[5], 1e-14);
            Assert.Equal(-0.3779734957034089490e-3, rbp[6], 1e-14);
            Assert.Equal(-0.1924880847894457113e-6, rbp[7], 1e-14);
            Assert.Equal(0.9999999285679971958, rbp[8], TOL);
        }

        [Fact]
        public void Pmat76()
        {
            const double TOL = 1e-12;
            double[] rmatp = new double[9];

            Sofa.Pmat76(2400000.5, 50123.9999, rmatp);

            Assert.Equal(0.9999995504328350733, rmatp[0], TOL);
            Assert.Equal(0.8696632209480960785e-3, rmatp[1], 1e-14);
            Assert.Equal(0.3779153474959888345e-3, rmatp[2], 1e-14);
            Assert.Equal(-0.8696632209485112192e-3, rmatp[3], 1e-14);
            Assert.Equal(0.9999996218428560614, rmatp[4], TOL);
            Assert.Equal(-0.1643284776111886407e-6, rmatp[5], 1e-14);
            Assert.Equal(-0.3779153474950335077e-3, rmatp[6], 1e-14);
            Assert.Equal(-0.1643306746147366896e-6, rmatp[7], 1e-14);
            Assert.Equal(0.9999999285899790119, rmatp[8], TOL);
        }

        [Fact]
        public void Pmp()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            // Act
            double[] amb = new double[3];
            Sofa.Pmp(a, b, amb);
            // Assert
            Assert.Equal(1.0, amb[0], TOLERANCE);
            Assert.Equal(-1.0, amb[1], TOLERANCE);
            Assert.Equal(-1.0, amb[2], TOLERANCE);
        }

        [Fact]
        public void Pmpx()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double rc = 1.234;
            double dc = 0.789;
            double pr = 1e-5;
            double pd = -2e-5;
            double px = 1e-2;
            double rv = 10.0;
            double pmt = 8.75;
            double[] pob = { 0.9, 0.4, 0.1 };
            // Act
            double[] pco = new double[3];
            Sofa.Pmpx(rc, dc, pr, pd, px, rv, pmt, pob, pco);
            // Assert
            Assert.Equal(0.2328137623960308438, pco[0], TOLERANCE);
            Assert.Equal(0.6651097085397855328, pco[1], TOLERANCE);
            Assert.Equal(0.7095257765896359837, pco[2], TOLERANCE);
        }

        [Fact]
        public void Pmsafe()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double ra1 = 1.234;
            double dec1 = 0.789;
            double pmr1 = 1e-5;
            double pmd1 = -2e-5;
            double px1 = 1e-2;
            double rv1 = 10.0;
            double ep1a = 2400000.5;
            double ep1b = 48348.5625;
            double ep2a = 2400000.5;
            double ep2b = 51544.5;
            // Act
            double ra2 = 0, dec2 = 0, pmr2 = 0, pmd2 = 0, px2 = 0, rv2 = 0;
            int j = Sofa.Pmsafe(ra1, dec1, pmr1, pmd1, px1, rv1,
                                ep1a, ep1b, ep2a, ep2b,
                                ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);
            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1.234087484501017061, ra2, TOLERANCE);
            Assert.Equal(0.7888249982450468567, dec2, TOLERANCE);
            Assert.Equal(0.9996457663586073988e-5, pmr2, TOLERANCE);
            Assert.Equal(-0.2000040085106754565e-4, pmd2, 1e-16);
            Assert.Equal(0.9999997295356830666e-2, px2, TOLERANCE);
            Assert.Equal(10.38468380293920069, rv2, 1e-10);
        }

        [Fact]
        public void Pn()
        {
            const double TOLERANCE = 1e-12;
            // Arrange
            double[] p = { 0.3, 1.2, -2.5 };
            // Act
            double r = 0;
            double[] u = new double[3];
            Sofa.Pn(p, ref r, u);
            // Assert
            Assert.Equal(2.789265136196270604, r, TOLERANCE);
            Assert.Equal(0.1075552109073112058, u[0], TOLERANCE);
            Assert.Equal(0.4302208436292448232, u[1], TOLERANCE);
            Assert.Equal(-0.8962934242275933816, u[2], TOLERANCE);
        }

        [Fact]
        public void Pn00()
        {
            const double TOLERANCE_12 = 1e-12;
            const double TOLERANCE_14 = 1e-14;
            const double TOLERANCE_16 = 1e-16;
            const double TOLERANCE_18 = 1e-18;

            // Arrange
            double dpsi = -0.9632552291149335877e-5;
            double deps = 0.4063197106621141414e-4;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn00(2400000.5, 53736.0, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090791789404229916, epsa, TOLERANCE_12);

            Assert.Equal(0.9999999999999942498, rb[0], TOLERANCE_12);
            Assert.Equal(-0.7078279744199196626e-7, rb[1], TOLERANCE_18);
            Assert.Equal(0.8056217146976134152e-7, rb[2], TOLERANCE_18);
            Assert.Equal(0.7078279477857337206e-7, rb[3], TOLERANCE_18);
            Assert.Equal(0.9999999999999969484, rb[4], TOLERANCE_12);
            Assert.Equal(0.3306041454222136517e-7, rb[5], TOLERANCE_18);
            Assert.Equal(-0.8056217380986972157e-7, rb[6], TOLERANCE_18);
            Assert.Equal(-0.3306040883980552500e-7, rb[7], TOLERANCE_18);
            Assert.Equal(0.9999999999999962084, rb[8], TOLERANCE_12);

            Assert.Equal(0.9999989300532289018, rp[0], TOLERANCE_12);
            Assert.Equal(-0.1341647226791824349e-2, rp[1], TOLERANCE_14);
            Assert.Equal(-0.5829880927190296547e-3, rp[2], TOLERANCE_14);
            Assert.Equal(0.1341647231069759008e-2, rp[3], TOLERANCE_14);
            Assert.Equal(0.9999990999908750433, rp[4], TOLERANCE_12);
            Assert.Equal(-0.3837444441583715468e-6, rp[5], TOLERANCE_14);
            Assert.Equal(0.5829880828740957684e-3, rp[6], TOLERANCE_14);
            Assert.Equal(-0.3984203267708834759e-6, rp[7], TOLERANCE_14);
            Assert.Equal(0.9999998300623538046, rp[8], TOLERANCE_12);

            Assert.Equal(0.9999989300052243993, rbp[0], TOLERANCE_12);
            Assert.Equal(-0.1341717990239703727e-2, rbp[1], TOLERANCE_14);
            Assert.Equal(-0.5829075749891684053e-3, rbp[2], TOLERANCE_14);
            Assert.Equal(0.1341718013831739992e-2, rbp[3], TOLERANCE_14);
            Assert.Equal(0.9999990998959191343, rbp[4], TOLERANCE_12);
            Assert.Equal(-0.3505759733565421170e-6, rbp[5], TOLERANCE_14);
            Assert.Equal(0.5829075206857717883e-3, rbp[6], TOLERANCE_14);
            Assert.Equal(-0.4315219955198608970e-6, rbp[7], TOLERANCE_14);
            Assert.Equal(0.9999998301093036269, rbp[8], TOLERANCE_12);

            Assert.Equal(0.9999999999536069682, rn[0], TOLERANCE_12);
            Assert.Equal(0.8837746144872140812e-5, rn[1], TOLERANCE_16);
            Assert.Equal(0.3831488838252590008e-5, rn[2], TOLERANCE_16);
            Assert.Equal(-0.8837590456633197506e-5, rn[3], TOLERANCE_16);
            Assert.Equal(0.9999999991354692733, rn[4], TOLERANCE_12);
            Assert.Equal(-0.4063198798559573702e-4, rn[5], TOLERANCE_16);
            Assert.Equal(-0.3831847930135328368e-5, rn[6], TOLERANCE_16);
            Assert.Equal(0.4063195412258150427e-4, rn[7], TOLERANCE_16);
            Assert.Equal(0.9999999991671806225, rn[8], TOLERANCE_12);

            Assert.Equal(0.9999989440499982806, rbpn[0], TOLERANCE_12);
            Assert.Equal(-0.1332880253640848301e-2, rbpn[1], TOLERANCE_14);
            Assert.Equal(-0.5790760898731087295e-3, rbpn[2], TOLERANCE_14);
            Assert.Equal(0.1332856746979948745e-2, rbpn[3], TOLERANCE_14);
            Assert.Equal(0.9999991109064768883, rbpn[4], TOLERANCE_12);
            Assert.Equal(-0.4097740555723063806e-4, rbpn[5], TOLERANCE_14);
            Assert.Equal(0.5791301929950205000e-3, rbpn[6], TOLERANCE_14);
            Assert.Equal(0.4020553681373702931e-4, rbpn[7], TOLERANCE_14);
            Assert.Equal(0.9999998314958529887, rbpn[8], TOLERANCE_12);
        }

        [Fact]
        public void Pn00a()
        {
            const double TOLERANCE_12 = 1e-12;
            const double TOLERANCE_14 = 1e-14;
            const double TOLERANCE_16 = 1e-16;

            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn00a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9630909107115518431e-5, dpsi, TOLERANCE_12);
            Assert.Equal(0.4063239174001678710e-4, deps, TOLERANCE_12);
            Assert.Equal(0.4090791789404229916, epsa, TOLERANCE_12);

            Assert.Equal(0.9999999999999942498, rb[0], TOLERANCE_12);
            Assert.Equal(-0.7078279744199196626e-7, rb[1], TOLERANCE_16);
            Assert.Equal(0.8056217146976134152e-7, rb[2], TOLERANCE_16);
            Assert.Equal(0.7078279477857337206e-7, rb[3], TOLERANCE_16);
            Assert.Equal(0.9999999999999969484, rb[4], TOLERANCE_12);
            Assert.Equal(0.3306041454222136517e-7, rb[5], TOLERANCE_16);
            Assert.Equal(-0.8056217380986972157e-7, rb[6], TOLERANCE_16);
            Assert.Equal(-0.3306040883980552500e-7, rb[7], TOLERANCE_16);
            Assert.Equal(0.9999999999999962084, rb[8], TOLERANCE_12);

            Assert.Equal(0.9999989300532289018, rp[0], TOLERANCE_12);
            Assert.Equal(-0.1341647226791824349e-2, rp[1], TOLERANCE_14);
            Assert.Equal(-0.5829880927190296547e-3, rp[2], TOLERANCE_14);
            Assert.Equal(0.1341647231069759008e-2, rp[3], TOLERANCE_14);
            Assert.Equal(0.9999990999908750433, rp[4], TOLERANCE_12);
            Assert.Equal(-0.3837444441583715468e-6, rp[5], TOLERANCE_14);
            Assert.Equal(0.5829880828740957684e-3, rp[6], TOLERANCE_14);
            Assert.Equal(-0.3984203267708834759e-6, rp[7], TOLERANCE_14);
            Assert.Equal(0.9999998300623538046, rp[8], TOLERANCE_12);

            Assert.Equal(0.9999989300052243993, rbp[0], TOLERANCE_12);
            Assert.Equal(-0.1341717990239703727e-2, rbp[1], TOLERANCE_14);
            Assert.Equal(-0.5829075749891684053e-3, rbp[2], TOLERANCE_14);
            Assert.Equal(0.1341718013831739992e-2, rbp[3], TOLERANCE_14);
            Assert.Equal(0.9999990998959191343, rbp[4], TOLERANCE_12);
            Assert.Equal(-0.3505759733565421170e-6, rbp[5], TOLERANCE_14);
            Assert.Equal(0.5829075206857717883e-3, rbp[6], TOLERANCE_14);
            Assert.Equal(-0.4315219955198608970e-6, rbp[7], TOLERANCE_14);
            Assert.Equal(0.9999998301093036269, rbp[8], TOLERANCE_12);

            Assert.Equal(0.9999999999536227949, rn[0], TOLERANCE_12);
            Assert.Equal(0.8836238544090873336e-5, rn[1], TOLERANCE_14);
            Assert.Equal(0.3830835237722400669e-5, rn[2], TOLERANCE_14);
            Assert.Equal(-0.8836082880798569274e-5, rn[3], TOLERANCE_14);
            Assert.Equal(0.9999999991354655028, rn[4], TOLERANCE_12);
            Assert.Equal(-0.4063240865362499850e-4, rn[5], TOLERANCE_14);
            Assert.Equal(-0.3831194272065995866e-5, rn[6], TOLERANCE_14);
            Assert.Equal(0.4063237480216291775e-4, rn[7], TOLERANCE_14);
            Assert.Equal(0.9999999991671660338, rn[8], TOLERANCE_12);

            Assert.Equal(0.9999989440476103435, rbpn[0], TOLERANCE_12);
            Assert.Equal(-0.1332881761240011763e-2, rbpn[1], TOLERANCE_14);
            Assert.Equal(-0.5790767434730085751e-3, rbpn[2], TOLERANCE_14);
            Assert.Equal(0.1332858254308954658e-2, rbpn[3], TOLERANCE_14);
            Assert.Equal(0.9999991109044505577, rbpn[4], TOLERANCE_12);
            Assert.Equal(-0.4097782710396580452e-4, rbpn[5], TOLERANCE_14);
            Assert.Equal(0.5791308472168152904e-3, rbpn[6], TOLERANCE_14);
            Assert.Equal(0.4020595661591500259e-4, rbpn[7], TOLERANCE_14);
            Assert.Equal(0.9999998314954572304, rbpn[8], TOLERANCE_12);
        }

        [Fact]
        public void Pn00b()
        {
            const double TOLERANCE_12 = 1e-12;
            const double TOLERANCE_14 = 1e-14;
            const double TOLERANCE_16 = 1e-16;

            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn00b(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9632552291148362783e-5, dpsi, TOLERANCE_12);
            Assert.Equal(0.4063197106621159367e-4, deps, TOLERANCE_12);
            Assert.Equal(0.4090791789404229916, epsa, TOLERANCE_12);

            Assert.Equal(0.9999999999999942498, rb[0], TOLERANCE_12);
            Assert.Equal(-0.7078279744199196626e-7, rb[1], TOLERANCE_16);
            Assert.Equal(0.8056217146976134152e-7, rb[2], TOLERANCE_16);
            Assert.Equal(0.7078279477857337206e-7, rb[3], TOLERANCE_16);
            Assert.Equal(0.9999999999999969484, rb[4], TOLERANCE_12);
            Assert.Equal(0.3306041454222136517e-7, rb[5], TOLERANCE_16);
            Assert.Equal(-0.8056217380986972157e-7, rb[6], TOLERANCE_16);
            Assert.Equal(-0.3306040883980552500e-7, rb[7], TOLERANCE_16);
            Assert.Equal(0.9999999999999962084, rb[8], TOLERANCE_12);

            Assert.Equal(0.9999989300532289018, rp[0], TOLERANCE_12);
            Assert.Equal(-0.1341647226791824349e-2, rp[1], TOLERANCE_14);
            Assert.Equal(-0.5829880927190296547e-3, rp[2], TOLERANCE_14);
            Assert.Equal(0.1341647231069759008e-2, rp[3], TOLERANCE_14);
            Assert.Equal(0.9999990999908750433, rp[4], TOLERANCE_12);
            Assert.Equal(-0.3837444441583715468e-6, rp[5], TOLERANCE_14);
            Assert.Equal(0.5829880828740957684e-3, rp[6], TOLERANCE_14);
            Assert.Equal(-0.3984203267708834759e-6, rp[7], TOLERANCE_14);
            Assert.Equal(0.9999998300623538046, rp[8], TOLERANCE_12);

            Assert.Equal(0.9999989300052243993, rbp[0], TOLERANCE_12);
            Assert.Equal(-0.1341717990239703727e-2, rbp[1], TOLERANCE_14);
            Assert.Equal(-0.5829075749891684053e-3, rbp[2], TOLERANCE_14);
            Assert.Equal(0.1341718013831739992e-2, rbp[3], TOLERANCE_14);
            Assert.Equal(0.9999990998959191343, rbp[4], TOLERANCE_12);
            Assert.Equal(-0.3505759733565421170e-6, rbp[5], TOLERANCE_14);
            Assert.Equal(0.5829075206857717883e-3, rbp[6], TOLERANCE_14);
            Assert.Equal(-0.4315219955198608970e-6, rbp[7], TOLERANCE_14);
            Assert.Equal(0.9999998301093036269, rbp[8], TOLERANCE_12);

            Assert.Equal(0.9999999999536069682, rn[0], TOLERANCE_12);
            Assert.Equal(0.8837746144871248011e-5, rn[1], TOLERANCE_14);
            Assert.Equal(0.3831488838252202945e-5, rn[2], TOLERANCE_14);
            Assert.Equal(-0.8837590456632304720e-5, rn[3], TOLERANCE_14);
            Assert.Equal(0.9999999991354692733, rn[4], TOLERANCE_12);
            Assert.Equal(-0.4063198798559591654e-4, rn[5], TOLERANCE_14);
            Assert.Equal(-0.3831847930134941271e-5, rn[6], TOLERANCE_14);
            Assert.Equal(0.4063195412258168380e-4, rn[7], TOLERANCE_14);
            Assert.Equal(0.9999999991671806225, rn[8], TOLERANCE_12);

            Assert.Equal(0.9999989440499982806, rbpn[0], TOLERANCE_12);
            Assert.Equal(-0.1332880253640849194e-2, rbpn[1], TOLERANCE_14);
            Assert.Equal(-0.5790760898731091166e-3, rbpn[2], TOLERANCE_14);
            Assert.Equal(0.1332856746979949638e-2, rbpn[3], TOLERANCE_14);
            Assert.Equal(0.9999991109064768883, rbpn[4], TOLERANCE_12);
            Assert.Equal(-0.4097740555723081811e-4, rbpn[5], TOLERANCE_14);
            Assert.Equal(0.5791301929950208873e-3, rbpn[6], TOLERANCE_14);
            Assert.Equal(0.4020553681373720832e-4, rbpn[7], TOLERANCE_14);
            Assert.Equal(0.9999998314958529887, rbpn[8], TOLERANCE_12);
        }

        [Fact]
        public void Pn06()
        {
            // Arrange
            double dpsi = -0.9632552291149335877e-5;
            double deps = 0.4063197106621141414e-4;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];
            double epsa = 0.0;

            // Act
            Sofa.Pn06(2400000.5, 53736.0, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090789763356509926, epsa, 12);

            // Assert - check key matrix elements
            Assert.Equal(0.9999999999999942497, rb[0], 12);
            Assert.Equal(-0.7078368960971557145e-7, rb[1], 14);
            Assert.Equal(0.8056213977613185606e-7, rb[2], 14);

            Assert.Equal(0.7078368694637674333e-7, rb[3], 14);
            Assert.Equal(0.9999999999999969484, rb[4], 12);
            Assert.Equal(0.3305943742989134124e-7, rb[5], 14);

            Assert.Equal(-0.8056214211620056792e-7, rb[6], 14);
            Assert.Equal(-0.3305943172740586950e-7, rb[7], 14);
            Assert.Equal(0.9999999999999962084, rb[8], 12);


            Assert.Equal(0.9999989300536854831, rp[0], 12);
            Assert.Equal(-0.1341646886204443795e-2, rp[1], 14);
            Assert.Equal(-0.5829880933488627759e-3, rp[2], 14);

            Assert.Equal(0.1341646890569782183e-2, rp[3], 14);
            Assert.Equal(0.9999990999913319321, rp[4], 12);
            Assert.Equal(-0.3835944216374477457e-6, rp[5], 14);

            Assert.Equal(0.5829880833027867368e-3, rp[6], 14);
            Assert.Equal(-0.3985701514686976112e-6, rp[7], 14);
            Assert.Equal(0.9999998300623534950, rp[8], 12);

            Assert.Equal(0.9999989300056797893, rbp[0], 12);
            Assert.Equal(-0.1341717650545059598e-2, rbp[1], 14);
            Assert.Equal(-0.5829075756493728856e-3, rbp[2], 14);

            Assert.Equal(0.1341717674223918101e-2, rbp[3], 14);
            Assert.Equal(0.9999990998963748448, rbp[4], 12);
            Assert.Equal(-0.3504269280170069029e-6, rbp[5], 14);

            Assert.Equal(0.5829075211461454599e-3, rbp[6], 14);
            Assert.Equal(-0.4316708436255949093e-6, rbp[7], 14);
            Assert.Equal(0.9999998301093032943, rbp[8], 12);

            Assert.Equal(0.9999999999536069682, rn[0], 12);
            Assert.Equal(0.8837746921149881914e-5, rn[1], 14);
            Assert.Equal(0.3831487047682968703e-5, rn[2], 14);

            Assert.Equal(-0.8837591232983692340e-5, rn[3], 14);
            Assert.Equal(0.9999999991354692664, rn[4], 12);
            Assert.Equal(-0.4063198798558931215e-4, rn[5], 14);

            Assert.Equal(-0.3831846139597250235e-5, rn[6], 14);
            Assert.Equal(0.4063195412258792914e-4, rn[7], 14);
            Assert.Equal(0.9999999991671806293, rn[8], 12);

            Assert.Equal(0.9999989440504506688, rbpn[0], 12);
            Assert.Equal(-0.1332879913170492655e-2, rbpn[1], 14);
            Assert.Equal(-0.5790760923225655753e-3, rbpn[2], 14);

            Assert.Equal(0.1332856406595754748e-2, rbpn[3], 14);
            Assert.Equal(0.9999991109069366795, rbpn[4], 12);
            Assert.Equal(-0.4097725651142641812e-4, rbpn[5], 14);

            Assert.Equal(0.5791301952321296716e-3, rbpn[6], 14);
            Assert.Equal(0.4020538796195230577e-4, rbpn[7], 14);
            Assert.Equal(0.9999998314958576778, rbpn[8], 12);
        }

        [Fact]
        public void Pn06a()
        {
            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090789763356509926, epsa, 12);
            Assert.Equal(-0.9630912025820308797e-5, dpsi, 12);
            Assert.Equal(0.4063238496887249798e-4, deps, 12);
        }

        [Fact]
        public void Pn06a_2()
        {
            const double TOLERANCE = 1e-12;
            // Act
            double dpsi = 0, deps = 0, epsa = 0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];
            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);
            // Assert
            Assert.Equal(-0.9630912025820308797e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063238496887249798e-4, deps, TOLERANCE);
        }

        [Fact]
        public void Pnm00a()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm00a(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832793134257, rbpn[0], 12);
            Assert.Equal(0.8372384254137809439e-3, rbpn[1], 14);
            Assert.Equal(0.3639684306407150645e-3, rbpn[2], 14);
            Assert.Equal(0.9999999329094390695, rbpn[8], 12);
        }

        [Fact]
        public void Pnm00b()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm00b(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832776208280, rbpn[0], 12);
            Assert.Equal(0.8372401264429654837e-3, rbpn[1], 14);
            Assert.Equal(0.3639691681450271771e-3, rbpn[2], 14);
        }

        [Fact]
        public void Pnm06a()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm06a(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832794205484, rbpn[0], 12);
            Assert.Equal(0.8372382772630962111e-3, rbpn[1], 14);
            Assert.Equal(0.3639684771140623099e-3, rbpn[2], 14);
        }

        [Fact]
        public void Pnm80()
        {
            // Arrange
            double[] rmatpn = new double[9];

            // Act
            Sofa.Pnm80(2400000.5, 50123.9999, rmatpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995831934611169, rmatpn[0], 12);
            Assert.Equal(0.8373654045728124011e-3, rmatpn[1], 14);
            Assert.Equal(0.3639121916933106191e-3, rmatpn[2], 14);
        }

        [Fact]
        public void Pom00()
        {
            // Arrange
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;
            double sp = -0.1367174580728891460e-10;
            double[] rpom = new double[9];

            // Act
            Sofa.Pom00(xp, yp, sp, rpom);

            // Assert - check key matrix elements
            Assert.Equal(0.9999999999999674721, rpom[0], 12);
            Assert.Equal(-0.1367174580728846989e-10, rpom[1], 15);
            Assert.Equal(0.2550602379999972345e-6, rpom[2], 15);
        }

        [Fact]
        public void Ppp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] apb = new double[3];

            Sofa.Ppp(a, b, apb);

            Assert.Equal(3.0, apb[0], 12);
            Assert.Equal(5.0, apb[1], 12);
            Assert.Equal(7.0, apb[2], 12);
        }

        [Fact]
        public void Ppsp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double s = 5.0;
            double[] b = { 1.0, 3.0, 4.0 };
            double[] apsb = new double[3];

            Sofa.Ppsp(a, s, b, apsb);

            Assert.Equal(7.0, apsb[0], 12);
            Assert.Equal(17.0, apsb[1], 12);
            Assert.Equal(23.0, apsb[2], 12);
        }

        [Fact]
        public void Pr00()
        {
            // Arrange
            double dpsipr = 0.0;
            double depspr = 0.0;

            // Act
            Sofa.Pr00(2400000.5, 53736, ref dpsipr, ref depspr);

            // Assert
            Assert.Equal(-0.8716465172668347629e-7, dpsipr, 15);
            Assert.Equal(-0.7342018386722813087e-8, depspr, 15);
        }

        [Fact]
        public void Prec76()
        {
            // Arrange
            double zeta = 0.0;
            double z = 0.0;
            double theta = 0.0;

            // Act
            Sofa.Prec76(2400000.5, 33282.0, 2400000.5, 51544.0, ref zeta, ref z, ref theta);

            // Assert
            Assert.Equal(0.5588961642000161243e-2, zeta, 12);
            Assert.Equal(0.5589922365870680624e-2, z, 12);
            Assert.Equal(0.4858945471687296760e-2, theta, 12);
        }

        [Fact]
        public void Pv2p()
        {
            double[] pv = new double[] { 0.3, 1.2, -2.5, -0.5, 3.1, 0.9 };
            double[] p = new double[3];

            Sofa.Pv2p(pv, p);

            Assert.Equal(0.3, p[0], 0);
            Assert.Equal(1.2, p[1], 0);
            Assert.Equal(-2.5, p[2], 0);
        }

        [Fact]
        public void Pv2s()
        {
            double[] pv = new double[] { -0.4514964673880165, 0.03093394277342585, 0.05594668105108779, 1.292270850663260e-5, 2.652814182060692e-6, 2.568431853930293e-6 };
            double theta = 0, phi = 0, r = 0, td = 0, pd = 0, rd = 0;

            Sofa.Pv2s(pv, ref theta, ref phi, ref r, ref td, ref pd, ref rd);

            Assert.Equal(3.073185307179586515, theta, 12);
            Assert.Equal(0.1229999999999999992, phi, 12);
            Assert.Equal(0.4559999999999999757, r, 12);
            Assert.Equal(-0.7800000000000000364e-5, td, 15);
            Assert.Equal(0.9010000000000001639e-5, pd, 15);
            Assert.Equal(-0.1229999999999999832e-4, rd, 15);
        }

        [Fact]
        public void Pvdpv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 6.0, 0.0, 4.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 0.0, 2.0, 8.0 };
            double[] adb = new double[2];

            Sofa.Pvdpv(a, b, adb);

            Assert.Equal(20.0, adb[0], 12);
            Assert.Equal(50.0, adb[1], 12);
        }

        [Fact]
        public void Pvm()
        {
            double[] pv = new double[] { 0.3, 1.2, -2.5, 0.45, -0.25, 1.1 };
            double r = 0, s = 0;

            Sofa.Pvm(pv, ref r, ref s);

            Assert.Equal(2.789265136196270604, r, 12);
            Assert.Equal(1.214495780149111922, s, 12);
        }

        [Fact]
        public void Pvmpv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 5.0, 6.0, 3.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 3.0, 2.0, 1.0 };
            double[] amb = new double[6];

            Sofa.Pvmpv(a, b, amb);

            Assert.Equal(1.0, amb[0], 12);
            Assert.Equal(-1.0, amb[1], 12);
            Assert.Equal(-1.0, amb[2], 12);
            Assert.Equal(2.0, amb[3], 12);
            Assert.Equal(4.0, amb[4], 12);
            Assert.Equal(2.0, amb[5], 12);
        }

        [Fact]
        public void Pvppv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 5.0, 6.0, 3.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 3.0, 2.0, 1.0 };
            double[] apb = new double[6];

            Sofa.Pvppv(a, b, apb);

            Assert.Equal(3.0, apb[0], 12);
            Assert.Equal(5.0, apb[1], 12);
            Assert.Equal(7.0, apb[2], 12);
            Assert.Equal(8.0, apb[3], 12);
            Assert.Equal(8.0, apb[4], 12);
            Assert.Equal(4.0, apb[5], 12);
        }

        [Fact]
        public void Pvstar()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            int j = Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.1686756e-1, ra, 12);
            Assert.Equal(-1.093989828, dec, 12);
            Assert.Equal(-0.1783235160000472788e-4, pmr, 15);
            Assert.Equal(0.2336024047000619347e-5, pmd, 15);
            Assert.Equal(0.74723, px, 12);
            Assert.Equal(-21.60000010107306010, rv, 11);
        }

        [Fact]
        public void Pvtob()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(4225081.367071159207, pv[0], 5);
            Assert.Equal(3681943.215856198144, pv[1], 5);
            Assert.Equal(3041149.399241260785, pv[2], 5);
            Assert.Equal(-268.4915389365998787, pv[3], 9);
            Assert.Equal(308.0977983288903123, pv[4], 9);
            Assert.Equal(0, pv[5], 0);
        }

        [Fact]
        public void Pvu()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] upv = new double[6];

            // Act
            Sofa.Pvu(dt, pv, upv);

            // Assert
            Assert.Equal(126656.7598605317105, upv[0], 6);
            Assert.Equal(2118.531271155726332, upv[1], 8);
            Assert.Equal(-245216.5048590656190, upv[2], 6);
            Assert.Equal(pv[3], upv[3], 12);
            Assert.Equal(pv[4], upv[4], 12);
            Assert.Equal(pv[5], upv[5], 12);
        }

        [Fact]
        public void Pvup()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] p = new double[3];

            // Act
            Sofa.Pvup(dt, pv, p);

            // Assert
            Assert.Equal(126656.7598605317105, p[0], 6);
            Assert.Equal(2118.531271155726332, p[1], 8);
            Assert.Equal(-245216.5048590656190, p[2], 6);
        }

        [Fact]
        public void Pvxpv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 6.0, 0.0, 4.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 0.0, 2.0, 8.0 };
            double[] axb = new double[6];

            Sofa.Pvxpv(a, b, axb);

            Assert.Equal(-1.0, axb[0], 12);
            Assert.Equal(-5.0, axb[1], 12);
            Assert.Equal(4.0, axb[2], 12);
            Assert.Equal(-2.0, axb[3], 12);
            Assert.Equal(-36.0, axb[4], 12);
            Assert.Equal(22.0, axb[5], 12);
        }

        [Fact]
        public void Pxp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] axb = new double[3];

            Sofa.Pxp(a, b, axb);

            Assert.Equal(-1.0, axb[0], 12);
            Assert.Equal(-5.0, axb[1], 12);
            Assert.Equal(4.0, axb[2], 12);
        }

        [Fact]
        public void Refco()
        {
            double phpa = 800.0;
            double tc = 10.0;
            double rh = 0.9;
            double wl = 0.4;
            double refa = 0, refb = 0;

            Sofa.Refco(phpa, tc, rh, wl, ref refa, ref refb);

            Assert.Equal(0.2264949956241415009e-3, refa, 15);
            Assert.Equal(-0.2598658261729343970e-6, refb, 15);
        }

        [Fact]
        public void Rm2v()
        {
            double[] r = new double[] { 0.00, -0.80, -0.60, 0.80, -0.36, 0.48, 0.60, 0.48, -0.64 };
            double[] w = new double[9];

            Sofa.Rm2v(r, w);

            Assert.Equal(0.0, w[0], 12);
            Assert.Equal(1.413716694115406957, w[1], 12);
            Assert.Equal(-1.884955592153875943, w[2], 12);
        }

        [Fact]
        public void Rv2m()
        {
            double[] w = { 0.0, 1.41371669, -1.88495559 };
            double[] r = new double[9];

            Sofa.Rv2m(w, r);

            Assert.Equal(-0.7071067782221119905, r[0], 14);
            Assert.Equal(-0.5656854276809129651, r[1], 14);
            Assert.Equal(-0.4242640700104211225, r[2], 14);
            Assert.Equal(0.5656854276809129651, r[3], 14);
            Assert.Equal(-0.0925483394532274246, r[4], 14);
            Assert.Equal(-0.8194112531408833269, r[5], 14);
            Assert.Equal(0.4242640700104211225, r[6], 14);
            Assert.Equal(-0.8194112531408833269, r[7], 14);
            Assert.Equal(0.3854415612311154341, r[8], 14);
        }

        [Fact]
        public void Rx()
        {
            double phi = 0.3456789;
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };

            Sofa.Rx(phi, r);

            Assert.Equal(2.0, r[0], 0);
            Assert.Equal(3.0, r[1], 0);
            Assert.Equal(2.0, r[2], 0);
            Assert.Equal(3.839043388235612460, r[3], 12);
            Assert.Equal(3.237033249594111899, r[4], 12);
            Assert.Equal(4.516714379005982719, r[5], 12);
            Assert.Equal(1.806030415924501684, r[6], 12);
            Assert.Equal(3.085711545336372503, r[7], 12);
            Assert.Equal(3.687721683977873065, r[8], 12);
        }

        [Fact]
        public void Rxp()
        {
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] rp = new double[3];

            Sofa.Rxp(r, p, rp);

            Assert.Equal(5.1, rp[0], 12);
            Assert.Equal(3.9, rp[1], 12);
            Assert.Equal(7.1, rp[2], 12);
        }

        [Fact]
        public void Rxpv()
        {
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] pv = new double[] { 0.2, 1.5, 0.1, 1.5, 0.2, 0.1 };
            double[] rpv = new double[6];

            Sofa.Rxpv(r, pv, rpv);

            Assert.Equal(5.1, rpv[0], 12);
            Assert.Equal(3.9, rpv[1], 12);
            Assert.Equal(7.1, rpv[2], 12);
            Assert.Equal(3.8, rpv[3], 12);
            Assert.Equal(5.2, rpv[4], 12);
            Assert.Equal(5.8, rpv[5], 12);
        }

        [Fact]
        public void Rxr()
        {
            double[] a = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] b = new double[] { 1.0, 2.0, 2.0, 4.0, 1.0, 1.0, 3.0, 0.0, 1.0 };
            double[] atb = new double[9];

            Sofa.Rxr(a, b, atb);

            Assert.Equal(20.0, atb[0], 12);
            Assert.Equal(7.0, atb[1], 12);
            Assert.Equal(9.0, atb[2], 12);
            Assert.Equal(20.0, atb[3], 12);
            Assert.Equal(8.0, atb[4], 12);
            Assert.Equal(11.0, atb[5], 12);
            Assert.Equal(34.0, atb[6], 12);
            Assert.Equal(10.0, atb[7], 12);
            Assert.Equal(15.0, atb[8], 12);
        }

        [Fact]
        public void Ry()
        {
            double theta = 0.3456789;
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };

            Sofa.Ry(theta, r);

            Assert.Equal(0.8651847818978159930, r[0], 12);
            Assert.Equal(1.467194920539316554, r[1], 12);
            Assert.Equal(0.1875137911274457342, r[2], 12);
            Assert.Equal(3, r[3], 12);
            Assert.Equal(2, r[4], 12);
            Assert.Equal(3, r[5], 12);
            Assert.Equal(3.500207892850427330, r[6], 12);
            Assert.Equal(4.779889022262298150, r[7], 12);
            Assert.Equal(5.381899160903798712, r[8], 12);
        }

        [Fact]
        public void Rz()
        {
            double psi = 0.3456789;
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };

            Sofa.Rz(psi, r);

            Assert.Equal(2.898197754208926769, r[0], 12);
            Assert.Equal(3.500207892850427330, r[1], 12);
            Assert.Equal(2.898197754208926769, r[2], 12);
            Assert.Equal(2.144865911309686813, r[3], 12);
            Assert.Equal(0.865184781897815993, r[4], 12);
            Assert.Equal(2.144865911309686813, r[5], 12);
            Assert.Equal(3.0, r[6], 12);
            Assert.Equal(4.0, r[7], 12);
            Assert.Equal(5.0, r[8], 12);
        }

        [Fact]
        public void S00()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;

            // Act
            double s = Sofa.S00(2400000.5, 53736.0, x, y);

            // Assert
            Assert.Equal(-0.1220036263270905693e-7, s, 15);
        }

        [Fact]
        public void S00a()
        {
            // Arrange & Act
            double s = Sofa.S00a(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340684448919163584e-7, s, 15);
        }

        [Fact]
        public void S00b()
        {
            // Arrange & Act
            double s = Sofa.S00b(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340695782951026584e-7, s, 15);
        }

        [Fact]
        public void S06()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;

            // Act
            double s = Sofa.S06(2400000.5, 53736.0, x, y);

            // Assert
            Assert.Equal(-0.1220032213076463117e-7, s, 15);
        }

        [Fact]
        public void S06a()
        {
            // Arrange & Act
            double s = Sofa.S06a(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340680437291812383e-7, s, 15);
        }

        [Fact]
        public void S2c()
        {
            double[] c = new double[3];

            Sofa.S2c(3.0123, -0.999, c);

            Assert.Equal(-0.5366267667260523906, c[0], 12);
            Assert.Equal(0.0697711109765145365, c[1], 12);
            Assert.Equal(-0.8409302618566214041, c[2], 12);
        }

        [Fact]
        public void S2p()
        {
            double[] p = new double[3];

            Sofa.S2p(-3.21, 0.123, 0.456, p);

            Assert.Equal(-0.4514964673880165228, p[0], 12);
            Assert.Equal(0.0309339427734258688, p[1], 12);
            Assert.Equal(0.0559466810510877933, p[2], 12);
        }

        [Fact]
        public void S2pv()
        {
            double[] pv = new double[6];

            Sofa.S2pv(-3.21, 0.123, 0.456, -7.8e-6, 9.01e-6, -1.23e-5, pv);

            Assert.Equal(-0.4514964673880165228, pv[0], 12);
            Assert.Equal(0.0309339427734258688, pv[1], 12);
            Assert.Equal(0.0559466810510877933, pv[2], 12);
            Assert.Equal(0.1292270850663260170e-4, pv[3], 15);
            Assert.Equal(0.2652814182060691422e-5, pv[4], 15);
            Assert.Equal(0.2568431853930292259e-5, pv[5], 15);
        }

        [Fact]
        public void S2xpv()
        {
            // Arrange
            double s1 = 2.0;
            double s2 = 3.0;
            double[] pv = new double[6] { 0.3, 1.2, -2.5, 0.5, 2.3, -0.4 };
            double[] spv = new double[6];

            // Act
            Sofa.S2xpv(s1, s2, pv, spv);

            // Assert
            Assert.Equal(0.6, spv[0], 12);
            Assert.Equal(2.4, spv[1], 12);
            Assert.Equal(-5.0, spv[2], 12);
            Assert.Equal(1.5, spv[3], 12);
            Assert.Equal(6.9, spv[4], 12);
            Assert.Equal(-1.2, spv[5], 12);
        }

        [Fact]
        public void Sepp()
        {
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };

            double s = Sofa.Sepp(a, b);

            Assert.Equal(2.860391919024660768, s, 12);
        }

        [Fact]
        public void Seps()
        {
            double al = 1.0;
            double ap = 0.1;
            double bl = 0.2;
            double bp = -3.0;

            double s = Sofa.Seps(al, ap, bl, bp);

            Assert.Equal(2.346722016996998842, s, 14);
        }

        [Fact]
        public void Sp00()
        {
            // Arrange & Act
            double sp = Sofa.Sp00(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.6216698469981019309e-11, sp, 12);
        }

        [Fact]
        public void Starpm()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            int j = Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                                2400000.5, 50083.0, 2400000.5, 53736.0,
                                ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.01668919069414256149, ra2, 13);
            Assert.Equal(-1.093966454217127897, dec2, 13);
            Assert.Equal(-0.1783662682153176524e-4, pmr2, 15);
            Assert.Equal(0.2338092915983989595e-5, pmd2, 15);
            Assert.Equal(0.7473533835317719243, px2, 13);
            Assert.Equal(-21.59905170476417175, rv2, 11);
        }

        [Fact]
        public void Starpv()
        {
            // Arrange
            double ra = 0.01686756;
            double dec = -1.093989828;
            double pmr = -1.78323516e-5;
            double pmd = 2.336024047e-6;
            double px = 0.74723;
            double rv = -21.6;
            double[] pv = new double[6];

            // Act
            int j = Sofa.Starpv(ra, dec, pmr, pmd, px, rv, pv);

            // Assert
            Assert.Equal(0, j);
#if NET8_0_OR_GREATER
            Assert.Equal(126668.5912743160601, pv[0], 10, MidpointRounding.ToPositiveInfinity);
#else
            Assert.Equal(126668.5912743160601, pv[0], 10, MidpointRounding.AwayFromZero);
#endif
            Assert.Equal(2136.792716839935195, pv[1], 10);
            Assert.Equal(-245251.2339876830091, pv[2], 10);
            Assert.Equal(-0.4051854008955659551e-2, pv[3], 13);
            Assert.Equal(-0.6253919754414777970e-2, pv[4], 15);
            Assert.Equal(0.1189353714588109341e-1, pv[5], 13);
        }

        [Fact]
        public void Sxp()
        {
            double s = 2.0;
            double[] p = { 0.3, 1.2, -2.5 };
            double[] sp = new double[3];

            Sofa.Sxp(s, p, sp);

            Assert.Equal(0.6, sp[0], 0);
            Assert.Equal(2.4, sp[1], 0);
            Assert.Equal(-5.0, sp[2], 0);
        }

        [Fact]
        public void Sxpv()
        {
            double s = 2.0;
            double[] pv = new double[] { 0.3, 1.2, -2.5, 0.5, 3.2, -0.7 };
            double[] spv = new double[6];

            Sofa.Sxpv(s, pv, spv);

            Assert.Equal(0.6, spv[0], 0);
            Assert.Equal(2.4, spv[1], 0);
            Assert.Equal(-5.0, spv[2], 0);
            Assert.Equal(1.0, spv[3], 0);
            Assert.Equal(6.4, spv[4], 0);
            Assert.Equal(-1.4, spv[5], 0);
        }

        [Fact]
        public void Taitt()
        {
            double t1 = 0.0;
            double t2 = 0.0;
            // TaiTT tests
            short j = Sofa.Taitt(2453750.5, 0.892482639, ref t1, ref t2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, t1, 6);
            Assert.Equal(0.892855139, t2, 12);
        }

        [Fact]
        public void Taiut1()
        {
            // Arrange
            double u1 = 0.0;
            double u2 = 0.0;

            // Act
            int j = Sofa.Taiut1(2453750.5, 0.892482639, -32.6659, ref u1, ref u2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, u1, 6);
            Assert.Equal(0.8921045614537037037, u2, 12);
        }

        [Fact]
        public void Taiutc()
        {
            double u1 = 0.0;
            double u2 = 0.0;

            // TaiUtc tests
            short j = Sofa.Taiutc(2453750.5, 0.892482639, ref u1, ref u2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, u1, 6);
            Assert.Equal(0.8921006945555555556, u2, 12);
        }

        [Fact]
        public void Tcbtdb()
        {
            double b1 = 0, b2 = 0;
            int j = Sofa.Tcbtdb(2453750.5, 0.893019599, ref b1, ref b2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, b1, 6);
            Assert.Equal(0.8928551362746343397, b2, 12);
        }

        [Fact]
        public void Tcgtt()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Tcgtt(2453750.5, 0.892862531, ref tt1, ref tt2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551387488816828, tt2, 12);
        }

        [Fact]
        public void Tdbtcb()
        {
            double tcb1 = 0, tcb2 = 0;
            int j = Sofa.Tdbtcb(2453750.5, 0.892855137, ref tcb1, ref tcb2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tcb1, 6);
            Assert.Equal(0.8930195997253656716, tcb2, 12);
        }

        [Fact]
        public void Tdbtt()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Tdbtt(2453750.5, 0.892855137, 0.0, ref tt1, ref tt2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551393263888889, tt2, 8);
        }

        [Fact]
        public void Tf2a()
        {
            double a = 0;
            // Tf2a tests
            short j = Sofa.Tf2a('+', 4, 58, 20.2, ref a);

            Assert.Equal(0, j);
            Assert.Equal(1.301739278189537429, a, 12);
        }

        [Fact]
        public void Tf2a_BadValue()
        {
            double rad = 0;
            int j = Sofa.Tf2a('+', 25, 0, 0, ref rad);
            Assert.Equal(1, j);
        }

        [Fact]
        public void Tf2a_NegativeSign()
        {
            double rad = 0;
            int j = Sofa.Tf2a('-', 4, 58, 20.2, ref rad);
            Assert.Equal(0, j);
            Assert.Equal(-1.301739278189537429, rad, 12);
        }

        [Fact]
        public void Tf2a_PositiveSign()
        {
            double rad = 0;
            int j = Sofa.Tf2a('+', 4, 58, 20.2, ref rad);
            Assert.Equal(0, j);
            Assert.Equal(1.301739278189537429, rad, 12);
        }

        [Fact]
        public void Tf2d()
        {
            double days = 0;
            int j = Sofa.Tf2d(' ', 23, 55, 10.9, ref days);
            Assert.Equal(0, j);
            Assert.Equal(0.9966539351851851852, days, 12);
        }

        [Fact]
        public void Tpors()
        {
            double xi = -0.03;
            double eta = 0.07;
            double ra = 1.3;
            double dec = 1.5;
            double az1 = 0, bz1 = 0, az2 = 0, bz2 = 0;
            int j = Sofa.Tpors(xi, eta, ra, dec, ref az1, ref bz1, ref az2, ref bz2);
            Assert.Equal(1.736621577783208748, az1, 13);
            Assert.Equal(1.436736561844090323, bz1, 13);
            Assert.Equal(4.004971075806584490, az2, 13);
            Assert.Equal(1.565084088476417917, bz2, 13);
            Assert.Equal(2, j);
        }

        [Fact]
        public void Tporv()
        {
            double[] v = new double[] { 0.0, 0.0, 0.0 };
            double[] vz1 = new double[3];
            double[] vz2 = new double[3];
            double xi = -0.03;
            double eta = 0.07;
            double ra = 1.3;
            double dec = 1.5;
            Sofa.S2c(ra, dec, v);
            int j = Sofa.Tporv(xi, eta, v, vz1, vz2);
            Assert.Equal(2, j);
            Assert.Equal(-0.02206252822366888610, vz1[0], 15);
            Assert.Equal(0.1318251060359645016, vz1[1], 14);
            Assert.Equal(0.9910274397144543895, vz1[2], 14);
            Assert.Equal(-0.003712211763801968173, vz2[0], 15);
            Assert.Equal(-0.004341519956299836813, vz2[1], 15);
            Assert.Equal(0.9999836852110587012, vz2[2], 14);
        }

        [Fact]
        public void Tpsts()
        {
            double a = 0, b = 0;
            Sofa.Tpsts(-0.03, 0.07, 2.3, 1.5, ref a, ref b);
            Assert.Equal(0.7596127167359629775, a, 14);
            Assert.Equal(1.540864645109263028, b, 13);
        }

        [Fact]
        public void Tpstv()
        {
            double[] vz = new double[3];
            double[] v = new double[3];
            Sofa.S2c(2.3, 1.5, vz);
            Sofa.Tpstv(-0.03, 0.07, vz, v);
            Assert.Equal(0.02170030454907376677, v[0], 15);
            Assert.Equal(0.02060909590535367447, v[1], 15);
            Assert.Equal(0.9995520806583523804, v[2], 14);
        }

        [Fact]
        public void Tpxes()
        {
            double xi = 0, eta = 0;
            int j = Sofa.Tpxes(1.3, 1.55, 2.3, 1.5, ref xi, ref eta);
            Assert.Equal(0, j);
            Assert.Equal(-0.01753200983236980595, xi, 15);
            Assert.Equal(0.05962940005778712891, eta, 15);
        }

        [Fact]
        public void Tpxev()
        {
            double[] v = new double[3];
            double[] vz = new double[3];
            double xi = 0, eta = 0;
            Sofa.S2c(1.3, 1.55, v);
            Sofa.S2c(2.3, 1.5, vz);
            int j = Sofa.Tpxev(v, vz, ref xi, ref eta);
            Assert.Equal(0, j);
            Assert.Equal(-0.01753200983236980595, xi, 15);
            Assert.Equal(0.05962940005778712891, eta, 15);
        }

        [Fact]
        public void Tr()
        {
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] rt = new double[9];

            Sofa.Tr(r, rt);

            Assert.Equal(2.0, rt[0], 0);
            Assert.Equal(3.0, rt[1], 0);
            Assert.Equal(3.0, rt[2], 0);
            Assert.Equal(3.0, rt[3], 0);
            Assert.Equal(2.0, rt[4], 0);
            Assert.Equal(4.0, rt[5], 0);
            Assert.Equal(2.0, rt[6], 0);
            Assert.Equal(3.0, rt[7], 0);
            Assert.Equal(5.0, rt[8], 0);
        }

        [Fact]
        public void Trxp()
        {
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] trp = new double[3];

            Sofa.Trxp(r, p, trp);

            Assert.Equal(5.2, trp[0], 12);
            Assert.Equal(4.0, trp[1], 12);
            Assert.Equal(5.4, trp[2], 12);
        }

        [Fact]
        public void Trxpv()
        {
            double[] r = new double[] { 2.0, 3.0, 2.0, 3.0, 2.0, 3.0, 3.0, 4.0, 5.0 };
            double[] pv = new double[] { 0.2, 1.5, 0.1, 1.5, 0.2, 0.1 };
            double[] trpv = new double[6];

            Sofa.Trxpv(r, pv, trpv);

            Assert.Equal(5.2, trpv[0], 12);
            Assert.Equal(4.0, trpv[1], 12);
            Assert.Equal(5.4, trpv[2], 12);
            Assert.Equal(3.9, trpv[3], 12);
            Assert.Equal(5.3, trpv[4], 12);
            Assert.Equal(4.1, trpv[5], 12);
        }

        [Fact]
        public void Tttai()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Tttai(2453750.5, 0.892482639, ref tai1, ref tai2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.892110139, tai2, 9);
        }

        [Fact]
        public void Tttcg()
        {
            double tcg1 = 0, tcg2 = 0;
            int j = Sofa.Tttcg(2453750.5, 0.892482639, ref tcg1, ref tcg2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tcg1, 6);
            Assert.NotEqual(0.892482639, tcg2);
        }

        [Fact]
        public void Tttdb()
        {
            double tdb1 = 0, tdb2 = 0;
            int j = Sofa.Tttdb(2453750.5, 0.892855139, -0.000201, ref tdb1, ref tdb2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tdb1, 6);
            Assert.Equal(0.8928551366736111111, tdb2, 12);
        }

        [Fact]
        public void Ttut1()
        {
            double ut11 = 0, ut12 = 0;
            int j = Sofa.Ttut1(2453750.5, 0.892855139, 64.8499, ref ut11, ref ut12);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, ut11, 6);
            Assert.Equal(0.8921045614537037037, ut12, 12);
        }

        [Fact]
        public void Ut1tai()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Ut1tai(2453750.5, 0.892104561, -32.6659, ref tai1, ref tai2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.8924826385462962963, tai2, 12);
        }

        [Fact]
        public void Ut1tt()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Ut1tt(2453750.5, 0.892104561, 64.8499, ref tt1, ref tt2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551385462962963, tt2, 12);
        }

        [Fact]
        public void Ut1utc()
        {
            double utc1 = 0, utc2 = 0;
            int j = Sofa.Ut1utc(2453750.5, 0.892104561, 0.3341, ref utc1, ref utc2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, utc1, 6);
            Assert.Equal(0.8921006941018518519, utc2, 12);
        }

        [Fact]
        public void Utctai()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Utctai(2453750.5, 0.892100694, ref tai1, ref tai2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.8924826384444444444, tai2, 12);
        }

        [Fact]
        public void Utcut1()
        {
            double ut11 = 0, ut12 = 0;
            int j = Sofa.Utcut1(2453750.5, 0.892100694, 0.3341, ref ut11, ref ut12);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, ut11, 6);
            Assert.Equal(0.8921045608981481481, ut12, 12);
        }

        [Fact]
        public void Xy06()
        {
            double x = 0, y = 0;
            Sofa.Xy06(2400000.5, 53736.0, ref x, ref y);
            Assert.Equal(0.5791308486706010975e-3, x, 15);
            Assert.Equal(0.4020579816732958141e-4, y, 15);
        }

        [Fact]
        public void Xys00a()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys00a(2400000.5, 53736.0, ref x, ref y, ref s);
            Assert.Equal(0.5791308472168152904e-3, x, 14);
            Assert.Equal(0.4020595661591500259e-4, y, 15);
            Assert.Equal(-0.1220040848471549623e-7, s, 15);
        }

        [Fact]
        public void Xys00b()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys00b(2400000.5, 53736.0, ref x, ref y, ref s);
            Assert.Equal(0.5791301929950208873e-3, x, 14);
            Assert.Equal(0.4020553681373720832e-4, y, 15);
            Assert.Equal(-0.1220027377285083189e-7, s, 15);
        }

        [Fact]
        public void Xys06a()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys06a(2400000.5, 53736.0, ref x, ref y, ref s);
            Assert.Equal(0.5791308482835292617e-3, x, 14);
            Assert.Equal(0.4020580099454020310e-4, y, 15);
            Assert.Equal(-0.1220032294164579896e-7, s, 15);
        }

        [Fact]
        public void Zp()
        {
            double[] p = { 0.3, 1.2, -2.5 };

            Sofa.Zp(p);

            Assert.Equal(0.0, p[0], 0);
            Assert.Equal(0.0, p[1], 0);
            Assert.Equal(0.0, p[2], 0);
        }

        [Fact]
        public void Zpv()
        {
            double[] pv = new double[] { 0.3, 1.2, -2.5, -0.5, 3.1, 0.9 };

            Sofa.Zpv(pv);

            Assert.Equal(0.0, pv[0], 0);
            Assert.Equal(0.0, pv[1], 0);
            Assert.Equal(0.0, pv[2], 0);
            Assert.Equal(0.0, pv[3], 0);
            Assert.Equal(0.0, pv[4], 0);
            Assert.Equal(0.0, pv[5], 0);
        }

        [Fact]
        public void Zr()
        {
            double[] r = new double[] { 2.0, 3.0, 3.0, 3.0, 2.0, 4.0, 2.0, 3.0, 5.0 };

            Sofa.Zr(r);

            for (int i = 0; i < 9; i++)
            {
                Assert.Equal(0.0, r[i], 0);
            }
        }

        #endregion

    }
}

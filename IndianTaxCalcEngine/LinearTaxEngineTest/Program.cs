using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using TaxEngine;

namespace LinearTaxEngineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                TestFile("TestString.txt");
            }
            else
            {
                switch (args[0].ToUpper())
                {
                    case "QD":
                        QuickNDirty();
                        break;
                    case "PERF":
                        Performance();
                        break;
                    default:
                        TestFile(args[0]);
                        break;
                }
            }
        }

        private static void Performance()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string text;
            using (StreamReader sr = File.OpenText("PerfTestString1.txt"))
            {
                text = sr.ReadToEnd();
            }
            stopwatch.Start();
            Result result = LinearTaxEngine.Calculate(text);
            stopwatch.Stop();
            Console.WriteLine(string.Format("PerfTestString1.txt cost {0}ms. X = {1}", stopwatch.ElapsedMilliseconds, result.X));

            using (StreamReader sr = File.OpenText("PerfTestString2.txt"))
            {
                text = sr.ReadToEnd();
            }
            stopwatch.Reset();
            stopwatch.Start();
            result = LinearTaxEngine.Calculate(text);
            stopwatch.Stop();
            Console.WriteLine(string.Format("PerfTestString2.txt cost {0}ms. X = {1}", stopwatch.ElapsedMilliseconds, result.X));

            using (StreamReader sr = File.OpenText("PerfTestString3.txt"))
            {
                text = sr.ReadToEnd();
            }
            stopwatch.Reset();
            stopwatch.Start();
            result = LinearTaxEngine.Calculate(text);
            stopwatch.Stop();
            Console.WriteLine(string.Format("PerfTestString3.txt cost {0}ms. X = {1}", stopwatch.ElapsedMilliseconds, result.X));
        }

        private static void QuickNDirty()
        {
            string BED = "BED";
            string VAT = "VAT";
            string testfile = "TestString1.txt";
            Result result1 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result1.X.Value, 826.4462809917355371900826446m);
            AssertValue(result1[BED].Amount, 115.70247933884297520661157024m);
            AssertValue(result1[VAT].Amount, 57.851239669421487603305785122m);

            testfile = "TestString2.txt";
            Result result2 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result2.X.Value, 820.6611570247933884297520661m);
            AssertValue(result2[BED].Amount, 114.89256198347107438016528925m);
            AssertValue(result2[VAT].Amount, 64.446280991735537190082644627m);

            testfile = "TestString3.txt";
            Result result3 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result3.X.Value, 863.6284571229779092016002783m);
            AssertValue(result3[BED].Amount, 120.90798399721690728822403896m);
            AssertValue(result3[VAT].Amount, 15.463558879805183510175682727m);

            testfile = "TestString4.txt";
            Result result4 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result4.X.Value, 856.6881196729866063663245782m);
            AssertValue(result4[BED].Amount, 133.93633675421812489128544095m);
            AssertValue(result4[VAT].Amount, 9.375543572795268742389980866m);

            testfile = "TestString5.txt";
            Result result5 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result5.X.Value, 850.6001043659766915985388763m);
            AssertValue(result5[BED].Amount, 133.08401461123673682379544268m);
            AssertValue(result5[VAT].Amount, 16.315881022786571577665680988m);

            testfile = "TestString6.txt";
            Result result6 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result6.X.Value, 801.7871782259386784718806362m);
            AssertValue(result6[BED].Amount, 126.25020495163141498606328907m);
            AssertValue(result6[VAT].Amount, 71.962616822429906542056074771m);

            testfile = "TestString7.txt";
            Result result7 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result7.X.Value, 665.4000m);
            AssertValue(result7[BED].Amount, 280.00m);
            AssertValue(result7[VAT].Amount, 54.6000m);

            testfile = "TestString8.txt";
            Result result8 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result8.X.Value, 1708.992868324926074099843451m);
            AssertValue(result8[BED].Amount, 239.25900156548965037397808314m);
            AssertValue(result8[VAT].Amount, 51.748130109584275526178465820m);

            testfile = "TestString9.txt";
            Result result9 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result9.X.Value, 1662.2807017543859649122807018m);
            AssertValue(result9[BED].Amount, 232.71929824561403508771929825m);
            AssertValue(result9[VAT].Amount, 105.00m);

            testfile = "TestString10.txt";
            Result result10 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result10.X.Value, 640.18691588785046728971962617m);
            AssertValue(result10[BED].Amount, 280.00m);
            AssertValue(result10[VAT].Amount, 79.81308411214953271028037383m);

            testfile = "TestString11.txt";
            Result result11 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result11.X.Value, 525.4000m);
            AssertValue(result11[BED].Amount, 280.00m);
            AssertValue(result11[VAT].Amount, 194.6000m);

            testfile = "TestString12.txt";
            Result result12 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result12.X.Value, 1610.9198229217904574520413182m);
            AssertValue(result12[BED].Amount, 225.52877520905066404328578455m);
            AssertValue(result12[VAT].Amount, 163.55140186915887850467289720m);

            testfile = "TestString13.txt";
            Result result13 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result13.X.Value, 2491.0280373831775700934579439m);
            AssertValue(result13[BED].Amount, 280.00m);
            AssertValue(result13[VAT].Amount, 228.97196261682242990654205607m);

            testfile = "TestString14.txt";
            Result result14 = TestFile(testfile);
            Console.WriteLine(testfile);
            AssertValue(result14.X.Value, 738.31775700934579439252336449m);
            AssertValue(result14[BED].Amount, 210.00m);
            AssertValue(result14[VAT].Amount, 51.682242990654205607476635514m);
        }

        private static Result TestFile(string filepath)
        {
            string text;
            using (StreamReader sr = File.OpenText(filepath))
            {
                text = sr.ReadToEnd();
            }
            Result result = LinearTaxEngine.Calculate(text);
            Console.Write(result.ToString());
            return result;
        }

        private static void AssertValue(decimal actrual, decimal expect)
        {
            if (actrual == expect)
            {
                Console.WriteLine("TRUE");
            }
            else
            {
                Console.WriteLine(string.Format("FALSE. Expected:{0} Actrual:{1} Diff:{2}", expect, actrual, actrual - expect));
            }
        }
    }
}

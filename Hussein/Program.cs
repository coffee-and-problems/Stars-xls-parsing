//ВНИМАНИЕ! Данный код будет работать только под Windows, не пытайтесь запустить его на других системах.
//Совместимисти с .NET Core нет

using System;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sandbox
{
    public class Read_From_Excel
    {
        public static void Main()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@".\gaia_bhr111.xls"); //программа ожадает входной файл в той же папке, где исполнимый файл
            Excel._Worksheet nomadWorksheet = xlWorkbook.Sheets[5];
            Excel._Worksheet inputWorksheet = xlWorkbook.Sheets[3];
            Excel._Worksheet ucacWorksheet = xlWorkbook.Sheets[6];
            Excel._Worksheet apassWorksheet = xlWorkbook.Sheets[7];
            Excel._Worksheet massWorksheet = xlWorkbook.Sheets[4];

            Excel.Range stars = inputWorksheet.UsedRange;

            int rowCount = stars.Rows.Count;
            var x = 10;
            var y = 11;

            var results = new StreamWriter(@".\nstars.txt");
            results.WriteLine("NAME    Nlines    + next lines: flag. band. value. error (comments)");

            for (int i = 1; i <= rowCount; i++) 
            {
                if (stars.Cells[i, x] != null && stars.Cells[i, x].Value2 != null) //полагаeм, если не пусты х, не пусты и у
                {
                    results.WriteLine("bhr111 m{0}  0018     notes here ======================", stars.Cells[i,1].Value2);
                    var stary = stars.Cells[i, x].Value2.ToString() + " " + stars.Cells[i, y].Value2.ToString();
                    FillGaia(i, inputWorksheet, results);
                    FindStarIn(stary, nomadWorksheet, "NOMAD", results);
                    FillZeros("Tycho-2", results); //даже если зведа не найдена в других каталогах Tycho заполнятся нулями
                    FindStarIn(stary, ucacWorksheet, "UCAC4", results);
                    FindStarIn(stary, apassWorksheet, "APASS", results);
                    FindStarIn(stary, massWorksheet, "2MASS", results);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Com объекты должны быть освобождены именно так. Не использовать двух точек, не передавать список!
            Marshal.ReleaseComObject(stars);
            Marshal.ReleaseComObject(inputWorksheet);
            Marshal.ReleaseComObject(nomadWorksheet);
            Marshal.ReleaseComObject(ucacWorksheet);
            Marshal.ReleaseComObject(apassWorksheet);
            Marshal.ReleaseComObject(massWorksheet);

            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            //Теперь все процессы Excel прекращены, появится всплывающее окно с вопросом о сохранении изменений.
            //Так как программа не вносит никаких изменений, можно отвечать "да", можно "нет". Однако, рекомендуется ответ "нет"
            results.Close();
        }

        public static void FindStarIn(string star, Excel._Worksheet thisWorksheet, string catalogName, StreamWriter results)
        {
            Excel.Range stars = thisWorksheet.UsedRange;
            int rowCount = stars.Rows.Count;
            bool found = false;

            for (int i = 1; i <= rowCount; i++)
            {
                if (stars.Cells[i, 2] != null && stars.Cells[i, 2].Value2 != null)
                {
                    if ((stars.Cells[i, 2].Value2.ToString() == star))
                    {
                        WhatWeNeed(i, thisWorksheet, catalogName, results);
                        found = true;
                        break;
                    }
                }
            }
            if (!found) FillZeros(catalogName, results);
        }

        public static void WhatWeNeed(int row, Excel._Worksheet thisWorksheet, string catalogName, StreamWriter results)
        {
            Excel.Range data = thisWorksheet.UsedRange;
            int colCount = data.Columns.Count;
            string[] columns;

            switch (catalogName)
            {
                case "NOMAD":
                    columns = new string[] {"+ Rc "};
                    break;
                case "2MASS":
                    columns = new string[] { "+ J2 ", "+ H2 ", "+ Ks "};
                    break;
                default:
                    columns = new string[] { "+ B ", "+ V ", "+ g1 ", "+ r1 ", "+ i1 " };
                    break;
            }

            var j = 0;
            for (int i = 6; i<=colCount; i+=2)
            {
                results.Write(columns[j]);
                if (data.Cells[row, i] != null && data.Cells[row, i].Value2 != null)
                {
                    results.Write("{0}  ", data.Cells[row, i].Value2.ToString());
                }
                else
                {
                    results.Write("{0}  ", "0.0");
                }
                if (data.Cells[row, i+1] != null && data.Cells[row, i+1].Value2 != null)
                {
                    results.Write("{0}  ", data.Cells[row, i+1].Value2.ToString());
                }
                else
                {
                    results.Write("{0}  ", "0.0");
                }
                if (i == 6)
                    results.WriteLine(catalogName);
                else
                    results.WriteLine();
                j++;
            }
        }

        public static void FillGaia(int row, Excel._Worksheet stars, StreamWriter results)
        {
            results.WriteLine("+Pa {0} {1} GAIA", stars.Cells[row, 17].Value2.ToString(), stars.Cells[row, 18].Value2.ToString());
            results.WriteLine("+G {0} {1}", stars.Cells[row, 19].Value2.ToString(), stars.Cells[row, 20].Value2.ToString());
        }

        public static void FillZeros(string catalogName, StreamWriter results)
        {
            switch (catalogName)
            {
                case "Tycho-2":
                    results.WriteLine("+ BT 0.0  0.0 Tycho-2");
                    results.WriteLine("+ VT 0.0  0.0");
                    break;
                case "NOMAD":
                    results.WriteLine("+Rc 0.0  0.0 NOMAD");
                    break;
                case "2MASS":
                    results.WriteLine("+ J2 0.0  0.0 2MASS");
                    results.WriteLine("+ H2 0.0  0.0");
                    results.WriteLine("+ kS 0.0  0.0");
                    break;
                case "UCAC4":
                    results.WriteLine("+ B 0.0  0.0 UCAC4");
                    results.WriteLine("+ V 0.0  0.0");
                    results.WriteLine("+ g1 0.0  0.0");
                    results.WriteLine("+ r1 0.0  0.0");
                    results.WriteLine("+ i1 0.0  0.0");
                    break;
                case "APASS":
                    results.WriteLine("+ B 0.0  0.0 APASS");
                    results.WriteLine("+ V 0.0  0.0");
                    results.WriteLine("+ g1 0.0  0.0");
                    results.WriteLine("+ r1 0.0  0.0");
                    results.WriteLine("+ i1 0.0  0.0");
                    break;
                default:
                    break;
                    
            }
        }
    }
}
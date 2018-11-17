using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stars_Parser
{
    public class Catalog
    {
        public List<Coordinates> Ra { get; set; }
        public List<Coordinates> Dec { get; set; }

        public string[] MakeCatalogHeader(String file)
        {
            string[] columnsNames = new string[] { null };
            try
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    var header = reader.ReadLine();
                    columnsNames = header.Split(',');
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file could not be found: {0}", file);
                Console.WriteLine("The corresponding catalog will be filled with zeros");
            }
            columnsNames = columnsNames.Select(x => x.ToLower()).ToArray();
            return columnsNames;
        }

        public void GetCoordinates(string file, string[] header)
        {
            var idx = Array.IndexOf(header, "ra dec");
            if (idx == -1) idx = Array.IndexOf(header, "_1");
            if (idx == -1) throw new Exception("No column named RA DEC");

            var Ra = new List<Coordinates>();
            var Dec = new List<Coordinates>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    var filePath = file.Split('\\', '/');
                    var fileName = filePath[filePath.Length - 1];

                    string ra; string dec;
                    if (fileName == "gaia.csv")
                    {
                        ra = filds[idx].Substring(0, 11);
                        dec = filds[idx].Substring(12, 11);
                    }
                    else
                    {
                        ra = filds[idx].Substring(0, 18);
                        dec = filds[idx].Substring(19, 18);
                    }

                    Ra.Add(new Coordinates(ra.Trim()));
                    Dec.Add(new Coordinates(dec.Trim()));
                }
            }
            this.Ra = Ra;
            this.Dec = Dec;
        }
    }

    public static class StarsList
    {
        private static string[] MakeCatalogHeader(string file)
        {
            string[] columnsNames = new string[] { null };
            using (StreamReader reader = new StreamReader(file))
            {
                var header = reader.ReadLine();
                columnsNames = header.Split(',');
            }
            columnsNames = columnsNames.Select(x => x.ToLower()).ToArray();
            return columnsNames;
        }

        public static List<Star> GetStars(String file)
        {
            var header = MakeCatalogHeader(file);

            if (header[0] == null) throw new Exception("No stars to find were given");

            var id = Array.IndexOf(header, "main_id");
            var idx = Array.IndexOf(header, "ra");
            var idxd = Array.IndexOf(header, "dec");

            var starList = new List<Star>();
            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    var a = Int32.Parse(filds[id]);
                    var b = new Coordinates(filds[idx]);
                    var c = new Coordinates(filds[idxd]);
                    starList.Add(new Star(a, b, c));
                }
            }
            return starList;
        }
    }

    public sealed class Gaia : Catalog
    {
        public List<ValueAndError> Pa { get; set; }
        public List<ValueAndError> G { get; set; }

        public Gaia(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var plx = Array.IndexOf(header, "plx");
            var g = Array.IndexOf(header, "gmag");

            if (plx == -1) throw new Exception("No column named plx, in Gaia");
            if (g == -1) throw new Exception("No column named gmag, in Gaia");

            var Pa = new List<ValueAndError>();
            var G = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    Pa.Add(new ValueAndError(filds[plx], filds[plx + 1]));
                    G.Add(new ValueAndError(filds[g], filds[g + 1]));
                }
            }
            this.Pa = Pa;
            this.G = G;
        }

        public Gaia(String path, String file)
        {
            new Gaia(Path.Combine(path, file));
        }
    }

    public sealed class NOMAD : Catalog
    {
        public List<ValueAndError> Rc { get; set; }

        public NOMAD(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var r = Array.IndexOf(header, "rmag");

            if (r == -1) throw new Exception("No column named _r, in nomad");

            var Rc = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    Rc.Add(new ValueAndError(filds[r], "0.0"));
                }
            }
            this.Rc = Rc;
        }
    }

    public sealed class Tycho2 : Catalog
    {
        public List<ValueAndError> BT { get; set; }
        public List<ValueAndError> VT { get; set; }

        public Tycho2(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var bt = Array.IndexOf(header, "bt");
            var vt = Array.IndexOf(header, "vt");

            if (bt == -1) throw new Exception("No column named bt, in Tycho2");
            if (vt == -1) throw new Exception("No column named vt, in Tycho2");

            var BT = new List<ValueAndError>();
            var VT = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    BT.Add(new ValueAndError(filds[bt], filds[bt + 1]));
                    VT.Add(new ValueAndError(filds[vt], filds[vt + 1]));
                }
            }
            this.BT = BT;
            this.VT = VT;
        }

        public Tycho2(String path, String file)
        {
            new Tycho2(Path.Combine(path, file));
        }
    }

    public sealed class TwoMASS : Catalog
    {
        public List<ValueAndError> J2 { get; set; }
        public List<ValueAndError> H2 { get; set; }
        public List<ValueAndError> Ks { get; set; }

        public TwoMASS(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var j = Array.IndexOf(header, "jmag");
            var h = Array.IndexOf(header, "hmag");
            var k = Array.IndexOf(header, "kmag");

            if (j == -1) throw new Exception("No column named jmag, in TwoMASS");
            if (h == -1) throw new Exception("No column named hmag, in TwoMASS");
            if (k == -1) throw new Exception("No column named kmag, in TwoMASS");

            var J2 = new List<ValueAndError>();
            var H2 = new List<ValueAndError>();
            var Ks = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    J2.Add(new ValueAndError(filds[j], filds[j + 1]));
                    H2.Add(new ValueAndError(filds[h], filds[h + 1]));
                    Ks.Add(new ValueAndError(filds[k], filds[k + 1]));
                }
            }
            this.J2 = J2;
            this.H2 = H2;
            this.Ks = Ks;
        }

        public TwoMASS(String path, String file)
        {
            new TwoMASS(Path.Combine(path, file));
        }
    }

    public sealed class UCAC4 : Catalog
    {
        public List<ValueAndError> B { get; set; }
        public List<ValueAndError> V { get; set; }
        public List<ValueAndError> g1 { get; set; }
        public List<ValueAndError> r1 { get; set; }
        public List<ValueAndError> i1 { get; set; }

        public UCAC4(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var b = Array.IndexOf(header, "bmag");
            var v = Array.IndexOf(header, "vmag");
            var g = Array.IndexOf(header, "gmag");
            var r = Array.IndexOf(header, "rmag");
            var i = Array.IndexOf(header, "imag");


            if (b == -1) throw new Exception("No column named bmag, in UCAC4");
            if (v == -1) throw new Exception("No column named vmag, in UCAC4");
            if (g == -1) throw new Exception("No column named gmag, in UCAC4");
            if (r == -1) throw new Exception("No column named rmag, in UCAC4");
            if (i == -1) throw new Exception("No column named imag, in UCAC4");


            var B = new List<ValueAndError>();
            var V = new List<ValueAndError>();
            var g1 = new List<ValueAndError>();
            var r1 = new List<ValueAndError>();
            var i1 = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    B.Add(new ValueAndError(filds[b], filds[b + 1]));
                    V.Add(new ValueAndError(filds[v], filds[v + 1]));
                    g1.Add(new ValueAndError(filds[g], filds[g + 1]));
                    r1.Add(new ValueAndError(filds[r], filds[r + 1]));
                    i1.Add(new ValueAndError(filds[i], filds[i + 1]));
                }
            }
            this.B = B;
            this.V = V;
            this.g1 = g1;
            this.r1 = r1;
            this.i1 = i1;
        }
        
    }

    public sealed class APASS : Catalog
    {
        public List<ValueAndError> V { get; set; }
        public List<ValueAndError> B { get; set; }
        public List<ValueAndError> g1 { get; set; }
        public List<ValueAndError> r1 { get; set; }
        public List<ValueAndError> i1 { get; set; }

        public APASS(String file)
        {
            var header = MakeCatalogHeader(file);
            GetCoordinates(file, header);

            var b = Array.IndexOf(header, "bmag");
            var v = Array.IndexOf(header, "vmag");
            var g = Array.IndexOf(header, "gmag");
            var r = Array.IndexOf(header, "rmag");
            var i = Array.IndexOf(header, "imag");


            if (b == -1) throw new Exception("No column named bmag, in APASS");
            if (v == -1) throw new Exception("No column named vmag, in APASS");
            if (g == -1) throw new Exception("No column named gmag, in APASS");
            if (r == -1) throw new Exception("No column named rmag, in APASS");
            if (i == -1) throw new Exception("No column named imag, in APASS");


            var B = new List<ValueAndError>();
            var V = new List<ValueAndError>();
            var g1 = new List<ValueAndError>();
            var r1 = new List<ValueAndError>();
            var i1 = new List<ValueAndError>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var filds = line.Split(',');
                    B.Add(new ValueAndError(filds[b], filds[b + 1]));
                    V.Add(new ValueAndError(filds[v], filds[v + 1]));
                    g1.Add(new ValueAndError(filds[g], filds[g + 1]));
                    r1.Add(new ValueAndError(filds[r], filds[r + 1]));
                    i1.Add(new ValueAndError(filds[i], filds[i + 1]));
                }
            }
            this.B = B;
            this.V = V;
            this.g1 = g1;
            this.r1 = r1;
            this.i1 = i1;
        }

        public APASS(String path, String file)
        {
            new APASS(Path.Combine(path, file));
        }
    }
}

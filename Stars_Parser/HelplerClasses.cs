using System;
using System.IO;

namespace Stars_Parser
{
    public class Coordinates
    {
        public readonly Int32 Degrees;
        public readonly Int32 Minutes;
        public readonly Double Seconds;

        public Coordinates(String HexCoordinates)
        {
            var coordinates = HexCoordinates.Split(':', ' ');
            if (coordinates.Length != 3) throw new Exception("The star's coordinate was not in HEX format");
            Degrees = Int32.Parse(coordinates[0]);
            Minutes = Int32.Parse(coordinates[1]);
            Seconds = Math.Round(Double.Parse(coordinates[2])); // В качесте второго парамется методу можно передать количество желаемых
                                                                   //знаков после запятой. Например, для трех знаков: Math.Round(Double.Parse(coordinates[2]), 3)
        }

        public override bool Equals(object obj)
        {
            var item = obj as Coordinates;

            return (Degrees == item.Degrees) && (Minutes == item.Minutes) && (Seconds == item.Seconds);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }

    public class Star
    {
        public Int32 id { get; set; }
        public Coordinates RA { get; set; }
        public Coordinates DEC { get; set; }

        public Star(Int32 id, Coordinates Ra, Coordinates Dec)
        {
            this.id = id;
            this.RA = Ra;
            this.DEC = Dec;
        }
    }

    public class ValueAndError
    {
        public Double Value { get; set; }
        public Double Error { get; set; }

        public ValueAndError(String v, String e)
        {
            try
            {
                this.Value = Double.Parse(v);
            }
            catch (System.FormatException)
            {
                this.Value = 0;
            }
            try
            {
                this.Error = Double.Parse(e);
            }
            catch (System.FormatException)
            {
                this.Error = 0;
            }
        }
    }

    public static class ResultsWriter
    {
        public static void FillZeros(string catalogName, StreamWriter results)
        {
            switch (catalogName)
            {
                case "GAIA":
                    results.WriteLine("+ Pa 0.0  0.0 GAIA");
                    results.WriteLine("+ G 0.0  0.0");
                    break;
                case "Tycho-2":
                    results.WriteLine("+ BT 0.0  0.0 Tycho-2");
                    results.WriteLine("+ VT 0.0  0.0");
                    break;
                case "NOMAD":
                    results.WriteLine("+ Rc 0.0  0.0 NOMAD");
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
                    results.WriteLine("+ V 0.0  0.0 APASS");
                    results.WriteLine("+ B 0.0  0.0");
                    results.WriteLine("+ g1 0.0  0.0");
                    results.WriteLine("+ r1 0.0  0.0");
                    results.WriteLine("+ i1 0.0  0.0");
                    break;
                default:
                    break;
            }
        }

        public static void WriteGaia(Star star, Gaia gaia, StreamWriter results)
        {
            if (gaia == null)
            {
                FillZeros("GAIA", results);
            }
            else
            {
                string format = "{0} 0.2 {1} ";
                int i = FindStarIndx(star, gaia);
                if (i == -1) FillZeros("GAIA", results);
                else
                {
                    if ((gaia.Pa[i].Error > 0.2) || (gaia.Pa[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ Pa "+ format + " GAIA", gaia.Pa[i].Value, gaia.Pa[i].Error);
                    if ((gaia.G[i].Error > 0.2) || (gaia.G[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ G " + format + " ", gaia.G[i].Value, gaia.G[i].Error);
                }
            }
        }

        public static void WriteNomad(Star star, NOMAD nomad, StreamWriter results)
        {
            if (nomad == null)
            {
                FillZeros("NOMAD", results);
            }
            else
            {
                string format = "{0} 0.3 {1} ";
                int i = FindStarIndx(star, nomad);
                if (i == -1) FillZeros("NOMAD", results);
                else
                {
                    if ((nomad.Rc[i].Error > 0.3) || (nomad.Rc[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ Rc " + format + " NOMAD", nomad.Rc[i].Value, nomad.Rc[i].Error);
                }
            }
        }

        public static void WriteTycho(Star star, Tycho2 tycho, StreamWriter results)
        {
            if (tycho == null)
            {
                FillZeros("Tycho-2", results);
            }
            else
            {
                string format = "{0} 0.05 {1} ";
                int i = FindStarIndx(star, tycho);
                if (i == -1) FillZeros("Tycho-2", results);
                else
                {
                    if ((tycho.BT[i].Error > 0.05) || (tycho.BT[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ BT " + format + " Tycho-2", tycho.BT[i].Value, tycho.BT[i].Error);
                    if ((tycho.VT[i].Error > 0.05) || (tycho.VT[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ VT " + format + " ", tycho.VT[i].Value, tycho.VT[i].Error);
                }
            }
        }

        public static void WriteUcac(Star star, UCAC4 ucac, StreamWriter results)
        {
            if (ucac == null)
            {
                FillZeros("UCAC4", results);
            }
            else
            {
                string format = "{0} 0.08 {1} ";
                int i = FindStarIndx(star, ucac);
                if (i == -1) FillZeros("UCAC4", results);
                else
                {
                    if ((ucac.B[i].Error * 0.001 > 0.008) || (ucac.B[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ B " + format + " UCAC4", ucac.B[i].Value, ucac.B[i].Error * 0.001);
                    if ((ucac.V[i].Error * 0.001 > 0.008) || (ucac.V[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ V " + format + " ", ucac.V[i].Value, ucac.V[i].Error * 0.001);

                    format = "{0} 0.1 {1} ";
                    if ((ucac.g1[i].Error * 0.001 > 0.1) || (ucac.g1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ g1 " + format + " ", ucac.g1[i].Value, ucac.g1[i].Error * 0.001);
                    if ((ucac.r1[i].Error * 0.001 > 0.1) || (ucac.r1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ r1 " + format + " ", ucac.r1[i].Value, ucac.r1[i].Error * 0.001);

                    format = "{0} 0.2 {1} ";
                    if ((ucac.i1[i].Error * 0.001 > 0.2) || (ucac.i1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ i1 " + format + " ", ucac.i1[i].Value, ucac.i1[i].Error * 0.001);
                }
            }
        }

        public static void WriteApass(Star star, APASS apass, StreamWriter results)
        {
            if (apass == null)
            {
                FillZeros("APASS", results);
            }
            else
            {
                int i = FindStarIndx(star, apass);
                if (i == -1) FillZeros("APASS", results);
                else
                {
                    string format = "{0} 0.001 {1} ";
                    if ((apass.V[i].Error * 0.001 > 0.008) || (apass.V[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ V " + format + " APASS", apass.V[i].Value, apass.V[i].Error * 0.001);

                    format = "{0} 0.008 {1} ";
                    if ((apass.B[i].Error * 0.001 > 0.008) || (apass.B[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ B " + format + " ", apass.B[i].Value, apass.B[i].Error * 0.001);
                    if ((apass.g1[i].Error * 0.001 > 0.1) || (apass.g1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ g1 " + format + " ", apass.g1[i].Value, apass.g1[i].Error * 0.001);

                    if ((apass.r1[i].Error * 0.001 > 0.1) || (apass.r1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ r1 " + format + " ", apass.r1[i].Value, apass.r1[i].Error * 0.001);

                    format = "{0} 0.2 {1} ";
                    if ((apass.i1[i].Error * 0.001 > 0.2) || (apass.i1[i].Error == 0))
                    {
                        format = "{0} {1} ";
                    }
                    results.WriteLine("+ i1 " + format + " ", apass.i1[i].Value, apass.i1[i].Error * 0.001);
                }
            }
        }

        public static void Write2mass(Star star, TwoMASS mass, StreamWriter results)
        {
            if (mass == null)
            {
                FillZeros("2MASS", results);
            }
            else
            {
                string format = "{0} {1} {2}";
                int i = FindStarIndx(star, mass);
                if (i == -1) FillZeros("2MASS", results);
                else
                {
                    results.WriteLine("+ J2 " + format + " 2MASS", mass.J2[i].Value, mass.J2[i].Error * 2, mass.J2[i].Error);
                    results.WriteLine("+ H2 " + format + " ", mass.H2[i].Value, mass.H2[i].Error * 2, mass.J2[i].Error);
                    results.WriteLine("+ Ks " + format + " ", mass.Ks[i].Value, mass.Ks[i].Error * 2, mass.J2[i].Error);
                }
            }
        }

        private static int FindStarIndx(Star star, Catalog catalog)
        {
            int index = -1;

            for (int j = 0; j < catalog.Ra.Count; j++)
            {
                if (star.RA.Equals(catalog.Ra[j]))
                {
                    if (star.DEC.Equals(catalog.Dec[j]))
                    {
                        index = j;
                        break;
                    }
                }
            }
            return index;
        }
    }
}

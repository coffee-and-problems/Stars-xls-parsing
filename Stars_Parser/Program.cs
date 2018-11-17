using System;
using System.IO;

namespace Stars_Parser
{
    static class Program
    {
        static void Main(string[] args)
        {
            var current_dir = Environment.CurrentDirectory;
            var path = Path.Combine(current_dir, "data");

            var stars = StarsList.GetStars(Path.Combine(path, "listofstars.csv")); 

            Tycho2 tycho; Gaia gaia; NOMAD nomad; APASS apass; TwoMASS mass; UCAC4 ucac;

            #region
            if (File.Exists(Path.Combine(path, "gaia.csv")))
            {
                gaia = new Gaia(Path.Combine(path, "gaia.csv"));
            }
            else
            {
                gaia = null;
            }

            if (File.Exists(Path.Combine(path, "nomad.csv")))
            {
                nomad = new NOMAD(Path.Combine(path, "nomad.csv"));
            }
            else
            {
                nomad = null;
            }

            if (File.Exists(Path.Combine(path, "apass.csv")))
            {
                apass = new APASS(Path.Combine(path, "apass.csv"));
            }
            else
            {
                apass = null;
            }

            if (File.Exists(Path.Combine(path, "2mass.csv")))
            {
                mass = new TwoMASS(Path.Combine(path, "2mass.csv"));
            }
            else
            {
                mass = null;
            }

            if (File.Exists(Path.Combine(path, "ucac4.csv")))
            {
                ucac = new UCAC4(Path.Combine(path, "ucac4.csv"));
            }
            else
            {
                ucac = null;
            }

            if (File.Exists(Path.Combine(path, "tycho-2")))
            {
                tycho = new Tycho2(Path.Combine(path, "tycho-2"));
            }
            else
            {
                tycho = null;
            }
            #endregion

            using (FileStream fileStream = File.Create(@".\nstars.txt"))
            {
                using (BufferedStream buffered = new BufferedStream(fileStream))
                {
                    using (var results = new StreamWriter(buffered))
                    {
                        foreach (var star in stars)
                        {
                            results.WriteLine("bhr111 m{0,0:D3}  0018     notes here ======================", star.id);
                            if (gaia == null) ResultsWriter.FillZeros("GAIA", results);
                            else ResultsWriter.WriteGaia(star, gaia, results);
                            if (nomad == null) ResultsWriter.FillZeros("NOMAD", results);
                            else ResultsWriter.WriteNomad(star, nomad, results);
                            if (tycho == null) ResultsWriter.FillZeros("Tycho-2", results);
                            else ResultsWriter.WriteTycho(star, tycho, results);
                            if (ucac == null) ResultsWriter.FillZeros("UCAC4", results);
                            else ResultsWriter.WriteUcac(star, ucac, results);
                            if (apass == null) ResultsWriter.FillZeros("APASS", results);
                            else ResultsWriter.WriteApass(star, apass, results);
                            if (mass == null) ResultsWriter.FillZeros("2MASS", results);
                            else ResultsWriter.Write2mass(star, mass, results);
                        }
                    }
                }
            }
        }
    }
}

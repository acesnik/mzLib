using NUnit.Framework;
using Proteomics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UsefulProteomicsDatabases;
using System;
using Proteomics.ProteolyticDigestion;

namespace Test
{
    [TestFixture]
    public class VariantProteinTests
    {
        //[Test]
        //public void MakeVariantFasta()
        //{
        //    List<Protein> proteins = ProteinDbLoader.LoadProteinXML(@"E:\ProjectsActive\JurkatProteogenomics\180413\combined_1-trimmed-pair1Aligned.sortedByCoord.outProcessed.out.fixedQuals.split.concat.sorted.snpEffAnnotated.protein.xml",
        //        true, DecoyType.None, null, false, null, out var un);
        //    List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();
        //    List<ProteinWithAppliedVariants> variantProteins2 = variantProteins.Select(p =>
        //        new ProteinWithAppliedVariants(
        //            p.BaseSequence,
        //            new Protein(p.Protein.BaseSequence, p.Protein.Accession, p.Protein.Organism, p.Protein.GeneNames.ToList(), p.Protein.OneBasedPossibleLocalizedModifications, p.Protein.ProteolysisProducts.ToList(), p.Protein.Name + string.Join(",", p.AppliedSequenceVariations.Select(d => d.Description)), p.Protein.FullName + string.Join(",", p.AppliedSequenceVariations.Select(d => d.Description)), p.IsDecoy, p.IsContaminant, p.DatabaseReferences.ToList(), p.SequenceVariations.ToList(), p.DisulfideBonds.ToList(), p.DatabaseFilePath),
        //            p.AppliedSequenceVariations, p.Individual)
        //        ).ToList();
        //    ProteinDbWriter.WriteFastaDatabase(variantProteins2.OfType<Protein>().ToList(), @"E:\ProjectsActive\Spritz\mmTesting\variantproteins.fasta", "|");
        //}

        [Test]
        public void VariantXml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVar.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();

            Assert.AreEqual(5, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(1, variantProteins.Count); // there is only one unique amino acid change
            Assert.AreNotEqual(proteins.First().BaseSequence, variantProteins.First().BaseSequence);
            Assert.AreEqual('C', proteins.First().BaseSequence[116]);
            Assert.AreEqual('Y', variantProteins.First().BaseSequence[116]);
            Assert.AreNotEqual(proteins.First().Name, variantProteins.First().Name);
            Assert.AreNotEqual(proteins.First().FullName, variantProteins.First().FullName);
            Assert.AreNotEqual(proteins.First().Accession, variantProteins.First().Accession);

            List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), null, null)).ToList();
        }

        [Test]
        public void VariantLongDeletionXml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVarLongDeletion.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();

            Assert.AreEqual(2, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(1, variantProteins.Count); // there is only one unique amino acid change
            Assert.AreNotEqual(proteins.First().BaseSequence, variantProteins.First().BaseSequence);
            Assert.AreEqual('A', proteins.First().BaseSequence[226]);
            Assert.AreNotEqual('A', variantProteins.First().BaseSequence[226]);
            Assert.AreNotEqual(proteins.First().Name, variantProteins.First().Name);
            Assert.AreNotEqual(proteins.First().FullName, variantProteins.First().FullName);
            Assert.AreNotEqual(proteins.First().Accession, variantProteins.First().Accession);

            List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), null, null)).ToList();
        }

        [Test]
        public void VariantSymbolWeirdnessXml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVarSymbolWeirdness.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();

            Assert.AreEqual(12, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(13, variantProteins.Count); // there is only one unique amino acid change
            Assert.AreEqual(1, variantProteins.Where(v => v.BaseSequence == proteins.First().BaseSequence).Count());
            Assert.AreNotEqual(proteins.First().Name, variantProteins.First().Name);
            Assert.AreNotEqual(proteins.First().FullName, variantProteins.First().FullName);
            Assert.AreNotEqual(proteins.First().Accession, variantProteins.First().Accession);

            List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), null, null)).ToList();
        }

        [Test]
        public void VariantSymbolWeirdness2Xml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVarSymbolWeirdness2.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();
            Assert.AreEqual(1, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(2, variantProteins.Count); // there is only one unique amino acid change
            Assert.AreEqual(1, variantProteins.Where(v => v.BaseSequence == proteins.First().BaseSequence).Count());
            Assert.AreEqual('R', proteins.First().BaseSequence[2386]);
            Assert.AreNotEqual('H', variantProteins.First().BaseSequence[2386]);
            Assert.AreNotEqual(proteins.First().Name, variantProteins.First().Name);
            Assert.AreNotEqual(proteins.First().FullName, variantProteins.First().FullName);
            Assert.AreNotEqual(proteins.First().Accession, variantProteins.First().Accession);
            List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), null, null)).ToList();
        }

        //[Test]
        //public void VariantXmlBig()
        //{
        //    string bigfile = @"E:\ProjectsActive\JurkatProteogenomics\180413\combined_1-trimmed-pair1Aligned.sortedByCoord.outProcessed.out.fixedQuals.split.concat.sorted.snpEffAnnotated.protein.xml";
        //    List<Protein> proteins = ProteinDbLoader.LoadProteinXML(bigfile, true, DecoyType.None, null, false, null, out var un);
        //    List<ProteinWithAppliedVariants> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();
        //    var mods = PtmListLoader.ReadModsFromFile(@"E:\source\repos\MetaMorpheus\GUI\bin\Debug\Mods\aListOfmods.txt");
        //    List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), mods.Where(m => m.modificationType == "Common Fixed").OfType<ModificationWithMass>().ToList(), mods.Where(m => m.modificationType == "Common Variable").OfType<ModificationWithMass>().ToList())).ToList();
        //    var p78318 = variantProteins.Where(vp => vp.Accession.StartsWith("P78318"));
        //}
    }
}
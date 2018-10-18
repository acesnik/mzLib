﻿using NUnit.Framework;
using Proteomics;
using Proteomics.ProteolyticDigestion;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UsefulProteomicsDatabases;
using System.Data;

namespace Test
{
    [TestFixture]
    public class VariantProteinTests
    {
        [Test]
        public void VariantXml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVar.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<Protein> variantProteins = proteins.SelectMany(p => p.GetVariantProteins(4)).ToList();

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
        public static void LoadSeqVarModifications()
        {
            var proteins = ProteinDbLoader.LoadProteinXML(Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "oblm2.xml"), true,
                DecoyType.None, null, false, null, out var unknownModifications);
            Assert.AreEqual(0, proteins[0].OneBasedPossibleLocalizedModifications.Count);
            Assert.AreEqual(1, proteins[0].SequenceVariations.Count());
            Assert.AreEqual(1, proteins[0].SequenceVariations.First().OneBasedModifications.Count);
            var variant = proteins[0].GetVariantProteins(4)[0];
            Assert.AreEqual(1, variant.OneBasedPossibleLocalizedModifications.Count);

            ProteinDbWriter.WriteXmlDatabase(null, proteins, Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "oblm2rewrite.xml"));
            proteins = ProteinDbLoader.LoadProteinXML(Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "oblm2rewrite.xml"), true,
                DecoyType.None, null, false, null, out unknownModifications);
            Assert.AreEqual(0, proteins[0].OneBasedPossibleLocalizedModifications.Count);
            Assert.AreEqual(1, proteins[0].SequenceVariations.Count());
            Assert.AreEqual(1, proteins[0].SequenceVariations.First().OneBasedModifications.Count);
            variant = proteins[0].GetVariantProteins(4)[0];
            Assert.AreEqual(1, variant.OneBasedPossibleLocalizedModifications.Count);
        }

        [Test]
        public static void HomozygousVariants()
        {
            var proteins = ProteinDbLoader.LoadProteinXML(Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "HomozygousHLA.xml"), true,
                DecoyType.None, null, false, null, out var unknownModifications);
            Assert.AreEqual(1, proteins.Count);
            Assert.AreEqual(63, proteins[0].SequenceVariations.Count()); // 63 variants, some redundant
            Assert.AreEqual(21, proteins[0].SequenceVariations.Select(v => v.SimpleString()).Distinct().Count()); // 21 unique changes
            Assert.AreEqual(1, proteins[0].GetVariantProteins().Count);
            var variantProteins = proteins[0].GetVariantProteins();
            int i = 0;
        }

        [Test]
        public void VariantLongDeletionXml()
        {
            string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "DatabaseTests", "SeqVarLongDeletion.xml");
            List<Protein> proteins = ProteinDbLoader.LoadProteinXML(file, true, DecoyType.None, null, false, null, out var un);

            List<Protein> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();

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
            Assert.AreEqual(12, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(2, proteins.First().SequenceVariations.Count(v => v.Description.Heterozygous.Any(kv => kv.Value)));

            List<Protein> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();

            Assert.AreEqual(1, variantProteins.Count); // Should be 2^2 from combinitorics of heterozygous, but the giant indels overwrite them
            Assert.AreEqual(0, variantProteins.Where(v => v.BaseSequence == proteins.First().BaseSequence).Count()); // Homozygous variations are included
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

            List<Protein> variantProteins = proteins.SelectMany(p => p.GetVariantProteins()).ToList();
            Assert.AreEqual(1, proteins.First().SequenceVariations.Count());
            Assert.AreEqual(2, variantProteins.Count); // there is only one unique amino acid change
            Assert.AreEqual(1, variantProteins.Where(v => v.BaseSequence == proteins.First().BaseSequence).Count());
            var variantProteinRef = variantProteins.First();
            var variantProteinAlt = variantProteins.Last();
            Assert.AreEqual('R', proteins.First().BaseSequence[2386]);
            Assert.AreEqual('R', variantProteinRef.BaseSequence[2386]);
            Assert.AreEqual('H', variantProteinAlt.BaseSequence[2386]);
            Assert.AreEqual(proteins.First().Name, variantProteinRef.Name);
            Assert.AreNotEqual(proteins.First().Name, variantProteinAlt.Name);
            Assert.AreEqual(proteins.First().FullName, variantProteinRef.FullName);
            Assert.AreNotEqual(proteins.First().FullName, variantProteinAlt.FullName);
            Assert.AreEqual(proteins.First().Accession, variantProteinRef.Accession);
            Assert.AreNotEqual(proteins.First().Accession, variantProteinAlt.Accession);
            List<PeptideWithSetModifications> peptides = variantProteins.SelectMany(vp => vp.Digest(new DigestionParams(), null, null)).ToList();
        }
    }
}
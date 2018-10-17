﻿using Proteomics.ProteolyticDigestion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proteomics
{
    public class Protein
    {
        /// <summary>
        /// Protein. Filters out modifications that do not match their amino acid target site.
        /// </summary>
        public Protein(string sequence, string accession, string organism = null, List<Tuple<string, string>> geneNames = null,
            IDictionary<int, List<Modification>> oneBasedModifications = null, List<ProteolysisProduct> proteolysisProducts = null,
            string name = null, string fullName = null, bool isDecoy = false, bool isContaminant = false, List<DatabaseReference> databaseReferences = null,
            List<SequenceVariation> sequenceVariations = null, List<DisulfideBond> disulfideBonds = null, List<SpliceSite> spliceSites = null, string databaseFilePath = null)
        {
            // Mandatory
            BaseSequence = sequence;
            Accession = accession;

            Name = name;
            Organism = organism;
            FullName = fullName;
            IsDecoy = isDecoy;
            IsContaminant = isContaminant;
            DatabaseFilePath = databaseFilePath;

            GeneNames = geneNames ?? new List<Tuple<string, string>>();
            ProteolysisProducts = proteolysisProducts ?? new List<ProteolysisProduct>();
            SequenceVariations = sequenceVariations ?? new List<SequenceVariation>();
            OriginalModifications = oneBasedModifications ?? new Dictionary<int, List<Modification>>();
            if (oneBasedModifications != null)
            {
                OneBasedPossibleLocalizedModifications = SelectValidOneBaseMods(oneBasedModifications);
            }
            else
            {
                OneBasedPossibleLocalizedModifications = new Dictionary<int, List<Modification>>();
            }
            DatabaseReferences = databaseReferences ?? new List<DatabaseReference>();
            DisulfideBonds = disulfideBonds ?? new List<DisulfideBond>();
            SpliceSites = spliceSites ?? new List<SpliceSite>();
        }

        public IDictionary<int, List<Modification>> OneBasedPossibleLocalizedModifications { get; private set; }

        /// <summary>
        /// The list of gene names consists of tuples, where Item1 is the type of gene name, and Item2 is the name. There may be many genes and names of a certain type produced when reading an XML protein database.
        /// </summary>
        public IEnumerable<Tuple<string, string>> GeneNames { get; }

        public string Accession { get; }
        public string BaseSequence { get; }
        public string Organism { get; }
        public bool IsDecoy { get; }
        public IEnumerable<SequenceVariation> SequenceVariations { get; }
        public IEnumerable<DisulfideBond> DisulfideBonds { get; }
        public IEnumerable<SpliceSite> SpliceSites { get; }
        public IEnumerable<ProteolysisProduct> ProteolysisProducts { get; }
        public IEnumerable<DatabaseReference> DatabaseReferences { get; }
        public string DatabaseFilePath { get; }

        public int Length
        {
            get
            {
                return BaseSequence.Length;
            }
        }

        public string FullDescription
        {
            get
            {
                return Accession + "|" + Name + "|" + FullName;
            }
        }

        public string Name { get; }
        public string FullName { get; }
        public bool IsContaminant { get; }
        internal IDictionary<int, List<Modification>> OriginalModifications { get; set; }

        public char this[int zeroBasedIndex]
        {
            get
            {
                return BaseSequence[zeroBasedIndex];
            }
        }

        /// <summary>
        /// Formats a string for a UniProt fasta header. See https://www.uniprot.org/help/fasta-headers.
        /// Note that the db field isn't very applicable here, so mz is placed in to denote written by mzLib.
        /// </summary>
        public string GetUniProtFastaHeader()
        {
            var n = GeneNames.FirstOrDefault();
            string geneName = n == null ? "" : n.Item2;
            return string.Format("mz|{0}|{1} {2} OS={3} GN={4}", Accession, Name, FullName, Organism, geneName);
        }

        /// <summary>
        /// Formats a string for an ensembl header
        /// </summary>
        public string GetEnsemblFastaHeader()
        {
            return string.Format("{0} {1}", Accession, FullName);
        }

        public override bool Equals(object obj)
        {
            Protein p = obj as Protein;

            return p != null
                && p.BaseSequence == BaseSequence
                && p.Name == Name
                && p.Accession == Accession
                && p.FullName == FullName
                && p.FullDescription == FullDescription
                && p.IsContaminant == IsContaminant
                && p.IsDecoy == IsDecoy
                && p.Organism == Organism
                && p.GeneNames.OrderBy(x => x).SequenceEqual(GeneNames.OrderBy(x => x))
                && p.SequenceVariations.OrderBy(x => x).SequenceEqual(SequenceVariations.OrderBy(x => x))
                && p.DatabaseReferences.OrderBy(x => x).SequenceEqual(DatabaseReferences.OrderBy(x => x))
                && p.DisulfideBonds.OrderBy(x => x).SequenceEqual(DisulfideBonds.OrderBy(x => x))
                && p.ProteolysisProducts.OrderBy(x => x).SequenceEqual(ProteolysisProducts.OrderBy(x => x))
                && p.OneBasedPossibleLocalizedModifications.OrderBy(x => x.Key).SelectMany(x => $"{x.Key.ToString()}{string.Join("", x.Value.OrderBy(mod => mod).Select(mod => mod.ToString()))}")
                    .SequenceEqual(OneBasedPossibleLocalizedModifications.OrderBy(x => x.Key).SelectMany(x => $"{x.Key.ToString()}{string.Join("", x.Value.OrderBy(mod => mod).Select(mod => mod.ToString()))}"));
           
        }

        public override int GetHashCode()
        {
            int hash = BaseSequence.GetHashCode() ^
                (Name ?? "").GetHashCode() ^
                (Accession ?? "").GetHashCode() ^
                (FullName ?? "").GetHashCode() ^
                (FullDescription ?? "").GetHashCode() ^
                IsContaminant.GetHashCode() ^
                IsDecoy.GetHashCode() ^
                (Organism ?? "").GetHashCode();

            foreach (Tuple<string, string> gn in GeneNames)
                hash ^= gn.GetHashCode();

            foreach (SequenceVariation sv in SequenceVariations)
                hash ^= sv.GetHashCode();

            foreach (DatabaseReference dr in DatabaseReferences)
                hash ^= dr.GetHashCode();

            foreach (DisulfideBond db in DisulfideBonds)
                hash ^= db.GetHashCode();

            foreach (ProteolysisProduct pp in ProteolysisProducts)
                hash ^= pp.GetHashCode();

            foreach (var kv in OneBasedPossibleLocalizedModifications)
            {
                foreach (Modification mod in kv.Value)
                {
                    hash ^= kv.Key.GetHashCode() ^ mod.GetHashCode();
                }
            }

            return hash;
        }

        /// <summary>
        /// Gets peptides for digestion of a protein
        /// </summary>
        public IEnumerable<PeptideWithSetModifications> Digest(DigestionParams digestionParams, IEnumerable<Modification> allKnownFixedModifications,
            List<Modification> variableModifications)
        {
            ProteinDigestion digestion = new ProteinDigestion(digestionParams, allKnownFixedModifications, variableModifications);
            return digestionParams.SearchModeType == CleavageSpecificity.Semi ? digestion.SpeedySemiSpecificDigestion(this) : digestion.Digestion(this);
        }

        /// <summary>
        /// Gets proteins with applied variants from this protein
        /// </summary>
        public List<ProteinWithAppliedVariants> GetVariantProteins(int maxAllowedVariantsForCombinitorics = 4)
        {
            ProteinWithAppliedVariants variantProtein = new ProteinWithAppliedVariants(BaseSequence, this, null, ProteolysisProducts, OneBasedPossibleLocalizedModifications, null);
            return variantProtein.ApplyVariants(SequenceVariations, maxAllowedVariantsForCombinitorics);
        }

        /// <summary>
        /// Restore all modifications that were read in, including those that did not match their target amino acid.
        /// </summary>
        public void RestoreUnfilteredModifications()
        {
            OneBasedPossibleLocalizedModifications = OriginalModifications;
        }

        /// <summary>
        /// Filters modifications that do not match their target amino acid.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private IDictionary<int, List<Modification>> SelectValidOneBaseMods(IDictionary<int, List<Modification>> dict)
        {
            Dictionary<int, List<Modification>> validModDictionary = new Dictionary<int, List<Modification>>();
            foreach (KeyValuePair<int, List<Modification>> entry in dict)
            {
                List<Modification> validMods = new List<Modification>();
                foreach (Modification m in entry.Value)
                {
                    //mod must be valid mod and the motif of the mod must be present in the protein at the specified location
                    if (m.ValidModification && ModificationLocalization.ModFits(m, BaseSequence, 0, BaseSequence.Length, entry.Key))
                    {
                        validMods.Add(m);
                    }
                }

                if (validMods.Any())
                {
                    if (validModDictionary.Keys.Contains(entry.Key))
                    {
                        validModDictionary[entry.Key].AddRange(validMods);
                    }
                    else
                    {
                        validModDictionary.Add(entry.Key, validMods);
                    }
                }
            }
            return validModDictionary;
        }
    }
}
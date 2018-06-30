﻿using Proteomics;
using Proteomics.ProteolyticDigestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulProteomicsDatabases
{
    public static class DecoyProteinGenerator
    {
        /// <summary>
        /// Generates decoys for a list of proteins
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="decoyType"></param>
        /// <param name="digestionParams"></param>
        /// <param name="randomSeed">Used when decoy type is shuffle for shuffling the peptides</param>
        /// <returns></returns>
        public static List<Protein> GenerateDecoys(List<Protein> proteins, DecoyType decoyType, DigestionParams digestionParams, int randomSeed)
        {
            if (decoyType == DecoyType.None)
            {
                return new List<Protein>();
            }
            else if (decoyType == DecoyType.Reverse)
            {
                return GenerateReverseDecoys(proteins);
            }
            else if (decoyType == DecoyType.Slide)
            {
                return GenerateSlideDecoys(proteins);
            }
            else if (decoyType == DecoyType.Shuffle)
            {
                return GenerateShuffledDecoys(proteins, digestionParams, randomSeed);
            }
            else
            {
                throw new ArgumentException("Decoy type " + decoyType.ToString() + " is not implemented.");
            }
        }

        /// <summary>
        /// Generates a reverse decoy sequence
        /// </summary>
        /// <param name="protein"></param>
        /// <returns></returns>
        private static List<Protein> GenerateReverseDecoys(List<Protein> proteins)
        {
            List<Protein> decoyProteins = new List<Protein>();
            Parallel.ForEach(proteins, protein =>
            {
                Dictionary<int, List<Modification>> decoyModifications = null;
                char[] sequence_array = protein.BaseSequence.ToCharArray();
                List<DisulfideBond> decoyDisulfides = new List<DisulfideBond>();
                if (protein.BaseSequence.StartsWith("M", StringComparison.Ordinal))
                {
                    // Do not include the initiator methionine in reversal!!!
                    Array.Reverse(sequence_array, 1, protein.BaseSequence.Length - 1);
                    decoyModifications = new Dictionary<int, List<Modification>>(protein.OneBasedPossibleLocalizedModifications.Count);
                    foreach (var kvp in protein.OneBasedPossibleLocalizedModifications)
                    {
                        if (kvp.Key > 1)
                        {
                            decoyModifications.Add(protein.BaseSequence.Length - kvp.Key + 2, kvp.Value);
                        }
                        else if (kvp.Key == 1)
                        {
                            decoyModifications.Add(1, kvp.Value);
                        }
                    }
                }
                else
                {
                    Array.Reverse(sequence_array);
                    decoyModifications = new Dictionary<int, List<Modification>>(protein.OneBasedPossibleLocalizedModifications.Count);
                    foreach (var kvp in protein.OneBasedPossibleLocalizedModifications)
                    {
                        decoyModifications.Add(protein.BaseSequence.Length - kvp.Key + 1, kvp.Value);
                    }
                }
                var reversed_sequence = new string(sequence_array);

                List<ProteolysisProduct> decoyPP = new List<ProteolysisProduct>();
                foreach (ProteolysisProduct pp in protein.ProteolysisProducts)
                {
                    decoyPP.Add(new ProteolysisProduct(protein.BaseSequence.Length - pp.OneBasedEndPosition + 1, protein.BaseSequence.Length - pp.OneBasedBeginPosition, pp.Type));
                }
                foreach (DisulfideBond disulfideBond in protein.DisulfideBonds)
                {
                    decoyDisulfides.Add(new DisulfideBond(protein.BaseSequence.Length - disulfideBond.OneBasedBeginPosition + 2, protein.BaseSequence.Length - disulfideBond.OneBasedEndPosition + 2, "DECOY DISULFIDE BOND: " + disulfideBond.Description));
                }

                List<SequenceVariation> decoyVariations = new List<SequenceVariation>();
                foreach (SequenceVariation sv in protein.SequenceVariations)
                {
                    char[] originalArray = sv.OriginalSequence.ToArray();
                    char[] variationArray = sv.VariantSequence.ToArray();
                    if (sv.OneBasedBeginPosition == 1)
                    {
                        bool orig_init_m = sv.OriginalSequence.StartsWith("M", StringComparison.Ordinal);
                        bool var_init_m = sv.VariantSequence.StartsWith("M", StringComparison.Ordinal);
                        if (orig_init_m && !var_init_m)
                        {
                            decoyVariations.Add(new SequenceVariation(1, "M", "", "DECOY VARIANT: Initiator Methionine Change in " + sv.Description));
                        }
                        originalArray = sv.OriginalSequence.Substring(Convert.ToInt32(orig_init_m)).ToArray();
                        variationArray = sv.VariantSequence.Substring(Convert.ToInt32(var_init_m)).ToArray();
                    }
                    int decoyEnd = protein.BaseSequence.Length - sv.OneBasedBeginPosition + 2 + Convert.ToInt32(sv.OneBasedEndPosition == reversed_sequence.Length) - Convert.ToInt32(sv.OneBasedBeginPosition == 1);
                    int decoyBegin = decoyEnd - originalArray.Length + 1;
                    Array.Reverse(originalArray);
                    Array.Reverse(variationArray);
                    decoyVariations.Add(new SequenceVariation(decoyBegin, decoyEnd, new string(originalArray), new string(variationArray), "DECOY VARIANT: " + sv.Description));
                }
                var decoyProtein = new Protein(reversed_sequence, "DECOY_" + protein.Accession, protein.Organism, protein.GeneNames.ToList(), decoyModifications, decoyPP,
                    protein.Name, protein.FullName, true, protein.IsContaminant, null, decoyVariations, decoyDisulfides, protein.DatabaseFilePath);
                lock (decoyProteins) { decoyProteins.Add(decoyProtein); }
            });
            return decoyProteins;
        }

        /// <summary>
        /// Generates a "slided" decoy sequence
        /// </summary>
        /// <param name="protein"></param>
        /// <returns></returns>
        private static List<Protein> GenerateSlideDecoys(List<Protein> proteins)
        {
            List<Protein> decoyProteins = new List<Protein>();
            Parallel.ForEach(proteins, protein =>
            {
                Dictionary<int, List<Modification>> decoyModifications = null;
                int numSlides = 20;
                char[] sequenceArrayUnslided = protein.BaseSequence.ToCharArray();
                char[] sequenceArraySlided = protein.BaseSequence.ToCharArray();
                decoyModifications = null;
                List<DisulfideBond> decoy_disulfides_slide = new List<DisulfideBond>();
                if (protein.BaseSequence.StartsWith("M", StringComparison.Ordinal))
                {
                    // Do not include the initiator methionine in shuffle!!!
                    if (numSlides % sequenceArraySlided.Length - 1 == 0)
                    {
                        numSlides++;
                    }
                    for (int i = 1; i < sequenceArraySlided.Length; i++)
                    {
                        sequenceArraySlided[i] = sequenceArrayUnslided[GetOldShuffleIndex(i, numSlides, protein.BaseSequence.Length, true)];
                    }

                    decoyModifications = new Dictionary<int, List<Modification>>(protein.OneBasedPossibleLocalizedModifications.Count);
                    foreach (var kvp in protein.OneBasedPossibleLocalizedModifications)
                    {
                        if (kvp.Key > 1)
                        {
                            decoyModifications.Add(GetOldShuffleIndex(kvp.Key - 1, numSlides, protein.BaseSequence.Length, true) + 1, kvp.Value);
                        }
                        else if (kvp.Key == 1)
                        {
                            decoyModifications.Add(1, kvp.Value);
                        }
                    }
                }
                else
                {
                    if (numSlides % sequenceArraySlided.Length == 0)
                    {
                        numSlides++;
                    }
                    for (int i = 0; i < sequenceArraySlided.Length; i++)
                    {
                        sequenceArraySlided[i] = sequenceArrayUnslided[GetOldShuffleIndex(i, numSlides, protein.BaseSequence.Length, false)];
                    }
                    decoyModifications = new Dictionary<int, List<Modification>>(protein.OneBasedPossibleLocalizedModifications.Count);
                    foreach (var kvp in protein.OneBasedPossibleLocalizedModifications)
                    {
                        decoyModifications.Add(GetOldShuffleIndex(kvp.Key - 1, numSlides, protein.BaseSequence.Length, false) + 1, kvp.Value);
                    }
                }
                var slided_sequence = new string(sequenceArraySlided);

                List<ProteolysisProduct> decoyPPSlide = new List<ProteolysisProduct>();
                foreach (ProteolysisProduct pp in protein.ProteolysisProducts)  //can't keep all aa like you can with reverse, just keep it the same length
                {
                    decoyPPSlide.Add(pp);
                }
                foreach (DisulfideBond disulfideBond in protein.DisulfideBonds) //these actually need the same cysteines...
                {
                    decoy_disulfides_slide.Add(new DisulfideBond(GetOldShuffleIndex(disulfideBond.OneBasedBeginPosition - 1, numSlides, slided_sequence.Length, false) + 1, GetOldShuffleIndex(disulfideBond.OneBasedEndPosition - 1, numSlides, slided_sequence.Length, false) + 1, "DECOY DISULFIDE BOND: " + disulfideBond.Description));
                }
                List<SequenceVariation> decoyVariationsSlide = new List<SequenceVariation>();
                foreach (SequenceVariation sv in protein.SequenceVariations) //No idea what's going on here. Review is appreciated.
                {
                    char[] originalArrayUnshuffled = sv.OriginalSequence.ToArray();
                    char[] variationArrayUnslided = sv.VariantSequence.ToArray();
                    if (sv.OneBasedBeginPosition == 1)
                    {
                        bool origInitM = sv.OriginalSequence.StartsWith("M", StringComparison.Ordinal);
                        bool varInitM = sv.VariantSequence.StartsWith("M", StringComparison.Ordinal);
                        if (origInitM && !varInitM)
                        {
                            decoyVariationsSlide.Add(new SequenceVariation(1, "M", "", "DECOY VARIANT: Initiator Methionine Change in " + sv.Description));
                        }
                        originalArrayUnshuffled = sv.OriginalSequence.Substring(Convert.ToInt32(origInitM)).ToArray();
                        variationArrayUnslided = sv.VariantSequence.Substring(Convert.ToInt32(varInitM)).ToArray();
                    }
                    int decoy_end = protein.BaseSequence.Length - sv.OneBasedBeginPosition + 2 +
                        Convert.ToInt32(sv.OneBasedEndPosition == slided_sequence.Length) - Convert.ToInt32(sv.OneBasedBeginPosition == 1);
                    int decoy_begin = decoy_end - originalArrayUnshuffled.Length + 1;
                    char[] originalArraySlided = sv.OriginalSequence.ToArray();
                    char[] variationArraySlided = sv.VariantSequence.ToArray();

                    if (numSlides % originalArraySlided.Length == 0)
                    {
                        numSlides++;
                    }
                    for (int i = 0; i < originalArraySlided.Length; i++)
                    {
                        originalArraySlided[i] = originalArrayUnshuffled[GetOldShuffleIndex(i, numSlides, originalArrayUnshuffled.Length, false)];
                    }

                    if (numSlides % variationArraySlided.Length == 0)
                    {
                        numSlides++;
                    }
                    for (int i = 0; i < variationArraySlided.Length; i++)
                    {
                        variationArraySlided[i] = variationArrayUnslided[GetOldShuffleIndex(i, numSlides, variationArrayUnslided.Length, false)];
                    }

                    decoyVariationsSlide.Add(new SequenceVariation(decoy_begin, decoy_end, new string(originalArraySlided), new string(variationArraySlided), "DECOY VARIANT: " + sv.Description));
                }
                var decoyProteinSlide = new Protein(slided_sequence, "DECOY_" + protein.Accession, protein.Organism, protein.GeneNames.ToList(), decoyModifications, decoyPPSlide,
                    protein.Name, protein.FullName, true, protein.IsContaminant, null, decoyVariationsSlide, decoy_disulfides_slide, protein.DatabaseFilePath);
                lock (decoyProteins) { decoyProteins.Add(decoyProteinSlide); }
            });
            return decoyProteins;
        }

        /// <summary>
        /// Generates a reverse decoy sequence
        /// </summary>
        /// <param name="protein"></param>
        /// <returns></returns>
        private static List<Protein> GenerateShuffledDecoys(List<Protein> proteins, DigestionParams digestionParams, int randomSeed)
        {
            List<Protein> decoyProteins = new List<Protein>();

            // Digest the proteins without considering fixed/variable mods
            List<PeptideWithSetModifications> targetPeptides = proteins.SelectMany(p => p.Digest(digestionParams, null, null)).ToList();
            HashSet<string> targetPeptideSequences = new HashSet<string>(targetPeptides.Select(p => p.BaseSequence));

            // Shuffle and concatenate decoy sequences
            var random = new Random(randomSeed);
            Parallel.ForEach(proteins, protein =>
            {
                List<PeptideWithSetModifications> peptides = protein.Digest(digestionParams, null, null).ToList();
                StringBuilder decoyProteinSeq = new StringBuilder();
                Dictionary<int, List<Modification>> decoyOneBasedModifications = new Dictionary<int, List<Modification>>();
                List<DisulfideBond> decoyDisulfides = protein.DisulfideBonds.Select(d =>
                    new DisulfideBond(d.OneBasedBeginPosition, d.OneBasedEndPosition, "DECOY DISULFIDE BOND: " + d.Description)).ToList();
                for (int i = 0; i < peptides.Count; i++)
                {
                    bool decoyPeptideAlreadyATarget = true;
                    while (decoyPeptideAlreadyATarget)
                    {
                        // Shuffle the one based indices and correct the start / end bases if important
                        string seq = peptides[i].BaseSequence;
                        bool keepFirstBase = i == 0 && seq[0] == 'M' || digestionParams.Protease.SequencesInducingCleavage.Any(x => x.Item2 == TerminusType.C && seq.StartsWith(x.Item1));
                        bool keepLastBase = i != peptides.Count - 1 && digestionParams.Protease.SequencesInducingCleavage.Any(x => x.Item2 == TerminusType.N && seq.EndsWith(x.Item1));
                        List<int> unshuffledOneBasedIndices = Enumerable.Range(peptides[i].OneBasedStartResidueInProtein, seq.Length).ToList();
                        List<int> shuffledOneBasedProteinIndices = unshuffledOneBasedIndices.OrderBy(x => random.Next()).ToList();
                        Dictionary<int, int> oneBasedIdxLookup = Enumerable.Range(0, seq.Length).ToDictionary(x => unshuffledOneBasedIndices[x], x => shuffledOneBasedProteinIndices[x]);
                        if (keepFirstBase)
                        {
                            shuffledOneBasedProteinIndices[shuffledOneBasedProteinIndices.IndexOf(peptides[i].OneBasedStartResidueInProtein)] = shuffledOneBasedProteinIndices[0];
                            shuffledOneBasedProteinIndices[0] = peptides[i].OneBasedStartResidueInProtein;
                        }
                        if (keepLastBase)
                        {
                            shuffledOneBasedProteinIndices[shuffledOneBasedProteinIndices.IndexOf(peptides[i].OneBasedEndResidueInProtein)] = shuffledOneBasedProteinIndices[seq.Length - 1];
                            shuffledOneBasedProteinIndices[seq.Length - 1] = peptides[i].OneBasedEndResidueInProtein;
                        }

                        // Construct the decoy sequence and modification set
                        // Append the decoy peptide if already in the database (and a valid length by digestion parameters)
                        string decoyPeptideSeq = new string(Enumerable.Range(0, seq.Length)
                            .Select(x => seq[shuffledOneBasedProteinIndices[x] - peptides[i].OneBasedStartResidueInProtein]).ToArray());
                        bool validTargetLength = decoyPeptideSeq.Length >= digestionParams.MinPeptideLength && digestionParams.MaxPeptideLength <= decoyPeptideSeq.Length;
                        if (validTargetLength && !targetPeptideSequences.Contains(decoyPeptideSeq.ToString()) || !validTargetLength)
                        {
                            decoyPeptideAlreadyATarget = false; // exit the while loop that keeps shuffling if in target database
                            decoyProteinSeq.Append(decoyPeptideSeq.ToString());
                            for (int j = 0; j < seq.Length; j++)
                            {
                                if (peptides[i].AllModsOneIsNterminus.TryGetValue(shuffledOneBasedProteinIndices[j], out var mod))
                                {
                                    if (decoyOneBasedModifications.TryGetValue(shuffledOneBasedProteinIndices[j], out var mods)) { mods.Add(mod); }
                                    else { decoyOneBasedModifications.Add(shuffledOneBasedProteinIndices[j], new List<Modification> { mod }); }
                                }
                            }
                            foreach (var d in decoyDisulfides)
                            {
                                if (oneBasedIdxLookup.TryGetValue(d.OneBasedBeginPosition, out int toNewBeginningsCheers)) { d.OneBasedBeginPosition = toNewBeginningsCheers; }
                                if (oneBasedIdxLookup.TryGetValue(d.OneBasedEndPosition, out int outWithTheEndInWithTheNew)) { d.OneBasedEndPosition = outWithTheEndInWithTheNew; }
                            }
                        }
                    }
                }

                var decoyProtein = new Protein(
                    decoyProteinSeq.ToString(),
                    "DECOY_" + protein.Accession,
                    protein.Organism,
                    protein.GeneNames.ToList(),
                    decoyOneBasedModifications,
                    protein.ProteolysisProducts.ToList(),
                    protein.Name,
                    protein.FullName,
                    true,
                    protein.IsContaminant,
                    null,
                    null, // sequence variants should be taken care of beforehand, since indels will be hard to recover in this decoy method
                    decoyDisulfides,
                    protein.DatabaseFilePath);

                lock (decoyProteins) { decoyProteins.Add(decoyProtein); }
            });
            return decoyProteins;
        }

        /// <summary>
        /// Not sure...
        /// </summary>
        /// <param name="i"></param>
        /// <param name="numSlides"></param>
        /// <param name="sequenceLength"></param>
        /// <param name="methioninePresent"></param>
        /// <returns></returns>
        private static int GetOldShuffleIndex(int i, int numSlides, int sequenceLength, bool methioninePresent)
        {
            if (methioninePresent)
            {
                i--;
                sequenceLength--;
            }
            bool positiveDirection = i % 2 == 0;
            int oldIndex = i;

            if (positiveDirection)
            {
                oldIndex += numSlides;
            }
            else
            {
                oldIndex -= numSlides;
            }

            while (true)
            {
                if (oldIndex < 0)
                {
                    positiveDirection = true;
                }
                else if (oldIndex >= sequenceLength)
                {
                    positiveDirection = false;
                }
                else
                {
                    return methioninePresent ? oldIndex + 1 : oldIndex;
                }

                if (positiveDirection)
                {
                    oldIndex = (oldIndex * -1) - 1;
                }
                else
                {
                    oldIndex = (sequenceLength * 2) - oldIndex - 1;
                }
            }
        }
    }
}
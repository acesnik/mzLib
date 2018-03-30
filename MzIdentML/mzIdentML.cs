using System;

namespace MzIdentML
{
    public abstract partial class MzIdentML : IdentifiableType
    {
        public abstract cvType[] cvList { get; set; }
        public abstract AnalysisSoftwareType[] AnalysisSoftwareList { get; set; }
        public abstract ProviderType Provider { get; set; }
        public abstract AbstractContactType[] AuditCollection { get; set; }
        public abstract SampleType[] AnalysisSampleCollection { get; set; }
        public abstract SequenceCollectionType SequenceCollection { get; set; }
        public abstract AnalysisCollectionType AnalysisCollection { get; set; }
        public abstract AnalysisProtocolCollectionType AnalysisProtocolCollection { get; set; }
        public abstract DataCollectionType DataCollection { get; set; }
        public abstract BibliographicReferenceType[] BibliographicReference { get; set; }
        public abstract DateTime creationDate { get; set; }
        public abstract bool creationDateSpecified { get; set; }
        public abstract string version { get; set; }
    }

    public abstract partial class cvType
    {
        public abstract string fullName { get; set; }
        public abstract string version { get; set; }
        public abstract string uri { get; set; }
        public abstract string id { get; set; }
    }

    public abstract partial class SpectrumIdentificationItemRefType
    {
        public abstract string spectrumIdentificationItem_ref { get; set; }
    }

    public abstract partial class PeptideHypothesisType
    {
        public abstract SpectrumIdentificationItemRefType[] SpectrumIdentificationItemRef { get; set; }
        public abstract string peptideEvidence_ref { get; set; }
    }

    public abstract partial class FragmentArrayType
    {
        public abstract float[] values { get; set; }
        public abstract string measure_ref { get; set; }
    }

    public abstract partial class IonTypeType
    {
        public abstract FragmentArrayType[] FragmentArray { get; set; }
        public abstract CVParamType cvParam { get; set; }
        public abstract string[] index { get; set; }
        public abstract int charge { get; set; }
    }

    public abstract partial class CVParamType : AbstractParamType
    {
        public abstract string cvRef { get; set; }
        public abstract string accession { get; set; }
    }

    public abstract partial class AbstractParamType
    {
        public abstract string name { get; set; }
        public abstract string value { get; set; }
        public abstract string unitAccession { get; set; }
        public abstract string unitName { get; set; }
        public abstract string unitCvRef { get; set; }
    }

    public abstract partial class UserParamType : AbstractParamType
    {
        public abstract string type { get; set; }
    }

    public abstract partial class PeptideEvidenceRefType
    {
        public abstract string peptideEvidence_ref { get; set; }
    }

    public abstract partial class AnalysisDataType
    {
        public abstract SpectrumIdentificationListType[] SpectrumIdentificationList { get; set; }
        public abstract ProteinDetectionListType ProteinDetectionList { get; set; }
    }

    public abstract partial class SpectrumIdentificationListType : IdentifiableType
    {
        public abstract MeasureType[] FragmentationTable { get; set; }
        public abstract SpectrumIdentificationResultType[] SpectrumIdentificationResult { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract long numSequencesSearched { get; set; }
        public abstract bool numSequencesSearchedSpecified { get; set; }
    }

    public abstract partial class MeasureType : IdentifiableType
    {
        public abstract CVParamType[] cvParam { get; set; }
    }

    public abstract partial class IdentifiableType
    {
        public abstract string id { get; set; }
        public abstract string name { get; set; }
    }

    public abstract partial class BibliographicReferenceType : IdentifiableType
    {
        public abstract string authors { get; set; }
        public abstract string publication { get; set; }
        public abstract string publisher { get; set; }
        public abstract string editor { get; set; }
        public abstract int year { get; set; }
        public abstract bool yearSpecified { get; set; }
        public abstract string volume { get; set; }
        public abstract string issue { get; set; }
        public abstract string pages { get; set; }
        public abstract string title { get; set; }
        public abstract string doi { get; set; }
    }

    public abstract partial class ProteinDetectionHypothesisType : IdentifiableType
    {
        public abstract PeptideHypothesisType[] PeptideHypothesis { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract string dBSequence_ref { get; set; }
        public abstract bool passThreshold { get; set; }
    }

    public abstract partial class ProteinAmbiguityGroupType : IdentifiableType
    {
        public abstract ProteinDetectionHypothesisType[] ProteinDetectionHypothesis { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class ProteinDetectionListType : IdentifiableType
    {
        public abstract ProteinAmbiguityGroupType[] ProteinAmbiguityGroup { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class SpectrumIdentificationItemType : IdentifiableType
    {
        public abstract PeptideEvidenceRefType[] PeptideEvidenceRef { get; set; }
        public abstract IonTypeType[] Fragmentation { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract int chargeState { get; set; }
        public abstract double experimentalMassToCharge { get; set; }
        public abstract double calculatedMassToCharge { get; set; }
        public abstract bool calculatedMassToChargeSpecified { get; set; }
        public abstract float calculatedPI { get; set; }
        public abstract bool calculatedPISpecified { get; set; }
        public abstract string peptide_ref { get; set; }
        public abstract int rank { get; set; }
        public abstract bool passThreshold { get; set; }
        public abstract string massTable_ref { get; set; }
        public abstract string sample_ref { get; set; }
    }

    public abstract partial class SpectrumIdentificationResultType : IdentifiableType
    {
        public abstract SpectrumIdentificationItemType[] SpectrumIdentificationItem { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract string spectrumID { get; set; }
        public abstract string spectraData_ref { get; set; }
    }

    public abstract partial class ExternalDataType : IdentifiableType
    {
        public abstract string ExternalFormatDocumentation { get; set; }
        public abstract FileFormatType FileFormat { get; set; }
        public abstract string location { get; set; }
    }

    public abstract partial class FileFormatType
    {
        public abstract CVParamType cvParam { get; set; }
    }

    public abstract partial class SpectraDataType : ExternalDataType
    {
        public abstract SpectrumIDFormatType SpectrumIDFormat { get; set; }
    }

    public abstract partial class SpectrumIDFormatType
    {
        public abstract CVParamType cvParam { get; set; }
    }

    public abstract partial class SourceFileType : ExternalDataType
    {
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class SearchDatabaseType : ExternalDataType
    {
        public abstract ParamType DatabaseName { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract string version { get; set; }
        public abstract DateTime releaseDate { get; set; }
        public abstract bool releaseDateSpecified { get; set; }
        public abstract long numDatabaseSequences { get; set; }
        public abstract bool numDatabaseSequencesSpecified { get; set; }
        public abstract long numResidues { get; set; }
        public abstract bool numResiduesSpecified { get; set; }
    }

    public abstract partial class ParamType
    {
        public abstract AbstractParamType Item { get; set; }
    }

    public abstract partial class ProteinDetectionProtocolType : IdentifiableType
    {
        public abstract ParamListType AnalysisParams { get; set; }
        public abstract ParamListType Threshold { get; set; }
        public abstract string analysisSoftware_ref { get; set; }
    }

    public abstract partial class ParamListType
    {
        public abstract AbstractParamType[] Items { get; set; }
    }

    public abstract partial class TranslationTableType : IdentifiableType
    {
        public abstract CVParamType[] cvParam { get; set; }
    }

    public abstract partial class MassTableType : IdentifiableType
    {
        public abstract ResidueType[] Residue { get; set; }
        public abstract AmbiguousResidueType[] AmbiguousResidue { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract string[] msLevel { get; set; }
    }

    public abstract partial class ResidueType
    {
        public abstract string code { get; set; }
        public abstract float mass { get; set; }
    }

    public abstract partial class AmbiguousResidueType
    {
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract string code { get; set; }
    }

    public abstract partial class EnzymeType : IdentifiableType
    {
        public abstract string SiteRegexp { get; set; }
        public abstract ParamListType EnzymeName { get; set; }
        public abstract string nTermGain { get; set; }
        public abstract string cTermGain { get; set; }
        public abstract bool semiSpecific { get; set; }
        public abstract bool semiSpecificSpecified { get; set; }
        public abstract int missedCleavages { get; set; }
        public abstract bool missedCleavagesSpecified { get; set; }
        public abstract int minDistance { get; set; }
        public abstract bool minDistanceSpecified { get; set; }
    }

    public abstract partial class SpectrumIdentificationProtocolType : IdentifiableType
    {
        public abstract ParamType SearchType { get; set; }
        public abstract ParamListType AdditionalSearchParams { get; set; }
        public abstract SearchModificationType[] ModificationParams { get; set; }
        public abstract EnzymesType Enzymes { get; set; }
        public abstract MassTableType[] MassTable { get; set; }
        public abstract CVParamType[] FragmentTolerance { get; set; }
        public abstract CVParamType[] ParentTolerance { get; set; }
        public abstract ParamListType Threshold { get; set; }
        public abstract FilterType[] DatabaseFilters { get; set; }
        public abstract DatabaseTranslationType DatabaseTranslation { get; set; }
        public abstract string analysisSoftware_ref { get; set; }
    }

    public abstract partial class SearchModificationType
    {
        public abstract CVParamType[] SpecificityRules { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract bool fixedMod { get; set; }
        public abstract float massDelta { get; set; }
        public abstract string residues { get; set; }
    }

    public abstract partial class EnzymesType
    {
        public abstract EnzymeType[] Enzyme { get; set; }
        public abstract bool independent { get; set; }
        public abstract bool independentSpecified { get; set; }
    }

    public abstract partial class FilterType
    {
        public abstract ParamType FilterType1 { get; set; }
        public abstract ParamListType Include { get; set; }
        public abstract ParamListType Exclude { get; set; }
    }

    public abstract partial class DatabaseTranslationType
    {
        public abstract TranslationTableType[] TranslationTable { get; set; }
        public abstract int[] frames { get; set; }
    }

    public abstract partial class ProtocolApplicationType : IdentifiableType
    {
        public abstract DateTime activityDate { get; set; }
        public abstract bool activityDateSpecified { get; set; }
    }

    public abstract partial class ProteinDetectionType : ProtocolApplicationType
    {
        public abstract InputSpectrumIdentificationsType[] InputSpectrumIdentifications { get; set; }
        public abstract string proteinDetectionList_ref { get; set; }
        public abstract string proteinDetectionProtocol_ref { get; set; }
    }

    public abstract partial class InputSpectrumIdentificationsType
    {
        public abstract string spectrumIdentificationList_ref { get; set; }
    }

    public abstract partial class SpectrumIdentificationType : ProtocolApplicationType
    {
        public abstract InputSpectraType[] InputSpectra { get; set; }
        public abstract SearchDatabaseRefType[] SearchDatabaseRef { get; set; }
        public abstract string spectrumIdentificationProtocol_ref { get; set; }
        public abstract string spectrumIdentificationList_ref { get; set; }
    }

    public abstract partial class InputSpectraType
    {
        public abstract string spectraData_ref { get; set; }
    }

    public abstract partial class SearchDatabaseRefType
    {
        public abstract string searchDatabase_ref { get; set; }
    }

    public abstract partial class PeptideEvidenceType : IdentifiableType
    {
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract string dBSequence_ref { get; set; }
        public abstract string peptide_ref { get; set; }
        public abstract int start { get; set; }
        public abstract bool startSpecified { get; set; }
        public abstract int end { get; set; }
        public abstract bool endSpecified { get; set; }
        public abstract string pre { get; set; }
        public abstract string post { get; set; }
        public abstract string translationTable_ref { get; set; }
        public abstract int frame { get; set; }
        public abstract bool frameSpecified { get; set; }
        public abstract bool isDecoy { get; set; }
    }

    public abstract partial class PeptideType : IdentifiableType
    {
        public abstract string PeptideSequence { get; set; }
        public abstract ModificationType[] Modification { get; set; }
        public abstract SubstitutionModificationType[] SubstitutionModification { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class ModificationType
    {
        public abstract CVParamType[] cvParam { get; set; }
        public abstract int location { get; set; }
        public abstract bool locationSpecified { get; set; }
        public abstract string[] residues { get; set; }
        public abstract double avgMassDelta { get; set; }
        public abstract bool avgMassDeltaSpecified { get; set; }
        public abstract double monoisotopicMassDelta { get; set; }
        public abstract bool monoisotopicMassDeltaSpecified { get; set; }
    }

    public abstract partial class SubstitutionModificationType
    {
        public abstract string originalResidue { get; set; }
        public abstract string replacementResidue { get; set; }
        public abstract int location { get; set; }
        public abstract bool locationSpecified { get; set; }
        public abstract double avgMassDelta { get; set; }
        public abstract bool avgMassDeltaSpecified { get; set; }
        public abstract double monoisotopicMassDelta { get; set; }
        public abstract bool monoisotopicMassDeltaSpecified { get; set; }
    }

    public abstract partial class DBSequenceType : IdentifiableType
    {
        public abstract string Seq { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
        public abstract int length { get; set; }
        public abstract bool lengthSpecified { get; set; }
        public abstract string searchDatabase_ref { get; set; }
        public abstract string accession { get; set; }
    }

    public abstract partial class SampleType : IdentifiableType
    {
        public abstract ContactRoleType[] ContactRole { get; set; }
        public abstract SubSampleType[] SubSample { get; set; }
        public abstract CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class ContactRoleType
    {
        public abstract RoleType Role { get; set; }
        public abstract string contact_ref { get; set; }
    }

    public abstract partial class RoleType
    {
        public abstract CVParamType cvParam { get; set; }
    }

    public abstract partial class SubSampleType
    {
        public abstract string sample_ref { get; set; }
    }

    public abstract partial class AbstractContactType : IdentifiableType
    {
        public CVParamType[] cvParam { get; set; }
        public abstract UserParamType[] userParam { get; set; }
    }

    public abstract partial class OrganizationType : AbstractContactType
    {
        public abstract ParentOrganizationType Parent { get; set; }
    }

    public abstract partial class ParentOrganizationType
    {
        public abstract string organization_ref { get; set; }
    }

    public abstract partial class PersonType : AbstractContactType
    {
        public abstract AffiliationType[] Affiliation { get; set; }
        public abstract string lastName { get; set; }
        public abstract string firstName { get; set; }
        public abstract string midInitials { get; set; }
    }

    public abstract partial class AffiliationType
    {
        public abstract string organization_ref { get; set; }
    }

    public abstract partial class ProviderType : IdentifiableType
    {
        public abstract ContactRoleType ContactRole { get; set; }
        public abstract string analysisSoftware_ref { get; set; }
    }

    public abstract partial class AnalysisSoftwareType : IdentifiableType
    {
        public abstract ContactRoleType ContactRole { get; set; }
        public abstract ParamType SoftwareName { get; set; }
        public abstract string Customizations { get; set; }
        public abstract string version { get; set; }
        public abstract string uri { get; set; }
    }

    public abstract partial class InputsType
    {
        public abstract SourceFileType[] SourceFile { get; set; }
        public abstract SearchDatabaseType[] SearchDatabase { get; set; }
        public abstract SpectraDataType[] SpectraData { get; set; }
    }

    public abstract partial class DataCollectionType
    {
        public abstract InputsType Inputs { get; set; }
        public abstract AnalysisDataType AnalysisData { get; set; }
    }

    public abstract partial class AnalysisProtocolCollectionType
    {
        public abstract SpectrumIdentificationProtocolType[] SpectrumIdentificationProtocol { get; set; }
        public abstract ProteinDetectionProtocolType ProteinDetectionProtocol { get; set; }
    }

    public abstract partial class AnalysisCollectionType
    {
        public abstract SpectrumIdentificationType[] SpectrumIdentification { get; set; }
        public abstract ProteinDetectionType ProteinDetection { get; set; }
    }

    public abstract partial class SequenceCollectionType
    {
        public abstract DBSequenceType[] DBSequence { get; set; }
        public abstract PeptideType[] Peptide { get; set; }
        public abstract PeptideEvidenceType[] PeptideEvidence { get; set; }
    }
}